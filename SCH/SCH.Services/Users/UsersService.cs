using Microsoft.AspNetCore.Identity;
using SCH.Models.Auth.Entities;
using SCH.Models.Users.ClientDtos;
using SCH.Repositories.Users;

namespace SCH.Services.Users
{


    internal class UsersService: IUsersService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;

        public UsersService(
            UserManager<ApplicationUser> userManager,
            IUserRepository userRepository) 
        { 
            this._userManager = userManager;
            this._userRepository = userRepository;
        }

        public async Task<List<UserDomainDto>> GetBasicOnlyUsersAsync()
        {
            // Collect already-linked user IDs for the entity type
 
            List<int> userIds = await _userRepository
                .GetBasicOnlyUserIdsAsync();

            // Get all users in the Basic role
            IList<ApplicationUser> basicUsers = await _userManager.GetUsersInRoleAsync("Basic");

            // Filter out: users who already have Admin/Teacher/Student role, or are linked to another record
            List<UserDomainDto> result = new List<UserDomainDto>();

            foreach (ApplicationUser user in basicUsers)
            {
                if (!userIds.Contains(user.Id))
                {
                    continue;
                }

                IList<string> roles = await _userManager.GetRolesAsync(user);
                if (roles.Any(r => r == "Admin" || r == "Teacher" || r == "Student"))
                {
                    continue;
                }

                result.Add(new UserDomainDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = $"{user.FirstName} {user.LastName}".Trim()
                });
            }

            return result;
        }


    }
}
