namespace SCH.Services.Dashboard
{
    using SCH.Models.Dashboard;

    public interface IDashboardService: IService
    {
        Task<List<CourseStudentCountDto>> GetCourseStudentCountAsync();
    }
}
