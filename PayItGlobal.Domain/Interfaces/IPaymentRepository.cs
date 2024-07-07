using System;
using System.Collections.Generic;
using PayItGlobal.Domain.Entities;

namespace PayItGlobal.Domain.Interfaces
{
    public partial interface IPaymentRepository : IRepository<Payment, int>
    {

    }
}
