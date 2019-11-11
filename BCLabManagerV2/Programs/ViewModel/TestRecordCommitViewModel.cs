using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Win32;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TestRecordCommitViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        //string _programName;
        //string _RecipeName;
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
        RelayCommand _openFilesCommand;
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
            //throw new NotImplementedException();
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

                RaisePropertyChanged("Status");
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

                RaisePropertyChanged("Battery");
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
                    RaisePropertyChanged("AllBatteries");
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

                RaisePropertyChanged("Chamber");
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

                RaisePropertyChanged("Channel");
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
            //dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.NetworkShortcuts); No use, cannot see network driver
            //dialog.InitialDirectory = @"Q:\807\Software\WH BC Lab\Raw Data";
            if (dialog.ShowDialog() == true)
            {
                FileList = new ObservableCollection<string>(dialog.FileNames.ToList());
            }
        }
    }
}
