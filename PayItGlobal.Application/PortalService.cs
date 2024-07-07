using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PayItGlobal.Application.Interfaces;
using PayItGlobal.Domain.Entities;
using PayItGlobal.Domain.Interfaces;
using PayItGlobal.DTOs;


namespace PayItGlobal.Application
{
    public class PortalService : IPortalService
    {
        private readonly IPortalRepository _portalRepository;
        private readonly ICreditCardUtility _creditCardUtility;
        private readonly IMiscRepository _miscRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAPIHelperService _apiHelperService;
        private readonly ILogger<PaymentService> _logger;
        public PortalService(ILogger<PaymentService> logger, IAPIHelperService apiHelperService, IHttpContextAccessor httpContextAccessor, IMiscRepository miscRepository, ICreditCardUtility creditCardUtility, IPortalRepository portalRepository)
        {
            _logger = logger;
            _apiHelperService = apiHelperService;
            _httpContextAccessor = httpContextAccessor;
            _miscRepository = miscRepository;
            _creditCardUtility = creditCardUtility;
            _portalRepository = portalRepository;
        }
        public async Task<PayOnlinePortal> GetByIdForAPI_Async(int id)
        {
            try
            {
                var portalQuery = _portalRepository.GetByIdAsQueryable(id);
                //portalQuery = portalQuery.Include(u => u.PayonlineMerchantCredentials);

                var portal = await portalQuery.FirstOrDefaultAsync();

                if (portal == null)
                {
                    _logger.LogWarning("portal with ID: {id} not found", id);
                    return null;
                }

                _logger.LogInformation("portal with ID: {id} retrieved successfully", id);
                return portal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving portal with ID: {id}", id);
                throw; // Rethrow the exception to handle it further up the call stack if necessary
            }
        }
        public async Task<PayOnlinePortal> GetByIdAsync(Guid id)
        {
            throw new NotSupportedException("Portal are identified by int , not guid. Use GetByIdAsync(int id) instead.");
        }
        public async Task<PayOnlinePortal> GetByIdAsync(int id)
        {
            try
            {
                var user = await _portalRepository.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("Portal with ID: {PayOnlinePortalID} not found", id);
                    return null;
                }

                _logger.LogInformation("Portal with ID: {PayOnlinePortalID} retrieved successfully", id);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving Portal with ID: {PayOnlinePortalID}", id);
                throw; // Rethrow the exception to handle it further up the call stack if necessary
            }
        }

        public async Task<bool> UpdateAsync(PayOnlinePortal portal)
        {
            try
            {
                // Perform any necessary business logic here
                // For example, validation or preprocessing before updating

                await _portalRepository.UpdateAsync(portal);

                _logger.LogInformation("portal with ID: {PayOnlinePortalID} updated successfully", portal.PayOnlinePortalID);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding or updating portal with ID: {PayOnlinePortalID}", portal.PayOnlinePortalID);
                return false;
            }
        }

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
