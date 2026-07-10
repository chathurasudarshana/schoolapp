namespace SCH.Tests.Courses
{
    using AutoMapper;
    using Moq;
    using SCH.Models.Courses.ClientDtos;
    using SCH.Models.Courses.Entities;
    using SCH.Models.StudentCourseMap.Entities;
    using SCH.Repositories.Courses;
    using SCH.Repositories.UnitOfWork;
    using SCH.Services.Courses;
    using SCH.Shared.Cache;
    using SCH.Shared.Exceptions;
    using Xunit;

    public class CoursesServiceTests
    {
        private readonly Mock<ISCHUnitOfWork> _unitOfWork = new();
        private readonly Mock<ICoursesRepository> _coursesRepository = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<ICacheService> _cacheService = new();
        private readonly CoursesService _sut;

        private const string CacheKey = "courses-list";

        public CoursesServiceTests()
        {
            _sut = new CoursesService(
                _unitOfWork.Object,
                _coursesRepository.Object,
                _mapper.Object,
                _cacheService.Object);
        }

        [Fact]
        public async Task GetCoursesAsync_WithoutCache_FetchesFromRepository()
        {
            var courses = new List<Course> { new() { Id = 1, Name = "Math", StudentCourseMaps = new List<StudentCourseMap>() } };
            var dtos = new List<CourseDto> { new() { Id = 1, Name = "Math" } };
            _coursesRepository.Setup(r => r.GetCoursesAsync()).ReturnsAsync(courses);
            _mapper.Setup(m => m.Map<List<CourseDto>>(courses)).Returns(dtos);

            var result = await _sut.GetCoursesAsync(useCache: false);

            Assert.Equal(dtos, result);
            _cacheService.Verify(c => c.Get<List<CourseDto>>(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetCoursesAsync_WithCache_WhenCacheHit_SkipsRepository()
        {
            var cached = new List<CourseDto> { new() { Id = 1, Name = "Math" } };
            _cacheService.Setup(c => c.Get<List<CourseDto>>(CacheKey)).Returns(cached);

            var result = await _sut.GetCoursesAsync(useCache: true);

            Assert.Equal(cached, result);
            _coursesRepository.Verify(r => r.GetCoursesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetCoursesAsync_WithCache_WhenCacheMiss_FetchesThenCaches()
        {
            var courses = new List<Course> { new() { Id = 1, Name = "Math", StudentCourseMaps = new List<StudentCourseMap>() } };
            var dtos = new List<CourseDto> { new() { Id = 1, Name = "Math" } };
            _cacheService.Setup(c => c.Get<List<CourseDto>>(CacheKey)).Returns((List<CourseDto>?)null);
            _coursesRepository.Setup(r => r.GetCoursesAsync()).ReturnsAsync(courses);
            _mapper.Setup(m => m.Map<List<CourseDto>>(courses)).Returns(dtos);

            var result = await _sut.GetCoursesAsync(useCache: true);

            Assert.Equal(dtos, result);
            _cacheService.Verify(
                c => c.Add(CacheKey, dtos, It.IsAny<TimeSpan?>(), It.IsAny<TimeSpan?>(), It.IsAny<DateTimeOffset?>()),
                Times.Once);
        }

        [Fact]
        public async Task GetCourseAsync_WhenFound_ReturnsCourseDto()
        {
            var course = new Course { Id = 1, Name = "Math", StudentCourseMaps = new List<StudentCourseMap>() };
            var dto = new CourseDto { Id = 1, Name = "Math" };
            _coursesRepository.Setup(r => r.GetCourseAsync(1)).ReturnsAsync(course);
            _mapper.Setup(m => m.Map<CourseDto>(course)).Returns(dto);

            var result = await _sut.GetCourseAsync(1);

            Assert.Equal(dto, result);
        }

        [Fact]
        public async Task GetCourseAsync_WhenNotFound_ReturnsNull()
        {
            _coursesRepository.Setup(r => r.GetCourseAsync(99)).ReturnsAsync((Course?)null);

            var result = await _sut.GetCourseAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task InsertCourseAsync_InsertsAndSavesAndInvalidatesCache()
        {
            var dto = new CourseDto { Name = "Science" };
            _coursesRepository.Setup(r => r.InsertCourseAsync(It.IsAny<Course>())).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _sut.InsertCourseAsync(dto);

            _coursesRepository.Verify(r => r.InsertCourseAsync(It.IsAny<Course>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            _cacheService.Verify(c => c.Remove(CacheKey), Times.Once);
        }

        [Fact]
        public async Task UpdateCourseAsync_WhenNotFound_ThrowsNotFound()
        {
            var dto = new CourseDto { Id = 99, Name = "Math" };
            _coursesRepository.Setup(r => r.GetCourseAsync(99)).ReturnsAsync((Course?)null);

            await Assert.ThrowsAsync<SCHDomainException>(() => _sut.UpdateCourseAsync(dto));
        }

        [Fact]
        public async Task UpdateCourseAsync_UpdatesAndSavesAndInvalidatesCache()
        {
            var entity = new Course { Id = 1, Name = "Old Name", StudentCourseMaps = new List<StudentCourseMap>() };
            var dto = new CourseDto { Id = 1, Name = "New Name" };
            _coursesRepository.Setup(r => r.GetCourseAsync(1)).ReturnsAsync(entity);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _sut.UpdateCourseAsync(dto);

            Assert.Equal("New Name", entity.Name);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            _cacheService.Verify(c => c.Remove(CacheKey), Times.Once);
        }

        [Fact]
        public async Task DeleteCourseAsync_DeletesAndSavesAndInvalidatesCache()
        {
            _coursesRepository.Setup(r => r.DeleteCourseAsync(1)).Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _sut.DeleteCourseAsync(1);

            _coursesRepository.Verify(r => r.DeleteCourseAsync(1), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
            _cacheService.Verify(c => c.Remove(CacheKey), Times.Once);
        }
    }
}
