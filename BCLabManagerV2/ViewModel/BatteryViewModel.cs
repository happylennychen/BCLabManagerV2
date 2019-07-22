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
    public class BatteryViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        readonly BatteryClass _battery;
        readonly BatteryRepository _batteryRepository;
        readonly BatteryTypeRepository _batterytypeRepository;
        //bool _isSelected;
        string _batteryType;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public BatteryViewModel(BatteryClass batterymodel, BatteryRepository batteryRepository, BatteryTypeRepository batterytypeRepository)
        {
            if (batterymodel == null)
                throw new ArgumentNullException("batterymodel");

            if (batteryRepository == null)
                throw new ArgumentNullException("batteryRepository");

            if (batterytypeRepository == null)
                throw new ArgumentNullException("batterymodelRepository");

            _battery = batterymodel;
            _batteryRepository = batteryRepository;
            _batterytypeRepository = batterytypeRepository;

            // Populate the AllCustomers collection with BatteryTypeViewModels.
            //this.CreateAllBatteryTypes();      
        }

        /*void CreateAllBatteryTypes()
        {
            List<BatteryTypeClass> all = _batterytypeRepository.GetItems();

            this.AllBatteryTypes = new ObservableCollection<BatteryTypeClass>(all);
        }*/

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


        public String BatteryType
        {
            get
            {
                if (_batteryType == null)
                {
                    if (_battery.BatteryType == null)
                        _batteryType = string.Empty;
                    else
                        _batteryType = _battery.BatteryType.Name;
                }
                return _batteryType;
            }
            set
            {
                if (value == _batteryType || String.IsNullOrEmpty(value))
                    return;

                _batteryType = value;

                _battery.BatteryType = _batterytypeRepository.GetItems().First(i => i.Name == _batteryType);

                base.OnPropertyChanged("BatteryType");
            }
        }

        public ObservableCollection<string> AllBatteryTypes
        {
            get
            {
                List<BatteryTypeClass> all = _batterytypeRepository.GetItems();
                List<string> allstring = (
                    from i in all
                    select i.Name).ToList();

                return new ObservableCollection<string>(allstring);
            }
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
                    _okCommand = new RelayCommand(
                        param => { this.OK(); },
                        param => this.CanOK
                        );
                }
                return _okCommand;
            }
        }

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
                int number = (
                    from bat in _batteryRepository.GetItems()
                    where bat.Name == _battery.Name     //名字（某一个属性）一样就认为是一样的
                    select bat).Count();
                if (number != 0)
                    return false; 
                return !_batteryRepository.ContainsItem(_battery);
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanOK
        {
            get { return IsNewBattery; }
        }

        #endregion // Private Helpers
    }
}