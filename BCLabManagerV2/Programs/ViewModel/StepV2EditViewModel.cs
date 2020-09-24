using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;
using Prism.Mvvm;
using System.Windows.Media;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class StepV2EditViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        public readonly StepV2 _step;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。
        RelayCommand _okCommand;

        #endregion // Fields

        #region Constructor

        public StepV2EditViewModel(StepV2 step)
        {
            _step = step;
            _step.PropertyChanged += _Step_PropertyChanged;
        }

        private void _Step_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region StepV2 Properties

        public int Id
        {
            get { return _step.Id; }
            set
            {
                if (value == _step.Id)
                    return;

                _step.Id = value;

                RaisePropertyChanged("Id");
            }
        }
        public int Index
        {
            get
            {
                return _step.Index;
            }
        }
        public List<ActionMode> ModeOptions
        {
            get
            {
                return new List<ActionMode>() { ActionMode.CC_CV_CHARGE, ActionMode.CC_DISCHARGE, ActionMode.CP_DISCHARGE, ActionMode.REST };
            }
        }
        public ActionMode Mode
        {
            get
            {
                return _step.Action.Mode;
            }
            set
            {
                if (value == _step.Action.Mode)
                    return;

                _step.Action.Mode = value;

                RaisePropertyChanged("Mode");
            }
        }
        public int Voltage
        {
            get
            {
                return _step.Action.Voltage;
            }
            set
            {
                if (value == _step.Action.Voltage)
                    return;

                _step.Action.Voltage = value;

                RaisePropertyChanged("Voltage");
            }
        }
        public int Current
        {
            get
            {
                return _step.Action.Current;
            }
            set
            {
                if (value == _step.Action.Current)
                    return;

                _step.Action.Current = value;

                RaisePropertyChanged("Current");
            }
        }
        public int Power
        {
            get
            {
                return _step.Action.Power;
            }
            set
            {
                if (value == _step.Action.Power)
                    return;

                _step.Action.Power = value;

                RaisePropertyChanged("Power");
            }
        }
        public string Loop1Label
        {
            get
            {
                return _step.Loop1Label;
            }
        }
        public string Loop2Label
        {
            get
            {
                return _step.Loop2Label;
            }
        }
        //public ObservableCollection<CutOffConditionViewModel> CutOffConditions
        //{ get; set; }
        #endregion // Customer Properties

        public bool IsOK { get; set; } = false;
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new RelayCommand(
                        param => { this.OK(); }
                        );
                }
                return _okCommand;
            }
        }

        private void OK()
        {
            IsOK = true;
        }
    }
}