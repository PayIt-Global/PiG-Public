using PayItGlobalApi.Domain.Entities;
using System.Security.Claims;

namespace PayItGlobalApi.Domain.Interfaces
{
    public partial interface IUserRepository 
    {

        Task<(bool IsValid, int UserId)> ValidateRefreshToken(string refreshToken);
    }
}
