using Microsoft.EntityFrameworkCore;
using CryptAplyApi.Domain.Entities;
using CryptAplyApi.Domain.Interfaces;
using CryptAplyApi.Infrastructure.Context;

namespace CryptAplyApi.Infrastructure.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly PayItGlobalDb _context;

        public RefreshTokenRepository(PayItGlobalDb context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }
        public async Task<RefreshToken> GetRefreshTokenByHashAsync(string tokenHash)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
        }
    }
}
