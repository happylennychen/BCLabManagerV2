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
    public enum CommandType
    {
        Create,
        Edit,
        SaveAs
    }
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class BatteryEditViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        readonly BatteryClass _battery;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public BatteryEditViewModel(BatteryClass battery, List<BatteryTypeClass> batteryTypes)
        {
            if (battery == null)
                throw new ArgumentNullException("batterymodel");

            _battery = battery;

            _battery.PropertyChanged += _battery_PropertyChanged;
            CreateAllBatteryTypes(batteryTypes);
        }

        private void _battery_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
            OnPropertyChanged(e.PropertyName);
        }

        void CreateAllBatteryTypes(List<BatteryTypeClass> batteryTypes)
        {
            //List<BatteryTypeClass> all = _batterytypeRepository.GetItems();

            this.AllBatteryTypes = batteryTypes;
        }

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

        public List<BatteryTypeClass> AllBatteryTypes
        {
            get;set;
        }

        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    switch (commandType)
                    {
                        case CommandType.Create:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); },
                                param => this.CanCreate
                                );
                            break;
                        case CommandType.Edit:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); }
                                );
                            break;
                        case CommandType.SaveAs:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); },
                                param => this.CanSaveAs
                                );
                            break;
                    }
                }
                return _okCommand;
            }
        }

        public CommandType commandType
        { get; set; }

        public bool IsOK
        {
            get { return _isOK; }
            set { _isOK = value; }
        }

        #endregion // Presentation Properties

        #region Public Methods

        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        public void OK()
        {
            //if (!_batterytype.IsValid)
            //throw new InvalidOperationException(Resources.BatteryTypeViewModel_Exception_CannotSave);

            //if (this.IsNewBatteryType)
            //_batterytypeRepository.AddItem(_batterytype);

            //base.OnPropertyChanged("DisplayName");
            IsOK = true;
        }

        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewBattery
        {
            get
            {
                var dbContext = new AppDbContext();
                int number = (
                    from bat in dbContext.Batteries
                    where bat.Name == _battery.Name     //名字（某一个属性）一样就认为是一样的
                    select bat).Count();
                if (number != 0)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanCreate
        {
            get { return IsNewBattery; }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSaveAs
        {
            get { return IsNewBattery; }
        }

        #endregion // Private Helpers
    }
}