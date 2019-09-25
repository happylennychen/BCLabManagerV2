using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class SubProgramTemplateViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields
        public readonly SubProgramTemplate _subProgramTemplate;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public SubProgramTemplateViewModel(SubProgramTemplate subProgramTemplate)
        {
            _subProgramTemplate = subProgramTemplate;
            _subProgramTemplate.PropertyChanged += _subProgramTemplate_PropertyChanged;
        }

        private void _subProgramTemplate_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region SubProgramTemplate Properties

        public int Id
        {
            get { return _subProgramTemplate.Id; }
            set
            {
                if (value == _subProgramTemplate.Id)
                    return;

                _subProgramTemplate.Id = value;

                base.OnPropertyChanged("Id");
            }
        }
        public string Name
        {
            get
            {
                return _subProgramTemplate.Name;
            }
        }
        public double ChargeTemperature
        {
            get { return _subProgramTemplate.ChargeTemperature; }
            set
            {
                if (value == _subProgramTemplate.ChargeTemperature)
                    return;

                _subProgramTemplate.ChargeTemperature = value;

                base.OnPropertyChanged("ChargeTemperature");
            }
        }
        public string ChargeTemperatureStr
        {
            get
            {
                if (_subProgramTemplate.ChargeTemperature == -9999)
                    return "Room";
                else
                    return _subProgramTemplate.ChargeTemperature.ToString() + " deg"; }
        }
        public CurrentTypeEnum ChargeCurrentType
        {
            get { return _subProgramTemplate.ChargeCurrentType; }
            set
            {
                if (value == _subProgramTemplate.ChargeCurrentType)
                    return;

                _subProgramTemplate.ChargeCurrentType = value;

                base.OnPropertyChanged("ChargeCurrentType");
            }
        }
        public double ChargeCurrent
        {
            get { return _subProgramTemplate.ChargeCurrent; }
            set
            {
                if (value == _subProgramTemplate.ChargeCurrent)
                    return;

                _subProgramTemplate.ChargeCurrent = value;

                base.OnPropertyChanged("ChargeCurrent");
            }
        }
        public string ChargeCurrentStr
        {
            get
            {
                if(ChargeCurrentType == CurrentTypeEnum.Absolute)
                    return _subProgramTemplate.ChargeCurrent.ToString() + "mA";
                else if(ChargeCurrentType == CurrentTypeEnum.Percentage)
                    return _subProgramTemplate.ChargeCurrent.ToString() + "C";
                else if (ChargeCurrentType == CurrentTypeEnum.Dynamic)
                    return "D" + _subProgramTemplate.ChargeCurrent.ToString();
                return "";
            }
        }
        public double DischargeTemperature
        {
            get { return _subProgramTemplate.DischargeTemperature; }
            set
            {
                if (value == _subProgramTemplate.DischargeTemperature)
                    return;

                _subProgramTemplate.DischargeTemperature = value;

                base.OnPropertyChanged("DischargeTemperature");
            }
        }
        public string DischargeTemperatureStr
        {
            get
            {
                if (_subProgramTemplate.DischargeTemperature == -9999)
                    return "Room";
                else
                    return _subProgramTemplate.DischargeTemperature.ToString() + " deg";
            }
        }
        public CurrentTypeEnum DischargeCurrentType
        {
            get { return _subProgramTemplate.DischargeCurrentType; }
            set
            {
                if (value == _subProgramTemplate.DischargeCurrentType)
                    return;

                _subProgramTemplate.DischargeCurrentType = value;

                base.OnPropertyChanged("DischargeCurrentType");
            }
        }
        public double DischargeCurrent
        {
            get { return _subProgramTemplate.DischargeCurrent; }
            set
            {
                if (value == _subProgramTemplate.DischargeCurrent)
                    return;

                _subProgramTemplate.DischargeCurrent = value;

                base.OnPropertyChanged("DischargeCurrent");
            }
        }
        public string DischargeCurrentStr
        {
            get
            {
                if (DischargeCurrentType == CurrentTypeEnum.Absolute)
                    return _subProgramTemplate.DischargeCurrent.ToString() + "mA";
                else if (DischargeCurrentType == CurrentTypeEnum.Percentage)
                    return _subProgramTemplate.DischargeCurrent.ToString() + "C";
                else if (DischargeCurrentType == CurrentTypeEnum.Dynamic)
                    return "D" + _subProgramTemplate.DischargeCurrent.ToString();
                return "";
            }
        }

        public TestCountEnum TestCount
        {
            get { return _subProgramTemplate.TestCount; }
            set
            {
                if (value == _subProgramTemplate.TestCount)
                    return;

                _subProgramTemplate.TestCount = value;

                base.OnPropertyChanged("TestCount");
            }
        }

        #endregion // Customer Properties

        #region Presentation Properties

        public List<TestCountEnum> TestCountOptions
        {
            get
            {
                return new List<TestCountEnum>()
                {
                    TestCountEnum.One,
                    TestCountEnum.Two
                };
            }
        }

        public List<CurrentTypeEnum> CurrentTypeOptions
        {
            get
            {
                return new List<CurrentTypeEnum>()
                {
                    CurrentTypeEnum.Absolute,
                    CurrentTypeEnum.Dynamic,
                    CurrentTypeEnum.Percentage
                };
            }
        }

        #endregion // Presentation Properties

    }
}