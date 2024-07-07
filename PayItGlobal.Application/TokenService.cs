using PayItGlobal.Application.Interfaces;
using PayItGlobal.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PayItGlobal.Domain.Interfaces;
using PayItGlobal.Domain.Entities;
using System.Security.Cryptography;

namespace PayItGlobal.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly AppSettings _appSettings;
        private readonly IUserRepository _userRepository; // Assuming this is needed for refresh token validation
        private readonly IRefreshTokenRepository _refreshTokenRepository; // Assuming this is needed for refresh token validation
        public TokenService(AppSettings appSettings, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _appSettings = appSettings;
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task<ClaimsPrincipal?> RefreshJwtToken(string refreshToken)
        {
            // Step 1: Validate the refresh token and retrieve the associated user ID.
            var (isValid, userId) = await _userRepository.ValidateRefreshToken(refreshToken);

            if (!isValid)
            {
                // The refresh token is invalid or expired.
                return null;
            }

            // Step 2: Optionally, perform additional checks, such as whether the user still exists or is active.
            // This step is skipped in this example for brevity.

            // Step 3: Create a ClaimsPrincipal for the user.
            var claims = new List<Claim>
            {
                new Claim("UserId", userId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "jwt");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return claimsPrincipal;
        }

        public async Task<string?> GenerateAccessTokenFromRefreshToken(string refreshToken)
        {
            // Validate the refresh token and retrieve the associated user ID.
            var (isValid, userId) = await _userRepository.ValidateRefreshToken(refreshToken);

            if (!isValid)
            {
                // The refresh token is invalid or expired.
                return null;
            }

            // Generate a new JWT token for the user.
            return GenerateJwtToken(userId);
        }


        public async Task<string> GenerateRefreshToken(Guid userId)
        {
            // Generate a secure random token using RandomNumberGenerator
            using (var rng = RandomNumberGenerator.Create())
            {
                var randomBytes = new byte[64]; // 512 bits
                rng.GetBytes(randomBytes);
                var refreshToken = Convert.ToBase64String(randomBytes); // Convert to base64 for easier storage and handling

                // Create a new RefreshToken entity and associate it with the userId
                var refreshTokenEntity = new RefreshToken
                {
                    UserId = userId,
                    Token = refreshToken,
                    // Set other necessary properties, such as expiry date
                };

                // Store the new refresh token in the repository
                await _refreshTokenRepository.SaveRefreshTokenAsync(refreshTokenEntity);

                return refreshToken;
            }
        }


        public string GenerateJwtToken(Guid userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim("UserId", userId.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Audience = _appSettings.Audience,
                Issuer = _appSettings.Issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
