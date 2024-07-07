using PayItGlobal.Domain.Entities;
using System.Security.Claims;

namespace PayItGlobal.Domain.Interfaces
{
    public partial interface IUserRepository 
    {

        Task<(bool IsValid, int UserId)> ValidateRefreshToken(string refreshToken);
    }
}
