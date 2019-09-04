using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TestRecordViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields
        readonly TestRecordClass _record;

        #endregion // Fields

        #region Constructor

        public TestRecordViewModel(
            TestRecordClass record)     //
        {
            _record = record;
            //_record.PropertyChanged += _record_PropertyChanged;
        }

        //private void _record_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion // Constructor

        #region Presentation Properties

        public int Id
        {
            get { return _record.Id; }
            set
            {
                if (value == _record.Id)
                    return;

                _record.Id = value;

                base.OnPropertyChanged("Id");
            }
        }
        public string BatteryTypeStr
        {
            get { return _record.BatteryTypeStr; }
            set
            {
                if (value == _record.BatteryTypeStr)
                    return;

                _record.BatteryTypeStr = value;

                base.OnPropertyChanged("BatteryTypeStr");
            }
        }
        public string BatteryStr
        {
            get { return _record.BatteryStr; }
            set
            {
                if (value == _record.BatteryStr)
                    return;

                _record.BatteryStr = value;

                base.OnPropertyChanged("BatteryStr");
            }
        }
        public string TesterStr
        {
            get { return _record.TesterStr; }
            set
            {
                if (value == _record.TesterStr)
                    return;

                _record.TesterStr = value;

                base.OnPropertyChanged("TesterStr");
            }
        }
        public string ChannelStr
        {
            get { return _record.ChannelStr; }
            set
            {
                if (value == _record.ChannelStr)
                    return;

                _record.ChannelStr = value;

                base.OnPropertyChanged("ChannelStr");
            }
        }
        public string ChamberStr
        {
            get { return _record.ChamberStr; }
            set
            {
                if (value == _record.ChamberStr)
                    return;

                _record.ChamberStr = value;

                base.OnPropertyChanged("ChamberStr");
            }
        }
        public TestStatus Status
        {
            get
            {
                return _record.Status;
            }
            set
            {
                if (value == _record.Status)
                    return;

                _record.Status = value;

                base.OnPropertyChanged("Status");
            }
        }

        public DateTime StartTime
        {
            get
            {
                return _record.StartTime;
            }
            set
            {
                if (value == _record.StartTime)
                    return;

                _record.StartTime = value;

                base.OnPropertyChanged("StartTime");
            }
        }

        public DateTime EndTime
        {
            get
            {
                return _record.EndTime;
            }
            set
            {
                if (value == _record.EndTime)
                    return;

                _record.EndTime = value;

                base.OnPropertyChanged("EndTime");
            }
        }

        public String Comment
        {
            get
            {
                return _record.Comment;
            }
            set
            {
                if (value == _record.Comment)
                    return;

                _record.Comment = value;

                base.OnPropertyChanged("Comment");
            }
        }
        public String Steps
        {
            get
            {
                return _record.Steps;
            }
            set
            {
                if (value == _record.Steps)
                    return;

                _record.Steps = value;

                base.OnPropertyChanged("Steps");
            }
        }

        public double NewCycle
        {
            get
            {
                return _record.NewCycle;
            }
            set
            {
                if (value == _record.NewCycle)
                    return;

                _record.NewCycle = value;

                base.OnPropertyChanged("NewCycle");
            }
        }

        #endregion // Presentation Properties

        #region Public Interface
        public void Execute()
        {
            //_record.Execute(this.Battery, this.Chamber, this.Channel, this.Steps, this.StartTime, this._programName, this._subProgramName);
            //OnPropertyChanged("Status");

            //battery.Records.Add(new AssetUsageRecordClass(startTime, AssetStatusEnum.USING, programName, subProgramName));
            //battery.Status = AssetStatusEnum.USING;
            //if (chamber != null)
            //{
            //    chamber.Records.Add(new AssetUsageRecordClass(startTime, AssetStatusEnum.USING, programName, subProgramName));
            //    chamber.Status = AssetStatusEnum.USING;
            //}
            //channel.Records.Add(new AssetUsageRecordClass(startTime, AssetStatusEnum.USING, programName, subProgramName));
            //channel.Status = AssetStatusEnum.USING;

            //this.Status = TestStatus.Executing;
            //this.BatteryTypeStr = battery.BatteryType.Name;
            //this.BatteryStr = battery.Name;
            //this.TesterStr = channel.Tester.Name;
            //this.ChannelStr = channel.Name;
            //if (chamber != null)
            //    this.ChamberStr = chamber.Name;
            //this.StartTime = startTime;
            //this.Steps = steps;
            //this.ProgramStr = programName;
            //this.SubProgramStr = subProgramName;

            //var dbContext = new AppDbContext();
            
            //var bat = dbContext.Batteries.SingleOrDefault(i => i.Id == this.Battery.Id);
            //dbContext.Entry(bat)
            //    .Collection(i => i.Records)
            //    .Load();
            //dbContext.Entry(bat)
            //    .Reference(i => i.BatteryType)
            //    .Load();
            //bat.Records.Add(new AssetUsageRecordClass(StartTime, AssetStatusEnum.USING, _programName, _subProgramName));
            //bat.Status = AssetStatusEnum.USING;

            //var tr = dbContext.TestRecords.SingleOrDefault(i => i.Id == this.Id);

            //tr.Status = TestStatus.Executing;
            //tr.BatteryTypeStr = bat.BatteryType.Name;
            //tr.BatteryStr = bat.Name;
            //tr.StartTime = StartTime;
            //tr.Steps = Steps;
            //tr.ProgramStr = _programName;
            //tr.SubProgramStr = _subProgramName;
            //dbContext.SaveChanges();
        }
        public void Commit()
        {
            //_record.Commit(this.Battery, this.Chamber, this.Channel, this.EndTime, null, this.NewCycle, this.Comment);
            //OnPropertyChanged("Status");


            //var dbContext = new AppDbContext();

            //var bat = dbContext.Batteries.SingleOrDefault(i => i.Id == this.Battery.Id);
            //dbContext.Entry(bat)
            //    .Collection(i => i.Records)
            //    .Load();
            //dbContext.Entry(bat)
            //    .Reference(i => i.BatteryType)
            //    .Load();
            //bat.Records.Add(new AssetUsageRecordClass(EndTime, AssetStatusEnum.IDLE, "", ""));
            //bat.Status = AssetStatusEnum.IDLE;

            //var tr = dbContext.TestRecords.SingleOrDefault(i => i.Id == this.Id);
            //tr.Status = TestStatus.Completed;
            //tr.EndTime = EndTime;
            //tr.RawData = null;
            //tr.NewCycle = NewCycle;
            //tr.Comment = Comment;
            //dbContext.SaveChanges();
        }
        public void Invalidate()
        {
            //_record.Invalidate(this.Comment);
            //OnPropertyChanged("Status");


            //var dbContext = new AppDbContext();

            //var tr = dbContext.TestRecords.SingleOrDefault(i => i.Id == this.Id);
            //tr.Status = TestStatus.Invalid;
            //tr.Comment += "\n" + Comment;

            //dbContext.SaveChanges();
        }
        #endregion
    }
}
