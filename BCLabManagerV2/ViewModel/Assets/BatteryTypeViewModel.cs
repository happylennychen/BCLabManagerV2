using System;
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
        //bool _isSelected;

        #endregion // Fields

        #region Constructor

        public BatteryTypeViewModel(BatteryTypeClass batterytype)  //构造函数里面之所以要batterytyperepository,是因为IsNewBattery需要用此进行判断
        {
            if (batterytype == null)
                throw new ArgumentNullException("batterytype");

            _batterytype = batterytype;
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

                base.OnPropertyChanged("Id");
            }
        }
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
    }
}
