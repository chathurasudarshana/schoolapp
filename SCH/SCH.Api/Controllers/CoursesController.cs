// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SCH.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SCH.Models.Courses.ClientDtos;
    using SCH.Services.Courses;
    using SCH.Shared.Exceptions;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all endpoints
    public class CoursesController : ControllerBase
    {
        private readonly ICoursesService coursesService;

        public CoursesController(ICoursesService coursesService)
        {
            this.coursesService = coursesService;
        }

        // GET: api/<CoursesController>
        [HttpGet]
        public async Task<IActionResult> GetCourseAsync()
        {
            List<CourseDto> courses = await coursesService
                .GetCoursesAsync();

            return Ok(courses);
        }

        // GET api/<CoursesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourseAsync(int id)
        {
            IActionResult actionResult;
            if (id < 1)
            {
                throw SCHDomainException.BadRequest("Id should grater than 0");
            }

            CourseDto? course = await coursesService
                .GetCourseAsync(id);

            if (course is not null)
            {
                actionResult = Ok(course);
            }
            else
            {
                actionResult = NotFound();
            }

            return actionResult;
        }

        // POST api/<CoursesController>
        [HttpPost]
        public async Task<IActionResult> PostCourseAsync([FromBody] CourseDto course)
        {
            int id = await coursesService
                .InsertCourseAsync(course);

            return Ok(id);
        }

        // PUT api/<CoursesController>/5
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchCourseAsync(int id, [FromBody] CourseDto course)
        {
            if (id < 1)
            {
                throw SCHDomainException.BadRequest("Id should grater than 0");
            }

            course.Id = id;
            await coursesService
                .UpdateCourseAsync(course);

            return Ok();
        }

        // DELETE api/<CoursesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1)
            {
                throw SCHDomainException.BadRequest("Id should grater than 0");
            }

            await coursesService
                .DeleteCourseAsync(id);

            return Ok();
        }
    }
}
