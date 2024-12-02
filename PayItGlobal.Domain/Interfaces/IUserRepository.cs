using CryptAplyApi.Domain.Entities;
using System.Security.Claims;

namespace CryptAplyApi.Domain.Interfaces
{
    public partial interface IUserRepository 
    {

        Task<(bool IsValid, int UserId)> ValidateRefreshToken(string refreshToken);
    }
}
