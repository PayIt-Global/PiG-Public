using System;
using System.Collections.Generic;
using CryptAplyApi.Domain.Entities;

namespace CryptAplyApi.Domain.Interfaces
{
    public partial interface IMiscRepository
    {
        Task<StateProvince>? GetStateProvinceByNameAsync(string name);
        Task<StateProvince>? GetStateProvinceByIdAsync(int id);
    }
}
