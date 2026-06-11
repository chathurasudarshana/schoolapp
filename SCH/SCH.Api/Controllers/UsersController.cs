namespace SCH.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using SCH.Models.Auth.Entities;
    using SCH.Models.Users.ClientDtos;
    using SCH.Repositories.Students;
    using SCH.Repositories.Teachers;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentsRepository _studentsRepository;
        private readonly ITeachersRepository _teachersRepository;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            IStudentsRepository studentsRepository,
            ITeachersRepository teachersRepository)
        {
            _userManager = userManager;
            _studentsRepository = studentsRepository;
            _teachersRepository = teachersRepository;
        }

        /// <summary>
        /// Returns Basic-role users not already linked to a record of the given entity type.
        /// Used to populate the UserId dropdown in Student/Teacher detail pages.
        /// </summary>
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableUsersAsync([FromQuery] string entityType)
        {
            IActionResult response;
            if (entityType == "Student" || entityType == "Teacher")
            {
                // Collect already-linked user IDs for the entity type
                IEnumerable<int> linkedUserIds;
                if (entityType == "Student")
                {
                    var students = await _studentsRepository.GetStudentsAsync(null);
                    linkedUserIds = students
                        .Where(s => s.UserId.HasValue)
                        .Select(s => s.UserId!.Value);
                }
                else
                {
                    var teachers = await _teachersRepository.GetTeachersAsync();
                    linkedUserIds = teachers
                        .Where(t => t.UserId.HasValue)
                        .Select(t => t.UserId!.Value);
                }

                var linkedSet = linkedUserIds.ToHashSet();

                // Get all users in the Basic role
                var basicUsers = await _userManager.GetUsersInRoleAsync("Basic");

                // Filter out: users who already have Admin/Teacher/Student role, or are linked to another record
                var result = new List<UserDomainDto>();
                foreach (var user in basicUsers)
                {
                    if (linkedSet.Contains(user.Id))
                        continue;

                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Any(r => r == "Admin" || r == "Teacher" || r == "Student"))
                        continue;

                    result.Add(new UserDomainDto
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        FullName = $"{user.FirstName} {user.LastName}".Trim()
                    });
                }

                response = Ok(result);
            }
            else
            {
                response = BadRequest("entityType must be 'Student' or 'Teacher'.");
            }

            return response;
        }
    }
}
