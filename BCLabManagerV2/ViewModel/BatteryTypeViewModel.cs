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

        public BatteryTypeViewModel(BatteryTypeClass batterytype, BatteryTypeRepository batteryTypeRepository)  //构造函数里面之所以要batterytyperepository,是因为IsNewBattery需要用此进行判断
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

        public void OK()        //就是将属性IsOK设置成true，从而在外层进行下一步动作
        {
            IsOK = true;
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
