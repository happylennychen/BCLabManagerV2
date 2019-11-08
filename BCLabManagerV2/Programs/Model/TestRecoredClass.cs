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
        private DateTime startTime;
        private DateTime completeTime;
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
        public String BatteryTypeStr { get; set; }
        public String BatteryStr { get; set; }
        public String TesterStr { get; set; }
        public String ChannelStr { get; set; }
        public String ChamberStr { get; set; }
        public String SubProgramStr { get; set; }
        public String ProgramStr { get; set; }
        public DateTime StartTime
        {
            get => startTime;
            set
            {
                startTime = value;
                RaisePropertyChanged("StartTime");
            }
        }
        public DateTime CompleteTime
        {
            get => completeTime;
            set
            {
                completeTime = value;
                RaisePropertyChanged("CompleteTime");
            }
        }
        public String Steps { get; set; }
        public String Comment { get; set; }
        public List<RawDataClass> RawDataList { get; set; }
        public Double NewCycle { get; set; }

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

        public TestRecordClass(/*SubProgramClass SubProgramStr*/)
        {
            this.Status = TestStatus.Waiting;
            this.BatteryTypeStr = String.Empty;
            this.BatteryStr = String.Empty;
            this.TesterStr = String.Empty;
            this.ChannelStr = String.Empty;
            this.ChamberStr = String.Empty;
            this.ProgramStr = String.Empty;
            this.SubProgramStr = String.Empty;
            this.StartTime = DateTime.MinValue;
            this.CompleteTime = DateTime.MinValue;
            this.Steps = String.Empty;
            this.Comment = String.Empty;
            this.RawDataList = new List<RawDataClass>();
            //this.RawData = ??
            //this.NewCycle = ??
        }

        public void AssetsExecute(BatteryClass battery, ChamberClass chamber, ChannelClass channel, String steps, DateTime startTime, string programName, string subProgramName)
        {
            //分配Assets
            AssignedBattery = battery;
            AssignedChamber = chamber;
            AssignedChannel = channel;

            battery.AssetUseCount++;
            battery.Records.Add(new AssetUsageRecordClass(startTime, battery.AssetUseCount, programName, subProgramName));
            if (chamber != null)
            {
                chamber.AssetUseCount++;
                chamber.Records.Add(new AssetUsageRecordClass(startTime, chamber.AssetUseCount, programName, subProgramName));
            }
            channel.AssetUseCount++;
            channel.Records.Add(new AssetUsageRecordClass(startTime, channel.AssetUseCount, programName, subProgramName));

            using (var dbContext = new AppDbContext())
            {
                var dbAssignedBattery = dbContext.Batteries.SingleOrDefault(o => o.Id == battery.Id);
                dbContext.Entry(dbAssignedBattery)
                    .Collection(o => o.Records)
                    .Load();

                dbAssignedBattery.AssetUseCount++;
                dbAssignedBattery.Records.Add(new AssetUsageRecordClass(startTime, dbAssignedBattery.AssetUseCount, programName, subProgramName));

                if (chamber != null)
                {
                    var dbAssignedChamber = dbContext.Chambers.SingleOrDefault(o => o.Id == chamber.Id);
                    dbContext.Entry(dbAssignedChamber)
                        .Collection(o => o.Records)
                        .Load();
                    dbAssignedChamber.AssetUseCount++;
                    dbAssignedChamber.Records.Add(new AssetUsageRecordClass(startTime, dbAssignedChamber.AssetUseCount, programName, subProgramName));
                }
                var dbAssignedChannel = dbContext.Channels.SingleOrDefault(o => o.Id == channel.Id);
                dbContext.Entry(dbAssignedChannel)
                    .Collection(o => o.Records)
                    .Load();

                dbAssignedChannel.AssetUseCount++;
                dbAssignedChannel.Records.Add(new AssetUsageRecordClass(startTime, dbAssignedChannel.AssetUseCount, programName, subProgramName));

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
            //this.SubProgramStr = subProgramName;
        }

        public void AssetsCommit(DateTime CompleteTime, RawDataClass rawData, double newCycle, string comment = "")  //Need to check the Executor Status to make sure it is executing
        {
            AssignedBattery.CycleCount += newCycle;
            AssignedBattery.AssetUseCount--;
            AssignedBattery.Records.Add(new AssetUsageRecordClass(CompleteTime, AssignedBattery.AssetUseCount, "", ""));
            if (AssignedChamber != null)
            {
                AssignedChamber.AssetUseCount--;
                AssignedChamber.Records.Add(new AssetUsageRecordClass(CompleteTime, AssignedChamber.AssetUseCount, "", ""));
            }
            AssignedChannel.AssetUseCount--;
            AssignedChannel.Records.Add(new AssetUsageRecordClass(CompleteTime, AssignedChannel.AssetUseCount, "", ""));

            using (var dbContext = new AppDbContext())
            {
                var dbAssignedBattery = dbContext.Batteries.SingleOrDefault(o => o.Id == AssignedBattery.Id);
                dbContext.Entry(dbAssignedBattery)
                    .Collection(o => o.Records)
                    .Load();
                dbAssignedBattery.CycleCount += newCycle;
                dbAssignedBattery.AssetUseCount--;
                dbAssignedBattery.Records.Add(new AssetUsageRecordClass(CompleteTime, dbAssignedBattery.AssetUseCount, "", ""));

                if (AssignedChamber != null)
                {
                    var dbAssignedChamber = dbContext.Chambers.SingleOrDefault(o => o.Id == AssignedChamber.Id);
                    dbContext.Entry(dbAssignedChamber)
                        .Collection(o => o.Records)
                        .Load();
                    dbAssignedChamber.AssetUseCount--;
                    dbAssignedChamber.Records.Add(new AssetUsageRecordClass(CompleteTime, dbAssignedChamber.AssetUseCount, "", ""));
                }
                var dbAssignedChannel = dbContext.Channels.SingleOrDefault(o => o.Id == AssignedChannel.Id);
                dbContext.Entry(dbAssignedChannel)
                    .Collection(o => o.Records)
                    .Load();

                dbAssignedChannel.AssetUseCount--;
                dbAssignedChannel.Records.Add(new AssetUsageRecordClass(CompleteTime, dbAssignedChannel.AssetUseCount, "", ""));

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

        public void ExeuteUpdateTime(ProgramClass _program, RecipeClass _subprogram)
        {

            if (IsSubStarted(_subprogram) == false)
                _subprogram.StartTime = this.StartTime;
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

        public void CommitUpdateTime(ProgramClass _program, RecipeClass _subprogram)
        {
            if (IsSubCompleted(_subprogram))
                _subprogram.CompleteTime = this.CompleteTime;
            if (IsProCompleted(_program))
                _program.CompleteTime = this.CompleteTime;

            using (var dbContext = new AppDbContext())
            {
                RecipeClass sub = null;
                ProgramClass pro = null;
                GetParentNode(dbContext, ref pro, ref sub);

                if (sub != null & pro != null)
                {
                    if (IsSubCompleted(sub))
                        sub.CompleteTime = this.CompleteTime;
                    if (IsProCompleted(pro))
                        pro.CompleteTime = this.CompleteTime;
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
