using Microsoft.EntityFrameworkCore;
using PayItGlobal.Domain.Interfaces;
using PayItGlobal.Infrastructure.Context;

namespace PayItGlobal.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly PayItGlobalDb _context;
        private readonly ApplicationDbContext _appContext;

        public UserRepository(ApplicationDbContext _appContext, PayItGlobalDb context)
        {
            _appContext = _appContext;
            _context = context;
        }


        #region User Methods


        // Method to validate and refresh a JWT token using a refresh token
        public async Task<(bool IsValid, int UserId)> ValidateRefreshToken(string refreshToken)
        {
            var tokenRecord = await _context.RefreshTokens
                                            .FirstOrDefaultAsync(t => t.Token == refreshToken && t.Expires > DateTime.UtcNow && t.Revoked == null);

            if (tokenRecord == null)
            {
                // Token is invalid, expired, or revoked
                return (false, 0);
            }

   
            // Optionally, you might want to revoke the old refresh token here and issue a new one
            return (true, tokenRecord.UserId);
            
        }

        #endregion


    }
}
