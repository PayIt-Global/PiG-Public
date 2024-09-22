using PayItGlobalApi.Application.Interfaces;
using PayItGlobalApi.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PayItGlobalApi.Domain.Interfaces;
using PayItGlobalApi.Domain.Entities;
using System.Security.Cryptography;

namespace PayItGlobalApi.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly AppSettings _appSettings;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenService _refreshTokenService;

        public TokenService(AppSettings appSettings, IUserRepository userRepository, IRefreshTokenService refreshTokenService)
        {
            _appSettings = appSettings;
            _userRepository = userRepository;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<ClaimsPrincipal?> RefreshJwtToken(string refreshToken)
        {
            var isValid = await _refreshTokenService.IsValidRefreshToken(refreshToken);
            if (!isValid)
            {
                return null;
            }

            var userId = await _refreshTokenService.GetUserIdFromRefreshToken(refreshToken);

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
            var isValid = await _refreshTokenService.IsValidRefreshToken(refreshToken);
            if (!isValid)
            {
                return null;
            }

            var userId = await _refreshTokenService.GetUserIdFromRefreshToken(refreshToken);
            return GenerateJwtToken(userId);
        }

        public async Task<string> GenerateRefreshToken(int userId, string createdByIp)
        {
            return await _refreshTokenService.GenerateRefreshToken(userId, createdByIp);
        }

        public string GenerateJwtToken(int userId)
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
