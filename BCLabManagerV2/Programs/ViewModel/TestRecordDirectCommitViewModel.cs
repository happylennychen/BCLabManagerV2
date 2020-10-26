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
using Microsoft.Win32;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TestRecordDirectCommitViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields
        readonly TestRecord _record;
        ObservableCollection<Battery> _batteries;
        ObservableCollection<Tester> _testers;
        ObservableCollection<Channel> _channels;
        ObservableCollection<Chamber> _chambers;
        ProgramType _type;
        string _programStr;
        string _recipeStr;

        BatteryType _batteryType;
        List<Battery> _allBatteries;
        Battery _battery;
        Tester _tester;
        ObservableCollection<Channel> _allChannels;
        Channel _channel;
        Chamber _chamber;
        RelayCommand _okCommand;
        RelayCommand _openFilesCommand;
        RelayCommand _splitterCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public TestRecordDirectCommitViewModel(
            TestRecord record,
        BatteryType batteryType,
        ObservableCollection<Battery> batteries,
            ObservableCollection<Tester> testers,
            ObservableCollection<Channel> channels,
            ObservableCollection<Chamber> chambers
,
            ProgramType type,
            string programStr,
            string recipeStr)     //
        {
            if (record == null)
                throw new ArgumentNullException("record");

            _record = record;
            _batteryType = batteryType;
            _batteries = batteries;
            _testers = testers;
            _channels = channels;
            _chambers = chambers;
            _type = type;
            _programStr = programStr;
            _recipeStr = recipeStr;

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
                    _allBatteries = _batteries.Where(o => o.BatteryType.Id == _batteryType.Id).ToList();
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

        private ObservableCollection<string> _fileList;

        public ObservableCollection<string> FileList
        {
            get { return _fileList; }
            set
            {
                if (value == _fileList)
                    return;
                _fileList = value;

                RaisePropertyChanged("FileList");
            }
        }


        private bool _isRename = true;
        public bool IsRename
        {
            get
            {
                return _isRename;
            }
            set
            {
                if (value == _isRename)
                    return;

                _isRename = value;

                RaisePropertyChanged("IsRename");
            }
        }


        private string _newName;
        public string NewName
        {
            get
            {
                return _newName;
            }
            set
            {
                if (value == _newName)
                    return;

                _newName = value;

                RaisePropertyChanged("NewName");
            }
        }
        private int _startIndex = 0;
        public int StartIndex
        {
            get
            {
                return _startIndex;
            }
            set
            {
                if (value == _startIndex)
                    return;

                _startIndex = value;

                RaisePropertyChanged("StartIndex");
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

        public ICommand OpenFilesCommand
        {
            get
            {
                if (_openFilesCommand == null)
                {
                    _openFilesCommand = new RelayCommand(
                        param => { this.OpenFiles(); }//,
                                                      //param => this.CanExecute
                        );
                }
                return _openFilesCommand;
            }
        }
        public void OpenFiles()
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == true)
            {
                TesterServiceClass _testerService = new TesterServiceClass();
                if (Channel != null)
                {
                    foreach (var file in dialog.FileNames)
                    {
                        if (!_testerService.CheckFileFormat(Channel.Tester.ITesterProcesser, file))
                        {
                            MessageBox.Show("Wrong File Format!");
                            return;
                        }
                        if (!_testerService.CheckChannelNumber(Channel.Tester.ITesterProcesser, file, Channel.Name))
                        {
                            MessageBox.Show("Wrong channel!");
                            return;
                        }
                    }

                    FileList = new ObservableCollection<string>(dialog.FileNames.ToList());
                    DateTime[] time = _testerService.GetTimeFromRawData(Channel.Tester.ITesterProcesser, FileList);
                    if (time != null)
                        NewName = $@"{_programStr}_{_recipeStr}_{_tester.Name}_{_channel.Name}_{_battery.Name}_{time[0].ToString("yyyyMMddHHmmss")}";
                    else
                        NewName = $@"{_programStr}_{_recipeStr}_{_tester.Name}_{_channel.Name}_{_battery.Name}";
                }
                else
                {

                }
            }
        }

        public ICommand SplitterCommand
        {
            get
            {
                if (_splitterCommand == null)
                {
                    _splitterCommand = new RelayCommand(
                        param => { this.Splitter(); }
                        );
                }
                return _splitterCommand;
            }
        }
        public void Splitter()
        {
            var SplitterViewInstance = new SplitterView();
            SplitterViewInstance.DataContext = new SplitterViewModel();
            SplitterViewInstance.ShowDialog();
        }
    }
}
