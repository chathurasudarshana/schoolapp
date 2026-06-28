// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SCH.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SCH.API.Authorization;
    using SCH.Models.Auth.Constants;
    using SCH.Models.Teachers.ClientDtos;
    using SCH.Services.Teachers;
    using SCH.Shared.Exceptions;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeachersController : ControllerBase
    {
        private readonly ITeachersService teachersService;

        public TeachersController(ITeachersService teachersService)
        {
            this.teachersService = teachersService;
        }

        // GET: api/<TeachersController>
        [HttpGet]
        [Authorize(Policy = Policy.ViewTeachers)]
        public async Task<IActionResult> GetTeacherAsync()
        {
            List<TeacherDto> teachers = await teachersService
                .GetTeachersAsync();

            return Ok(teachers);
        }

        // GET api/<TeachersController>/5
        [HttpGet("{id}")]
        [Authorize(Policy = Policy.ViewTeachers)]
        public async Task<IActionResult> GetTeacherAsync(int id)
        {
            IActionResult actionResult;
            if (id < 1)
            {
                throw SCHDomainException.BadRequest("Id should be greater than 0");
            }

            TeacherDto? teacher = await teachersService
                .GetTeacherAsync(id);

            if (teacher is not null)
            {
                actionResult = Ok(teacher);
            }
            else
            {
                actionResult = NotFound();
            }

            return actionResult;
        }

        // POST api/<TeachersController>
        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> PostTeacherAsync([FromBody] TeacherDto teacher)
        {
            int id = await teachersService
                .InsertTeacherAsync(teacher);

            return Ok(id);
        }

        // PATCH api/<TeachersController>/5
        [HttpPatch("{id}")]
        [Authorize(Policy = Policy.EditTeachers)]
        public async Task<IActionResult> PatchTeacherAsync(int id, [FromBody] TeacherDto teacher)
        {
            if (id < 1)
            {
                throw SCHDomainException.BadRequest("Id should be greater than 0");
            }

            teacher.Id = id;
            await teachersService
                .UpdateTeacherAsync(teacher);

            return Ok();
        }

        // DELETE api/<TeachersController>/5
        [HttpDelete("{id}")]
        [Authorize(Policy = Policy.DeleteTeachers)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1)
            {
                throw SCHDomainException.BadRequest("Id should be greater than 0");
            }

            await teachersService
                .DeleteTeacherAsync(id);

            return Ok();
        }
    }
}
