namespace SCH.Services.Students
{
    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
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
    using SCH.Shared.Exceptions;

    internal class StudentsService: IStudentsService
    {
        private readonly ISCHUnitOfWork unitOfWork;
        private readonly IStudentsRepository studentsRepository;
        private readonly ICoursesRepository coursesRepository;
        private readonly IStudentCourseMapRepository studentCourseMapRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public StudentsService(
            ISCHUnitOfWork unitOfWork,
            IStudentsRepository studentsRepository,
            ICoursesRepository coursesRepository,
            IStudentCourseMapRepository studentCourseMapRepository,
            UserManager<ApplicationUser> userManager,
            IAuthService authService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor) 
        { 
            this.unitOfWork = unitOfWork;
            this.studentsRepository = studentsRepository;
            this.coursesRepository = coursesRepository;
            this.studentCourseMapRepository = studentCourseMapRepository;
            this.userManager = userManager;
            this.authService = authService;
            this.mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<StudentDto>> GetStudentsAsync(bool? isActive)
        {
            List<Student> students = await studentsRepository
                .GetStudentsAsync(isActive);

            return mapper.Map<List<StudentDto>>(students);
        }

        public async Task<StudentDto?> GetStudentAsync(int id)
        {
            Student? student = await studentsRepository.GetStudentAsync(id);

            if (student == null)
            {
                return null;
            }

            return mapper.Map<StudentDto>(student);
        }

        public async Task<int> InsertStudentAsync(StudentDto student)
        {
            await ValidateCourses(student);

            Student studentEntity = new Student
            {
                Id = 0,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                Image = student.Image,
                IsActive = student.IsActive,
                PhoneNumber = student.PhoneNumber,
                SSN = student.SSN,
                StartDate = student.StartDate,
                UserId = student.UserId,
                StudentCourseMaps = student.Courses
                    .Select(c => new StudentCourseMap
                    {
                        CourseId = c.CourseId,
                        EnrollmentDate = c.EnrollmentDate,
                    }).ToList()
            };

            await studentsRepository.InsertStudentAsync(studentEntity);
            await unitOfWork.SaveChangesAsync();

            // If a UserId is linked, assign the Student role
            if (student.UserId.HasValue)
            {
                await AssignStudentRoleAsync(student.UserId.Value);
            }

            return studentEntity.Id;
        }

        public async Task UpdateStudentAsync(StudentDto student)
        {
            Student? studentEntity = await studentsRepository
                .GetStudentAsync(student.Id);

            if (studentEntity == null)
            {
                throw SCHDomainException.NotFound();
            }

            await ValidateCourses(student);

            bool isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(Role.Admin) == true;

            int? oldUserId = studentEntity.UserId;
            int? newUserId = student.UserId;

            studentEntity.FirstName = student.FirstName;
            studentEntity.LastName = student.LastName;
            studentEntity.Email = student.Email;
            studentEntity.Image = student.Image;
            studentEntity.IsActive = student.IsActive;
            studentEntity.PhoneNumber = student.PhoneNumber;
            studentEntity.SSN = student.SSN;
            studentEntity.StartDate = student.StartDate;

            if (isAdmin)
            {
                studentEntity.UserId = newUserId;
                studentEntity.User = null;
            }

            // Include RowVersion from frontend for concurrency check
            studentEntity.RowVersion = student.RowVersion ?? studentEntity.RowVersion;

            List<StudentCourseMap> deletedMaps = studentEntity
                .StudentCourseMaps
                .Where(scm => !student
                    .Courses.Any(c => c.CourseId == scm.CourseId))
                .ToList();

            foreach (StudentCourseMap sc in deletedMaps)
            {
                studentEntity.StudentCourseMaps.Remove(sc);
            }

            List<StudentCourseMap> newMaps = student
                .Courses
                .Where(c => !studentEntity
                    .StudentCourseMaps.Any(scm => scm.CourseId == c.CourseId))
                .Select(c => new StudentCourseMap
                {
                    CourseId = c.CourseId,
                    EnrollmentDate = c.EnrollmentDate,
                    StudentId = studentEntity.Id
                }).ToList();

            foreach (StudentCourseMap sc in newMaps)
            {
                studentEntity.StudentCourseMaps.Add(sc);
            }

            // Repository handles concurrency check
            studentsRepository.UpdateAsync(studentEntity);
            await unitOfWork.SaveChangesAsync();

            // Handle UserId change: manage roles and revoke stale sessions
            if (isAdmin && oldUserId != newUserId)
            {
                if (oldUserId.HasValue)
                {
                    await RemoveStudentRoleAsync(oldUserId.Value);
                    await authService.RevokeAllUserSessionsAsync(oldUserId.Value);
                }
                if (newUserId.HasValue)
                {
                    await AssignStudentRoleAsync(newUserId.Value);
                }

            }
        }

        public async Task DeleteStudentAsync(int id)
        {
            await studentsRepository
                .DeleteStudentAsync(id);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task<List<StudentCourseMapDto>> GetCoursesAsync(int id)
        {
            List<StudentCourseMap> courses = await studentCourseMapRepository
                .GetStudentCourseMapsByStudentAsync(id);

            return mapper.Map<List<StudentCourseMapDto>>(courses);
        }

        public async Task InsertCourseAsync(StudentCourseMapDto studentCourseMap)
        {
            Student? student = await studentsRepository
                .GetStudentAsync(studentCourseMap.StudentId);

            if (student == null)
            {
                throw SCHDomainException.NotFound("Student not found.");
            }

            Course? course = await coursesRepository
                .GetCourseAsync(studentCourseMap.CourseId);

            if (course == null)
            {
                throw SCHDomainException.NotFound("Course not found.");
            }

            StudentCourseMap? existingStudentCourseMap = await studentCourseMapRepository
                .GetStudentCourseMapAsync(
                    studentCourseMap.StudentId,
                    studentCourseMap.CourseId);

            if (existingStudentCourseMap != null)
            {
                await this.DeleteCourseAsync(
                    student.Id, course.Id);
            }

            StudentCourseMap newStudentCourseMap = new StudentCourseMap
            {
                StudentId = student.Id,
                CourseId = course.Id,
                EnrollmentDate = studentCourseMap.EnrollmentDate
            };

            await studentCourseMapRepository
                .InsertStudentCourseMapAsync(newStudentCourseMap);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(int id, int courseId)
        {
            await studentCourseMapRepository
                .DeleteStudentCourseMapAsync(id, courseId);

            await unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedResult<StudentDto>> GetStudentGridAsync(StudentGridRequest request)
        {
            PagedResult<Student> result = await studentsRepository
                .GetStudentGridAsync(request);

            return new PagedResult<StudentDto>
            {
                Items      = mapper.Map<List<StudentDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize   = result.PageSize,
            };
        }

        private async Task ValidateCourses(StudentDto student)
        {
            if (student.Courses.Count > 0)
            {
                List<int> courseIds = student.Courses
                    .Select(c => c.CourseId)
                    .ToList();

                List<Course> courses = await coursesRepository
                    .GetCoursesAsync(courseIds);

                if (courses.Count != student.Courses.Count)
                {
                    throw SCHDomainException.Conflict(
                        "Some of the courses are not found.");
                }
            }
        }

        private async Task AssignStudentRoleAsync(int userId)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString());
            if (user != null && !await userManager.IsInRoleAsync(user, Role.Student))
            {
                await userManager.AddToRoleAsync(user, Role.Student);
            }

        }

        private async Task RemoveStudentRoleAsync(int userId)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString());
            if (user != null && await userManager.IsInRoleAsync(user, Role.Student)) 
            {
                await userManager.RemoveFromRoleAsync(user, Role.Student);
            }

        }
    }
}
