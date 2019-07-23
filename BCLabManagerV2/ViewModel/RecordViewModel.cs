using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class RecordViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        readonly RecordClass _record;
        //readonly BatteryRepository _batteryRepository;
        //readonly BatteryTypeRepository _batterytypeRepository;
        //bool _isSelected;
        string _batteryType;
        RelayCommand _saveCommand;

        #endregion // Fields

        #region Constructor

        public RecordViewModel(RecordClass record/*, BatteryRepository batteryRepository*/)     //AllBatteriesView需要
        {
            if (record == null)
                throw new ArgumentNullException("record");

            //if (batteryRepository == null)
                //throw new ArgumentNullException("batteryRepository");

            //_battery = batterymodel;
            //_batteryRepository = batteryRepository;
            _record = record;
        }

        /*public BatteryViewModel(BatteryClass batterymodel, BatteryRepository batteryRepository, BatteryTypeRepository batterytypeRepository)  //BatteryView需要
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
        }*/

        /*void CreateAllBatteryTypes()
        {
            List<BatteryTypeClass> all = _batterytypeRepository.GetItems();

            this.AllBatteryTypes = new ObservableCollection<BatteryTypeClass>(all);
        }*/

        #endregion // Constructor

        #region Presentation Properties

        public String Status
        {
            get
            {
                return _record.Status.ToString();
            }
        }

        public String Time
        {
            get
            {
                return _record.Timestamp.ToString();
            }
        }

        #endregion // Presentation Properties
    }
}
