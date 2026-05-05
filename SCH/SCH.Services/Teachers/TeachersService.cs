namespace SCH.Services.Teachers
{
    using AutoMapper;
    using SCH.Models.Teachers.ClientDtos;
    using SCH.Models.Teachers.Entities;
    using SCH.Repositories.Teachers;
    using SCH.Repositories.UnitOfWork;
    using SCH.Shared.Exceptions;

    internal class TeachersService: ITeachersService
    {
        private readonly ISCHUnitOfWork unitOfWork;
        private readonly ITeachersRepository teachersRepository;
        private readonly IMapper mapper;

        public TeachersService(
            ISCHUnitOfWork unitOfWork,
            ITeachersRepository teachersRepository,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.teachersRepository = teachersRepository;
            this.mapper = mapper;
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
                Name = teacher.Name
            };

            await teachersRepository.InsertTeacherAsync(teacherEntity);
            await unitOfWork.SaveChangesAsync();

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

            // Map DTO to entity
            teacherEntity.Name = teacher.Name;

            // Include RowVersion from frontend for concurrency check
            teacherEntity.RowVersion = teacher.RowVersion ?? teacherEntity.RowVersion;

            // Repository handles concurrency check
            teachersRepository.UpdateAsync(teacherEntity);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteTeacherAsync(int id)
        {
            await teachersRepository
                .DeleteTeacherAsync(id);

            await unitOfWork.SaveChangesAsync();
        }
    }
}


