namespace SCH.Services.Auth
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Implementation of token service for JWT generation and validation
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _accessTokenExpirationMinutes;

        public TokenService(IConfiguration configuration)
        {
            _secretKey = configuration["JwtSettings:SecretKey"] 
                ?? throw new InvalidOperationException("JWT SecretKey is not configured");
            _issuer = configuration["JwtSettings:Issuer"] 
                ?? throw new InvalidOperationException("JWT Issuer is not configured");
            _audience = configuration["JwtSettings:Audience"] 
                ?? throw new InvalidOperationException("JWT Audience is not configured");
            _accessTokenExpirationMinutes = int.Parse(
                configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "30");
        }

        /// <summary>
        /// Generates a JWT access token with user claims
        /// </summary>
        public async Task<(string Token, string TokenId)> GenerateAccessTokenAsync(
            int userId,
            string username,
            string email,
            IList<string> roles,
            IList<string>? permissions = null,
            int? ownStudentId = null,
            int? ownTeacherId = null)
        {
            string tokenId = Guid.NewGuid().ToString();
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, tokenId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email)
            };

            // Add roles as claims
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add deduplicated permission claims
            if (permissions != null)
            {
                foreach (string permission in permissions.Distinct())
                {
                    claims.Add(new Claim("permission", permission));
                }
            }

            // Add own-record claims so frontend/backend can enforce record-level access
            if (ownStudentId.HasValue)
            {
                claims.Add(new Claim("own_student_id", ownStudentId.Value.ToString()));
            }


            if (ownTeacherId.HasValue)
            {
                claims.Add(new Claim("own_teacher_id", ownTeacherId.Value.ToString()));
            }


            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
                signingCredentials: credentials
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return await Task.FromResult((tokenString, tokenId));
        }

        /// <summary>
        /// Generates a cryptographically secure random refresh token
        /// </summary>
        public string GenerateRefreshToken()
        {
            byte[] randomBytes = new byte[64];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        /// <summary>
        /// Extracts claims from an expired token (used during token refresh)
        /// </summary>
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = false, // Don't validate expiration for refresh
                    ClockSkew = TimeSpan.Zero
                };

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Validates token structure without checking expiration
        /// </summary>
        public bool ValidateTokenStructure(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                if (!tokenHandler.CanReadToken(token))
                    return false;

                JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);
                return jwtToken != null;
            }
            catch
            {
                return false;
            }
        }
    }
}

