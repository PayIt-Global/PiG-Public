using Microsoft.EntityFrameworkCore;
using PayItGlobal.Domain.Interfaces;
using PayItGlobal.Infrastructure.Context;
using PayItGlobal.Domain.Entities;
using System.Security.Cryptography;
using System.Text;
using System.Linq.Expressions;

namespace PayItGlobal.Infrastructure.Repository
{
    public class PortalRepository : IPortalRepository
    {
        private readonly PayEzDb _context;

        public PortalRepository(PayEzDb context)
        {
            _context = context;
        }

        public async Task AddAsync(PayOnlinePortal entity)
        {
            await _context.Set<PayOnlinePortal>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PayOnlinePortal entity)
        {
            _context.Set<PayOnlinePortal>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PayOnlinePortal>> FindAsync(Expression<Func<PayOnlinePortal, bool>> predicate)
        {
            return await _context.Set<PayOnlinePortal>().Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<PayOnlinePortal>> GetAllAsync()
        {
            return await _context.Set<PayOnlinePortal>().ToListAsync();
        }

        public IQueryable<PayOnlinePortal> GetByIdAsQueryable(int id)
        {
            // Note: We're not executing the query here, just constructing it
            return _context.PayOnlinePortals.Where(u => u.PayOnlinePortalID == id);
        }

        public async Task<PayOnlinePortal> GetByIdAsync(int id)
        {
            return await _context.Set<PayOnlinePortal>().FindAsync(id);
        }

        public async Task<bool> UpdateAsync(PayOnlinePortal entity)
        {
            var existingPayment = await _context.Payments.FindAsync(entity.PayOnlinePortalID);
            if (existingPayment == null)
            {
                // The payment doesn't exist, so we add it as a new record
                _context.PayOnlinePortals.Add(entity);
            }
            else
            {
                // The payment exists, so we update the existing record
                _context.Entry(existingPayment).CurrentValues.SetValues(entity);
            }

            // Save changes to the database
            int changes = await _context.SaveChangesAsync();
            return changes > 0; // returns true if any changes were made, indicating success
        }

        #region Private Methods

        #endregion
    }
}
