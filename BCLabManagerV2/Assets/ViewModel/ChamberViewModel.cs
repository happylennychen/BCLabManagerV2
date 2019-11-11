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
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class ChamberViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        readonly ChamberClass _chamber;

        #endregion // Fields

        #region Constructor

        public ChamberViewModel(ChamberClass chamber)
        {
            //if (chamberRepository == null)
            //    throw new ArgumentNullException("chamberRepository");

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
            set
            {
                if (value == _chamber.Id)
                    return;

                _chamber.Id = value;

                RaisePropertyChanged("Id");
            }
        }
        public string Name
        {
            get { return _chamber.Name; }
            set
            {
                if (value == _chamber.Name)
                    return;

                _chamber.Name = value;

                RaisePropertyChanged("Name");
            }
        }

        public string Manufactor
        {
            get { return _chamber.Manufactor; }
            set
            {
                if (value == _chamber.Manufactor)
                    return;

                _chamber.Manufactor = value;

                RaisePropertyChanged("Manufactor");
            }
        }

        public Double LowTemp
        {
            get { return _chamber.LowestTemperature; }
            set
            {
                if (value == _chamber.LowestTemperature)
                    return;

                _chamber.LowestTemperature = value;

                RaisePropertyChanged("LowTemp");
            }
        }

        public Double HighTemp
        {
            get { return _chamber.HighestTemperature; }
            set
            {
                if (value == _chamber.HighestTemperature)
                    return;

                _chamber.HighestTemperature = value;

                RaisePropertyChanged("HighTemp");
            }
        }

        public int AssetUseCount
        {
            get { return _chamber.AssetUseCount; }
            set
            {
                if (value == _chamber.AssetUseCount)
                    return;

                _chamber.AssetUseCount = value;

                RaisePropertyChanged("AssetUseCount");
            }
        }

        #endregion // Customer Properties
    }
}