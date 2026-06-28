// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SCH.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SCH.Models.Students.ClientDtos;
    using SCH.Models.StudentCourseMap.ClientDtos;
    using SCH.Services.Students;
    using SCH.Shared.Exceptions;
    using System;
    using SCH.Models.Common.GridEntities;
    using SCH.Models.Auth.Constants;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsService studentsService;

        public StudentsController(IStudentsService studentsService) 
        {
            this.studentsService = studentsService;     
        }

        // GET: api/students/grid
        [HttpGet("grid")]
        [Authorize(Policy = Policy.ViewStudents)]
        public async Task<IActionResult> GetStudentGridAsync(
            [FromQuery] StudentGridRequest request)
        {
            PagedResult<StudentDto> result = await studentsService
                .GetStudentGridAsync(request);
            return Ok(result);
        }

        // GET: api/<StudentsController>
        [HttpGet]
        [Authorize(Policy = Policy.ViewStudents)]
        public async Task<IActionResult> GetStudentAsync(
            bool? isActive = null)
        {
            List<StudentDto> students = await studentsService
                .GetStudentsAsync(isActive);

            return Ok(students);
        }

        // GET api/<StudentsController>/5
        [HttpGet("{id}")]
        [Authorize(Policy = Policy.ViewStudents)]
        public async Task<IActionResult> GetStudentAsync(int id)
        {
            IActionResult actionResult;
            if (id < 1)
            {
                throw SCHDomainException.BadRequest("Id should grater than 0");
            }

            StudentDto? student = await studentsService
                .GetStudentAsync(id);

            if (student is not null)
            {
                actionResult = Ok(student);
            }
            else
            {
                actionResult = NotFound();
            }

            return actionResult;
        }

        // POST api/<StudentsController>
        [HttpPost]
        [Authorize(Policy = Policy.AddStudents)]
        public async Task<IActionResult> PostStudentAsync([FromBody] StudentDto student)
        {
            ValidateCourses(student);

            int id = await studentsService
                .InsertStudentAsync(student);

            return Ok(id);
        }

        // PATCH api/<StudentsController>/5
        [HttpPatch("{id}")]
        [Authorize(Policy = Policy.EditStudents)]
        public async Task<IActionResult> PatchStudentAsync(int id, [FromBody] StudentDto student)
        {
            if (id < 1)
            {
                throw SCHDomainException.BadRequest("Id should grater than 0");
            }

            ValidateCourses(student);

            student.Id = id;
            await studentsService
                .UpdateStudentAsync(student);

            return Ok();
        }

        // DELETE api/<StudentsController>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = Policy.DeleteStudents)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1)
            {
                throw SCHDomainException.BadRequest("Id should grater than 0");
            }

            await studentsService
                .DeleteStudentAsync(id);

            return Ok();
        }

        [HttpGet("{id}/courses")]
        [Authorize(Policy = Policy.ViewStudents)]
        public async Task<IActionResult> GetCoursesAsync(int id)
        {
            if (id < 1)
            {
                throw SCHDomainException.BadRequest("Id should grater than 0");
            }

            List<StudentCourseMapDto> courses = await studentsService
                .GetCoursesAsync(id);

            return Ok(courses);
        }

        [HttpPut("{id}/courses/{courseId}")]
        [Authorize(Policy = Policy.EditStudents)]
        public async Task<IActionResult> PutCourseAsync(
            int id, 
            int courseId, 
            [FromBody] StudentCourseMapDto studentCourseMap)
        {
            if (id < 1)
            {
                throw SCHDomainException.BadRequest("Id should grater than 0");
            }

            if (courseId < 1)
            {
                throw SCHDomainException.BadRequest("Course Id should grater than 0");
            }

            if (studentCourseMap == null) 
            {
                throw SCHDomainException.BadRequest("Student course map is not set");
            }

            studentCourseMap.CourseId = courseId;
            studentCourseMap.StudentId = id;

            await studentsService.InsertCourseAsync(studentCourseMap);

            return Ok();
        }

        [HttpDelete("{id}/courses/{courseId}")]
        [Authorize(Policy = Policy.EditStudents)]
        public async Task<IActionResult> DeleteCourseAsync(int id, int courseId)
        {
            if (id < 1)
            {
                throw SCHDomainException.BadRequest("Id should grater than 0");
            }

            if (courseId < 1)
            {
                throw SCHDomainException.BadRequest("Course Id should grater than 0");
            }

            await studentsService.DeleteCourseAsync(id, courseId);

            return Ok();
        }

        private void ValidateCourses(StudentDto student)
        {
            if (student.Courses.Count > 0
                && student.Courses
                .GroupBy(c => c.CourseId).Any(g => g.Count() > 1))
            {
                throw SCHDomainException
                    .BadRequest("Duplicate courses are not allowed.");
            }
        }
    }
}
