namespace SCH.Repositories.Users
{
    using Microsoft.EntityFrameworkCore;
    using SCH.Models.Users.Entities;
    using SCH.Repositories.DbContexts;

    /// <summary>
    /// Repository implementation for User entity (domain user table)
    /// </summary>
    internal class UserRepository : IUserRepository
    {
        private readonly SCHContext _context;

        public UserRepository(SCHContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task InsertAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public async Task DeleteAsync(int id)
        {
            User? user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }

        public async Task<List<int>> GetBasicOnlyUserIdsAsync()
        {
            IQueryable<int> linkedUserIds = _context.Student
                .Where(s => s.UserId != null)
                .Select(s => s.UserId!.Value)
                .Union(
                    _context.Teacher
                        .Where(t => t.UserId != null)
                        .Select(t => t.UserId!.Value)
                );

            List<int> userIds = await _context.Users
                .AsNoTracking()
                .Select(u => u.Id)
                .Except(linkedUserIds)
                .ToListAsync();
            return userIds;
        }

    }
}

