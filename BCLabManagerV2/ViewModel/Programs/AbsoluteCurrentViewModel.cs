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
    public class AbsoluteCurrentViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields
        private readonly AbsoluteCurrentClass _chargeTemperature;

        #endregion // Fields

        #region Constructor

        public AbsoluteCurrentViewModel(AbsoluteCurrentClass chargeTemperature)
        {
            _chargeTemperature = chargeTemperature;
            _chargeTemperature.PropertyChanged += _chargeTemperature_PropertyChanged;
        }

        private void _chargeTemperature_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region DischargeTemperatureClass Properties

        public int Id
        {
            get { return _chargeTemperature.Id; }
            set
            {
                if (value == _chargeTemperature.Id)
                    return;

                _chargeTemperature.Id = value;

                base.OnPropertyChanged("Id");
            }
        }
        public double Value
        {
            get { return _chargeTemperature.Value; }
            set
            {
                if (value == _chargeTemperature.Value)
                    return;

                _chargeTemperature.Value = value;

                base.OnPropertyChanged("Value");
            }
        }
        #endregion
    }
}