namespace SCH.Services.Teachers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using SCH.Models.Auth.Constants;
    using SCH.Models.Auth.Entities;
    using SCH.Models.Teachers.ClientDtos;
    using SCH.Models.Teachers.Entities;
    using SCH.Repositories.Teachers;
    using SCH.Repositories.UnitOfWork;
    using SCH.Services.Auth;
    using SCH.Shared.Exceptions;

    internal class TeachersService: ITeachersService
    {
        private readonly ISCHUnitOfWork unitOfWork;
        private readonly ITeachersRepository teachersRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAuthService authService;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TeachersService(
            ISCHUnitOfWork unitOfWork,
            ITeachersRepository teachersRepository,
            UserManager<ApplicationUser> userManager,
            IAuthService authService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            this.unitOfWork = unitOfWork;
            this.teachersRepository = teachersRepository;
            this.userManager = userManager;
            this.authService = authService;
            this.mapper = mapper;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<TeacherDto>> GetTeachersAsync()
        {
            List<Teacher> teachers = await teachersRepository
                .GetTeachersAsync();

            return mapper.Map<List<TeacherDto>>(teachers);
        }

        public async Task<TeacherDto?> GetTeacherAsync(int id)
        {
            Teacher? teacher = await teachersRepository.GetTeacherAsync(id);
            return teacher == null ? null : mapper.Map<TeacherDto>(teacher);
        }

        public async Task<int> InsertTeacherAsync(TeacherDto teacher)
        {
            Teacher teacherEntity = new Teacher
            {
                Id = 0,
                Name = teacher.Name,
                UserId = teacher.UserId
            };

            await teachersRepository.InsertTeacherAsync(teacherEntity);
            await unitOfWork.SaveChangesAsync();

            // If a UserId is linked, assign the Teacher role
            if (teacher.UserId.HasValue)
                await AssignTeacherRoleAsync(teacher.UserId.Value);

            return teacherEntity.Id;
        }

        public async Task UpdateTeacherAsync(TeacherDto teacher)
        {
            Teacher? teacherEntity = await teachersRepository
                .GetTeacherAsync(teacher.Id);

            if (teacherEntity == null)
            {
                throw SCHDomainException.NotFound();
            }

            bool isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole(Role.Admin) == true;
            int? oldUserId = teacherEntity.UserId;
            int? newUserId = teacher.UserId;

            // Map DTO to entity
            teacherEntity.Name = teacher.Name;

            if (isAdmin) 
            {
                teacherEntity.UserId = newUserId;
                teacherEntity.User = null;
            }

            // Include RowVersion from frontend for concurrency check
            teacherEntity.RowVersion = teacher.RowVersion ?? teacherEntity.RowVersion;

            // Repository handles concurrency check
            teachersRepository.UpdateAsync(teacherEntity);
            await unitOfWork.SaveChangesAsync();

            // Handle UserId change: manage roles and revoke stale sessions
            if (isAdmin && oldUserId != newUserId)
            {
                if (oldUserId.HasValue)
                {
                    await RemoveTeacherRoleAsync(oldUserId.Value);
                    await authService.RevokeAllUserSessionsAsync(oldUserId.Value);
                }
                if (newUserId.HasValue)
                    await AssignTeacherRoleAsync(newUserId.Value);
            }
        }

        public async Task DeleteTeacherAsync(int id)
        {
            await teachersRepository
                .DeleteTeacherAsync(id);

            await unitOfWork.SaveChangesAsync();
        }

        private async Task AssignTeacherRoleAsync(int userId)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString());
            if (user != null && !await userManager.IsInRoleAsync(user, Role.Teacher))
            {
                await userManager.AddToRoleAsync(user, Role.Teacher);
            }
        }

        private async Task RemoveTeacherRoleAsync(int userId)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userId.ToString());
            if (user != null && await userManager.IsInRoleAsync(user, Role.Teacher))
            {
                await userManager.RemoveFromRoleAsync(user, Role.Teacher);
            }

        }
    }
}


