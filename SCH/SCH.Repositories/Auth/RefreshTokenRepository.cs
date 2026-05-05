namespace SCH.Repositories.Auth
{
    using Microsoft.EntityFrameworkCore;
    using SCH.Models.Auth.Entities;
    using SCH.Repositories.DbContexts;

    /// <summary>
    /// Repository implementation for RefreshToken entity
    /// </summary>
    internal class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IdentityContext _context;

        public RefreshTokenRepository(IdentityContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(int userId)
        {
            return await _context.RefreshTokens
                .Where(rt => rt.UserId == userId
                    && !rt.IsRevoked
                    && rt.ExpiryDate > DateTime.UtcNow)
                .OrderByDescending(rt => rt.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<RefreshToken>> GetNonRevokedTokensByUserIdAsync(int userId)
        {
            return await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();
        }

        public async Task<RefreshToken?> GetByIdAndUserIdAsync(int tokenId, int userId)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Id == tokenId && rt.UserId == userId);
        }

        public async Task InsertAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
        }

        public void Update(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
        }

        public void UpdateRange(IEnumerable<RefreshToken> tokens)
        {
            _context.RefreshTokens.UpdateRange(tokens);
        }

        public async Task<List<RefreshToken>> GetNonRevokedTokensByFamilyIdAsync(Guid familyId)
        {
            return await _context.RefreshTokens
                .Where(rt => rt.FamilyId == familyId && !rt.IsRevoked)
                .ToListAsync();
        }
    }
}

