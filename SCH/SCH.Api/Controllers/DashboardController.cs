namespace SCH.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SCH.Models.Dashboard;
    using SCH.Services.Dashboard;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            this.dashboardService = dashboardService;
        }

        // GET: api/dashboard/course-student-count
        [HttpGet("course-student-count")]
        public async Task<IActionResult> GetCourseStudentCountAsync()
        {
            List<CourseStudentCountDto> result = await dashboardService
                .GetCourseStudentCountAsync();

            return Ok(result);
        }
    }
}
