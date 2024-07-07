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
        private readonly PayEzDb _context;
        private readonly ApplicationDbContext _appContext;

        public UserRepository(ApplicationDbContext _appContext, PayEzDb context)
        {
            _appContext = _appContext;
            _context = context;
        }


        #region User Methods
        public IQueryable<User> GetByIdAsQueryable(string id)
        {
            // Note: We're not executing the query here, just constructing it
            return _context.Users.Where(u => u.AspNetUserId == id);
        }

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

            // Assuming tokenRecord.UserId is of type string and needs to be parsed to Guid
            if (Guid.TryParse(tokenRecord.UserId, out Guid userId))
            {
                // Optionally, you might want to revoke the old refresh token here and issue a new one
                return (true, userId);
            }

            return (false, Guid.Empty);
        }

        #endregion


    }
}
