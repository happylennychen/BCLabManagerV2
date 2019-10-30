using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.DataAccess
{
    public class UnitOfWork : IDisposable
    {
        private bool _disposed = false;
        private readonly AppDbContext _context;
        public IBatteryRepository Batteries { get; private set; }
        public IBatteryTypeRepository BatteryTypes { get; private set; }
        public ITesterRepository Testers { get; private set; }
        public UnitOfWork(AppDbContext dbContext)
        {
            _context = dbContext;
            Batteries = new BatteryRepository(_context);
            BatteryTypes = new BatteryTypeRepository(_context);
            Testers = new TesterRepository(_context);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _context.Dispose();
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
