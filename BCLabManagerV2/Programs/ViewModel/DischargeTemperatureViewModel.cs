﻿using System;
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
    public class DischargeTemperatureViewModel : BindBase//, IDataErrorInfo
    {
        #region Fields
        private readonly DischargeTemperatureClass _chargeTemperature;

        #endregion // Fields

        #region Constructor

        public DischargeTemperatureViewModel(DischargeTemperatureClass chargeTemperature)
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
        public string Name
        {
            get { return _chargeTemperature.Name; }
            set
            {
                if (value == _chargeTemperature.Name)
                    return;

                _chargeTemperature.Name = value;

                base.OnPropertyChanged("Name");
            }
        }
        #endregion
    }
}