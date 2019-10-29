using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class AssetUsageRecordViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        readonly AssetUsageRecordClass _record;
        //readonly BatteryRepository _batteryRepository;
        //readonly BatteryTypeRepository _batterytypeRepository;
        //bool _isSelected;
        string _batteryType;
        RelayCommand _saveCommand;

        #endregion // Fields

        #region Constructor

        public AssetUsageRecordViewModel(AssetUsageRecordClass record/*, BatteryRepository batteryRepository*/)     //AllBatteriesView需要
        {
            if (record == null)
                throw new ArgumentNullException("record");

            //if (batteryRepository == null)
                //throw new ArgumentNullException("batteryRepository");

            //_battery = batterymodel;
            //_batteryRepository = batteryRepository;
            _record = record;
            _record.PropertyChanged += _record_PropertyChanged;
        }

        private void _record_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
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

        public int Id
        {
            get
            {
                return _record.Id;
            }
        }
        public String AssetUseCount
        {
            get
            {
                return _record.AssetUseCount.ToString();
            }
        }

        public String Time
        {
            get
            {
                return _record.Timestamp.ToString();
            }
        }
        public String ProgramName
        {
            get
            {
                return _record.ProgramName.ToString();
            }
        }
        public String SubProgramName
        {
            get
            {
                return _record.SubProgramName.ToString();
            }
        }

        #endregion // Presentation Properties
    }
}
