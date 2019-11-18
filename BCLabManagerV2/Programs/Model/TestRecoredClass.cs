using BCLabManager.DataAccess;
using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCLabManager.Model
{
    public enum TestStatus
    {
        Waiting,
        Executing,
        Completed,
        Invalid,
        Abandoned,
    }
    public class StatusChangedEventArgs : EventArgs
    {
        public StatusChangedEventArgs(TestStatus newStatus)
        {
            this.Status = newStatus;
        }

        public TestStatus Status { get; private set; }
    }
    public class TestRecordClass : BindableBase
    {
        public int Id { get; set; }
        private TestStatus status = TestStatus.Waiting;
        public TestStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                if (value != status)
                {
                    status = value;
                    OnRasieStatusChangedEvent(new StatusChangedEventArgs(status));
                }
            }
        }
        //public String BatteryTypeStr { get; set; }
        private string _batteryTypeStr;
        public string BatteryTypeStr
        {
            get { return _batteryTypeStr; }
            set { SetProperty(ref _batteryTypeStr, value); }
        }
        //public String BatteryStr { get; set; }
        private string _batteryStr;
        public string BatteryStr
        {
            get { return _batteryStr; }
            set { SetProperty(ref _batteryStr, value); }
        }
        //public String TesterStr { get; set; }
        private string _testerStr;
        public string TesterStr
        {
            get { return _testerStr; }
            set { SetProperty(ref _testerStr, value); }
        }
        //public String ChannelStr { get; set; }
        private string _channelStr;
        public string ChannelStr
        {
            get { return _channelStr; }
            set { SetProperty(ref _channelStr, value); }
        }
        //public String ChamberStr { get; set; }
        private string _chamberStr;
        public string ChamberStr
        {
            get { return _chamberStr; }
            set { SetProperty(ref _chamberStr, value); }
        }
        //public String RecipeStr { get; set; }
        private string _recipeStr;
        public string RecipeStr
        {
            get { return _recipeStr; }
            set { SetProperty(ref _recipeStr, value); }
        }
        //public String ProgramStr { get; set; }
        private string _programStr;
        public string ProgramStr
        {
            get { return _programStr; }
            set { SetProperty(ref _programStr, value); }
        }

        private DateTime _startTime;
        public DateTime StartTime
        {
            get => _startTime;
            set { SetProperty(ref _startTime, value); }
        }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get => _endTime;
            set { SetProperty(ref _endTime, value); }
        }
        public String Steps { get; set; }
        public String Comment { get; set; }
        public List<RawDataClass> RawDataList { get; set; }
        //public Double NewCycle { get; set; }
        private double _newCycle;
        public double NewCycle
        {
            get { return _newCycle; }
            set { SetProperty(ref _newCycle, value); }
        }

        #region Store the assets in use
        public BatteryClass AssignedBattery { get; set; }
        public ChamberClass AssignedChamber { get; set; }
        public ChannelClass AssignedChannel { get; set; }
        #endregion

        public event EventHandler<StatusChangedEventArgs> StatusChanged;

        protected virtual void OnRasieStatusChangedEvent(StatusChangedEventArgs e)
        {
            EventHandler<StatusChangedEventArgs> handler = StatusChanged;

            if (handler != null)
            {
                e = new StatusChangedEventArgs(this.Status);
                handler(this, e);
            }
        }

        public TestRecordClass(/*RecipeClass RecipeStr*/)
        {
            this.Status = TestStatus.Waiting;
            this.BatteryTypeStr = String.Empty;
            this.BatteryStr = String.Empty;
            this.TesterStr = String.Empty;
            this.ChannelStr = String.Empty;
            this.ChamberStr = String.Empty;
            this.ProgramStr = String.Empty;
            this.RecipeStr = String.Empty;
            this.StartTime = DateTime.MinValue;
            this.EndTime = DateTime.MinValue;
            this.Steps = String.Empty;
            this.Comment = String.Empty;
            this.RawDataList = new List<RawDataClass>();
            //this.RawData = ??
            //this.NewCycle = ??
        }

        public void AssetsExecute(BatteryClass battery, ChamberClass chamber, ChannelClass channel, String steps, DateTime startTime, string programName, string RecipeName)
        {
            //分配Assets
            AssignedBattery = battery;
            AssignedChamber = chamber;
            AssignedChannel = channel;

            battery.AssetUseCount++;
            battery.Records.Add(new AssetUsageRecordClass(startTime, battery.AssetUseCount, programName, RecipeName));
            if (chamber != null)
            {
                chamber.AssetUseCount++;
                chamber.Records.Add(new AssetUsageRecordClass(startTime, chamber.AssetUseCount, programName, RecipeName));
            }
            channel.AssetUseCount++;
            channel.Records.Add(new AssetUsageRecordClass(startTime, channel.AssetUseCount, programName, RecipeName));

            using (var dbContext = new AppDbContext())
            {
                var dbAssignedBattery = dbContext.Batteries.SingleOrDefault(o => o.Id == battery.Id);
                dbContext.Entry(dbAssignedBattery)
                    .Collection(o => o.Records)
                    .Load();

                dbAssignedBattery.AssetUseCount++;
                dbAssignedBattery.Records.Add(new AssetUsageRecordClass(startTime, dbAssignedBattery.AssetUseCount, programName, RecipeName));

                if (chamber != null)
                {
                    var dbAssignedChamber = dbContext.Chambers.SingleOrDefault(o => o.Id == chamber.Id);
                    dbContext.Entry(dbAssignedChamber)
                        .Collection(o => o.Records)
                        .Load();
                    dbAssignedChamber.AssetUseCount++;
                    dbAssignedChamber.Records.Add(new AssetUsageRecordClass(startTime, dbAssignedChamber.AssetUseCount, programName, RecipeName));
                }
                var dbAssignedChannel = dbContext.Channels.SingleOrDefault(o => o.Id == channel.Id);
                dbContext.Entry(dbAssignedChannel)
                    .Collection(o => o.Records)
                    .Load();

                dbAssignedChannel.AssetUseCount++;
                dbAssignedChannel.Records.Add(new AssetUsageRecordClass(startTime, dbAssignedChannel.AssetUseCount, programName, RecipeName));

                dbContext.SaveChanges();
            }
            //this.Status = TestStatus.Executing;
            //this.BatteryTypeStr = battery.BatteryType.Name;
            //this.BatteryStr = battery.Name;
            //this.TesterStr = channel.Tester.Name;
            //this.ChannelStr = channel.Name;
            //if(chamber!=null)
            //    this.ChamberStr = chamber.Name;
            //this.StartTime = startTime;
            //this.Steps = steps;
            //this.ProgramStr = programName;
            //this.RecipeStr = RecipeName;
        }

        public void AssetsCommit(DateTime EndTime, RawDataClass rawData, double newCycle, string comment = "")  //Need to check the Executor Status to make sure it is executing
        {
            AssignedBattery.CycleCount += newCycle;
            AssignedBattery.AssetUseCount--;
            AssignedBattery.Records.Add(new AssetUsageRecordClass(EndTime, AssignedBattery.AssetUseCount, "", ""));
            if (AssignedChamber != null)
            {
                AssignedChamber.AssetUseCount--;
                AssignedChamber.Records.Add(new AssetUsageRecordClass(EndTime, AssignedChamber.AssetUseCount, "", ""));
            }
            AssignedChannel.AssetUseCount--;
            AssignedChannel.Records.Add(new AssetUsageRecordClass(EndTime, AssignedChannel.AssetUseCount, "", ""));

            using (var dbContext = new AppDbContext())
            {
                var dbAssignedBattery = dbContext.Batteries.SingleOrDefault(o => o.Id == AssignedBattery.Id);
                dbContext.Entry(dbAssignedBattery)
                    .Collection(o => o.Records)
                    .Load();
                dbAssignedBattery.CycleCount += newCycle;
                dbAssignedBattery.AssetUseCount--;
                dbAssignedBattery.Records.Add(new AssetUsageRecordClass(EndTime, dbAssignedBattery.AssetUseCount, "", ""));

                if (AssignedChamber != null)
                {
                    var dbAssignedChamber = dbContext.Chambers.SingleOrDefault(o => o.Id == AssignedChamber.Id);
                    dbContext.Entry(dbAssignedChamber)
                        .Collection(o => o.Records)
                        .Load();
                    dbAssignedChamber.AssetUseCount--;
                    dbAssignedChamber.Records.Add(new AssetUsageRecordClass(EndTime, dbAssignedChamber.AssetUseCount, "", ""));
                }
                var dbAssignedChannel = dbContext.Channels.SingleOrDefault(o => o.Id == AssignedChannel.Id);
                dbContext.Entry(dbAssignedChannel)
                    .Collection(o => o.Records)
                    .Load();

                dbAssignedChannel.AssetUseCount--;
                dbAssignedChannel.Records.Add(new AssetUsageRecordClass(EndTime, dbAssignedChannel.AssetUseCount, "", ""));

                dbContext.SaveChanges();
            }
            //释放Assets
            AssignedBattery = null;
            AssignedChamber = null;
            AssignedChannel = null;
        }

        public void Abandon(String comment = "")
        {
            this.Status = TestStatus.Abandoned;
        }

        public void Invalidate(String comment = "")
        {
            this.Status = TestStatus.Invalid;
            this.Comment = comment;
        }

        public void ExeuteUpdateTime(ProgramClass _program, RecipeClass _Recipe)
        {

            if (IsSubStarted(_Recipe) == false)
                _Recipe.StartTime = this.StartTime;
            if (IsProStarted(_program) == false)
                _program.StartTime = this.StartTime;

            using (var dbContext = new AppDbContext())
            {
                RecipeClass sub = null;
                ProgramClass pro = null;
                GetParentNode(dbContext, ref pro, ref sub);

                if (sub != null & pro != null)
                {
                    if (IsSubStarted(sub) == false)
                        sub.StartTime = this.StartTime;
                    if (IsProStarted(pro) == false)
                        pro.StartTime = this.StartTime;
                }
                dbContext.SaveChanges();
            }
        }

        public void CommitUpdateTime(ProgramClass _program, RecipeClass _Recipe)
        {
            if (IsSubCompleted(_Recipe))
                _Recipe.EndTime = this.EndTime;
            if (IsProCompleted(_program))
                _program.EndTime = this.EndTime;

            using (var dbContext = new AppDbContext())
            {
                RecipeClass sub = null;
                ProgramClass pro = null;
                GetParentNode(dbContext, ref pro, ref sub);

                if (sub != null & pro != null)
                {
                    if (IsSubCompleted(sub))
                        sub.EndTime = this.EndTime;
                    if (IsProCompleted(pro))
                        pro.EndTime = this.EndTime;
                }
                dbContext.SaveChanges();
            }
        }

        private void GetParentNode(AppDbContext dbContext, ref ProgramClass Pro, ref RecipeClass Sub)
        {
            var pros = dbContext.Programs
                 .Include(pro => pro.Recipes)
                    .ThenInclude(sub => sub.TestRecords)
                 .ToList();
            foreach (var p in pros)
            {
                foreach (var s in p.Recipes)
                {
                    foreach (var t in s.TestRecords)
                    {
                        if (t.Id == this.Id)
                        {
                            Sub = s;
                            Pro = p;
                            break;
                        }
                    }
                    if (Sub != null)
                        break;
                }
                if (Sub != null)
                    break;
            }
        }

        private bool IsProStarted(ProgramClass pro)
        {
            return pro.StartTime != DateTime.MinValue;
        }

        private bool IsSubStarted(RecipeClass sub)
        {
            return sub.StartTime != DateTime.MinValue;
        }

        private bool IsProCompleted(ProgramClass pro)
        {
            foreach (var sub in pro.Recipes)
            {
                if (IsSubCompleted(sub) == false)
                    return false;
            }
            return true;
        }

        private bool IsSubCompleted(RecipeClass sub)
        {
            return sub.TestRecords[sub.TestRecords.Count - 1].Status == TestStatus.Completed;
        }
    }
    public class TestRecordAddedEventArgs : EventArgs
    {
        public TestRecordAddedEventArgs(TestRecordClass newTestRecord, bool isFirst)
        {
            this.NewTestRecord = newTestRecord;
            this.IsFirst = isFirst;
        }

        public TestRecordClass NewTestRecord { get; private set; }
        public bool IsFirst { get; private set; }
    }

}
