using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;
using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// Editable: no need
    /// Updateable: true
    /// </summary>
    public class BatteryViewModel : BindableBase //, IDataErrorInfo
    {
        #region Fields

        readonly Battery _battery;

        #endregion // Fields

        #region Constructor

        public BatteryViewModel(Battery battery)
        {
            if (battery == null)
                throw new ArgumentNullException("batterymodel");

            _battery = battery;

            _battery.PropertyChanged += _battery_PropertyChanged;
        }

        private void _battery_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region BatteryClass Properties

        public int Id
        {
            get { return _battery.Id; }
        }
        public string Name
        {
            get { return _battery.Name; }
        }

        public double CycleCount
        {
            get { return _battery.CycleCount; }
        }

        public int AssetUseCount
        {
            get { return _battery.AssetUseCount; }
        }
        public BatteryType BatteryType
        {
            get { return _battery.BatteryType; }
        }


        #endregion
        
    }
}