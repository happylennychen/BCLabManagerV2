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
    public class ChargeCurrentViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields
        private readonly ChargeCurrentClass _chargeCurrent;

        #endregion // Fields

        #region Constructor

        public ChargeCurrentViewModel(ChargeCurrentClass chargeCurrent)
        {
            _chargeCurrent = chargeCurrent;
            _chargeCurrent.PropertyChanged += _chargeCurrent_PropertyChanged;
        }

        private void _chargeCurrent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region ChargeCurrentClass Properties

        public int Id
        {
            get { return _chargeCurrent.Id; }
            set
            {
                if (value == _chargeCurrent.Id)
                    return;

                _chargeCurrent.Id = value;

                base.OnPropertyChanged("Id");
            }
        }
        public string Name
        {
            get { return _chargeCurrent.Name; }
            set
            {
                if (value == _chargeCurrent.Name)
                    return;

                _chargeCurrent.Name = value;

                base.OnPropertyChanged("Name");
            }
        }
        #endregion
    }
}