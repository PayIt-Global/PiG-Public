using System;
using PayItGlobalApi.Infrastructure;
using PayItGlobalApi.Domain.Interfaces;
using PayItGlobalApi.Domain.Entities;

namespace PayItGlobalApi.Infrastructure.Interfaces
{
    public partial interface IUnitOfWork
    {
        IUserRepository Users { get; }
   
    }
}
