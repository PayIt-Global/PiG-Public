using PayEz.Domain.Entities;
using PayEz.DTOs;
using PayEz.Application.Interfaces;
using PayEz.TSYSCore;
using System;
using System.Threading.Tasks;

namespace PayEz.GatewayOrchestrator
{
    public class APIHelperService : IAPIHelperService
    {
        #region Constructors
        private readonly ITSYSService _tsysService;
        private readonly bool isDev = true;
        public APIHelperService(ITSYSService tsysService)
        {
            _tsysService = tsysService;
        }
        #endregion

        private PayonlineProcessor DevTest(PayonlineProcessor processor)
        {
            if (!isDev) { return processor; } else { processor.ProcessorID = 0; return processor; }
        }

        #region CC Methods

        public async Task<Transaction> TransactionRequestAsync(decimal amount, DTOPaymentData paymentData, PayOnlinePortal portal, Transaction transactionToReturn, Guid? userId, int? profileId)
        {
            return await TransactionRequestInternalAsync(amount, paymentData, portal, transactionToReturn, userId, profileId, false);
        }

        private async Task<Transaction> TransactionRequestInternalAsync(decimal amount, DTOPaymentData paymentData, PayOnlinePortal portal, Transaction transactionToReturn, Guid? userId, int? profileId, bool isRecur)
        {
            if (amount < 5)
            {
                transactionToReturn.Response = "Under 5 Invalid Service";
                transactionToReturn.ResponseCode = "Under 5 Invalid Service";
                transactionToReturn.ResponseText = "Under 5 Invalid Service";
                transactionToReturn.TransactionType = "NPCVoid";
                return transactionToReturn;
            }

            var ccProcessor = portal.CCPayOnlineProcessor;
            if (ccProcessor == null || ccProcessor.ProcessorID == 0) { return await TransactionRequestDemoAsync(transactionToReturn, false); }
            try
            {
                if (ccProcessor.Name == "TSYS")
                {
                    transactionToReturn.TransactionType = "NPCSale";
                    return _tsysService.TransactionRequest(amount, paymentData, portal, transactionToReturn, userId, profileId);
                }
            }
            catch (Exception ex)
            {
                transactionToReturn.TransactionType += "Fail13";
                transactionToReturn.ResponseText = ex.Message.Trim().Substring(0, Math.Min(ex.Message.Trim().Length, 1490));
                transactionToReturn.Response = "597";
            }
            return transactionToReturn;
        }

        public async Task<Transaction> TransactionRequestDemoAsync(Transaction transactionToReturn, bool doFail)
        {
            transactionToReturn.TransactionType = "DemoSale";
            return await _tsysService.TransactionRequestDemoAsync(transactionToReturn, doFail);
        }

        #endregion
    }
}