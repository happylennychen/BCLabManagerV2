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
    /// Editable: no need
    /// Updateable: true
    /// </summary>
    public class BatteryTypeViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        readonly BatteryTypeClass _batterytype;

        #endregion // Fields

        #region Constructor

        public BatteryTypeViewModel(BatteryTypeClass batterytype)  //构造函数里面之所以要batterytyperepository,是因为IsNewBattery需要用此进行判断
        {
            if (batterytype == null)
                throw new ArgumentNullException("batterytype");

            _batterytype = batterytype;
            _batterytype.PropertyChanged += _batterytype_PropertyChanged;
        }

        private void _batterytype_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region BatteryTypeClass Properties

        public int Id
        {
            get { return _batterytype.Id; }
        }
        public string Manufactor
        {
            get { return _batterytype.Manufactor; }
        }

        public string Name
        {
            get { return _batterytype.Name; }
        }

        public string Material
        {
            get { return _batterytype.Material; }
        }

        public int TypicalCapacity
        {
            get { return _batterytype.TypicalCapacity; }
        }

        public int CutoffDischargeVoltage
        {
            get { return _batterytype.CutoffDischargeVoltage; }
        }

        public int FullyChargedEndCurrent
        {
            get { return _batterytype.FullyChargedEndCurrent; }
        }

        public int FullyChargedEndingTimeout
        {
            get { return _batterytype.FullyChargedEndingTimeout; }
        }

        public int LimitedChargeVoltage
        {
            get { return _batterytype.LimitedChargeVoltage; }
        }

        public int NominalVoltage
        {
            get { return _batterytype.NominalVoltage; }
        }

        public int RatedCapacity
        {
            get { return _batterytype.RatedCapacity; }
        }

        #endregion // Customer Properties
    }
}
