using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TestRecordCommitViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields
        //string _programName;
        //string _subProgramName;
        readonly TestRecordClass _record;
        //List<BatteryTypeClass> _batteryTypes;
        //List<BatteryClass> _batteries;
        //List<TesterClass> _testers;
        //List<ChannelClass> _channels;
        //List<ChamberClass> _chambers;

        //ObservableCollection<BatteryTypeClass> _allBatteryTypes;
        BatteryTypeClass _batteryType;
        ObservableCollection<BatteryClass> _allBatteries;
        BatteryClass _battery;
        //ObservableCollection<TesterClass> _allTesters;
        TesterClass _tester;
        ObservableCollection<ChannelClass> _allChannels;
        ChannelClass _channel;
        //ObservableCollection<ChamberClass> _allChambers;
        ChamberClass _chamber;
        //RelayCommand _executeCommand;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public TestRecordCommitViewModel(
            TestRecordClass record
            )     //
        {
            if (record == null)
                throw new ArgumentNullException("record");

            _record = record;
            //_batteryTypes = batteryTypes;
            //_batteries = batteries;
            //_testers = testers;
            //_channels = channels;
            //_chambers = chambers;

            _record.PropertyChanged += _record_PropertyChanged;
        }

        private void _record_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion // Constructor

        #region Presentation Properties

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

        public BatteryClass Battery   //选中项
        {
            get
            {
                //if (_record.BatteryStr == null)
                //return "/";
                return _battery;
            }
            set
            {
                if (value == _battery)
                    return;

                _battery = value;

                base.OnPropertyChanged("Battery");
            }
        }

        public ObservableCollection<BatteryClass> AllBatteries    //供选项
        {
            get
            {
                if (_allBatteries == null)
                    _allBatteries = new ObservableCollection<BatteryClass>();
                return _allBatteries;
            }
            set
            {
                if (value != _allBatteries)
                {
                    _allBatteries = value;
                    base.OnPropertyChanged("AllBatteries");
                }
            }
        }

        public ChamberClass Chamber   //选中项
        {
            get
            {
                //if (_record.ChamberStr == null)
                //return "/";
                return _chamber;
            }
            set
            {
                if (value == _chamber)
                    return;

                _chamber = value;

                base.OnPropertyChanged("Chamber");
            }
        }

        public ChannelClass Channel
        {
            get
            {
                //if (_channel == null)
                //return "/";
                return _channel;
            }
            set
            {
                if (value == _channel)
                    return;

                _channel = value;

                base.OnPropertyChanged("Channel");
            }
        }

        public ObservableCollection<ChannelClass> AllChannels    //供选项
        {
            get
            {
                if (_allChannels == null)
                    _allChannels = new ObservableCollection<ChannelClass>();
                return _allChannels;
            }
            set
            {
                if (value != _allChannels)
                {
                    _allChannels = value;
                    base.OnPropertyChanged("AllChannels");
                }
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


        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    switch (operationType)
                    {
                        case OperationType.Execute:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); }//,
                                //param => this.CanExecute
                                );
                            break;
                    }
                }
                return _okCommand;
            }
        }
        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        public void OK()
        {
            //if (!_programtype.IsValid)
            //throw new InvalidOperationException(Resources.ProgramTypeViewModel_Exception_CannotSave);

            //if (this.IsNewProgramType)
            //_programtypeRepository.AddItem(_programtype);

            //base.OnPropertyChanged("DisplayName");
            IsOK = true;
        }

        public OperationType operationType
        { get; set; }

        public bool IsOK
        {
            get { return _isOK; }
            set { _isOK = value; }
        }
    }
}
