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
using Microsoft.Win32;

namespace BCLabManager.ViewModel
{

    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class ProjectSettingCreateViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields

        readonly ProjectSetting _ProjectSetting;
        ObservableCollection<Project> _projects;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public ProjectSettingCreateViewModel(
            ProjectSetting ProjectSetting,
            ObservableCollection<Project> projects)
        {
            if (ProjectSetting == null)
                throw new ArgumentNullException("ProjectSetting");

            _ProjectSetting = ProjectSetting;
            _projects = projects;
        }

        #endregion // Constructor

        #region ProjectSettingClass Properties

        public int Id
        {
            get { return _ProjectSetting.Id; }
            set
            {
                if (value == _ProjectSetting.Id)
                    return;

                _ProjectSetting.Id = value;

                RaisePropertyChanged("Id");
            }
        }
        public int design_capacity_mahr
        {
            get { return _ProjectSetting.design_capacity_mahr; }
            set
            {
                if (value == _ProjectSetting.design_capacity_mahr)
                    return;

                _ProjectSetting.design_capacity_mahr = value;

                RaisePropertyChanged("design_capacity_mahr");
            }
        }
        public Int32 limited_charge_voltage_mv
        {
            get { return _ProjectSetting.limited_charge_voltage_mv; }
            set
            {
                if (value == _ProjectSetting.limited_charge_voltage_mv)
                    return;

                _ProjectSetting.limited_charge_voltage_mv = value;

                RaisePropertyChanged("limited_charge_voltage_mv");
            }
        }
        public Int32 fully_charged_end_current_ma
        {
            get { return _ProjectSetting.fully_charged_end_current_ma; }
            set
            {
                if (value == _ProjectSetting.fully_charged_end_current_ma)
                    return;

                _ProjectSetting.fully_charged_end_current_ma = value;

                RaisePropertyChanged("fully_charged_end_current_ma");
            }
        }
        public Int32 fully_charged_ending_time_ms
        {
            get { return _ProjectSetting.fully_charged_ending_time_ms; }
            set
            {
                if (value == _ProjectSetting.fully_charged_ending_time_ms)
                    return;

                _ProjectSetting.fully_charged_ending_time_ms = value;

                RaisePropertyChanged("fully_charged_ending_time_ms");
            }
        }
        public Int32 discharge_end_voltage_mv
        {
            get { return _ProjectSetting.discharge_end_voltage_mv; }
            set
            {
                if (value == _ProjectSetting.discharge_end_voltage_mv)
                    return;

                _ProjectSetting.discharge_end_voltage_mv = value;

                RaisePropertyChanged("discharge_end_voltage_mv");
            }
        }
        public int threshold_1st_facc_mv
        {
            get { return _ProjectSetting.threshold_1st_facc_mv; }
            set
            {
                if (value == _ProjectSetting.threshold_1st_facc_mv)
                    return;

                _ProjectSetting.threshold_1st_facc_mv = value;

                RaisePropertyChanged("threshold_1st_facc_mv");
            }
        }
        public int threshold_2nd_facc_mv
        {
            get { return _ProjectSetting.threshold_2nd_facc_mv; }
            set
            {
                if (value == _ProjectSetting.threshold_2nd_facc_mv)
                    return;

                _ProjectSetting.threshold_2nd_facc_mv = value;

                RaisePropertyChanged("threshold_2nd_facc_mv");
            }
        }
        public int threshold_3rd_facc_mv
        {
            get { return _ProjectSetting.threshold_3rd_facc_mv; }
            set
            {
                if (value == _ProjectSetting.threshold_3rd_facc_mv)
                    return;

                _ProjectSetting.threshold_3rd_facc_mv = value;

                RaisePropertyChanged("threshold_3rd_facc_mv");
            }
        }
        public int threshold_4th_facc_mv
        {
            get { return _ProjectSetting.threshold_4th_facc_mv; }
            set
            {
                if (value == _ProjectSetting.threshold_4th_facc_mv)
                    return;

                _ProjectSetting.threshold_4th_facc_mv = value;

                RaisePropertyChanged("threshold_4th_facc_mv");
            }
        }
        public int initial_ratio_fcc
        {
            get { return _ProjectSetting.initial_ratio_fcc; }
            set
            {
                if (value == _ProjectSetting.initial_ratio_fcc)
                    return;

                _ProjectSetting.initial_ratio_fcc = value;

                RaisePropertyChanged("initial_ratio_fcc");
            }
        }
        public int accumulated_capacity_mahr
        {
            get { return _ProjectSetting.accumulated_capacity_mahr; }
            set
            {
                if (value == _ProjectSetting.accumulated_capacity_mahr)
                    return;

                _ProjectSetting.accumulated_capacity_mahr = value;

                RaisePropertyChanged("accumulated_capacity_mahr");
            }
        }
        public int dsg_low_volt_mv
        {
            get { return _ProjectSetting.dsg_low_volt_mv; }
            set
            {
                if (value == _ProjectSetting.dsg_low_volt_mv)
                    return;

                _ProjectSetting.dsg_low_volt_mv = value;

                RaisePropertyChanged("dsg_low_volt_mv");
            }
        }
        public int dsg_low_temp_01dc
        {
            get { return _ProjectSetting.dsg_low_temp_01dc; }
            set
            {
                if (value == _ProjectSetting.dsg_low_temp_01dc)
                    return;

                _ProjectSetting.dsg_low_temp_01dc = value;

                RaisePropertyChanged("dsg_low_temp_01dc");
            }
        }
        public int initial_soc_start_ocv
        {
            get { return _ProjectSetting.initial_soc_start_ocv; }
            set
            {
                if (value == _ProjectSetting.initial_soc_start_ocv)
                    return;

                _ProjectSetting.initial_soc_start_ocv = value;

                RaisePropertyChanged("initial_soc_start_ocv");
            }
        }
        public int system_line_impedance
        {
            get { return _ProjectSetting.system_line_impedance; }
            set
            {
                if (value == _ProjectSetting.system_line_impedance)
                    return;

                _ProjectSetting.system_line_impedance = value;

                RaisePropertyChanged("system_line_impedance");
            }
        }
        public bool is_valid
        {
            get { return _ProjectSetting.is_valid; }
            set
            {
                if (value == _ProjectSetting.is_valid)
                    return;

                _ProjectSetting.is_valid = value;

                RaisePropertyChanged("is_valid");
            }
        }
        public string extend_cfg
        {
            get { return _ProjectSetting.extend_cfg; }
            set
            {
                if (value == _ProjectSetting.extend_cfg)
                    return;

                _ProjectSetting.extend_cfg = value;

                RaisePropertyChanged("extend_cfg");
            }
        }

