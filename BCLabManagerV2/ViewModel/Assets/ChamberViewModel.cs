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
    public class ChamberViewModel : ViewModelBase//, IDataErrorInfo
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
            OnPropertyChanged(e.PropertyName);
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

                base.OnPropertyChanged("Id");
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

                base.OnPropertyChanged("Name");
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

                base.OnPropertyChanged("Manufactor");
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

                base.OnPropertyChanged("LowTemp");
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

                base.OnPropertyChanged("HighTemp");
            }
        }

        public AssetStatusEnum Status
        {
            get { return _chamber.Status; }
            set
            {
                if (value == _chamber.Status)
                    return;

                _chamber.Status = value;

                base.OnPropertyChanged("Status");
            }
        }

        #endregion // Customer Properties
    }
}