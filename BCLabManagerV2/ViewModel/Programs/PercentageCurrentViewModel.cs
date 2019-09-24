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
    public class PercentageCurrentViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields
        private readonly PercentageCurrentClass _chargeCurrent;

        #endregion // Fields

        #region Constructor

        public PercentageCurrentViewModel(PercentageCurrentClass chargeCurrent)
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
        public double Value
        {
            get { return _chargeCurrent.Value; }
            set
            {
                if (value == _chargeCurrent.Value)
                    return;

                _chargeCurrent.Value = value;

                base.OnPropertyChanged("Value");
            }
        }
        #endregion
    }
}