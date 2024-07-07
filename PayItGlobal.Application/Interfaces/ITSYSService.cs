using PayItGlobal.Domain.Entities;
using PayItGlobal.DTOs;
using System;
using System.Threading.Tasks;

namespace PayItGlobal.Application.Interfaces
{
    public interface ITSYSService
    {
        Transaction TransactionRequest(decimal amount, DTOPaymentData paymentData, PayOnlinePortal portal, Transaction transaction, Guid? userId, int? profileId);
        Task<Transaction> TransactionRequestDemoAsync(Transaction transaction, bool doFail);
    }
}

