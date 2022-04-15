using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TemperatureViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields

        #endregion // Fields

        #region Constructor

        public TemperatureViewModel(
            double temperature)     //
        {
            _temperature = temperature;
        }

        #endregion // Constructor

        #region Presentation Properties

        private double _temperature;
        public double Temperature
        {
            get
            {
                return _temperature;
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
        #endregion // Presentation Properties
    }
}
