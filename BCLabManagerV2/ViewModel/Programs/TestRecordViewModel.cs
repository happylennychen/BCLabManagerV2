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
        }

        private void _record_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
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
        public string FilePath
        {
            get
            {
                return _record.FilePath;
            }
            set
            {
                if (value == _record.FilePath)
                    return;

                _record.FilePath = value;

                base.OnPropertyChanged("FilePath");
            }
        }
        #endregion // Presentation Properties


        #region Public Interface
        public void ExecuteOnAssets(BatteryClass battery, ChamberClass chamber, ChannelClass channel)
        {
            _record.AssetsExecute(battery, chamber, channel, this.Steps, this.StartTime, "", "");
            OnPropertyChanged("Status");
        }
        public void CommitOnAssets()
        {
            _record.AssetsCommit(this.EndTime, null, this.NewCycle, this.Comment);
            OnPropertyChanged("Status");
        }
        public void Invalidate()
        {
            _record.Invalidate(this.Comment);
            //OnPropertyChanged("Status");
        }
        #endregion
    }
}
