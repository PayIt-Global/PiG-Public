using Microsoft.EntityFrameworkCore;
using PayItGlobal.Domain.Interfaces;
using PayItGlobal.Infrastructure.Context;
using PayItGlobal.Domain.Entities;
using System.Security.Cryptography;
using System.Text;
using System.Linq.Expressions;

namespace PayItGlobal.Infrastructure.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PayEzDb _context;

        public PaymentRepository(PayEzDb context)
        {
            _context = context;
        }

        public async Task AddAsync(Payment entity)
        {
            await _context.Set<Payment>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Payment entity)
        {
            _context.Set<Payment>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Payment>> FindAsync(Expression<Func<Payment, bool>> predicate)
        {
            return await _context.Set<Payment>().Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Set<Payment>().ToListAsync();
        }

        public IQueryable<Payment> GetByIdAsQueryable(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Payment> GetByIdAsync(int id)
        {
            return await _context.Set<Payment>().FindAsync(id);
        }

        public async Task<bool> UpdateAsync(Payment entity)
        {
            var existingPayment = await _context.Payments.FindAsync(entity.PaymentID);
            if (existingPayment == null)
            {
                // The payment doesn't exist, so we add it as a new record
                _context.Payments.Add(entity);
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
