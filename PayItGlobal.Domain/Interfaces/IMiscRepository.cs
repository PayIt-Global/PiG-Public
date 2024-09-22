using System;
using System.Collections.Generic;
using PayItGlobalApi.Domain.Entities;

namespace PayItGlobalApi.Domain.Interfaces
{
    public partial interface IMiscRepository
    {
        Task<StateProvince>? GetStateProvinceByNameAsync(string name);
        Task<StateProvince>? GetStateProvinceByIdAsync(int id);
    }
}
