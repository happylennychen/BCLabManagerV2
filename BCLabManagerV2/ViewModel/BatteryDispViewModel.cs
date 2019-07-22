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
    public class BatteryDispViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        readonly BatteryClass _battery;
        #endregion // Fields

        #region Constructor

        public BatteryDispViewModel(BatteryClass batterymodel)
        {
            if (batterymodel == null)
                throw new ArgumentNullException("batterymodel");

            _battery = batterymodel;
        }

        #endregion // Constructor

        #region BatteryClass Properties

        public string Name
        {
            get { return _battery.Name; }
            set
            {
                if (value == _battery.Name)
                    return;

                _battery.Name = value;

                base.OnPropertyChanged("Name");
            }
        }

        public double CycleCount
        {
            get { return _battery.CycleCount; }
            set
            {
                if (value == _battery.CycleCount)
                    return;

                _battery.CycleCount = value;

                base.OnPropertyChanged("CycleCount");
            }
        }

        public string Status
        {
            get { return _battery.Status.ToString(); }
        }

        #endregion // Customer Properties
    }
}