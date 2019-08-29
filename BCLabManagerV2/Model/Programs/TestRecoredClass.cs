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
    public class RawDataClass : ModelBase
    {
        public int Id { get; set; }
    }
    public class StatusChangedEventArgs : EventArgs
    {
        public StatusChangedEventArgs(TestStatus newStatus)
        {
            this.Status = newStatus;
        }

        public TestStatus Status { get; private set; }
    }
    public class TestRecordClass : ModelBase
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
        public String BatteryTypeStr { get; set; }
        public String BatteryStr { get; set; }
        public String TesterStr { get; set; }
        public String ChannelStr { get; set; }
        public String ChamberStr { get; set; }
        public String SubProgramStr { get; set; }
        public String ProgramStr { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public String Steps { get; set; }
        public String Comment { get; set; }
        public RawDataClass RawData { get; set; }
        public Double NewCycle { get; set; }

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
            //this.StartTime = ??;
            this.Steps = String.Empty;
            this.Comment = String.Empty;
            //this.RawData = ??
            //this.NewCycle = ??
        }

        public void Execute(BatteryClass battery, ChamberClass chamber, ChannelClass channel, String steps, DateTime startTime, string programName, string subProgramName)
        {
            battery.Records.Add(new AssetUsageRecordClass(startTime, AssetStatusEnum.USING, programName, subProgramName));
            battery.Status = AssetStatusEnum.USING;
            if (chamber != null)
            {
                chamber.Records.Add(new AssetUsageRecordClass(startTime, AssetStatusEnum.USING, programName, subProgramName));
                chamber.Status = AssetStatusEnum.USING;
            }
            channel.Records.Add(new AssetUsageRecordClass(startTime, AssetStatusEnum.USING, programName, subProgramName));
            channel.Status = AssetStatusEnum.USING;

            this.Status = TestStatus.Executing;
            this.BatteryTypeStr = battery.BatteryType.Name;
            this.BatteryStr = battery.Name;
            this.TesterStr = channel.Tester.Name;
            this.ChannelStr = channel.Name;
            if(chamber!=null)
                this.ChamberStr = chamber.Name;
            this.StartTime = startTime;
            this.Steps = steps;
            this.ProgramStr = programName;
            this.SubProgramStr = subProgramName;
        }

        public void Commit(BatteryClass battery, ChamberClass chamber, ChannelClass channel, DateTime endTime, RawDataClass rawData, Double newCycle, String comment = "")  //Need to check the Executor Status to make sure it is executing
        {
            battery.Records.Add(new AssetUsageRecordClass(endTime, AssetStatusEnum.IDLE, "", ""));
            battery.Status = AssetStatusEnum.IDLE;
            if (chamber != null)
            {
                chamber.Records.Add(new AssetUsageRecordClass(endTime, AssetStatusEnum.IDLE, "", ""));
                chamber.Status = AssetStatusEnum.IDLE;
            }
            channel.Records.Add(new AssetUsageRecordClass(endTime, AssetStatusEnum.IDLE, "", ""));
            channel.Status = AssetStatusEnum.IDLE;

            this.Status = TestStatus.Completed;
            this.EndTime = endTime;
            this.RawData = rawData;
            this.NewCycle = newCycle;
            this.Comment = comment;
        }

        public void Abandon(String comment = "")
        {
        }

        public void Invalidate(String comment = "")
        {
            this.Status = TestStatus.Invalid;
            this.Comment += "\n" + comment;
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
    // Summary:
    //     Represents a sub program which can be united to form a program

}
