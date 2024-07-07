using System;
using PayItGlobal.Infrastructure;
using PayItGlobal.Domain.Interfaces;
using PayItGlobal.Domain.Entities;

namespace PayItGlobal.Infrastructure.Interfaces
{
    public partial interface IUnitOfWork
    {
        IUserRepository Users { get; }
   
    }
}
