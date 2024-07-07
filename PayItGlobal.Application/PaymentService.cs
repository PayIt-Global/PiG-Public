using PayItGlobal.Application.Interfaces;
using PayItGlobal.Domain.Interfaces;
using PayItGlobal.Domain.Entities;
using PayItGlobal.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace PayItGlobal.Application
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICreditCardUtility _creditCardUtility;
        private readonly IMiscRepository _miscRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAPIHelperService _apiHelperService;
        private readonly ILogger<PaymentService> _logger;
        private readonly IPortalService _portalService;
        public PaymentService(IPortalService portalService, ILogger<PaymentService> logger, IAPIHelperService apiHelperService, IHttpContextAccessor httpContextAccessor, IMiscRepository miscRepository, ICreditCardUtility creditCardUtility, IPaymentRepository paymentRepository)
        {
            _portalService = portalService;
            _logger = logger;
            _apiHelperService = apiHelperService;
            _httpContextAccessor = httpContextAccessor;
            _miscRepository = miscRepository;
            _creditCardUtility = creditCardUtility;
            _paymentRepository = paymentRepository;
        }
        public async Task<EnterCCTransactionResponse> EnterCCTransactionAsync(PaymentDataCCDto newPayment, decimal amountRequested, User user)
        {
            var response = new EnterCCTransactionResponse();
            //var portal = user.Client.Portals.FirstOrDefault(p => p.PortalTypeID == 2);
            var portal = await _portalService.GetByIdForAPI_Async(15);

            #region Load PD
            var paymentData = new DTOPaymentData();
            paymentData.Addr1 = newPayment.Addr1;
            paymentData.CCNumber = newPayment.CCNumber;
            paymentData.City = newPayment.City;
            paymentData.CSC = newPayment.CSC;
            paymentData.ExpMonth = newPayment.ExpMonth;
            paymentData.ExpYear = newPayment.ExpYear;
            paymentData.FirstName = newPayment.FirstName;
            paymentData.LastName = newPayment.LastName;
            paymentData.BusinessName = newPayment.BusinessName;
            if (!string.IsNullOrEmpty(newPayment.StateName))
            {
                paymentData.StateProvinceID = _miscRepository.GetStateProvinceByNameAsync(newPayment.StateName).Result.StateProvinceID;
            }
            paymentData.Zip = newPayment.Zip;
            paymentData.Email = newPayment.Email;

            #endregion

            var newTransaction = GetNewTransaction();
            decimal amount = amountRequested;

            CreditCardTypeType? cardType = _creditCardUtility.GetCardTypeFromNumber(paymentData.CCNumber);
            string strCardType = cardType?.ToString() ?? "Unknown";
            newTransaction.CardType = strCardType;

            newTransaction = await _apiHelperService.TransactionRequestAsync(amount, paymentData, portal, newTransaction, user.UserID, null);

            var paymentToReturn = new Payment
            {
                IsFromMobile = false,
                User = user,
                Total = amount,
                CreatedOn = DateTime.Now,             
                PayerFirstName = paymentData.FirstName,
                PayerLastName = paymentData.LastName,
                Portal = portal
            };
            var last4 = "";
            CCString.ToLast4CC(paymentData.CCNumber, out last4);
            paymentToReturn.Last4CC = last4;
            paymentToReturn.Transactions.Add(newTransaction);

            var changes = await _paymentRepository.UpdateAsync(paymentToReturn);
            if (!changes)
            {
                // MvcCmsEmails.DynamicEmail("save rest cc payment failed", "support@upsilonpayments.com", "support@upsilonpayments.com", "a payment was made and not recorded for " + newTransaction.TDTransactionID + " at:" + portal.Title);
            }

            response.TransactionID = newTransaction.TDTransactionID;
            response.ResponseCD = newTransaction.Response;
            response.ResponseDesc = newTransaction.ResponseText;

            return response;
        }

        public Transaction GetNewTransaction()
        {
            var newTransaction = new Transaction();
            newTransaction.TransactionDate = DateTime.Now;

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Request != null)
            {
                var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
                newTransaction.IPAddress = ipAddress ?? "209.235.246.210";
            }

            return newTransaction;
        }

        public async Task<Payment> GetByIdAsync(Guid id)
        {
            throw new NotSupportedException("Users are identified by Guid, not int. Use GetByIdAsync(Guid id) instead.");
        }
        public async Task<Payment> GetByIdAsync(int id)
        {
            try
            {
                var user = await _paymentRepository.GetByIdAsync(id);
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

        public async Task<bool> UpdateAsync(Payment payment)
        {
            try
            {
                // Perform any necessary business logic here
                // For example, validation or preprocessing before updating

                await _paymentRepository.UpdateAsync(payment);

                _logger.LogInformation("Payment with ID: {PaymentID} updated successfully", payment.PaymentID);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding or updating payment with ID: {PaymentID}", payment.PaymentID);
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
    static class CCString
    {
        public static bool ToLast4CC(this string ccNumber, out string result)
        {
            if (!string.IsNullOrEmpty(ccNumber))
            {
                var len = ccNumber.Length;
                if (len > 3)
                {
                    result = ccNumber.Substring(len - 4, 4);
                    return true;
                }
                else
                {
                    result = ccNumber;
                    return false;
                }
            }
            else
            {
                result = ccNumber;
                return false;
            }
        }
    }
}
