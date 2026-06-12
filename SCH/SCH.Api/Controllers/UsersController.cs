namespace SCH.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SCH.Models.Auth.Constants;
    using SCH.Models.Users.ClientDtos;
    using SCH.Services.Users;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Role.Admin)]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(
                IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Returns Basic-role users not already linked to a record of the given entity type.
        /// Used to populate the UserId dropdown in Student/Teacher detail pages.
        /// </summary>
        [HttpGet("basic-only")]
        public async Task<IActionResult> GetBasicOnlyUsersAsync()
        {
            List<UserDomainDto> result = await _usersService.GetBasicOnlyUsersAsync();
            return Ok(result);
        }
    }
}
