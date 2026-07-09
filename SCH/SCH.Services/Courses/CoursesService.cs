namespace SCH.Services.Courses
{
    using AutoMapper;
    using SCH.Models.StudentCourseMap.Entities;
    using SCH.Models.Courses.ClientDtos;
    using SCH.Models.Courses.Entities;
    using SCH.Repositories.Courses;
    using SCH.Repositories.UnitOfWork;
    using SCH.Shared.Cache;
    using SCH.Shared.Exceptions;

    internal class CoursesService: ICoursesService
    {
        private readonly ISCHUnitOfWork unitOfWork;
        private readonly ICoursesRepository coursesRepository;
        private readonly IMapper mapper;
        private readonly ICacheService cacheService;
        private const string CoursesListCacheKey = "courses-list";


        public CoursesService(
            ISCHUnitOfWork unitOfWork,
            ICoursesRepository coursesRepository,
            IMapper mapper,
            ICacheService cacheService) 
        { 
            this.unitOfWork = unitOfWork;
            this.coursesRepository = coursesRepository;
            this.mapper = mapper;
            this.cacheService = cacheService;
        }

        public async Task<List<CourseDto>> GetCoursesAsync(bool useCache = false)
        {
            List<CourseDto>? coursesDto = null;
            if (useCache)
            {
                coursesDto = cacheService.Get<List<CourseDto>>(CoursesListCacheKey);

            }

            if (coursesDto is null)
            {
                List<Course> courses = await coursesRepository
                    .GetCoursesAsync();

                coursesDto = mapper.Map<List<CourseDto>>(courses);

                if (useCache)
                {
                    cacheService.Add(CoursesListCacheKey, coursesDto);
                }
            }
           
            return coursesDto;
        }

        public async Task<CourseDto?> GetCourseAsync(int id)
        {
            Course? course = await coursesRepository.GetCourseAsync(id);
            return course == null ? null : mapper.Map<CourseDto>(course);
        }


        public async Task<int> InsertCourseAsync(CourseDto course)
        {
            Course courseEntity = new Course
            {
                Id = 0,
                Name = course.Name,
                StudentCourseMaps = new List<StudentCourseMap>()
            };

            await coursesRepository.InsertCourseAsync(courseEntity);
            await unitOfWork.SaveChangesAsync();

            cacheService.Remove(CoursesListCacheKey);
            return courseEntity.Id;
        }

        public async Task UpdateCourseAsync(CourseDto course)
        {
            Course? courseEntity = await coursesRepository
                .GetCourseAsync(course.Id);

            if (courseEntity == null)
            {
                throw SCHDomainException.NotFound();
            }

            // Map DTO to entity
            courseEntity.Name = course.Name;
            // Include RowVersion from frontend for concurrency check
            courseEntity.RowVersion = course.RowVersion ?? courseEntity.RowVersion;

            // Repository handles concurrency check
            coursesRepository.UpdateAsync(courseEntity);
            await unitOfWork.SaveChangesAsync();
            cacheService.Remove(CoursesListCacheKey);
        }

        public async Task DeleteCourseAsync(int id)
        {
            await coursesRepository
                .DeleteCourseAsync(id);

            await unitOfWork.SaveChangesAsync();
            cacheService.Remove(CoursesListCacheKey);
        }
    }
}
