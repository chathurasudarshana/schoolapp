namespace SCH.Models.Auth.Entities
{
    using SCH.Models.Auth.Enums;
    using SCH.Models.Common.ConcurrencyEntities;

    /// <summary>
    /// Represents a refresh token for JWT authentication
    /// </summary>
    public class RefreshToken : IConcurrencyEntity
    {
        /// <summary>
        /// Unique identifier for the refresh token
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User ID who owns this token
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Family ID - groups related tokens together (for multi-device support)
        /// All tokens spawned from the same login share the same FamilyId
        /// </summary>
        public Guid FamilyId { get; set; }

        /// <summary>
        /// Parent token ID - tracks which token spawned this one
        /// NULL for root tokens (created by username/password login)
        /// </summary>
        public int? ParentTokenId { get; set; }

        /// <summary>
        /// Indicates how this token was generated
        /// </summary>
        public RefreshTokenGeneratedBy GeneratedBy { get; set; }

        /// <summary>
        /// The actual refresh token value
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// JWT ID (jti claim) - links refresh token to access token
        /// </summary>
        public string JwtId { get; set; } = string.Empty;

        /// <summary>
        /// IP address from where the token was created
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// User agent (browser/device info) from where the token was created
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// Friendly device name (e.g., "Chrome on Windows", "iPhone 13")
        /// </summary>
        public string? DeviceName { get; set; }

        /// <summary>
        /// Indicates if the token has been used
        /// </summary>
        public bool IsUsed { get; set; } = false;

        /// <summary>
        /// Indicates if the token has been revoked
        /// </summary>
        public bool IsRevoked { get; set; } = false;

        /// <summary>
        /// Date when the token was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the token expires
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Date when the token was used (if IsUsed = true)
        /// </summary>
        public DateTime? UsedDate { get; set; }

        /// <summary>
        /// Date when the token was revoked (if IsRevoked = true)
        /// </summary>
        public DateTime? RevokedDate { get; set; }

        /// <summary>
        /// Navigation property to the user
        /// </summary>
        public virtual ApplicationUser User { get; set; } = null!;

        /// <summary>
        /// Checks if the token is valid (not expired, not used, not revoked)
        /// </summary>
        public bool IsValid => !IsUsed && !IsRevoked && ExpiryDate > DateTime.UtcNow;

        // Concurrency control
        /// <summary>
        /// Row version for optimistic concurrency control
        /// </summary>
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}

