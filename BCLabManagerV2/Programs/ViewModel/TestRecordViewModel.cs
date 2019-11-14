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
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TestRecordViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        private TestRecordClass _record;

        public TestRecordClass Record
        {
            get { return _record; }
            set { _record = value; }
        }


        #endregion // Fields

        #region Constructor

        public TestRecordViewModel(
            TestRecordClass record)     //
        {
            _record = record;
            _record.PropertyChanged += _record_PropertyChanged;
            _record.StatusChanged += _record_StatusChanged;
        }

        private void _record_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            RaisePropertyChanged("WaitingPercentage");
            RaisePropertyChanged("ExecutingPercentage");
            RaisePropertyChanged("CompletedPercentage");
            RaisePropertyChanged("InvalidPercentage");
            RaisePropertyChanged("AbandonedPercentage");
        }

        private void _record_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            if (e.PropertyName == "CompleteTime")
                RaisePropertyChanged("Duration");
        }

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

                RaisePropertyChanged("Id");
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

                RaisePropertyChanged("BatteryTypeStr");
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

                RaisePropertyChanged("BatteryStr");
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

                RaisePropertyChanged("TesterStr");
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

                RaisePropertyChanged("ChannelStr");
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

                RaisePropertyChanged("ChamberStr");
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

                RaisePropertyChanged("Status");
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

                RaisePropertyChanged("StartTime");
            }
        }

        public DateTime CompleteTime
        {
            get
            {
                return _record.CompleteTime;
            }
            set
            {
                if (value == _record.CompleteTime)
                    return;

                _record.CompleteTime = value;

                RaisePropertyChanged("CompleteTime");
            }
        }

        public TimeSpan Duration
        {
            get
            {
                if (CompleteTime == DateTime.MinValue)
                    return TimeSpan.Zero;
                return CompleteTime - StartTime;
            }
        }

        public string ProgramStr
        {
            get
            {
                return _record.ProgramStr;
            }
            set
            {
                if (value == _record.ProgramStr)
                    return;

                _record.ProgramStr = value;

                RaisePropertyChanged("ProgramStr");
            }
        }

        public string RecipeStr
        {
            get
            {
                return _record.RecipeStr;
            }
            set
            {
                if (value == _record.RecipeStr)
                    return;

                _record.RecipeStr = value;

                RaisePropertyChanged("ProgramStr");
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

                RaisePropertyChanged("Comment");
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

                RaisePropertyChanged("Steps");
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

                RaisePropertyChanged("NewCycle");
            }
        }
        #endregion // Presentation Properties

        #region Presentation logic
        public string WaitingPercentage
        {
            get
            {
                if (_record.Status == TestStatus.Waiting)
                    return "1*";
                else
                    return "0*";
            }
        }

        public string ExecutingPercentage
        {
            get
            {
                if (_record.Status == TestStatus.Executing)
                    return "1*";
                else
                    return "0*";
            }
        }
        public string CompletedPercentage
        {
            get
            {
                if (_record.Status == TestStatus.Completed)
                    return "1*";
                else
                    return "0*";
            }
        }
        public string InvalidPercentage
        {
            get
            {
                if (_record.Status == TestStatus.Invalid)
                    return "1*";
                else
                    return "0*";
            }
        }
        public string AbandonedPercentage
        {
            get
            {
                if (_record.Status == TestStatus.Abandoned)
                    return "1*";
                else
                    return "0*";
            }
        }
        #endregion
    }
}
