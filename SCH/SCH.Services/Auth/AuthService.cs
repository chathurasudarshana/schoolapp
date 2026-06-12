namespace SCH.Services.Auth
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using SCH.Models.Auth.ClientDtos;
    using SCH.Models.Auth.Constants;
    using SCH.Models.Auth.Entities;
    using SCH.Models.Auth.Enums;
    using SCH.Models.Students.Entities;
    using SCH.Models.Teachers.Entities;
    using SCH.Models.Users.Entities;
    using SCH.Repositories.Auth;
    using SCH.Repositories.Students;
    using SCH.Repositories.Teachers;
    using SCH.Repositories.UnitOfWork;
    using SCH.Repositories.Users;
    using SCH.Shared.Exceptions;
    using SCH.Shared.Logger;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;

    /// <summary>
    /// Implementation of authentication service
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStudentsRepository _studentsRepository;
        private readonly ITeachersRepository _teachersRepository;
        private readonly IIdentityUnitOfWork _identityUnitOfWork;
        private readonly ISCHUnitOfWork _schUnitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IAuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository,
            IStudentsRepository studentsRepository,
            ITeachersRepository teachersRepository,
            IIdentityUnitOfWork identityUnitOfWork,
            ISCHUnitOfWork schUnitOfWork,
            IConfiguration configuration,
            ILogger<IAuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _studentsRepository = studentsRepository;
            _teachersRepository = teachersRepository;
            _identityUnitOfWork = identityUnitOfWork;
            _schUnitOfWork = schUnitOfWork;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates user and generates JWT tokens
        /// </summary>
        public async Task<LoginResponseDto> LoginAsync(
            LoginRequestDto request, 
            string? ipAddress, 
            string? userAgent)
        {
            _logger.Info($"Login attempt for user: {request.Username}");

            // Find user by username or email
            ApplicationUser? user = await _userManager.FindByNameAsync(request.Username) 
                ?? await _userManager.FindByEmailAsync(request.Username);

            if (user == null)
            {
                _logger.Warn($"Failed login attempt: User not found - {request.Username}");
                throw SCHDomainException.Unauthorized("Invalid username or password");
            }

            if (!user.IsActive)
            {
                _logger.Warn($"Failed login attempt: User inactive - {request.Username}");
                throw SCHDomainException.Unauthorized("Account is deactivated");
            }

            // Check password
            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                _logger.Warn($"Account locked: {request.Username}");
                throw SCHDomainException.Unauthorized("Account is locked due to multiple failed login attempts");
            }

            if (!result.Succeeded)
            {
                _logger.Warn($"Failed login attempt: Invalid password - {request.Username}");
                throw SCHDomainException.Unauthorized("Invalid username or password");
            }

            // Update last login date
            user.LastLoginDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Generate tokens
            IList<string> roles = await _userManager.GetRolesAsync(user);
            (List<string> permissions, int? ownStudentId, int? ownTeacherId) = await BuildUserContextAsync(user.Id, roles);
            (string accessToken, string tokenId) = await _tokenService.GenerateAccessTokenAsync(
                user.Id, user.UserName!, user.Email!, roles, permissions, ownStudentId, ownTeacherId);
            string refreshToken = _tokenService.GenerateRefreshToken();

            // Store refresh token
            int refreshTokenExpirationDays = int.Parse(
                _configuration["JwtSettings:RefreshTokenExpirationDays"] ?? "7");

            // Revoke any existing non-revoked tokens for same user+IP+UserAgent (only 1 session per browser)
            List<RefreshToken> existingTokens = await _refreshTokenRepository
                .GetNonRevokedTokensByUserIdAsync(user.Id);

            List<RefreshToken> tokensToRevoke = existingTokens
                .Where(t => t.IpAddress == ipAddress && t.UserAgent == userAgent)
                .ToList();

            foreach (RefreshToken token in tokensToRevoke)
            {
                token.IsRevoked = true;
                token.RevokedDate = DateTime.UtcNow;
            }

            if (tokensToRevoke.Any())
            {
                _logger.Info($"Revoked {tokensToRevoke.Count} existing tokens for user {user.Id} from same browser");
            }

            RefreshToken refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                FamilyId = Guid.NewGuid(), // New family for this login session
                ParentTokenId = null, // Root token (no parent)
                GeneratedBy = RefreshTokenGeneratedBy.UsernamePassword,
                Token = refreshToken,
                JwtId = tokenId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                DeviceName = ParseDeviceName(userAgent),
                ExpiryDate = DateTime.UtcNow.AddDays(refreshTokenExpirationDays)
            };

            await _refreshTokenRepository.InsertAsync(refreshTokenEntity);
            await _identityUnitOfWork.SaveChangesAsync();

            _logger.Info($"User logged in successfully: {user.UserName} from IP: {ipAddress}");

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "30") * 60,
                RefreshTokenExpiresIn = refreshTokenExpirationDays * 24 * 60 * 60,
                TokenType = "Bearer",
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName!,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    Roles = roles.ToList(),
                    Permissions = permissions,
                    OwnStudentId = ownStudentId,
                    OwnTeacherId = ownTeacherId,
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate,
                    LastLoginDate = user.LastLoginDate,
                    ConcurrencyStamp = user.ConcurrencyStamp
                }
            };
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            _logger.Info($"Registration attempt for username: {request.Username}");

            // Check if user exists
            ApplicationUser? existingUser = await _userManager.FindByNameAsync(request.Username);
            if (existingUser != null)
            {
                throw SCHDomainException.BadRequest("Username already exists");
            }

            existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw SCHDomainException.BadRequest("Email already exists");
            }

            // Create user
            ApplicationUser user = new ApplicationUser
            {
                UserName = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.Warn($"Registration failed for {request.Username}: {errors}");
                throw SCHDomainException.BadRequest($"Registration failed: {errors}");
            }

            // Assign default "Basic" role
            await EnsureRoleExistsAsync(Role.Basic);
            await _userManager.AddToRoleAsync(user, Role.Basic);

            // Create corresponding domain user
            // Wrap in try-catch to rollback ApplicationUser if domain user creation fails
            try
            {
                User domainUser = new User
                {
                    Id = user.Id,  // Use ApplicationUser.Id as primary key
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };

                await _userRepository.InsertAsync(domainUser);
                await _schUnitOfWork.SaveChangesAsync();

                _logger.Info($"User registered successfully: {user.UserName} (Identity ID: {user.Id}, Domain ID: {domainUser.Id})");
            }
            catch (Exception ex)
            {
                // Rollback: Delete the ApplicationUser that was just created
                _logger.Error($"Failed to create domain user for {user.UserName}. Rolling back ApplicationUser creation.", ex);
                
                try
                {
                    await _userManager.DeleteAsync(user);
                    _logger.Warn($"Successfully rolled back ApplicationUser creation for {user.UserName}");
                }
                catch (Exception rollbackEx)
                {
                    _logger.Error($"Critical: Failed to rollback ApplicationUser {user.UserName}. Manual cleanup required.", rollbackEx);
                }

                throw SCHApplicationException.InternalServerError($"Registration failed");
            }

            // Auto-login after registration
            return await LoginAsync(new LoginRequestDto
            {
                Username = request.Username,
                Password = request.Password
            }, null, null);
        }

        /// <summary>
        /// Refreshes access token using refresh token
        /// </summary>
        public async Task<LoginResponseDto> RefreshTokenAsync(
            RefreshTokenRequestDto request,
            string? ipAddress,
            string? userAgent)
        {
            _logger.Info("Token refresh attempt");

            // Validate access token structure
            ClaimsPrincipal? principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                throw SCHDomainException.Unauthorized("Invalid access token");
            }

            string? userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw SCHDomainException.Unauthorized("Invalid token claims");
            }

            int userId = int.Parse(userIdClaim);

            // Find refresh token in database
            RefreshToken? storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            if (storedToken == null)
            {
                _logger.Warn($"Refresh token not found for user: {userId}");
                throw SCHDomainException.Unauthorized("Invalid refresh token");
            }

            // Validate refresh token
            if (storedToken.UserId != userId)
            {
                _logger.Warn($"Token user mismatch: Expected {userId}, got {storedToken.UserId}");
                throw SCHDomainException.Unauthorized("Token user mismatch");
            }

            // CRITICAL SECURITY: Detect token reuse (potential theft)
            if (storedToken.IsUsed)
            {
                _logger.Warn($"SECURITY ALERT: Refresh token reuse detected for user {userId}, token {storedToken.Id}. Revoking entire token family {storedToken.FamilyId}.");
                
                // Revoke all tokens in this family (compromised session)
                await RevokeTokenFamilyAsync(storedToken.FamilyId);
                
                throw SCHDomainException.Unauthorized("Potential security breach detected. All sessions have been revoked. Please log in again.");
            }

            if (storedToken.IsRevoked)
            {
                _logger.Warn($"Refresh token revoked: {storedToken.Id}");
                throw SCHDomainException.Unauthorized("Refresh token has been revoked");
            }

            if (storedToken.ExpiryDate < DateTime.UtcNow)
            {
                _logger.Warn($"Refresh token expired: {storedToken.Id}");
                throw SCHDomainException.Unauthorized("Refresh token expired");
            }

            // Validate JWT ID matches
            string? jtiClaim = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (storedToken.JwtId != jtiClaim)
            {
                _logger.Warn($"JWT ID mismatch for user: {userId}");
                throw SCHDomainException.Unauthorized("Token mismatch");
            }

            // Check IP address change (log only, don't block)
            if (!string.IsNullOrEmpty(storedToken.IpAddress) && storedToken.IpAddress != ipAddress)
            {
                _logger.Warn(
                    $"IP address change detected for user {userId}: {storedToken.IpAddress} -> {ipAddress}");
            }

            // Mark old token as used
            storedToken.IsUsed = true;
            storedToken.UsedDate = DateTime.UtcNow;

            // Generate new tokens
            ApplicationUser user = storedToken.User;
            IList<string> roles = await _userManager.GetRolesAsync(user);
            (List<string> permissions, int? ownStudentId, int? ownTeacherId) = await BuildUserContextAsync(user.Id, roles);
            (string newAccessToken, string newTokenId) = await _tokenService.GenerateAccessTokenAsync(
                user.Id, user.UserName!, user.Email!, roles, permissions, ownStudentId, ownTeacherId);
            string newRefreshToken = _tokenService.GenerateRefreshToken();

            // Store new refresh token
            RefreshToken newRefreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                FamilyId = storedToken.FamilyId, // Inherit family ID
                ParentTokenId = storedToken.Id, // Track lineage
                GeneratedBy = RefreshTokenGeneratedBy.RefreshToken,
                Token = newRefreshToken,
                JwtId = newTokenId,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                DeviceName = ParseDeviceName(userAgent),
                ExpiryDate = storedToken.ExpiryDate // CRITICAL: Inherit original expiration, don't extend!
            };

            await _refreshTokenRepository.InsertAsync(newRefreshTokenEntity);
            await _identityUnitOfWork.SaveChangesAsync();

            _logger.Info($"Token refreshed successfully for user: {user.UserName}");

            // Calculate remaining seconds until the inherited expiry date
            int remainingSeconds = (int)(storedToken.ExpiryDate - DateTime.UtcNow).TotalSeconds;

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = int.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"] ?? "30") * 60,
                RefreshTokenExpiresIn = remainingSeconds, // Use actual remaining time, not full duration
                TokenType = "Bearer",
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName!,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    Roles = roles.ToList(),
                    Permissions = permissions,
                    OwnStudentId = ownStudentId,
                    OwnTeacherId = ownTeacherId,
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate,
                    LastLoginDate = user.LastLoginDate,
                    ConcurrencyStamp = user.ConcurrencyStamp
                }
            };
        }

        /// <summary>
        /// Logs out user and revokes refresh tokens based on scope
        /// </summary>
        public async Task LogoutAsync(int userId, string? currentRefreshToken = null, LogoutScope scope = LogoutScope.CurrentBrowser)
        {
            _logger.Info($"Logout for user: {userId}, scope: {scope}");

            List<RefreshToken> tokensToRevoke = new();

            switch (scope)
            {
                case LogoutScope.CurrentSession:
                    // Revoke only the current refresh token
                    if (!string.IsNullOrEmpty(currentRefreshToken))
                    {
                        RefreshToken? token = await _refreshTokenRepository.GetByTokenAsync(currentRefreshToken);
                        if (token != null && token.UserId == userId && !token.IsRevoked)
                        {
                            tokensToRevoke.Add(token);
                        }
                    }
                    break;

                case LogoutScope.CurrentBrowser:
                    // Revoke all tokens in the same family
                    if (!string.IsNullOrEmpty(currentRefreshToken))
                    {
                        RefreshToken? currentToken = await _refreshTokenRepository.GetByTokenAsync(currentRefreshToken);
                        if (currentToken != null && currentToken.UserId == userId)
                        {
                            List<RefreshToken> familyTokens = await _refreshTokenRepository.GetNonRevokedTokensByFamilyIdAsync(currentToken.FamilyId);
                            tokensToRevoke.AddRange(familyTokens);
                        }
                    }
                    break;

                case LogoutScope.AllDevices:
                    // Revoke all user tokens (all devices)
                    List<RefreshToken> allTokens = await _refreshTokenRepository.GetNonRevokedTokensByUserIdAsync(userId);
                    tokensToRevoke.AddRange(allTokens);
                    break;
            }

            if (tokensToRevoke.Any())
            {
                foreach (RefreshToken token in tokensToRevoke)
                {
                    token.IsRevoked = true;
                    token.RevokedDate = DateTime.UtcNow;
                }

                _refreshTokenRepository.UpdateRange(tokensToRevoke);
                await _identityUnitOfWork.SaveChangesAsync();

                _logger.Info($"User logged out: {userId}, {tokensToRevoke.Count} tokens revoked (scope: {scope})");
            }
            else
            {
                _logger.Warn($"No tokens found to revoke for user: {userId} (scope: {scope})");
            }
        }

        /// <summary>
        /// Revokes a specific refresh token
        /// </summary>
        public async Task RevokeTokenAsync(int tokenId, int userId)
        {
            RefreshToken? token = await _refreshTokenRepository.GetByIdAndUserIdAsync(tokenId, userId);

            if (token == null)
            {
                throw SCHDomainException.NotFound("Token not found");
            }

            token.IsRevoked = true;
            token.RevokedDate = DateTime.UtcNow;

            _refreshTokenRepository.Update(token);
            await _identityUnitOfWork.SaveChangesAsync();

            _logger.Info($"Token revoked: {tokenId} for user: {userId}");
        }

        /// <summary>
        /// Gets all active sessions for a user
        /// </summary>
        public async Task<List<SessionDto>> GetActiveSessionsAsync(int userId)
        {
            List<RefreshToken> tokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(userId);

            List<SessionDto> sessions = tokens.Select(rt => new SessionDto
            {
                Id = rt.Id,
                DeviceName = rt.DeviceName,
                IpAddress = rt.IpAddress,
                Location = null, // TODO: Implement GeoIP lookup if needed
                LastActive = rt.CreatedDate,
                IsCurrent = false, // Will be set by controller
                ExpiryDate = rt.ExpiryDate
            }).ToList();

            return sessions;
        }

        /// <summary>
        /// Changes user password
        /// </summary>
        public async Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw SCHDomainException.NotFound("User not found");
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(
                user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw SCHDomainException.BadRequest($"Password change failed: {errors}");
            }

            // Revoke all refresh tokens for security
            await LogoutAsync(userId);

            _logger.Info($"Password changed for user: {userId}");
        }

        /// <summary>
        /// Ensures a role exists, creates it if not
        /// </summary>
        private async Task EnsureRoleExistsAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                ApplicationRole role = new ApplicationRole
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    Description = $"{roleName} role",
                    IsActive = true
                };
                await _roleManager.CreateAsync(role);
            }
        }

        /// <summary>
        /// Checks if a username is available
        /// </summary>
        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            ApplicationUser? user = await _userManager.FindByNameAsync(username);
            return user == null;
        }

        /// <summary>
        /// Checks if an email is available
        /// </summary>
        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            return user == null;
        }

        /// <summary>
        /// Revokes all tokens in a token family (used for security breach detection)
        /// </summary>
        private async Task RevokeTokenFamilyAsync(Guid familyId)
        {
            List<RefreshToken> familyTokens = await _refreshTokenRepository.GetNonRevokedTokensByFamilyIdAsync(familyId);

            foreach (RefreshToken token in familyTokens)
            {
                token.IsRevoked = true;
                token.RevokedDate = DateTime.UtcNow;
            }

            _refreshTokenRepository.UpdateRange(familyTokens);
            await _identityUnitOfWork.SaveChangesAsync();

            _logger.Info($"Revoked {familyTokens.Count} tokens in family {familyId} due to security breach");
        }

        /// <summary>
        /// Parses device name from user agent
        /// </summary>
        private static string? ParseDeviceName(string? userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return null;

            // Simple parsing - can be enhanced with a library like UAParser
            if (userAgent.Contains("Chrome"))
                return "Chrome Browser";
            if (userAgent.Contains("Firefox"))
                return "Firefox Browser";
            if (userAgent.Contains("Safari"))
                return "Safari Browser";
            if (userAgent.Contains("Edge"))
                return "Edge Browser";

            return "Unknown Device";
        }

        /// <summary>
        /// Builds the user's merged permission list and own-record IDs.
        /// Collects claims from all assigned roles (AspNetRoleClaims) and direct user claims (AspNetUserClaims).
        /// Also queries Student/Teacher tables for records linked to this user.
        /// </summary>
        private async Task<(List<string> Permissions, int? OwnStudentId, int? OwnTeacherId)> BuildUserContextAsync(
            int userId,
            IList<string> roles)
        {
            HashSet<string> permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Collect role claims
            foreach (string roleName in roles)
            {
                ApplicationRole? role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    IList<Claim> roleClaims = await _roleManager.GetClaimsAsync(role);
                    IList<Claim> permissionClaims = roleClaims.Where(c => c.Type == "permission").ToList();
                    foreach (Claim claim in permissionClaims)
                    {
                        permissions.Add(claim.Value);
                    }

                }
            }

            // Collect direct user claims
            ApplicationUser? appUser = await _userManager.FindByIdAsync(userId.ToString());
            if (appUser != null)
            {
                IList<Claim> userClaims = await _userManager.GetClaimsAsync(appUser);
                IList<Claim> permissionClaims = userClaims.Where(c => c.Type == "permission").ToList();
                foreach (Claim claim in permissionClaims)
                {
                    permissions.Add(claim.Value);
                }

            }

            // Resolve own-record IDs
            Student? ownStudent = await _studentsRepository.GetStudentByUserIdAsync(userId);
            Teacher? ownTeacher = await _teachersRepository.GetTeacherByUserIdAsync(userId);

            return (permissions.ToList(), ownStudent?.Id, ownTeacher?.Id);
        }

        /// <summary>
        /// Revokes all active refresh tokens for a user (called when UserId is removed from a Student/Teacher record).
        /// This forces the user to re-login, generating a fresh JWT without stale own-record claims.
        /// </summary>
        public async Task RevokeAllUserSessionsAsync(int userId)
        {
            List<RefreshToken> tokens = await _refreshTokenRepository.GetNonRevokedTokensByUserIdAsync(userId);

            if (tokens.Any())
            {
                foreach (RefreshToken token in tokens)
                {
                    token.IsRevoked = true;
                    token.RevokedDate = DateTime.UtcNow;
                }

                _refreshTokenRepository.UpdateRange(tokens);
                await _identityUnitOfWork.SaveChangesAsync();

                _logger.Info($"Revoked {tokens.Count} sessions for user {userId} due to UserId unlink");
            }
        }
    }
}

