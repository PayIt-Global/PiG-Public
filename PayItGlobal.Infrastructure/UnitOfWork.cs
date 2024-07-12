using PayItGlobal.Domain.Interfaces;
using PayItGlobal.Infrastructure.Context;
using PayItGlobal.Infrastructure.Interfaces;
using PayItGlobal.Infrastructure.Repository;

namespace PayItGlobal.Infrastructure
{
    public partial class UnitOfWork : IUnitOfWork
    {
        private readonly PayItGlobalDb _context;
        private readonly ApplicationDbContext _appContext; // Add this line
        private IUserRepository _users;

        // Modify the constructor to accept both contexts
        public UnitOfWork(PayItGlobalDb context, ApplicationDbContext appContext)
        {
            _context = context;
            _appContext = appContext; // Initialize the new field
        }

        public IUserRepository Users
        {
            get
            {
                if (_users == null)
                {
                    // Pass both contexts to the UserRepository
                    _users = new UserRepository(_appContext, _context);
                }
                return _users;
            }
        }
        protected virtual void CloseContext()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }

        #region IDisposable Methods

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Close_context();
                }
            }
            this.disposed = true;
        }

        private void Close_context()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IUnitOfWork Members

        public virtual void Save()
        {
            if (_context == null)
                throw new InvalidOperationException("Context has not been initialized.");
            _context.SaveChanges();
        }

        #endregion
    }
}
