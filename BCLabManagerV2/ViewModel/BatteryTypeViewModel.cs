﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.Model;
using BCLabManager.View;
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
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public BatteryTypeViewModel(BatteryTypeClass batterytype, BatteryTypeRepository batteryTypeRepository)
        {
            if (batterytype == null)
                throw new ArgumentNullException("batterytype");

            if (batteryTypeRepository == null)
                throw new ArgumentNullException("batteryTypeRepository");

            _batterytype = batterytype;
            _batterytypeRepository = batteryTypeRepository;
            _isOK = false;
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
            BatteryTypeViewModel newbtvm = new BatteryTypeViewModel(newBatteryType, this._batterytypeRepository);
            newbtvm.DisplayName = "Battery Type-Save As";
            var BatteryTypeViewInstance = new BatteryTypeView();
            BatteryTypeViewInstance.DataContext = newbtvm;
            BatteryTypeViewInstance.Show();
        }
        public void Edit()
        {
            BatteryTypeViewModel newbtvm = new BatteryTypeViewModel(_batterytype, this._batterytypeRepository);
            newbtvm.DisplayName = "Battery Type-Edit";
            var BatteryTypeViewInstance = new BatteryTypeView();
            BatteryTypeViewInstance.DataContext = newbtvm;
            BatteryTypeViewInstance.Show();
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
        bool CanOK
        {
            get { return IsNewBatteryType; }
        }

        #endregion // Private Helpers
    }
}
