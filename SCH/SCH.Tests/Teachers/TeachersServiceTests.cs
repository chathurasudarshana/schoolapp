namespace SCH.Tests.Teachers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using Moq;
    using SCH.Models.Auth.Constants;
    using SCH.Models.Auth.Entities;
    using SCH.Models.Teachers.ClientDtos;
    using SCH.Models.Teachers.Entities;
    using SCH.Repositories.Teachers;
    using SCH.Repositories.UnitOfWork;
    using SCH.Services.Auth;
    using SCH.Services.Teachers;
    using SCH.Shared.Exceptions;
    using SCH.Shared.HttpContext;
    using Xunit;

    public class TeachersServiceTests
    {
        private readonly Mock<ISCHUnitOfWork> _unitOfWork = new();
        private readonly Mock<ITeachersRepository> _teachersRepository = new();
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<IAuthService> _authService = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<IUserInfo> _userInfo = new();
        private readonly TeachersService _sut;

        public TeachersServiceTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            _sut = new TeachersService(
                _unitOfWork.Object,
                _teachersRepository.Object,
                _userManager.Object,
                _authService.Object,
                _mapper.Object,
                _userInfo.Object);
        }

        [Fact]
        public async Task GetTeachersAsync_ReturnsListOfTeacherDtos()
        {
            var teachers = new List<Teacher> { new() { Id = 1, Name = "Alice" } };
            var dtos = new List<TeacherDto> { new() { Id = 1, Name = "Alice" } };
            _teachersRepository.Setup(r => r.GetTeachersAsync()).ReturnsAsync(teachers);
            _mapper.Setup(m => m.Map<List<TeacherDto>>(teachers)).Returns(dtos);

            var result = await _sut.GetTeachersAsync();

            Assert.Equal(dtos, result);
        }

        [Fact]
        public async Task GetTeacherAsync_WhenFound_ReturnsTeacherDto()
        {
            var teacher = new Teacher { Id = 1, Name = "Alice" };
            var dto = new TeacherDto { Id = 1, Name = "Alice" };
            _teachersRepository.Setup(r => r.GetTeacherAsync(1)).ReturnsAsync(teacher);
            _mapper.Setup(m => m.Map<TeacherDto>(teacher)).Returns(dto);

            var result = await _sut.GetTeacherAsync(1);

            Assert.Equal(dto, result);
        }

        [Fact]
        public async Task GetTeacherAsync_WhenNotFound_ReturnsNull()
        {
            _teachersRepository.Setup(r => r.GetTeacherAsync(99)).ReturnsAsync((Teacher?)null);

            var result = await _sut.GetTeacherAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task InsertTeacherAsync_WithUserId_SavesAndAssignsTeacherRole()
        {
            var dto = new TeacherDto { Name = "Alice", UserId = 5 };
            var user = new ApplicationUser { Id = 5 };
            _teachersRepository.Setup(r => r.InsertTeacherAsync(It.IsAny<Teacher>())).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            _userManager.Setup(m => m.FindByIdAsync("5")).ReturnsAsync(user);
            _userManager.Setup(m => m.IsInRoleAsync(user, Role.Teacher)).ReturnsAsync(false);
            _userManager.Setup(m => m.AddToRoleAsync(user, Role.Teacher)).ReturnsAsync(IdentityResult.Success);

            await _sut.InsertTeacherAsync(dto);

            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            _userManager.Verify(m => m.AddToRoleAsync(user, Role.Teacher), Times.Once);
        }

        [Fact]
        public async Task InsertTeacherAsync_WithoutUserId_DoesNotAssignRole()
        {
            var dto = new TeacherDto { Name = "Alice", UserId = null };
            _teachersRepository.Setup(r => r.InsertTeacherAsync(It.IsAny<Teacher>())).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _sut.InsertTeacherAsync(dto);

            _userManager.Verify(
                m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateTeacherAsync_WhenNotFound_ThrowsNotFound()
        {
            var dto = new TeacherDto { Id = 99, Name = "Alice" };
            _teachersRepository.Setup(r => r.GetTeacherAsync(99)).ReturnsAsync((Teacher?)null);

            await Assert.ThrowsAsync<SCHDomainException>(() => _sut.UpdateTeacherAsync(dto));
        }

        [Fact]
        public async Task UpdateTeacherAsync_AsNonAdmin_DoesNotChangeUserId()
        {
            var existing = new Teacher { Id = 1, Name = "Alice", UserId = 10 };
            var dto = new TeacherDto { Id = 1, Name = "Alice Updated", UserId = 99 };
            _teachersRepository.Setup(r => r.GetTeacherAsync(1)).ReturnsAsync(existing);
            _userInfo.Setup(u => u.IsInRole(Role.Admin)).Returns(false);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _sut.UpdateTeacherAsync(dto);

            Assert.Equal(10, existing.UserId);
            _userManager.Verify(
                m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateTeacherAsync_AsAdmin_UserIdChanged_RevokesOldAndAssignsNew()
        {
            var oldUser = new ApplicationUser { Id = 10 };
            var newUser = new ApplicationUser { Id = 20 };
            var existing = new Teacher { Id = 1, Name = "Alice", UserId = 10 };
            var dto = new TeacherDto { Id = 1, Name = "Alice", UserId = 20 };

            _teachersRepository.Setup(r => r.GetTeacherAsync(1)).ReturnsAsync(existing);
            _userInfo.Setup(u => u.IsInRole(Role.Admin)).Returns(true);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);
            _authService.Setup(a => a.RevokeAllUserSessionsAsync(10)).Returns(Task.CompletedTask);
            _userManager.Setup(m => m.FindByIdAsync("10")).ReturnsAsync(oldUser);
            _userManager.Setup(m => m.IsInRoleAsync(oldUser, Role.Teacher)).ReturnsAsync(true);
            _userManager.Setup(m => m.RemoveFromRoleAsync(oldUser, Role.Teacher)).ReturnsAsync(IdentityResult.Success);
            _userManager.Setup(m => m.FindByIdAsync("20")).ReturnsAsync(newUser);
            _userManager.Setup(m => m.IsInRoleAsync(newUser, Role.Teacher)).ReturnsAsync(false);
            _userManager.Setup(m => m.AddToRoleAsync(newUser, Role.Teacher)).ReturnsAsync(IdentityResult.Success);

            await _sut.UpdateTeacherAsync(dto);

            _userManager.Verify(m => m.RemoveFromRoleAsync(oldUser, Role.Teacher), Times.Once);
            _authService.Verify(a => a.RevokeAllUserSessionsAsync(10), Times.Once);
            _userManager.Verify(m => m.AddToRoleAsync(newUser, Role.Teacher), Times.Once);
        }

        [Fact]
        public async Task DeleteTeacherAsync_CallsDeleteAndSaves()
        {
            _teachersRepository.Setup(r => r.DeleteTeacherAsync(1)).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _sut.DeleteTeacherAsync(1);

            _teachersRepository.Verify(r => r.DeleteTeacherAsync(1), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
