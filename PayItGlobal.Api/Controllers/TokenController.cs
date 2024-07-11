using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PayItGlobal.Application.Interfaces;
using PayItGlobal.Application.Models;
using PayItGlobal.Infrastructure.Identity;
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
                var userId = await _refreshTokenService.GetUserIdFromRefreshToken(model.RefreshToken);
                var isValidRefreshToken = await _refreshTokenService.IsValidRefreshToken(model.RefreshToken);

                if (!isValidRefreshToken)
                {
                    return Unauthorized("Invalid refresh token.");
                }

                await _refreshTokenService.RevokeRefreshToken(model.RefreshToken);

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return Unauthorized("User not found.");
                }

                var newJwtToken = _tokenService.GenerateJwtToken(userId);
                var newRefreshToken = await _refreshTokenService.GenerateRefreshToken(userId, Request.HttpContext.Connection.RemoteIpAddress.ToString());

                return Ok(new { Token = newJwtToken, RefreshToken = newRefreshToken });
            }
            catch (InvalidOperationException)
            {
                return Unauthorized("Invalid refresh token.");
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

        [HttpGet("test")]
        public IActionResult TestEndpoint()
        {
            return Ok("TokenController is reachable");
        }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }
}
