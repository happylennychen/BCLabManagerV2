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
    public class TableMakerProductTypeViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        readonly TableMakerProductType _programType;

        #endregion // Fields

        #region Constructor

        public TableMakerProductTypeViewModel(TableMakerProductType programType)  //构造函数里面之所以要batterytyperepository,是因为IsNewBattery需要用此进行判断
        {
            if (programType == null)
                throw new ArgumentNullException("programType");

            _programType = programType;
            _programType.PropertyChanged += _programType_PropertyChanged;
        }

        private void _programType_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region TableMakerProductTypeClass Properties

        public int Id
        {
            get { return _programType.Id; }
        }

        public string Description
        {
            get { return _programType.Description; }
        }

        #endregion // Customer Properties
    }
}
