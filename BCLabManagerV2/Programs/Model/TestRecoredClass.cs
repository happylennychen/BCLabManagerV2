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
        public String Comment { get; set; }
        public List<RawDataClass> RawDataList { get; set; }
        //public Double NewCycle { get; set; }
        private double _newCycle = 1;
        public double NewCycle
        {
            get { return _newCycle; }
            set { SetProperty(ref _newCycle, value); }
        }
        private double _measurementGain  ;
        public double MeasurementGain
        {
            get { return _measurementGain; }
            set { SetProperty(ref _measurementGain, value); }
        }
        private double _measurementOffset;
        public double MeasurementOffset
        {
            get { return _measurementOffset; }
            set { SetProperty(ref _measurementOffset, value); }
        }
        private double _traceResistance;
        public double TraceResistance
        {
            get { return _traceResistance; }
            set { SetProperty(ref _traceResistance, value); }
        }
        private double _capacityDifference;
        public double CapacityDifference
        {
            get { return _capacityDifference; }
            set { SetProperty(ref _capacityDifference, value); }
        }
        private string _testFilePath;
        public string TestFilePath
        {
            get { return _testFilePath; }
            set { SetProperty(ref _testFilePath, value); }
        }
        private string _operator;
        public string Operator
        {
            get { return _operator; }
            set { SetProperty(ref _operator, value); }
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
            this.Comment = String.Empty;
            this.RawDataList = new List<RawDataClass>();
            //this.RawData = ??
            //this.NewCycle = ??
        }

        public void Abandon(String comment = "")
        {
            this.Status = TestStatus.Abandoned;
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
