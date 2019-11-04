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
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class BatteryViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        readonly BatteryClass _battery;

        #endregion // Fields

        #region Constructor

        public BatteryViewModel(BatteryClass battery)
        {
            if (battery == null)
                throw new ArgumentNullException("batterymodel");

            _battery = battery;

            _battery.PropertyChanged += _battery_PropertyChanged;
        }

        private void _battery_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
            RaisePropertyChanged(e.PropertyName);
        }

        /*void CreateAllBatteryTypes()
        {
            List<BatteryTypeClass> all = _batterytypeRepository.GetItems();

            this.AllBatteryTypes = new ObservableCollection<BatteryTypeClass>(all);
        }*/

        #endregion // Constructor

        #region BatteryClass Properties

        public int Id
        {
            get { return _battery.Id; }
            set
            {
                if (value == _battery.Id)
                    return;

                _battery.Id = value;

                RaisePropertyChanged("Id");
            }
        }
        public string Name
        {
            get { return _battery.Name; }
            set
            {
                if (value == _battery.Name)
                    return;

                _battery.Name = value;

                RaisePropertyChanged("Name");
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

                RaisePropertyChanged("CycleCount");
            }
        }

        public int AssetUseCount
        {
            get { return _battery.AssetUseCount; }
            set
            {
                if (value == _battery.AssetUseCount)
                    return;

                _battery.AssetUseCount = value;

                RaisePropertyChanged("AssetUseCount");
            }
        }

        #endregion // Customer Properties

        #region Presentation Properties


        //public String BatteryType
        //{
        //    get
        //    {
        //        if (_batteryType == null)
        //        {
        //            if (_battery.BatteryType == null)
        //                _batteryType = string.Empty;
        //            else
        //                _batteryType = _battery.BatteryType.Name;
        //        }
        //        return _batteryType;
        //    }
        //    set
        //    {
        //        if (value == _batteryType || String.IsNullOrEmpty(value))
        //            return;

        //        _batteryType = value;

        //        //_battery.BatteryType = _batterytypeRepository.GetItems().First(i => i.Name == _batteryType);
        //        using (var dbContext = new AppDbContext())
        //        {
        //            _battery.BatteryType = dbContext.BatteryTypes.SingleOrDefault(bt => bt.Name == _batteryType);
        //        }

        //        base.OnPropertyChanged("BatteryType");
        //    }
        //}

        public BatteryTypeClass BatteryType
        {
            get { return _battery.BatteryType; }
            set
            {
                if (value == _battery.BatteryType)
                    return;

                _battery.BatteryType = value;

                RaisePropertyChanged("BatteryType");
            }
        }
        #endregion
    }
}