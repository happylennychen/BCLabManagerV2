using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.Model;
using BCLabManager.View;
using BCLabManager.DataAccess;
using System.Windows.Input;
using BCLabManager.Properties;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// Editable: true
    /// Updateable: no need
    /// </summary>
    public class BatteryTypeEditViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        readonly BatteryType _batterytype;
        //bool _isSelected;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public BatteryTypeEditViewModel(BatteryType batterytype)  //构造函数里面之所以要batterytyperepository,是因为IsNewBattery需要用此进行判断
        {
            if (batterytype == null)
                throw new ArgumentNullException("batterytype");

            _batterytype = batterytype;
            _isOK = false;
        }

        #endregion // Constructor

        #region BatteryTypeClass Properties

        public int Id
        {
            get { return _batterytype.Id; }
            set
            {
                if (value == _batterytype.Id)
                    return;

                _batterytype.Id = value;

                RaisePropertyChanged("Id");
            }
        }
        public string Manufacturer
        {
            get { return _batterytype.Manufacturer; }
            set
            {
                if (value == _batterytype.Manufacturer)
                    return;

                _batterytype.Manufacturer = value;

                RaisePropertyChanged("Manufacturer");
            }
        }

        public string Name
        {
            get { return _batterytype.Name; }
            set
            {
                if (value == _batterytype.Name)
                    return;

                _batterytype.Name = value.Trim();

                RaisePropertyChanged("Name");
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

                RaisePropertyChanged("Material");
            }
        }

        public int TypicalCapacity
        {
            get { return _batterytype.TypicalCapacity; }
            set
            {
                if (value == _batterytype.TypicalCapacity)
                    return;

                _batterytype.TypicalCapacity = value;

                RaisePropertyChanged();
            }
        }

        public int LimitedChargeVoltage
        {
            get { return _batterytype.LimitedChargeVoltage; }
            set
            {
                if (value == _batterytype.LimitedChargeVoltage)
                    return;

                _batterytype.LimitedChargeVoltage = value;

                RaisePropertyChanged();
            }
        }

        public int RatedCapacity
        {
            get { return _batterytype.RatedCapacity; }
            set
            {
                if (value == _batterytype.RatedCapacity)
                    return;

                _batterytype.RatedCapacity = value;

                RaisePropertyChanged();
            }
        }

        public int NominalVoltage
        {
            get { return _batterytype.NominalVoltage; }
            set
            {
                if (value == _batterytype.NominalVoltage)
                    return;

                _batterytype.NominalVoltage = value;

                RaisePropertyChanged();
            }
        }

        public int CutoffDischargeVoltage
        {
            get { return _batterytype.CutoffDischargeVoltage; }
            set
            {
                if (value == _batterytype.CutoffDischargeVoltage)
                    return;

                _batterytype.CutoffDischargeVoltage = value;

                RaisePropertyChanged();
            }
        }

        public int FullyChargedEndCurrent
        {
            get { return _batterytype.FullyChargedEndCurrent; }
            set
            {
                if (value == _batterytype.FullyChargedEndCurrent)
                    return;

                _batterytype.FullyChargedEndCurrent = value;

                RaisePropertyChanged();
            }
        }

        public int FullyChargedEndingTimeout
        {
            get { return _batterytype.FullyChargedEndingTimeout; }
            set
            {
                if (value == _batterytype.FullyChargedEndingTimeout)
                    return;

                _batterytype.FullyChargedEndingTimeout = value;

                RaisePropertyChanged();
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
                //var dbContext = new AppDbContext();
                //int number = (
                //    from bat in dbContext.BatteryTypes
                //    where bat.Name == _batterytype.Name
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
        bool CanOK
        {
            get { return IsNewBatteryType; }
        }

        #endregion // Private Helpers
    }
}
