﻿using System;
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
    public class TestRecordViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields
        string _programName;
        string _subProgramName;
        readonly TestRecordClass _record;
        readonly BatteryTypeRepository _batterytypeRepository;
        readonly BatteryRepository _batteryRepository;
        readonly ChamberRepository _chamberRepository;
        readonly TesterRepository _testerRepository;
        readonly ChannelRepository _channelRepository;

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

        public TestRecordViewModel(TestRecordClass record, string programName, string subProgramName)     //
        {
            if (record == null)
                throw new ArgumentNullException("record");

            _record = record;
            _programName = programName;
            _subProgramName = subProgramName;
            _batteryRepository = Repositories._batteryRepository;
            _batterytypeRepository = Repositories._batterytypeRepository;
            _chamberRepository = Repositories._chamberRepository;
            _testerRepository = Repositories._testerRepository;
            _channelRepository = Repositories._channelRepository;
            _record.PropertyChanged += _record_PropertyChanged;
        }

        private void _record_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
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

        public BatteryTypeClass BatteryType       //选中项
        {
            get
            {
                //if (_batteryType == null)
                    //return null;
                return _batteryType;
            }
            set
            {
                if (value == _batteryType)
                    return;

                _batteryType = value;

                base.OnPropertyChanged("BatteryType");


                List<BatteryClass> all = _batteryRepository.GetItems();
                List<BatteryClass> allstring = (
                    from i in all
                    where (i.BatteryType == BatteryType) && i.Status == AssetStatusEnum.IDLE
                    select i).ToList();

                AllBatteries = new ObservableCollection<BatteryClass>(allstring);
            }
        }

        public ObservableCollection<BatteryTypeClass> AllBatteryTypes //供选项
        {
            get
            {
                List<BatteryTypeClass> all = _batterytypeRepository.GetItems();

                return new ObservableCollection<BatteryTypeClass>(all);
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

        public ObservableCollection<ChamberClass> AllChambers    //供选项
        {
            get
            {
                List<ChamberClass> all = _chamberRepository.GetItems();
                List<ChamberClass> allstring = (
                    from i in all
                    where i.Status == AssetStatusEnum.IDLE
                    select i).ToList();

                return new ObservableCollection<ChamberClass>(allstring);
            }
        }

        public TesterClass Tester
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

                base.OnPropertyChanged("Tester");


                List<ChannelClass> all = _channelRepository.GetItems();
                List<ChannelClass> allstring = (
                    from i in all
                    where (i.Tester == Tester) && i.Status == AssetStatusEnum.IDLE
                    select i).ToList();

                AllChannels = new ObservableCollection<ChannelClass>(allstring);
            }
        }

        public ObservableCollection<TesterClass> AllTesters    //供选项
        {
            get
            {
                List<TesterClass> all = _testerRepository.GetItems();

                return new ObservableCollection<TesterClass>(all);
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

        #region Public Interface
        public void Execute()
        {
            _record.Execute(this.Battery, this.Chamber, this.Channel, this.Steps, this.StartTime, this._programName, this._subProgramName);
            OnPropertyChanged("Status");
        }
        public void Commit()
        {
            _record.Commit(this.Battery, this.Chamber, this.Channel, this.EndTime, null, this.NewCycle, this.Comment);
            OnPropertyChanged("Status");
        }
        public void Invalidate()
        {
            _record.Invalidate(this.Comment);
            OnPropertyChanged("Status");
        }
        #endregion
    }
}
