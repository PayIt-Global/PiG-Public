using Microsoft.EntityFrameworkCore;
using PayItGlobal.Application.Interfaces;
using PayItGlobal.Domain.Entities;
using PayItGlobal.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace PayItGlobal.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }
        public async Task RevokeRefreshToken(string refreshToken)
        {
            var token = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);
            if (token != null)
            {
                token.Revoked = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateRefreshTokenAsync(token);
            }
        }
        public async Task<string> GenerateRefreshToken(int userId, string createdByIp)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.UtcNow.AddDays(7), // Set expiration to 7 days, adjust as needed
                Created = DateTime.UtcNow,
                CreatedByIp = createdByIp
            };

            await _refreshTokenRepository.SaveRefreshTokenAsync(refreshToken);
            return refreshToken.Token;
        }

        public async Task<bool> IsValidRefreshToken(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(token);
            return refreshToken != null && refreshToken.Expires > DateTime.UtcNow && refreshToken.Revoked == null;
        }

        public async Task<int> GetUserIdFromRefreshToken(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(token);
            if (refreshToken != null && refreshToken.Expires > DateTime.UtcNow && refreshToken.Revoked == null)
            {
                // Optionally, revoke the token to ensure it can't be used again
                refreshToken.Revoked = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateRefreshTokenAsync(refreshToken);

                return refreshToken.UserId;
            }

            throw new InvalidOperationException("Invalid refresh token");
        }
    }
}
