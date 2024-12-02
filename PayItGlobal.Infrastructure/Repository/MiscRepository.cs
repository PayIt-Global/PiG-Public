using Microsoft.EntityFrameworkCore;
using CryptAplyApi.Domain.Entities;
using CryptAplyApi.Domain.Interfaces;
using CryptAplyApi.Infrastructure.Context;

public class MiscRepository : IMiscRepository
{
    private readonly PayItGlobalDb _context;

    public MiscRepository(PayItGlobalDb context)
    {
        _context = context;
    }

    public async Task<StateProvince>? GetStateProvinceByNameAsync(string name)
    {
        // Example: Asynchronously query the database for a StateProvince by name
        var stateProvince = await _context.StateProvinces
            .FirstOrDefaultAsync(sp => sp.StateProvinceCode == name);
        return stateProvince;
    }

    public async Task<StateProvince>? GetStateProvinceByIdAsync(int id)
    {
        // Example: Asynchronously query the database for a StateProvince by ID
        var stateProvince = await _context.StateProvinces
            .FindAsync(id);
        return stateProvince;
    }
}
