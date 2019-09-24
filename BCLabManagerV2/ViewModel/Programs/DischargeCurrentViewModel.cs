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
    public class DynamicCurrentViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields
        private readonly DynamicCurrentClass _dynamicCurrent;

        #endregion // Fields

        #region Constructor

        public DynamicCurrentViewModel(DynamicCurrentClass dynamicCurrent)
        {
            _dynamicCurrent = dynamicCurrent;
            _dynamicCurrent.PropertyChanged += _dynamicCurrent_PropertyChanged;
        }

        private void _dynamicCurrent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region DynamicCurrentClass Properties

        public int Id
        {
            get { return _dynamicCurrent.Id; }
            set
            {
                if (value == _dynamicCurrent.Id)
                    return;

                _dynamicCurrent.Id = value;

                base.OnPropertyChanged("Id");
            }
        }
        public double Value
        {
            get { return _dynamicCurrent.Value; }
            set
            {
                if (value == _dynamicCurrent.Value)
                    return;

                _dynamicCurrent.Value = value;

                base.OnPropertyChanged("Value");
            }
        }
        #endregion
    }
}