        public Project Project       //选中项
        {
            get
            {
                //if (_batteryType == null)
                //return null;
                return _ProjectSetting.Project;
            }
            set
            {
                if (value == _ProjectSetting.Project)
                    return;

                _ProjectSetting.Project = value;

                RaisePropertyChanged("Project");
                design_capacity_mahr = _ProjectSetting.Project.AbsoluteMaxCapacity;
                limited_charge_voltage_mv = _ProjectSetting.Project.LimitedChargeVoltage;
                fully_charged_end_current_ma = _ProjectSetting.Project.BatteryType.FullyChargedEndCurrent;
                fully_charged_ending_time_ms = _ProjectSetting.Project.BatteryType.FullyChargedEndingTimeout;
                discharge_end_voltage_mv = _ProjectSetting.Project.BatteryType.CutoffDischargeVoltage;
            }
        }

        public ObservableCollection<Project> AllProjects //供选项
        {
            get
            {
                ObservableCollection<Project> all = _projects;

                return new ObservableCollection<Project>(all);
            }
        }

        #endregion // Customer Properties

        #region Presentation Properties


        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    switch (commandType)
                    {
                        case CommandType.Create:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); },
                                param => this.CanCreate
                                );
                            break;
                        case CommandType.Edit:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); }
                                );
                            break;
                        case CommandType.SaveAs:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); },
                                param => this.CanSaveAs
                                );
                            break;
                    }
                }
                return _okCommand;
            }
        }

        public CommandType commandType
        { get; set; }

        public bool IsOK
        {
            get { return _isOK; }
            set { _isOK = value; }
        }

        #endregion // Presentation Properties

        #region Public Methods

        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        public void OK()
        {
            //if (!_ProjectSettingtype.IsValid)
            //throw new InvalidOperationException(Resources.ProjectSettingTypeViewModel_Exception_CannotSave);

            //if (this.IsNewProjectSettingType)
            //_ProjectSettingtypeRepository.AddItem(_ProjectSettingtype);

            //RaisePropertyChanged("DisplayName");
            IsOK = true;
        }

        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewProjectSetting
        {
            get
            {
                //var dbContext = new AppDbContext();
                //int number = (
                //    from bat in dbContext.ProjectSettings
                //    where bat.Name == _ProjectSetting.Name     //名字（某一个属性）一样就认为是一样的
                //    select bat).Count();
                //if (number != 0)
                //    return false;
                //else
                //    return true;
                return true;
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanCreate
        {
            get { return IsNewProjectSetting; }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSaveAs
        {
            get { return IsNewProjectSetting; }
        }

        #endregion // Private Helpers
    }
}