using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PayItGlobalApi.Application.Interfaces;
using PayItGlobalApi.Domain.Models;
using PayItGlobalApi.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using PayItGlobalApi.Domain.Entities;
using PayItGlobalApi.Infrastructure.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PayItGlobalApi.Helper
{
    public class JwtMiddleware
    {
        #region ===[ Private Members ]=============================================================

        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        #endregion

        #region ===[ Constructor ]=================================================================

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings, UserManager<ApplicationUser> userManager)
        {
            _next = next;
            _appSettings = appSettings.Value;
            _userManager = userManager;
        }

        #endregion

        #region ===[ Public Methods ]==============================================================

        public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork, IServiceProvider serviceProvider)
        {
            var tokenService = serviceProvider.GetService<ITokenService>();
            var accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var refreshToken = context.Request.Headers["X-Refresh-Token"].FirstOrDefault();

            if (!string.IsNullOrEmpty(accessToken))
            {
                // Handle access token validation and user attachment
                await AttachUserToContext(context, accessToken, serviceProvider);
            }
            else if (!string.IsNullOrEmpty(refreshToken))
            {
                // Handle refresh token and issue new access token if valid
                await HandleRefreshToken(context, refreshToken, serviceProvider);
            }

            await _next(context);
        }

        private async Task HandleRefreshToken(HttpContext context, string refreshToken, IServiceProvider serviceProvider)
        {
            var tokenService = serviceProvider.GetService<ITokenService>();
            if (tokenService == null)
            {
                throw new InvalidOperationException("TokenService is not registered.");
            }

            try
            {
                // Validate the refresh token and generate a new access token
                var newAccessToken = await tokenService.GenerateAccessTokenFromRefreshToken(refreshToken);
                if (newAccessToken == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Invalid refresh token.");
                    return;
                }

                // Extract userId from the new access token
                var userId = ExtractUserIdFromAccessToken(newAccessToken);
                if (userId == 0)
                {
                    throw new Exception("Failed to extract userId from access token.");
                }

                // Generate a new refresh token using the extracted userId
                var newRefreshToken = await tokenService.GenerateRefreshToken(userId, context.Connection.RemoteIpAddress?.ToString());

                // Return the new access token and refresh token to the client
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.Headers["Authorization"] = $"Bearer {newAccessToken}";
                context.Response.Headers["X-Refresh-Token"] = newRefreshToken;
            }
            catch (SecurityTokenException ex)
            {
                // Handle specific token validation exceptions if necessary
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync($"Token validation error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log the exception and return a generic error message
                Console.WriteLine($"An error occurred while refreshing the token: {ex.Message}");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An error occurred while processing your request.");
            }
        }

        private int ExtractUserIdFromAccessToken(string accessToken)
        {
            // Remove the 'Bearer ' prefix if it exists
            accessToken = accessToken.Replace("Bearer ", "");

            // Initialize a new JwtSecurityTokenHandler to read the JWT
            var tokenHandler = new JwtSecurityTokenHandler();

            // Read the JWT
            var jwtToken = tokenHandler.ReadJwtToken(accessToken);

            // Attempt to extract the 'UserId' claim
            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "UserId" || claim.Type == "sub");

            if (userIdClaim != null)
            {
                return int.Parse(userIdClaim.Value);
            }

            // Return 0 if the 'UserId' claim is not found or cannot be parsed
            return 0;
        }

        #endregion

        #region ===[ Private Methods ]=============================================================

        private async Task AttachUserToContext(HttpContext context, string accessToken, IServiceProvider serviceProvider)
        {
            try
            {
                // Remove the 'Bearer ' prefix if it exists
                accessToken = accessToken.Replace("Bearer ", "");

                // Initialize a new JwtSecurityTokenHandler to read and validate the JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret)),
                    ValidateIssuer = false, // Set to true and specify `ValidIssuer` if your application requires issuer validation
                    ValidateAudience = false, // Set to true and specify `ValidAudience` if your application requires audience validation
                    ClockSkew = TimeSpan.Zero // Optional: adjust the clock skew if there's a time difference between the token issuer and the server
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out validatedToken);

                // Extract the user ID claim from the validated token
                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "UserId" || c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null) throw new Exception("UserId claim is missing");

                var userId = Guid.Parse(userIdClaim.Value);

                // Use UserManager to find the user by ID
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null) throw new Exception("User not found");

                // Attach user to context on successful Jwt validation
                context.Items["User"] = user;
            }
            catch (SecurityTokenValidationException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return; // Ensure the method execution stops here
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging purposes.
                Console.WriteLine($"Authentication failed: {ex.Message}");

                // Respond with a 401 Unauthorized status code to indicate that the request cannot proceed without proper authentication.
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Authentication failed. The request cannot proceed.");
                return; // Ensure the method execution stops here to prevent further processing of the request.
            }
        }

        #endregion
    }
}
