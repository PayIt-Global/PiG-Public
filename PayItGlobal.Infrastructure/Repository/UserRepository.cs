using Microsoft.EntityFrameworkCore;
using PayItGlobal.Domain.Interfaces;
using PayItGlobal.Infrastructure.Context;
using PayItGlobal.Domain.Entities;
using System.Security.Cryptography;
using System.Text;
using System.Linq.Expressions;
using System.Security.Claims;

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
        public async Task<(bool IsValid, Guid UserId)> ValidateRefreshToken(string refreshToken)
        {
            var tokenRecord = await _context.RefreshTokens
                                            .FirstOrDefaultAsync(t => t.Token == refreshToken && t.Expires > DateTime.UtcNow && t.Revoked == null);

            if (tokenRecord == null)
            {
                // Token is invalid, expired, or revoked
                return (false, Guid.Empty);
            }

   
            // Optionally, you might want to revoke the old refresh token here and issue a new one
            return (true, tokenRecord.UserId);
            
        }

        #endregion


    }
}
