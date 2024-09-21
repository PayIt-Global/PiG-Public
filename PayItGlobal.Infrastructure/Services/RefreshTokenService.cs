using PayItGlobal.Application.Interfaces;
using PayItGlobal.Domain.Entities;
using PayItGlobal.Domain.Interfaces;
using PayItGlobal.Infrastructure.Utilities;

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
            var tokenHash = TokenEncryption.Hash(refreshToken);
            var token = await _refreshTokenRepository.GetRefreshTokenByHashAsync(tokenHash);
            if (token != null)
            {
                token.Revoked = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateRefreshTokenAsync(token);
            }
        }

        public async Task<string> GenerateRefreshToken(int userId, string createdByIp)
        {
            var token = TokenEncryption.GenerateSecureToken();
            var tokenHash = TokenEncryption.Hash(token);
            var (encryptedToken, encryptionIv) = TokenEncryption.Encrypt(token);
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Iv = encryptionIv,
                Token = encryptedToken,
                TokenHash = tokenHash,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = createdByIp
            };

            await _refreshTokenRepository.SaveRefreshTokenAsync(refreshToken);
            return token;
        }

        public async Task<bool> IsValidRefreshToken(string token)
        {
            var tokenHash = TokenEncryption.Hash(token);
            var refreshToken = await _refreshTokenRepository.GetRefreshTokenByHashAsync(tokenHash);

            if (refreshToken == null)
            {
                return false;
            }

            var decryptedToken = TokenEncryption.Decrypt(refreshToken.Token, refreshToken.Iv);
            return decryptedToken == token && refreshToken.Expires > DateTime.UtcNow && refreshToken.Revoked == null;
        }

        public async Task<int> GetUserIdFromRefreshToken(string token)
        {
            var tokenHash = TokenEncryption.Hash(token);
            var refreshToken = await _refreshTokenRepository.GetRefreshTokenByHashAsync(tokenHash);

            if (refreshToken != null && refreshToken.Expires > DateTime.UtcNow && refreshToken.Revoked == null)
            {
                var decryptedToken = TokenEncryption.Decrypt(refreshToken.Token, refreshToken.Iv);
                if (decryptedToken == token)
                {
                    return refreshToken.UserId;
                }
            }

            throw new InvalidOperationException("Invalid refresh token");
        }
    }
}
