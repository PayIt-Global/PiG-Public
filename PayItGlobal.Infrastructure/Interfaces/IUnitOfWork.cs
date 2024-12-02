using System;
using CryptAplyApi.Infrastructure;
using CryptAplyApi.Domain.Interfaces;
using CryptAplyApi.Domain.Entities;

namespace CryptAplyApi.Infrastructure.Interfaces
{
    public partial interface IUnitOfWork
    {
        IUserRepository Users { get; }
   
    }
}
