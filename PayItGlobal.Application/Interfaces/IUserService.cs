using PayItGlobal.Domain.Entities;

namespace PayItGlobal.Application.Interfaces
{
    public interface IUserService 
    {
        Task<User> GetByIdForAPI_Async(string id);
    }
}
