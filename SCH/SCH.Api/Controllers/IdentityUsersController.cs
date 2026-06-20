namespace SCH.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SCH.Models.Auth.Constants;
    using SCH.Models.Users.ClientDtos;
    using SCH.Services.IdentityUsers;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Role.Admin)]
    public class IdentityUsersController : ControllerBase
    {
        private readonly IIdentityUsersService _identityUsersService;

        public IdentityUsersController(
            IIdentityUsersService identityUsersService)
        {
            _identityUsersService = identityUsersService;
        }

        /// <summary>
        /// Returns Basic-role users not already linked to a record of the given entity type.
        /// Used to populate the UserId dropdown in Student/Teacher detail pages.
        /// </summary>
        [HttpGet("basic-only")]
        public async Task<IActionResult> GetBasicOnlyUsersAsync()
        {
            List<UserDomainDto> result = await _identityUsersService
                .GetBasicOnlyUsersAsync();
            return Ok(result);
        }
    }
}
