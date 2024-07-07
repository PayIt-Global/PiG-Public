using Microsoft.Extensions.Logging;
using PayItGlobal.Application.Interfaces;
using PayItGlobal.Domain.Entities;
using PayItGlobal.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PayItGlobal.Application
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserRepository _userRepository;
        public UserService(ILogger<UserService> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }


        public async Task<User> GetByIdForAPI_Async(string id)
        {
            try
            {
                var userQuery = _userRepository.GetByIdAsQueryable(id);
                userQuery = userQuery.Include(u => u.Client).ThenInclude(c => c.Portals);

                var user = await userQuery.FirstOrDefaultAsync();

                if (user == null)
                {
                    _logger.LogWarning("User with ID: {UserId} not found", id);
                    return null;
                }

                _logger.LogInformation("User with ID: {UserId} retrieved successfully", id);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user with ID: {UserId}", id);
                throw; // Rethrow the exception to handle it further up the call stack if necessary
            }
        }

        //public async Task<User> GetByIdAsync(Guid id)
        //{
        //    try
        //    {
        //        var user = await _userRepository.GetByIdAsync(id);
        //        if (user == null)
        //        {
        //            _logger.LogWarning("User with ID: {UserId} not found", id);
        //            return null;
        //        }

        //        _logger.LogInformation("User with ID: {UserId} retrieved successfully", id);
        //        return user;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while retrieving user with ID: {UserId}", id);
        //        throw; // Rethrow the exception to handle it further up the call stack if necessary
        //    }
        //}

        //public async Task<bool> UpdateAsync(User user)
        //{
        //    try
        //    {
        //        // Perform any necessary business logic here
        //        // For example, validation or preprocessing before updating

        //        await _userRepository.UpdateAsync(user);

        //        _logger.LogInformation("User with ID: {UserId} updated successfully", user.UserID);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while adding or updating user with ID: {UserId}", user.UserID);
        //        return false;
        //    }
        //}

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        public void LogError(string message, Exception exception)
        {
            _logger.LogError(exception, message);
        }


    }
}
