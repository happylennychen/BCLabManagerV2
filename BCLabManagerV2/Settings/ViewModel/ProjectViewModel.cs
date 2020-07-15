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
    public class ProjectViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        readonly Project _project;

        #endregion // Fields

        #region Constructor

        public ProjectViewModel(Project project)  //构造函数里面之所以要batterytyperepository,是因为IsNewBattery需要用此进行判断
        {
            if (project == null)
                throw new ArgumentNullException("project");

            _project = project;
            _project.PropertyChanged += _project_PropertyChanged;
        }

        private void _project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region ProjectClass Properties

        public int Id
        {
            get { return _project.Id; }
        }
        public string Customer
        {
            get { return _project.Customer; }
        }

        public string Name
        {
            get { return _project.Name; }
        }

        public BatteryType BatteryType
        {
            get { return _project.BatteryType; }
        }

        public int CutoffDischargeVoltage
        {
            get { return _project.CutoffDischargeVoltage; }
        }

        public string Description
        {
            get { return _project.Description; }
        }

        public int LimitedChargeVoltage
        {
            get { return _project.LimitedChargeVoltage; }
        }

        public int AbsoluteMaxCapacity
        {
            get { return _project.AbsoluteMaxCapacity; }
        }

        public string VoltagePoints
        {
            get { return _project.VoltagePoints; }
        }

        #endregion // Customer Properties
    }
}
