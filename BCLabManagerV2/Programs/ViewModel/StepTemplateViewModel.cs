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
    public class StepTemplateViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        public readonly StepTemplate _stepTemplate;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public StepTemplateViewModel(StepTemplate StepTemplate)
        {
            _stepTemplate = StepTemplate;
            _stepTemplate.PropertyChanged += _StepTemplate_PropertyChanged;
        }

        private void _StepTemplate_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
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

        public double Temperature
        {
            get { return _stepTemplate.Temperature; }
            set
            {
                if (value == _stepTemplate.Temperature)
                    return;

                _stepTemplate.Temperature = value;
                RaisePropertyChanged();
            }
        }
        public double CutOffConditionValue
        {
            get { return _stepTemplate.CutOffConditionValue; }
            set
            {
                if (value == _stepTemplate.CutOffConditionValue)
                    return;

                _stepTemplate.CutOffConditionValue = value;
                RaisePropertyChanged();
            }
        }
        public CutOffConditionTypeEnum CutOffConditionType
        {
            get { return _stepTemplate.CutOffConditionType; }
            set
            {
                if (value == _stepTemplate.CutOffConditionType)
                    return;

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
                all.Add(CurrentUnitEnum.C);
                all.Add(CurrentUnitEnum.mA);

                return all;
            }
        }
        #endregion // Presentation Properties

    }
}