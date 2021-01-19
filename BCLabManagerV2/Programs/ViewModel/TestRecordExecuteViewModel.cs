using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    public enum OperationType
    {
        Execute,
        Commit,
        Invalidate,
        Abandon
    }
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TestRecordExecuteViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields
        //string _programName;
        //string _RecipeName;
        readonly TestRecord _record;
        //readonly BatteryTypeRepository _batterytypeRepository;
        //readonly BatteryRepository _batteryRepository;
        //readonly ChamberRepository _chamberRepository;
        //readonly TesterRepository _testerRepository;
        //readonly ChannelRepository _channelRepository;
        ObservableCollection<Battery> _batteries;
        ObservableCollection<Tester> _testers;
        ObservableCollection<Channel> _channels;
        ObservableCollection<Chamber> _chambers;
        ProgramType _type;

        //ObservableCollection<BatteryTypeClass> _allBatteryTypes;
        BatteryType _batteryType;
        List<Battery> _allBatteries;
        Battery _battery;
        //ObservableCollection<Tester> _allTesters;
        Tester _tester;
        ObservableCollection<Channel> _allChannels;
        Channel _channel;
        //ObservableCollection<ChamberClass> _allChambers;
        Chamber _chamber;
        //RelayCommand _executeCommand;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public TestRecordExecuteViewModel(
            TestRecord record,
        BatteryType batteryType,
        ObservableCollection<Battery> batteries,
            ObservableCollection<Tester> testers,
            ObservableCollection<Channel> channels,
            ObservableCollection<Chamber> chambers
,
            ProgramType type)     //
        {
            if (record == null)
                throw new ArgumentNullException("record");

            _record = record;
            //_batteryRepository = Repositories._batteryRepository;
            //_batterytypeRepository = Repositories._batterytypeRepository;
            //_chamberRepository = Repositories._chamberRepository;
            //_testerRepository = Repositories._testerRepository;
            //_channelRepository = Repositories._channelRepository;
            _batteryType = batteryType;
            _batteries = batteries;
            _testers = testers;
            _channels = channels;
            _chambers = chambers;
            _type = type;

            //_record.PropertyChanged += _record_PropertyChanged;
        }

        //private void _record_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

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

                RaisePropertyChanged("Status");
            }
        }

        public Battery Battery   //选中项
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

                RaisePropertyChanged("Battery");
            }
        }

        public List<Battery> AllBatteries    //供选项
        {
            get
            {
                if (_allBatteries == null)
                {
                    _allBatteries = _batteries.Where(o=>o.BatteryType.Id == _batteryType.Id && o.AssetUseCount == 0).ToList();
                }
                return _allBatteries;
            }
            //set
            //{
            //    if (value != _allBatteries)
            //    {
            //        _allBatteries = value;
            //        RaisePropertyChanged("AllBatteries");
            //    }
            //}
        }

        public Chamber Chamber   //选中项
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

                RaisePropertyChanged("Chamber");
                //if (_chamber.AssetUseCount > 0)
                //    MessageBox.Show("Please note that this one is in use by another test");
            }
        }

        public ObservableCollection<Chamber> AllChambers    //供选项
        {
            get
            {
                ObservableCollection<Chamber> all = _chambers;
                List<Chamber> allstring = (
                    from i in all
                    //where i.Status == AssetStatusEnum.IDLE    //正在使用的chamber也可能被选来使用
                    select i).ToList();

                return new ObservableCollection<Chamber>(allstring);
            }
        }

        public Tester Tester
        {
            get
            {
                //if (_record.ChannelStr == null)
                //return "/";
                return _tester;
            }
            set
            {
                if (value == _tester)
                    return;

                _tester = value;

                RaisePropertyChanged("Tester");


                ObservableCollection<Channel> all = _channels;
                List<Channel> allstring = (
                    from i in all
                    where (i.Tester.Id == Tester.Id) && i.AssetUseCount == 0
                    select i).ToList();

                AllChannels = new ObservableCollection<Channel>(allstring);
            }
        }

        public ObservableCollection<Tester> AllTesters    //供选项
        {
            get
            {
                ObservableCollection<Tester> all = _testers;

                return new ObservableCollection<Tester>(all);
            }
        }

        public Channel Channel
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

                RaisePropertyChanged("Channel");
            }
        }

        public ObservableCollection<Channel> AllChannels    //供选项
        {
            get
            {
                if (_allChannels == null)
                    _allChannels = new ObservableCollection<Channel>();
                return _allChannels;
            }
            set
            {
                if (value != _allChannels)
                {
                    _allChannels = value;
                    RaisePropertyChanged("AllChannels");
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

                RaisePropertyChanged("StartTime");
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

                RaisePropertyChanged("EndTime");
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
        public String Operator
        {
            get
            {
                return _record.Operator;
            }
            set
            {
                if (value == _record.Operator)
                    return;

                _record.Operator = value;

                RaisePropertyChanged("Operator");
            }
        }

        public double LastCycle
        {
            get
            {
                return _record.LastCycle;
            }
            set
            {
                if (value == _record.LastCycle)
                    return;

                _record.LastCycle = value;

                RaisePropertyChanged("LastCycle");
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

        public double Current
        {
            get
            {
                return _record.Current;
            }
            set
            {
                if (value == _record.Current)
                    return;

                _record.Current = value;

                RaisePropertyChanged("Current");
            }
        }

        public bool ShowCurrentAndTemperature
        {
            get
            {
                return _type.Name == "RC" || _type.Name == "OCV";
            }
        }

        public double Temperature
        {
            get
            {
                return _record.Temperature;
            }
            set
            {
                if (value == _record.Temperature)
                    return;

                _record.Temperature = value;

                RaisePropertyChanged("Temperature");
            }
        }

        private bool _isSkip = false;
        public bool IsSkip
        {
            get
            {
                return _isSkip;
            }
            set
            {
                if (value == _isSkip)
                    return;

                _isSkip = value;

                RaisePropertyChanged("IsSkip");
            }
        }

        public double MeasurementGain
        {
            get
            {
                return _record.MeasurementGain;
            }
            set
            {
                if (value == _record.MeasurementGain)
                    return;

                _record.MeasurementGain = value;

                RaisePropertyChanged("MeasurementGain");
            }
        }

        public double MeasurementOffset
        {
            get
            {
                return _record.MeasurementOffset;
            }
            set
            {
                if (value == _record.MeasurementOffset)
                    return;

                _record.MeasurementOffset = value;

                RaisePropertyChanged("MeasurementOffset");
            }
        }

        public double TraceResistance
        {
            get
            {
                return _record.TraceResistance;
            }
            set
            {
                if (value == _record.TraceResistance)
                    return;

                _record.TraceResistance = value;

                RaisePropertyChanged("TraceResistance");
            }
        }

        public double CapacityDifference
        {
            get
            {
                return _record.CapacityDifference;
            }
            set
            {
                if (value == _record.CapacityDifference)
                    return;

                _record.CapacityDifference = value;

                RaisePropertyChanged("CapacityDifference");
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

            //RaisePropertyChanged("DisplayName");
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
