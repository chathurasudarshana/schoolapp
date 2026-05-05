namespace SCH.API.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SCH.Models.Auth.ClientDtos;
    using SCH.Models.Auth.Enums;
    using SCH.Services.Auth;
    using System.Security.Claims;

    /// <summary>
    /// Authentication and authorization controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
        }

        /// <summary>
        /// Login endpoint - authenticates user and returns JWT tokens
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>JWT access token and refresh token</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequestDto request,
            [FromHeader(Name = "User-Agent")] string? userAgent = null)
        {
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            LoginResponseDto response = await _authService.LoginAsync(request, ipAddress, userAgent);

            return Ok(response);
        }

        /// <summary>
        /// Register endpoint - creates a new user account
        /// </summary>
        /// <param name="request">Registration details</param>
        /// <returns>JWT access token and refresh token</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            LoginResponseDto response = await _authService.RegisterAsync(request);

            return Ok(response);
        }

        /// <summary>
        /// Refresh token endpoint - gets a new access token using refresh token
        /// </summary>
        /// <param name="request">Access token and refresh token</param>
        /// <returns>New JWT access token and refresh token</returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenRequestDto request,
            [FromHeader(Name = "User-Agent")] string? userAgent = null)
        {
            string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            LoginResponseDto response = await _authService.RefreshTokenAsync(request, ipAddress, userAgent);

            return Ok(response);
        }

        /// <summary>
        /// Logout endpoint - revokes refresh tokens based on scope
        /// </summary>
        /// <param name="scope">Logout scope - determines which tokens to revoke</param>
        /// <param name="request">Logout request containing optional refresh token</param>
        /// <param name="refreshTokenHeader">Refresh token from header (optional)</param>
        /// <returns>Success message</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout(
            [FromQuery] LogoutScope scope = LogoutScope.CurrentSession,
            [FromBody] LogoutRequestDto? request = null,
            [FromHeader(Name = "X-Refresh-Token")] string? refreshTokenHeader = null)
        {
            int userId = GetCurrentUserId();

            // Get refresh token from body first, then fall back to header
            string? refreshToken = request?.RefreshToken ?? refreshTokenHeader;

            await _authService.LogoutAsync(userId, refreshToken, scope);

            return Ok(new { message = $"Logged out successfully (scope: {scope})" });
        }

        /// <summary>
        /// Get current user information
        /// </summary>
        /// <returns>Current user details</returns>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            int userId = GetCurrentUserId();
            string? username = User.FindFirst(ClaimTypes.Name)?.Value;
            string? email = User.FindFirst(ClaimTypes.Email)?.Value;
            List<string> roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            return Ok(new
            {
                id = userId,
                username,
                email,
                roles
            });
        }

        /// <summary>
        /// Get all active sessions for the current user
        /// </summary>
        /// <returns>List of active sessions</returns>
        [HttpGet("sessions")]
        [Authorize]
        public async Task<IActionResult> GetActiveSessions()
        {
            int userId = GetCurrentUserId();
            List<SessionDto> sessions = await _authService.GetActiveSessionsAsync(userId);

            return Ok(sessions);
        }

        /// <summary>
        /// Revoke a specific session (refresh token)
        /// </summary>
        /// <param name="sessionId">Session ID (RefreshToken ID)</param>
        /// <returns>Success message</returns>
        [HttpDelete("sessions/{sessionId}")]
        [Authorize]
        public async Task<IActionResult> RevokeSession(int sessionId)
        {
            int userId = GetCurrentUserId();

            await _authService.RevokeTokenAsync(sessionId, userId);

            return Ok(new { message = "Session revoked successfully" });
        }

        /// <summary>
        /// Change password for the current user
        /// </summary>
        /// <param name="request">Current and new password</param>
        /// <returns>Success message</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            int userId = GetCurrentUserId();

            await _authService.ChangePasswordAsync(userId, request);

            return Ok(new { message = "Password changed successfully. Please log in again." });
        }

        /// <summary>
        /// Check if username is available
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>Boolean indicating availability</returns>
        [HttpGet("check-username/{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckUsername(string username)
        {
            bool isAvailable = await _authService.IsUsernameAvailableAsync(username);
            return Ok(new { isAvailable });
        }

        /// <summary>
        /// Check if email is available
        /// </summary>
        /// <param name="email">Email to check</param>
        /// <returns>Boolean indicating availability</returns>
        [HttpGet("check-email/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmail(string email)
        {
            bool isAvailable = await _authService.IsEmailAvailableAsync(email);
            return Ok(new { isAvailable });
        }

        /// <summary>
        /// Test endpoint to verify authentication
        /// </summary>
        /// <returns>Success message</returns>
        [HttpGet("test-auth")]
        [Authorize]
        public IActionResult TestAuth()
        {
            return Ok(new { message = "You are authenticated!" });
        }

        /// <summary>
        /// Test endpoint to verify admin role
        /// </summary>
        /// <returns>Success message</returns>
        [HttpGet("test-admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult TestAdmin()
        {
            return Ok(new { message = "You are an admin!" });
        }

        /// <summary>
        /// Helper method to get current user ID from claims
        /// </summary>
        private int GetCurrentUserId()
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return int.Parse(userIdClaim);
        }
    }
}

