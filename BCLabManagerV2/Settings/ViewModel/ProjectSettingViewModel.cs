using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;
using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// Editable: no need
    /// Updateable: true
    /// </summary>
    public class ProjectSettingViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        readonly ProjectSetting _ProjectSetting;

        #endregion // Fields

        #region Constructor

        public ProjectSettingViewModel(ProjectSetting ProjectSetting)
        {
            if (ProjectSetting == null)
                throw new ArgumentNullException("ProjectSetting");

            _ProjectSetting = ProjectSetting;

            _ProjectSetting.PropertyChanged += _ProjectSetting_PropertyChanged;
        }

        private void _ProjectSetting_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region ProjectSettingClass Properties

        public int Id
        {
            get { return _ProjectSetting.Id; }
        }
        public int design_capacity_mahr
        {
            get { return _ProjectSetting.design_capacity_mahr; }
        }
        public Int32 limited_charge_voltage_mv
        {
            get { return _ProjectSetting.limited_charge_voltage_mv; }
        }
        public Int32 fully_charged_end_current_ma
        {
            get { return _ProjectSetting.fully_charged_end_current_ma; }
        }
        public Int32 fully_charged_ending_time_ms
        {
            get { return _ProjectSetting.fully_charged_ending_time_ms; }
        }
        public Int32 discharge_end_voltage_mv
        {
            get { return _ProjectSetting.discharge_end_voltage_mv; }
        }
        public int threshold_1st_facc_mv
        {
            get { return _ProjectSetting.threshold_1st_facc_mv; }
        }
        public int threshold_2nd_facc_mv
        {
            get { return _ProjectSetting.threshold_2nd_facc_mv; }
        }
        public int threshold_3rd_facc_mv
        {
            get { return _ProjectSetting.threshold_3rd_facc_mv; }
        }
        public int threshold_4th_facc_mv
        {
            get { return _ProjectSetting.threshold_4th_facc_mv; }
        }
        public int initial_ratio_fcc
        {
            get { return _ProjectSetting.initial_ratio_fcc; }
        }
        public int accumulated_capacity_mahr
        {
            get { return _ProjectSetting.accumulated_capacity_mahr; }
        }
        public int dsg_low_volt_mv
        {
            get { return _ProjectSetting.dsg_low_volt_mv; }
        }
        public int dsg_low_temp_01dc
        {
            get { return _ProjectSetting.dsg_low_temp_01dc; }
        }
        public int initial_soc_start_ocv
        {
            get { return _ProjectSetting.initial_soc_start_ocv; }
        }
        public int system_line_impedance
        {
            get { return _ProjectSetting.system_line_impedance; }
        }
        public bool is_valid
        {
            get { return _ProjectSetting.is_valid; }
        }
        public string extend_cfg
        {
            get { return _ProjectSetting.extend_cfg; }
        }

        public Project Project
        {
            get { return _ProjectSetting.Project; }
            set
            {
                if (value == _ProjectSetting.Project)
                    return;

                _ProjectSetting.Project = value;

                RaisePropertyChanged("Project");
            }
        }
        #endregion
    }
}