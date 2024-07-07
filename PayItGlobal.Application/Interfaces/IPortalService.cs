using PayItGlobal.Domain.Entities;
using PayItGlobal.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayItGlobal.Application.Interfaces
{
    public interface IPortalService : IBaseService<PayOnlinePortal>
    {
        Task<PayOnlinePortal> GetByIdForAPI_Async(int id);
    }
}
