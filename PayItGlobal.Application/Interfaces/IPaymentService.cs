using PayItGlobal.Domain.Entities;
using PayItGlobal.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayItGlobal.Application.Interfaces
{
    public interface IPaymentService : IBaseService<Payment>
    {
        Task<EnterCCTransactionResponse> EnterCCTransactionAsync(PaymentDataCCDto newPayment, decimal amountRequested, User user);
    }
}
