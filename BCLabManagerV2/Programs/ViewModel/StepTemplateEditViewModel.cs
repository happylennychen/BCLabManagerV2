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

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class StepTemplateEditViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        public readonly StepTemplate _stepTemplate;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public StepTemplateEditViewModel(
            StepTemplate StepTemplateModel
            )
        {
            _stepTemplate = StepTemplateModel;
        }
        #endregion // Constructor

        #region StepTemplate Properties

        public int Id
        {
            get { return _stepTemplate.Id; }
            set
            {
                if (value == _stepTemplate.Id)
                    return;

                _stepTemplate.Id = value;

                RaisePropertyChanged("Id");
            }
        }

        public double CurrentInput
        {
            get { return _stepTemplate.CurrentInput; }
            set
            {
                if (value == _stepTemplate.CurrentInput)
                    return;

                _stepTemplate.CurrentInput = value;
                RaisePropertyChanged();
            }
        }
        public CurrentUnitEnum CurrentUnit
        {
            get { return _stepTemplate.CurrentUnit; }
            set
            {
                if (value == _stepTemplate.CurrentUnit)
                    return;

                _stepTemplate.CurrentUnit = value;

                RaisePropertyChanged();
            }
        }

        public double CutOffConditionValue
        {
            get { return _stepTemplate.CutOffConditionValue; }
            set
            {
                _stepTemplate.CutOffConditionValue = value;
                RaisePropertyChanged();
            }
        }

        public CutOffConditionTypeEnum CutOffConditionType
        {
            get { return _stepTemplate.CutOffConditionType; }
            set
            {
                _stepTemplate.CutOffConditionType = value;
                RaisePropertyChanged();
            }
        }


        #endregion // Customer Properties

        #region Presentation Properties


        public List<CurrentUnitEnum> CurrentUnitOptions
        {
            get
            {
                List<CurrentUnitEnum> all = new List<CurrentUnitEnum>();
                all.Add(CurrentUnitEnum.mA);
                all.Add(CurrentUnitEnum.C);

                return all;
            }
        }


        public List<CutOffConditionTypeEnum> CutOffConditionTypeOptions
        {
            get
            {
                List<CutOffConditionTypeEnum> all = new List<CutOffConditionTypeEnum>();
                all.Add(CutOffConditionTypeEnum.Time_s);
                all.Add(CutOffConditionTypeEnum.C_mAH);
                all.Add(CutOffConditionTypeEnum.CRate);

                return all;
            }
        }
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    switch (commandType)
                    {
                        case CommandType.Create:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); },
                                param => this.CanCreate
                                );
                            break;
                        case CommandType.Edit:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); }
                                );
                            break;
                        case CommandType.SaveAs:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); },
                                param => this.CanSaveAs
                                );
                            break;
                    }
                }
                return _okCommand;
            }
        }

        public CommandType commandType
        { get; set; }

        public bool IsOK
        {
            get { return _isOK; }
            set { _isOK = value; }
        }

        #endregion // Presentation Properties

        #region Public Methods

        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        public void OK()
        {
            IsOK = true;
        }

        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewStepTemplate
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanCreate
        {
            get { return IsNewStepTemplate; }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSaveAs
        {
            get { return IsNewStepTemplate; }
        }

        #endregion // Private Helpers
    }
}