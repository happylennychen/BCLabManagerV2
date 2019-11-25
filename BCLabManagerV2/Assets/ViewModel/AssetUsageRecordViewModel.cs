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
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// Editable: false
    /// Updateable: false
    /// </summary>
    public class AssetUsageRecordViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        readonly AssetUsageRecordClass _record;

        #endregion // Fields

        #region Constructor

        public AssetUsageRecordViewModel(AssetUsageRecordClass record/*, BatteryRepository batteryRepository*/)     //AllBatteriesView需要
        {
            if (record == null)
                throw new ArgumentNullException("record");

            _record = record;
            _record.PropertyChanged += _record_PropertyChanged;
        }

        private void _record_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

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
                return _record.ProgramName;
            }
        }
        public String RecipeName
        {
            get
            {
                return _record.RecipeName;
            }
        }

        #endregion // Presentation Properties
    }
}
