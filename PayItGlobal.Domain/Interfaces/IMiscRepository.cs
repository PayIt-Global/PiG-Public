using System;
using System.Collections.Generic;
using PayItGlobal.Domain.Entities;

namespace PayItGlobal.Domain.Interfaces
{
    public partial interface IMiscRepository
    {
        Task<StateProvince>? GetStateProvinceByNameAsync(string name);
        Task<StateProvince>? GetStateProvinceByIdAsync(int id);
    }
}
