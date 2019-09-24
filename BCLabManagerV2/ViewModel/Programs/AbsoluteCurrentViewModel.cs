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
        private readonly AbsoluteCurrentClass _model;

        #endregion // Fields

        #region Constructor

        public AbsoluteCurrentViewModel(AbsoluteCurrentClass model)
        {
            _model = model;
            _model.PropertyChanged += _chargeTemperature_PropertyChanged;
        }

        private void _chargeTemperature_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region DischargeTemperatureClass Properties

        public int Id
        {
            get { return _model.Id; }
            set
            {
                if (value == _model.Id)
                    return;

                _model.Id = value;

                base.OnPropertyChanged("Id");
            }
        }
        public double Value
        {
            get { return _model.Value; }
            set
            {
                if (value == _model.Value)
                    return;

                _model.Value = value;

                base.OnPropertyChanged("Value");
            }
        }
        #endregion
    }
}