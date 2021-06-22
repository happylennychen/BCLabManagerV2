using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// Editable: True
    /// Updateable: 1. ProjectSettings: True, 2. Records: True
    /// </summary>
    public class AllProjectSettingsViewModel : BindableBase
    {
        #region Fields
        private ProjectSettingServiceClass _ProjectSettingService;
        private ProjectServiceClass _projectService;
        ProjectSettingViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _deleteCommand;

        #endregion // Fields

        #region Constructor

        public AllProjectSettingsViewModel(ProjectSettingServiceClass ProjectSettingService, ProjectServiceClass projectService)
        {
            _ProjectSettingService = ProjectSettingService;
            _projectService = projectService;
            this.CreateAllProjectSettings(_ProjectSettingService.Items);
            _ProjectSettingService.Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var ProjectSetting = item as ProjectSetting;
                        this.AllProjectSettings.Add(new ProjectSettingViewModel(ProjectSetting));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var ProjectSetting = item as ProjectSetting;
                        var deletetarget = this.AllProjectSettings.SingleOrDefault(o => o.Id == ProjectSetting.Id);
                        this.AllProjectSettings.Remove(deletetarget);
                    }
                    break;
            }
        }

        void CreateAllProjectSettings(ObservableCollection<ProjectSetting> projectSettings)
        {
            List<ProjectSetting> allProjectSettings =
                (from ps in projectSettings
                 select ps).ToList();

            var all = allProjectSettings.Select(i=>new ProjectSettingViewModel(i)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllProjectSettings = new ObservableCollection<ProjectSettingViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the ProjectSettingModelViewModel objects.
        /// </summary>
        public ObservableCollection<ProjectSettingViewModel> AllProjectSettings { get; private set; }

        public ProjectSettingViewModel SelectedItem    //绑定选中项，从而改变batteries
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                }
            }
        }

        public ICommand CreateCommand
        {
            get
            {
                if (_createCommand == null)
                {
                    _createCommand = new RelayCommand(
                        param => { this.Create();}
                        );
                }
                return _createCommand;
            }
        }
        public ICommand EditCommand
        {
            get
            {
                if (_editCommand == null)
                {
                    _editCommand = new RelayCommand(
                        param => { this.Edit(); },
                        param => this.CanEdit
                        );
                }
                return _editCommand;
            }
        }
        public ICommand SaveAsCommand
        {
            get
            {
                if (_saveAsCommand == null)
                {
                    _saveAsCommand = new RelayCommand(
                        param => { this.SaveAs(); },
                        param => this.CanSaveAs
                        );
                }
                return _saveAsCommand;
            }
        }
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(
                        param => { this.Delete(); },
                        param => this.CanDelete
                        );
                }
                return _deleteCommand;
            }
        }

        #endregion // Public Interface

        #region Private Helper
        private void Create()
        {
            ProjectSetting editItem = new ProjectSetting();      //实例化一个新的model
            ProjectSettingCreateViewModel bevm = new ProjectSettingCreateViewModel(editItem, _projectService.Items);      //实例化一个新的view model
            bevm.DisplayName = "Project Setting-Create";
            bevm.commandType = CommandType.Create;
            var ProjectSettingViewInstance = new ProjectSettingView();      //实例化一个新的view
            ProjectSettingViewInstance.DataContext = bevm;
            ProjectSettingViewInstance.ShowDialog();                   //设置viewmodel属性
            if (bevm.IsOK == true)
            {
                _ProjectSettingService.SuperAdd(editItem);

            }
        }
        private void Edit()
        {
            ProjectSetting editItem = new ProjectSetting();      //实例化一个新的model
            ProjectSettingEditViewModel bevm = new ProjectSettingEditViewModel(editItem, _projectService.Items);      //实例化一个新的view model
            bevm.Id = _selectedItem.Id;
            bevm.design_capacity_mahr = _selectedItem.design_capacity_mahr;
            bevm.limited_charge_voltage_mv = _selectedItem.limited_charge_voltage_mv;
            bevm.fully_charged_end_current_ma = _selectedItem.fully_charged_end_current_ma;
            bevm.fully_charged_ending_time_ms = _selectedItem.fully_charged_ending_time_ms;
            bevm.discharge_end_voltage_mv = _selectedItem.discharge_end_voltage_mv;
            bevm.threshold_1st_facc_mv = _selectedItem.threshold_1st_facc_mv;
            bevm.threshold_2nd_facc_mv = _selectedItem.threshold_2nd_facc_mv;
            bevm.threshold_3rd_facc_mv = _selectedItem.threshold_3rd_facc_mv;
            bevm.threshold_4th_facc_mv = _selectedItem.threshold_4th_facc_mv;
            bevm.initial_ratio_fcc = _selectedItem.initial_ratio_fcc;
            bevm.accumulated_capacity_mahr = _selectedItem.accumulated_capacity_mahr;
            bevm.dsg_low_volt_mv = _selectedItem.dsg_low_volt_mv;
            bevm.dsg_low_temp_01dc = _selectedItem.dsg_low_temp_01dc;
            bevm.initial_soc_start_ocv = _selectedItem.initial_soc_start_ocv;
            bevm.system_line_impedance = _selectedItem.system_line_impedance;
            bevm.is_valid = _selectedItem.is_valid;
            bevm.extend_cfg = _selectedItem.extend_cfg;
            bevm.Project = bevm.AllProjects.SingleOrDefault(i => i.Id == _selectedItem.Project.Id);
            bevm.DisplayName = "Project Setting-Edit";
            bevm.commandType = CommandType.Edit;
            var ProjectSettingViewInstance = new ProjectSettingView();      //实例化一个新的view
            ProjectSettingViewInstance.DataContext = bevm;
            ProjectSettingViewInstance.ShowDialog();
            if (bevm.IsOK == true)
            {
                _ProjectSettingService.SuperUpdate(editItem);
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            ProjectSetting bc = new ProjectSetting();      //实例化一个新的model
            ProjectSettingEditViewModel bevm = new ProjectSettingEditViewModel(bc, _projectService.Items);      //实例化一个新的view model
            bevm.design_capacity_mahr = _selectedItem.design_capacity_mahr;
            bevm.Project = bevm.AllProjects.SingleOrDefault(i => i.Id == _selectedItem.Project.Id);
            bevm.DisplayName = "Project Setting-Save As";
            bevm.commandType = CommandType.SaveAs;
            var ProjectSettingViewInstance = new ProjectSettingView();      //实例化一个新的view
            ProjectSettingViewInstance.DataContext = bevm;
            ProjectSettingViewInstance.ShowDialog();
            if (bevm.IsOK == true)
            {
                _ProjectSettingService.SuperAdd(bc);
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedItem != null; }
        }
        private void Delete()
        {
            var model = _ProjectSettingService.Items.SingleOrDefault(o => o.Id == _selectedItem.Id);
            if (MessageBox.Show("Are you sure?", "Delete ProjectSetting", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _ProjectSettingService.SuperRemove(_selectedItem.Id);
            }
        }
        private bool CanDelete
        {
            get { return _selectedItem != null; }
        }
        #endregion //Private Helper
    }
}
