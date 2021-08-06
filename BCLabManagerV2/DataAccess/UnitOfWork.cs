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
        public IProjectRepository Projects { get; private set; }
        public IProjectSettingRepository ProjectSettings { get; private set; }
        public ITesterRepository Testers { get; private set; }
        public IChannelRepository Channels { get; private set; }
        public IChamberRepository Chambers { get; private set; }
        public IProgramTypeRepository ProgramTypes { get; private set; }
        public ITableMakerRecordRepository TableMakerRecords { get; private set; }
        public ITableMakerProductRepository TableMakerProducts { get; private set; }
        public ITableMakerProductTypeRepository TableMakerProductTypes { get; private set; }
        public ITestRecordRepository TestRecords { get; private set; }
        public IStepTemplateRepository StepTemplates { get; private set; }
        public IStepRepository Steps { get; private set; }
        public IStepV2Repository StepV2s { get; private set; }
        public ITesterActionRepository TesterActions { get; private set; }
        public ICutOffConditionRepository CutOffConditions { get; private set; }
        public ICutOffBehaviorRepository CutOffBehaviors { get; private set; }
        public IJumpBehaviorRepository JumpBehaviors { get; private set; }
        public IStepRuntimeRepository StepRuntimes { get; private set; }
        public IRecipeTemplateRepository RecipeTemplates { get; private set; }
        public IRecipeTemplateGroupRepository RecipeTemplateGroups { get; private set; }
        public IProtectionRepository Protections { get; private set; }
        public IRecipeRepository Recipies { get; private set; }
        public IProgramRepository Programs { get; private set; }
        public IEventRepository Events { get; private set; }
        public UnitOfWork(AppDbContext dbContext)
        {
            _context = dbContext;
            Batteries = new BatteryRepository(_context);
            BatteryTypes = new BatteryTypeRepository(_context);
            Testers = new TesterRepository(_context);
            Channels = new ChannelRepository(_context);
            Chambers = new ChamberRepository(_context);
            TestRecords = new TestRecordRepository(_context);
            Steps = new StepRepository(_context);
            StepV2s = new StepV2Repository(_context);
            TesterActions = new TesterActionRepository(_context);
            CutOffConditions = new CutOffConditionRepository(_context);
            CutOffBehaviors = new CutOffBehaviorRepository(_context);
            JumpBehaviors = new JumpBehaviorRepository(_context);
            StepTemplates = new StepTemplateRepository(_context);
            StepRuntimes = new StepRuntimeRepository(_context);
            RecipeTemplates = new RecipeTemplateRepository(_context);
            RecipeTemplateGroups = new RecipeTemplateGroupRepository(_context);
            Protections = new ProtectionRepository(_context);
            Recipies = new RecipeRepository(_context);
            Programs = new ProgramRepository(_context);
            Projects = new ProjectRepository(_context);
            ProjectSettings = new ProjectSettingRepository(_context);
            ProgramTypes = new ProgramTypeRepository(_context);
            TableMakerRecords = new TableMakerRecordRepository(_context);
            TableMakerProducts = new TableMakerProductRepository(_context);
            TableMakerProductTypes = new TableMakerProductTypeRepository(_context);
            Events = new EventRepository(_context);
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
