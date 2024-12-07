using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Api.Services;
using CryptAplyApp.Application.Services;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly JwtTokenService _tokenService;
        private readonly UserService _userService;
        private readonly AuditLoggingService _auditLoggingService;

        public AuthController(
            ILogger<AuthController> logger,
            JwtTokenService tokenService,
            UserService userService,
            AuditLoggingService auditLoggingService)
        {
            _logger = logger;
            _tokenService = tokenService;
            _userService = userService;
            _auditLoggingService = auditLoggingService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _userService.AuthenticateAsync(request.Username, request.Password);
                if (user == null)
                {
                    await _auditLoggingService.LogActivityAsync(new AuditLogEntry
                    {
                        UserId = request.Username,
                        Action = "FailedLogin",
                        ResourceType = "Authentication",
                        Timestamp = DateTime.UtcNow,
                        Sensitivity = ActivitySensitivity.High,
                        Details = "Invalid username or password"
                    });

                    return Unauthorized(new { message = "Invalid username or password" });
                }

                var token = await _tokenService.GenerateTokenAsync(user);

                await _auditLoggingService.LogActivityAsync(new AuditLogEntry
                {
                    UserId = user.Id,
                    Action = "SuccessfulLogin",
                    ResourceType = "Authentication",
                    Timestamp = DateTime.UtcNow,
                    Sensitivity = ActivitySensitivity.Medium
                });

                return Ok(new
                {
                    Token = token,
                    User = new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.Roles
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login attempt for user {Username}", request.Username);
                return StatusCode(500, "An error occurred during login");
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized();
                }

                var token = authHeader.Substring("Bearer ".Length);
                if (!_tokenService.ValidateToken(token))
                {
                    return Unauthorized();
                }

                var userId = User.Identity.Name;
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized();
                }

                var newToken = await _tokenService.GenerateTokenAsync(user);

                await _auditLoggingService.LogActivityAsync(new AuditLogEntry
                {
                    UserId = userId,
                    Action = "TokenRefresh",
                    ResourceType = "Authentication",
                    Timestamp = DateTime.UtcNow,
                    Sensitivity = ActivitySensitivity.Medium
                });

                return Ok(new { Token = newToken });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return StatusCode(500, "An error occurred while refreshing the token");
            }
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
