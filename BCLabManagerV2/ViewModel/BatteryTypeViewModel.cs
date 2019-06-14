using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.Model;
using BCLabManager.DataAccess;
using System.Windows.Input;
using BCLabManager.Properties;

namespace BCLabManager.ViewModel
{
    public class BatteryTypeViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        readonly BatteryTypeClass _batterytype;
        readonly BatteryTypeRepository _batterytypeRepository;
        //bool _isSelected;
        RelayCommand _saveCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public BatteryTypeViewModel(BatteryTypeClass batterytype, BatteryTypeRepository batteryModelRepository)
        {
            if (batterytype == null)
                throw new ArgumentNullException("batterytype");

            if (batteryModelRepository == null)
                throw new ArgumentNullException("batteryModelRepository");

            _batterytype = batterytype;
            _batterytypeRepository = batteryModelRepository;
        }

        #endregion // Constructor

        #region BatteryTypeClass Properties

        public string Manufactor
        {
            get { return _batterytype.Manufactor; }
            set
            {
                if (value == _batterytype.Manufactor)
                    return;

                _batterytype.Manufactor = value;

                base.OnPropertyChanged("Manufactor");
            }
        }

        public string Name
        {
            get { return _batterytype.Name; }
            set
            {
                if (value == _batterytype.Name)
                    return;

                _batterytype.Name = value;

                base.OnPropertyChanged("Name");
            }
        }

        public string Material
        {
            get { return _batterytype.Material; }
            set
            {
                if (value == _batterytype.Material)
                    return;

                _batterytype.Material = value;

                base.OnPropertyChanged("Material");
            }
        }

        #endregion // Customer Properties

        #region Presentation Properties


        public override string DisplayName
        {
            get
            {
                if (this.IsNewBatteryType)
                {
                    return Resources.BatteryTypeViewModel_DisplayName;
                }
                else
                {
                    return _batterytype.Name;
                }
            }
        }

        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        param => { this.Save(); },
                        param => this.CanSave
                        );
                }
                return _saveCommand;
            }
        }
        public ICommand SaveAsCommand
        {
            get
            {
                if (_saveAsCommand == null)
                {
                    _saveAsCommand = new RelayCommand(
                        param => { this.SaveAs(); }
                        );
                }
                return _saveAsCommand;
            }
        }

        #endregion // Presentation Properties

        #region Public Methods

        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        public void Save()
        {
            //if (!_batterytype.IsValid)
            //throw new InvalidOperationException(Resources.BatteryTypeViewModel_Exception_CannotSave);

            if (this.IsNewBatteryType)
                _batterytypeRepository.AddItem(_batterytype);

            base.OnPropertyChanged("DisplayName");
        }

        /// <summary>
        /// Create a new battery type, based on the given value.  This method is invoked by the SaveAsCommand.
        /// </summary>
        public void SaveAs()
        {
            BatteryTypeClass newBatteryType =
                new BatteryTypeClass(
                    this._batterytype.Manufactor,
                    this._batterytype.Name,
                    this._batterytype.Material,
                    this._batterytype.LimitedChargeVoltage,
                    this._batterytype.RatedCapacity,
                    this._batterytype.NominalVoltage,
                    this._batterytype.TypicalCapacity,
                    this._batterytype.CutoffDischargeVoltage);
            BatteryTypeViewModel workspace = new BatteryTypeViewModel(newBatteryType, this._batterytypeRepository);
        }

        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewBatteryType
        {
            get
            {
                int number = (
                    from bat in _batterytypeRepository.GetItems()
                    where bat.Name == _batterytype.Name
                    select bat).Count();
                if (number != 0)
                    return false;
                return !_batterytypeRepository.ContainsItem(_batterytype);
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSave
        {
            get { return IsNewBatteryType; }
        }

        #endregion // Private Helpers
    }
}
