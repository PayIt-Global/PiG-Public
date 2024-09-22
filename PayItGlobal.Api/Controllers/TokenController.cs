using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PayItGlobalApi.Application.Interfaces;
using PayItGlobalApi.Application.Models;
using PayItGlobalApi.Infrastructure.Identity;
using PayItGlobalApi.Models;
using System;
using System.Threading.Tasks;

namespace PayItGlobalApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        public TokenController(ITokenService tokenService, UserManager<ApplicationUser> userManager, IRefreshTokenService refreshTokenService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateRequest model)
        {
            if (model == null)
            {
                return BadRequest("Invalid request.");
            }

            var user = await _userManager.FindByNameAsync(model.Username);

#if DEBUG
            if (user == null)
            {
                user = await CreateUserForDevelopmentAsync(model.Username, model.Password);
            }
#endif

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var token = _tokenService.GenerateJwtToken(user.Id);
                var refreshToken = await _refreshTokenService.GenerateRefreshToken(user.Id, Request.HttpContext.Connection.RemoteIpAddress.ToString());
                return Ok(new { Token = token, RefreshToken = refreshToken });
            }
            return Unauthorized("Invalid username or password.");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            try
            {
                // Get the user ID from the refresh token
                var userId = await _refreshTokenService.GetUserIdFromRefreshToken(model.RefreshToken);

                // Check if the refresh token is valid
                var isValidRefreshToken = await _refreshTokenService.IsValidRefreshToken(model.RefreshToken);
                if (!isValidRefreshToken)
                {
                    return Unauthorized("Invalid refresh token.");
                }

                // Revoke the old refresh token
                await _refreshTokenService.RevokeRefreshToken(model.RefreshToken);

                // Generate new tokens
                var newJwtToken = _tokenService.GenerateJwtToken(userId);
                var newRefreshToken = await _refreshTokenService.GenerateRefreshToken(userId, Request.HttpContext.Connection.RemoteIpAddress.ToString());

                return Ok(new { Token = newJwtToken, RefreshToken = newRefreshToken });
            }
            catch (InvalidOperationException ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "An error occurred while refreshing the token.");

                return Unauthorized("Invalid refresh token.");
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "An unexpected error occurred.");

                return StatusCode(500, "An unexpected error occurred.");
            }
        }


        private async Task<ApplicationUser> CreateUserForDevelopmentAsync(string username, string password)
        {
            var newUser = new ApplicationUser { UserName = username, Email = $"{username}@example.com" };
            var createUserResult = await _userManager.CreateAsync(newUser, password);
            if (!createUserResult.Succeeded)
            {
                throw new InvalidOperationException("Failed to create user for development purposes.");
            }
            return newUser;
        }

    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }
}
