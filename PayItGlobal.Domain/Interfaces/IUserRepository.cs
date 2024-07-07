using PayItGlobal.Domain.Entities;
using System.Security.Claims;

namespace PayItGlobal.Domain.Interfaces
{
    public partial interface IUserRepository 
    {
        IQueryable<User> GetByIdAsQueryable(string id);

        Task<(bool IsValid, Guid UserId)> ValidateRefreshToken(string refreshToken);
    }
}
