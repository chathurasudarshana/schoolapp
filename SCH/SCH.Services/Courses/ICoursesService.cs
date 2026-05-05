namespace SCH.Services.Courses
{
    using SCH.Models.Courses.ClientDtos;

    public interface ICoursesService: IService
    {
        Task<List<CourseDto>> GetCoursesAsync();

        Task<CourseDto?> GetCourseAsync(int id);

        Task<int> InsertCourseAsync(CourseDto course);

        Task UpdateCourseAsync(CourseDto course);

        Task DeleteCourseAsync(int id);
    }
}
