using PayItGlobal.Domain.Entities;
using PayItGlobal.DTOs;

namespace PayItGlobal.Application.Interfaces
{
    public interface IAPIHelperService
    {
        #region CC methods

  
       Task<Transaction> TransactionRequestAsync(decimal amount, DTOPaymentData paymentData, PayOnlinePortal portal, Transaction transactionToReturn, Guid? userId, int? profileId);
     
        #endregion
    }
}
