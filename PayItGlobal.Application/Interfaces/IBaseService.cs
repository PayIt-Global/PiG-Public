using System;
using System.Threading.Tasks;

namespace PayItGlobal.Application.Interfaces
{
    public interface IBaseService<T> where T : class
    {
        // Example of a common method that might be useful across services
        Task<bool> UpdateAsync(T entity);
        //Task<bool> AddAsync(T entity);
        //Task<bool> UpdateAsync(T entity);
        //Task<bool> DeleteAsync(T entity);
        //Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetByIdAsync(int id);
        // You might also include methods for logging, error handling, or other cross-cutting concerns
        void LogInformation(string message);
        void LogError(string message, Exception exception);

        // Other common service functionality...
    }
}
