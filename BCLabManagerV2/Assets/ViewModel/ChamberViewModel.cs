using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// Editable: no need
    /// Updateable: true
    /// </summary>
    public class ChamberViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        readonly ChamberClass _chamber;

        #endregion // Fields

        #region Constructor

        public ChamberViewModel(ChamberClass chamber)
        {
            _chamber = chamber;
            _chamber.PropertyChanged += _chamber_PropertyChanged;
        }

        private void _chamber_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region ChamberClass Properties

        public int Id
        {
            get { return _chamber.Id; }
        }
        public string Name
        {
            get { return _chamber.Name; }
        }

        public string Manufacturer
        {
            get { return _chamber.Manufacturer; }
        }

        public Double LowTemp
        {
            get { return _chamber.LowestTemperature; }
        }

        public Double HighTemp
        {
            get { return _chamber.HighestTemperature; }
        }

        public int AssetUseCount
        {
            get { return _chamber.AssetUseCount; }
        }

        #endregion // Customer Properties
    }
}