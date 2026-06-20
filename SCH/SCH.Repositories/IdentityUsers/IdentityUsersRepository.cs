namespace SCH.Repositories.IdentityUsers
{
    using Microsoft.EntityFrameworkCore;
    using SCH.Models.Auth.Constants;
    using SCH.Models.Auth.Entities;
    using SCH.Repositories.DbContexts;

    /// <summary>
    /// Repository implementation for IdentityUsers entity
    /// </summary>
    internal class IdentityUsersRepository : IIdentityUsersRepository
    {
        private readonly IdentityContext _context;

        public IdentityUsersRepository(IdentityContext context)
        {
            _context = context;
        }

        public async Task<List<ApplicationUser>> GetOnlyBasicUsersAsync()
        {
            int basicRoleId = await _context.Roles
                .Where(r => r.Name == Role.Basic)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            List<ApplicationUser> users = await _context.Users
                .Where(u =>
                    _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == basicRoleId)
                    && !_context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId != basicRoleId))
                .AsNoTracking()
                .ToListAsync();
            return users;
        }

    }
}

