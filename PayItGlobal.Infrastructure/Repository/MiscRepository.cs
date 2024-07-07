using Microsoft.EntityFrameworkCore;
using PayItGlobal.Domain.Entities;
using PayItGlobal.Domain.Interfaces;
using PayItGlobal.Infrastructure.Context;

public class MiscRepository : IMiscRepository
{
    private readonly PayEzDb _context;

    public MiscRepository(PayEzDb context)
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
