namespace SCH.Repositories.Auth
{
    using SCH.Models.Auth.Entities;

    /// <summary>
    /// Repository interface for RefreshToken entity
    /// </summary>
    public interface IRefreshTokenRepository : IRepository
    {
        /// <summary>
        /// Gets a refresh token by token string (includes User navigation property)
        /// </summary>
        Task<RefreshToken?> GetByTokenAsync(string token);

        /// <summary>
        /// Gets all active (non-revoked, non-expired) tokens for a user
        /// </summary>
        Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(int userId);

        /// <summary>
        /// Gets all non-revoked tokens for a user
        /// </summary>
        Task<List<RefreshToken>> GetNonRevokedTokensByUserIdAsync(int userId);

        /// <summary>
        /// Gets a specific token by ID for a user
        /// </summary>
        Task<RefreshToken?> GetByIdAndUserIdAsync(int tokenId, int userId);

        /// <summary>
        /// Inserts a new refresh token
        /// </summary>
        Task InsertAsync(RefreshToken token);

        /// <summary>
        /// Updates an existing refresh token
        /// </summary>
        void Update(RefreshToken token);

        /// <summary>
        /// Updates multiple refresh tokens
        /// </summary>
        void UpdateRange(IEnumerable<RefreshToken> tokens);

        /// <summary>
        /// Gets all non-revoked tokens by family ID
        /// </summary>
        Task<List<RefreshToken>> GetNonRevokedTokensByFamilyIdAsync(Guid familyId);
    }
}

