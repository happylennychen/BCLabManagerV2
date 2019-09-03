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

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class BatteryViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        readonly BatteryClass _battery;
        //bool _isSelected;
        string _batteryType;

        #endregion // Fields

        #region Constructor

        public BatteryViewModel(BatteryClass batterymodel)
        {
            if (batterymodel == null)
                throw new ArgumentNullException("batterymodel");

            _battery = batterymodel;

            _battery.PropertyChanged += _battery_PropertyChanged;
        }

        private void _battery_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
            OnPropertyChanged(e.PropertyName);
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

                base.OnPropertyChanged("Id");
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

        public AssetStatusEnum Status
        {
            get { return _battery.Status; }
            set
            {
                if (value == _battery.Status)
                    return;

                _battery.Status = value;

                base.OnPropertyChanged("Status");
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

                base.OnPropertyChanged("BatteryType");
            }
        }
        #endregion
    }
}