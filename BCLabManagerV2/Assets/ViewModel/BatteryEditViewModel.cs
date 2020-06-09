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
    /// Editable: true
    /// Updateable: no need
    /// </summary>
    public enum CommandType     //这里是偷懒的做法，不符合OCP。正确做法应该是为每一个view制作一个view model，而不是混用viewmodel然后用一个属性来做区分
    {
        Create,
        Edit,
        SaveAs
    }
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class BatteryEditViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        readonly Battery _battery;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public BatteryEditViewModel(Battery battery, ObservableCollection<BatteryType> batteryTypes)
        {
            if (battery == null)
                throw new ArgumentNullException("batterymodel");

            _battery = battery;

            CreateAllBatteryTypes(batteryTypes);
        }

        void CreateAllBatteryTypes(ObservableCollection<BatteryType> batteryTypes)
        {

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

        public BatteryType BatteryType
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

        public ObservableCollection<BatteryType> AllBatteryTypes
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

            //RaisePropertyChanged("DisplayName");
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
                //var dbContext = new AppDbContext();
                //int number = (
                //    from bat in dbContext.Batteries
                //    where bat.Name == _battery.Name     //名字（某一个属性）一样就认为是一样的
                //    select bat).Count();
                //if (number != 0)
                //    return false;
                //else
                //    return true;
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