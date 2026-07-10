namespace SCH.Tests.Students
{
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using Moq;
    using SCH.Models.Auth.Constants;
    using SCH.Models.Auth.Entities;
    using SCH.Models.Common.GridEntities;
    using SCH.Models.Courses.Entities;
    using SCH.Models.StudentCourseMap.ClientDtos;
    using SCH.Models.StudentCourseMap.Entities;
    using SCH.Models.Students.ClientDtos;
    using SCH.Models.Students.Entities;
    using SCH.Repositories.Courses;
    using SCH.Repositories.StudentCourseMap;
    using SCH.Repositories.Students;
    using SCH.Repositories.UnitOfWork;
    using SCH.Services.Auth;
    using SCH.Services.Students;
    using SCH.Shared.Exceptions;
    using SCH.Shared.HttpContext;
    using Xunit;

    public class StudentsServiceTests
    {
        private readonly Mock<ISCHUnitOfWork> _unitOfWork = new();
        private readonly Mock<IStudentsRepository> _studentsRepository = new();
        private readonly Mock<ICoursesRepository> _coursesRepository = new();
        private readonly Mock<IStudentCourseMapRepository> _studentCourseMapRepository = new();
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<IAuthService> _authService = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<IUserInfo> _userInfo = new();
        private readonly StudentsService _sut;

        public StudentsServiceTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _sut = new StudentsService(
                _unitOfWork.Object,
                _studentsRepository.Object,
                _coursesRepository.Object,
                _studentCourseMapRepository.Object,
                _userManager.Object,
                _authService.Object,
                _mapper.Object,
                _userInfo.Object);
        }

        [Fact]
        public async Task GetStudentsAsync_ReturnsListOfStudentDtos()
        {
            var students = new List<Student> { new() { Id = 1, FirstName = "John", StudentCourseMaps = new List<StudentCourseMap>() } };
            var dtos = new List<StudentDto> { new() { Id = 1, FirstName = "John" } };
            _studentsRepository.Setup(r => r.GetStudentsAsync(null)).ReturnsAsync(students);
            _mapper.Setup(m => m.Map<List<StudentDto>>(students)).Returns(dtos);

            var result = await _sut.GetStudentsAsync(null);

            Assert.Equal(dtos, result);
        }

        [Fact]
        public async Task GetStudentAsync_WhenFound_ReturnsStudentDto()
        {
            var student = new Student { Id = 1, FirstName = "John", StudentCourseMaps = new List<StudentCourseMap>() };
            var dto = new StudentDto { Id = 1, FirstName = "John" };
            _studentsRepository.Setup(r => r.GetStudentAsync(1)).ReturnsAsync(student);
            _mapper.Setup(m => m.Map<StudentDto>(student)).Returns(dto);

            var result = await _sut.GetStudentAsync(1);

            Assert.Equal(dto, result);
        }

        [Fact]
        public async Task GetStudentAsync_WhenNotFound_ReturnsNull()
        {
            _studentsRepository.Setup(r => r.GetStudentAsync(99)).ReturnsAsync((Student?)null);

            var result = await _sut.GetStudentAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task InsertStudentAsync_WithNoCourses_InsertsAndSaves()
        {
            var dto = new StudentDto { FirstName = "John", Courses = new List<StudentCourseMapDto>() };
            _studentsRepository.Setup(r => r.InsertStudentAsync(It.IsAny<Student>())).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _sut.InsertStudentAsync(dto);

            _studentsRepository.Verify(r => r.InsertStudentAsync(It.IsAny<Student>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task InsertStudentAsync_WithCourseCountMismatch_ThrowsConflict()
        {
            var dto = new StudentDto
            {
                FirstName = "John",
                Courses = new List<StudentCourseMapDto>
                {
                    new() { CourseId = 1, EnrollmentDate = DateTime.Today },
                    new() { CourseId = 2, EnrollmentDate = DateTime.Today }
                }
            };
            // Repository returns only 1 course instead of 2 — triggers conflict
            _coursesRepository
                .Setup(r => r.GetCoursesAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Course> { new() { Id = 1, Name = "Math", StudentCourseMaps = new List<StudentCourseMap>() } });

            await Assert.ThrowsAsync<SCHDomainException>(() => _sut.InsertStudentAsync(dto));
        }

        [Fact]
        public async Task InsertStudentAsync_WithUserId_AssignsStudentRole()
        {
            var dto = new StudentDto { FirstName = "John", UserId = 7, Courses = new List<StudentCourseMapDto>() };
            var user = new ApplicationUser { Id = 7 };
            _studentsRepository.Setup(r => r.InsertStudentAsync(It.IsAny<Student>())).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            _userManager.Setup(m => m.FindByIdAsync("7")).ReturnsAsync(user);
            _userManager.Setup(m => m.IsInRoleAsync(user, Role.Student)).ReturnsAsync(false);
            _userManager.Setup(m => m.AddToRoleAsync(user, Role.Student)).ReturnsAsync(IdentityResult.Success);

            await _sut.InsertStudentAsync(dto);

            _userManager.Verify(m => m.AddToRoleAsync(user, Role.Student), Times.Once);
        }

        [Fact]
        public async Task UpdateStudentAsync_WhenNotFound_ThrowsNotFound()
        {
            var dto = new StudentDto { Id = 99, FirstName = "John" };
            _studentsRepository.Setup(r => r.GetStudentAsync(99)).ReturnsAsync((Student?)null);

            await Assert.ThrowsAsync<SCHDomainException>(() => _sut.UpdateStudentAsync(dto));
        }

        [Fact]
        public async Task UpdateStudentAsync_AsAdmin_UserIdChanged_RevokesOldAndAssignsNew()
        {
            var oldUser = new ApplicationUser { Id = 10 };
            var newUser = new ApplicationUser { Id = 20 };
            var existing = new Student
            {
                Id = 1,
                FirstName = "John",
                UserId = 10,
                StudentCourseMaps = new List<StudentCourseMap>()
            };
            var dto = new StudentDto
            {
                Id = 1,
                FirstName = "John",
                UserId = 20,
                Courses = new List<StudentCourseMapDto>()
            };

            _studentsRepository.Setup(r => r.GetStudentAsync(1)).ReturnsAsync(existing);
            _userInfo.Setup(u => u.IsInRole(Role.Admin)).Returns(true);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            _authService.Setup(a => a.RevokeAllUserSessionsAsync(10)).Returns(Task.CompletedTask);
            _userManager.Setup(m => m.FindByIdAsync("10")).ReturnsAsync(oldUser);
            _userManager.Setup(m => m.IsInRoleAsync(oldUser, Role.Student)).ReturnsAsync(true);
            _userManager.Setup(m => m.RemoveFromRoleAsync(oldUser, Role.Student)).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(m => m.FindByIdAsync("20")).ReturnsAsync(newUser);
            _userManager.Setup(m => m.IsInRoleAsync(newUser, Role.Student)).ReturnsAsync(false);
            _userManager.Setup(m => m.AddToRoleAsync(newUser, Role.Student)).ReturnsAsync(IdentityResult.Success);

            await _sut.UpdateStudentAsync(dto);

            _userManager.Verify(m => m.RemoveFromRoleAsync(oldUser, Role.Student), Times.Once);
            _authService.Verify(a => a.RevokeAllUserSessionsAsync(10), Times.Once);
            _userManager.Verify(m => m.AddToRoleAsync(newUser, Role.Student), Times.Once);
        }

        [Fact]
        public async Task DeleteStudentAsync_DeletesAndSaves()
        {
            _studentsRepository.Setup(r => r.DeleteStudentAsync(1)).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _sut.DeleteStudentAsync(1);

            _studentsRepository.Verify(r => r.DeleteStudentAsync(1), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetCoursesAsync_ReturnsMappedList()
        {
            var maps = new List<StudentCourseMap> { new() { StudentId = 1, CourseId = 2 } };
            var dtos = new List<StudentCourseMapDto> { new() { StudentId = 1, CourseId = 2, EnrollmentDate = DateTime.Today } };
            _studentCourseMapRepository.Setup(r => r.GetStudentCourseMapsByStudentAsync(1)).ReturnsAsync(maps);
            _mapper.Setup(m => m.Map<List<StudentCourseMapDto>>(maps)).Returns(dtos);

            var result = await _sut.GetCoursesAsync(1);

            Assert.Equal(dtos, result);
        }

        [Fact]
        public async Task InsertCourseAsync_WhenStudentNotFound_ThrowsNotFound()
        {
            var dto = new StudentCourseMapDto { StudentId = 99, CourseId = 1, EnrollmentDate = DateTime.Today };
            _studentsRepository.Setup(r => r.GetStudentAsync(99)).ReturnsAsync((Student?)null);

            await Assert.ThrowsAsync<SCHDomainException>(() => _sut.InsertCourseAsync(dto));
        }

        [Fact]
        public async Task InsertCourseAsync_WhenCourseNotFound_ThrowsNotFound()
        {
            var student = new Student { Id = 1, FirstName = "John", StudentCourseMaps = new List<StudentCourseMap>() };
            var dto = new StudentCourseMapDto { StudentId = 1, CourseId = 99, EnrollmentDate = DateTime.Today };
            _studentsRepository.Setup(r => r.GetStudentAsync(1)).ReturnsAsync(student);
            _coursesRepository.Setup(r => r.GetCourseAsync(99)).ReturnsAsync((Course?)null);

            await Assert.ThrowsAsync<SCHDomainException>(() => _sut.InsertCourseAsync(dto));
        }

        [Fact]
        public async Task GetStudentGridAsync_ReturnsMappedPagedResult()
        {
            var request = new StudentGridRequest();
            var students = new List<Student>();
            var pagedStudents = new PagedResult<Student> { Items = students, TotalCount = 5, PageNumber = 1, PageSize = 10 };
            var studentDtos = new List<StudentDto>();
            _studentsRepository.Setup(r => r.GetStudentGridAsync(request)).ReturnsAsync(pagedStudents);
            _mapper.Setup(m => m.Map<List<StudentDto>>(students)).Returns(studentDtos);

            var result = await _sut.GetStudentGridAsync(request);

            Assert.Equal(5, result.TotalCount);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(studentDtos, result.Items);
        }
    }
}
