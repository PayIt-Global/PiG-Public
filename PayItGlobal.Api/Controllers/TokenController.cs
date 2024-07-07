using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PayItGlobal.Application.Interfaces;
using PayItGlobal.Infrastructure.Identity;
using PayEzPaymentApi.Models;
using System;
using System.Threading.Tasks;

namespace PayEzPaymentApi.Controllers
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

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            #region debug code
#if DEBUG
            if (user == null)
            {
                user = await CreateUserForDevelopmentAsync(model.Username, model.Password);
            }
#endif
            #endregion

            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var token = _tokenService.GenerateJwtToken(user.Id);
                var refreshToken = await _refreshTokenService.GenerateRefreshToken(user.Id, Request.HttpContext.Connection.RemoteIpAddress.ToString());
                return Ok(new { Token = token, RefreshToken = refreshToken });
            }
            return Unauthorized();
        }

        /// <summary>
        /// Refreshes the JWT token using a valid refresh token.
        /// </summary>
        /// <remarks>
        /// This endpoint is used to obtain a new JWT access token by submitting a valid refresh token. 
        /// Upon receiving a refresh token, the system will automatically detect if the associated access token has expired.
        /// If the access token is expired and the refresh token is valid, a new access token and a new refresh token are issued.
        /// The old refresh token is immediately revoked upon successful issuance of a new set of tokens to ensure it cannot be reused.
        /// </remarks>
        /// <param name="model">The refresh token request model containing the refresh token.</param>
        /// <returns>A new JWT access token and refresh token if the provided refresh token is valid; otherwise, an unauthorized response.</returns>
        /// <response code="200">Returns the new access and refresh tokens</response>
        /// <response code="400">If the request model is null or invalid</response>
        /// <response code="401">If the refresh token is invalid, expired, or revoked</response>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            int userId;
            try
            {
                userId = await _refreshTokenService.GetUserIdFromRefreshToken(model.RefreshToken);
            }
            catch (InvalidOperationException)
            {
                return Unauthorized("Invalid refresh token.");
            }

            var isValidRefreshToken = await _refreshTokenService.IsValidRefreshToken(model.RefreshToken);
            if (!isValidRefreshToken)
            {
                return Unauthorized("Invalid refresh token.");
            }

            // Revoke the old refresh token
            await _refreshTokenService.RevokeRefreshToken(model.RefreshToken);

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            // Issue a new JWT token
            var newJwtToken = _tokenService.GenerateJwtToken(userId);

            // Issue a new refresh token
            var newRefreshToken = await _refreshTokenService.GenerateRefreshToken(userId, Request.HttpContext.Connection.RemoteIpAddress.ToString());

            // Save the new refresh token to the datastore
            // Assuming _refreshTokenService handles saving the new refresh token internally within GenerateRefreshToken method.
            // If not, you would need to explicitly save it using a method like _refreshTokenService.SaveRefreshToken(userId, newRefreshToken);

            return Ok(new { Token = newJwtToken, RefreshToken = newRefreshToken });
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

    // Define the RefreshTokenRequest model
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }
}
