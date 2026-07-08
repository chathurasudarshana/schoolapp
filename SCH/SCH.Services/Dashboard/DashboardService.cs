namespace SCH.Services.Dashboard
{
    using SCH.Models.Dashboard;
    using SCH.Repositories.Courses;

    internal class DashboardService : IDashboardService
    {
        private readonly ICoursesRepository coursesRepository;

        public DashboardService(ICoursesRepository coursesRepository)
        {
            this.coursesRepository = coursesRepository;
        }

        public async Task<List<CourseStudentCountDto>> GetCourseStudentCountAsync()
        {
            return await coursesRepository.GetCourseStudentCountAsync();
        }
    }
}
