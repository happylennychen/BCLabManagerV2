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
        readonly TestRecord _record;
        RelayCommand _okCommand;
        RelayCommand _openFilesCommand;
        RelayCommand _splitterCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public TestRecordCommitViewModel(
            TestRecord record
            )     //
        {
            _record = record ?? throw new ArgumentNullException("record");

            _record.PropertyChanged += _record_PropertyChanged;
        }

        private void _record_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion // Constructor

        #region Presentation Properties

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
            //dialog.InitialDirectory = $@"Q:\807\Software\WH BC Lab\Data\{_record.BatteryTypeStr}\{_record.ProjectStr}\Raw Data";
            //dialog.InitialDirectory = $@"Q:\807\Software\WH BC Lab\Data\{_record.BatteryTypeStr}\High Power\Raw Data";
            //dialog.InitialDirectory = $@"{GlobalSettings.RootPath}{_record.BatteryTypeStr}\High Power\{GlobalSettings.TestDataFolderName}";
            if (dialog.ShowDialog() == true)
            {
                FileList = new ObservableCollection<string>(dialog.FileNames.ToList());
                TesterServiceClass _testerService = new TesterServiceClass();
                DateTime[] time = _testerService.GetTimeFromRawData(_record.AssignedChannel.Tester.ITesterProcesser, FileList);
                if (time != null)
                    NewName = $@"{_record.ProgramStr}_{_record.RecipeStr}_{_record.TesterStr}_{_record.ChannelStr}_{_record.BatteryStr}_{time[0].ToString("yyyyMMddHHmmss")}";
                else
                    NewName = $@"{_record.ProgramStr}_{_record.RecipeStr}_{_record.TesterStr}_{_record.ChannelStr}_{_record.BatteryStr}";
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
