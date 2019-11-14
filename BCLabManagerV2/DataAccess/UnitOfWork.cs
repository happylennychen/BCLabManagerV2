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
        public IChannelRepository Channels { get; private set; }
        public IChamberRepository Chambers { get; private set; }
        public ITestRecordRepository TestRecords { get; private set; }
        public IRecipeRepository Recipies { get; private set; }
        public IProgramRepository Programs { get; private set; }
        public UnitOfWork(AppDbContext dbContext)
        {
            _context = dbContext;
            Batteries = new BatteryRepository(_context);
            BatteryTypes = new BatteryTypeRepository(_context);
            Testers = new TesterRepository(_context);
            Channels = new ChannelRepository(_context);
            Chambers = new ChamberRepository(_context);
            TestRecords = new TestRecordRepository(_context);
            Recipies = new RecipeRepository(_context);
            Programs = new ProgramRepository(_context);
        }
        public void Commit()
        {
            _context.SaveChanges();
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
