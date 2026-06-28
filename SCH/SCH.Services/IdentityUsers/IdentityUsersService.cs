namespace SCH.Services.IdentityUsers
{
    using SCH.Models.Auth.Entities;
    using SCH.Models.Users.ClientDtos;
    using SCH.Repositories.IdentityUsers;


    internal class IdentityUsersService: IIdentityUsersService
    {
        private readonly IIdentityUsersRepository _identityUsersRepository;

        public IdentityUsersService(
            IIdentityUsersRepository identityUsersRepository) 
        { 
            this._identityUsersRepository = identityUsersRepository;
        }

        public async Task<List<UserDomainDto>> GetBasicOnlyUsersAsync()
        {
            // Get identity users that have only the Basic role (no Admin/Teacher/Student)
            List<ApplicationUser> basicOnlyUsers = await _identityUsersRepository.GetOnlyBasicUsersAsync();

            // Return only users present in both sets
            List<UserDomainDto> users = basicOnlyUsers
                .Select(u => new UserDomainDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    FullName = $"{u.FirstName} {u.LastName}".Trim()
                })
                .ToList();
            return users;
        }


    }
}
