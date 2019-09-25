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
    public class TemperatureViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields
        private readonly TemperatureClass _temperature;

        #endregion // Fields

        #region Constructor

        public TemperatureViewModel(TemperatureClass temperature)
        {
            _temperature = temperature;
            _temperature.PropertyChanged += _chargeTemperature_PropertyChanged;
        }

        private void _chargeTemperature_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region ChargeTemperatureClass Properties

        public int Id
        {
            get { return _temperature.Id; }
            set
            {
                if (value == _temperature.Id)
                    return;

                _temperature.Id = value;

                base.OnPropertyChanged("Id");
            }
        }
        public double Value
        {
            get { return _temperature.Value; }
            set
            {
                if (value == _temperature.Value)
                    return;

                _temperature.Value = value;

                base.OnPropertyChanged("Value");
            }
        }
        public string ValueStr
        {
            get
            {
                if (_temperature.Value == GlobalSettings.RoomTemperatureConstant)
                    return "Room";
                else
                    return _temperature.Value.ToString();
            }
        }
        #endregion
    }
}