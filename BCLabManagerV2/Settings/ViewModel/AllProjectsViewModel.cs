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
using System.IO;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// Editable: True
    /// Updateable: 1. Battery Types: True, 2. Batteries: True
    /// </summary>
    public class AllProjectsViewModel : BindableBase
    {
        #region Fields
        ProjectViewModel _selectedItem;
        RelayCommand _tableMakerCommand;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _deleteCommand;
        private ProjectServiceClass _projectService;
        private ProgramServiceClass _programService;
        private ProjectSettingServiceClass _projectSettingService;
        private TableMakerProductServiceClass _tableMakerProductService;
        private TesterServiceClass _testerService;
        private BatteryTypeServiceClass _batteryTypeService;

        #endregion // Fields

        #region Constructor

        public AllProjectsViewModel(TesterServiceClass testerService, ProjectServiceClass projectService, BatteryTypeServiceClass batteryTypeServie, ProgramServiceClass programService, ProjectSettingServiceClass projectSettingService, TableMakerProductServiceClass tableMakerProductService)
        {
            _testerService = testerService;
            _projectService = projectService;
            _batteryTypeService = batteryTypeServie;
            _programService = programService;
            _projectSettingService = projectSettingService;
            _tableMakerProductService = tableMakerProductService;
            this.CreateAllProjects(_projectService.Items);
            _projectService.Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var project = item as Project;
                        this.AllProjects.Add(new ProjectViewModel(project));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var project = item as Project;
                        var deletetarget = this.AllProjects.SingleOrDefault(o => o.Id == project.Id);
                        this.AllProjects.Remove(deletetarget);
                    }
                    break;
            }
        }

        void CreateAllProjects(ObservableCollection<Project> projects)
        {
            List<ProjectViewModel> all =
                (from proj in projects
                 select new ProjectViewModel(proj)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllProjects = new ObservableCollection<ProjectViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the BatteryModelViewModel objects.
        /// </summary>
        public ObservableCollection<ProjectViewModel> AllProjects { get; private set; }

        public ProjectViewModel SelectedItem    //绑定选中项
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

        public ICommand TableMakerCommand
        {
            get
            {
                if (_tableMakerCommand == null)
                {
                    _tableMakerCommand = new RelayCommand(
                        param => { this.TableMakerDialog(); },
                        param => this.CanMakeTable
                        );
                }
                return _tableMakerCommand;
            }
        }

        public ICommand CreateCommand
        {
            get
            {
                if (_createCommand == null)
                {
                    _createCommand = new RelayCommand(
                        param => { this.Create(); }
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
        private void TableMakerDialog()
        {
            var testers = _testerService.Items.ToList();
            var project = _selectedItem._project;
            //var projectSetting = _projectSettingService.Items.SingleOrDefault(o => o.Project.Id == project.Id && o.is_valid == true);
            var programs = _programService.Items.Select(o=>o).Where(o=>o.Project.Id == project.Id && o.IsInvalid == false && (o.Type.Name == "RC" || o.Type.Name == "OCV")).ToList();
            project.VoltagePoints = LoadVCFGFile(@"D:\Issues\Open\BC_Lab\Table Maker\30Q.vcfg");
            TableMaker.Make(project, programs, testers, true, true, true, true, true);
        }
        private List<UInt32> LoadVCFGFile(string fullpath)
        {
            List<UInt32> output = new List<uint>();
            FileStream file = new FileStream(fullpath, FileMode.Open);
            StreamReader sr = new StreamReader(file);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                output.Add(Convert.ToUInt32(line));
            }
            sr.Close();
            file.Close();
            output.Sort();
            return output;
        }
        private void Create()
        {
            Project proj = new Project();      //实例化一个新的model
            ProjectEditViewModel projevm = new ProjectEditViewModel(proj, _batteryTypeService.Items);      //实例化一个新的view model
            projevm.DisplayName = "Project-Create";
            var ProjectEditViewInstance = new ProjectView();      //实例化一个新的view
            ProjectEditViewInstance.DataContext = projevm;
            ProjectEditViewInstance.ShowDialog();                   //设置viewmodel属性
            if (projevm.IsOK == true)
            {
                _projectService.SuperAdd(proj);
            }
        }
        private void Edit()
        {
            Project proj = new Project();      //实例化一个新的model
            ProjectEditViewModel projevm = new ProjectEditViewModel(proj, _batteryTypeService.Items);      //实例化一个新的view model
            projevm.DisplayName = "Project-Edit";
            projevm.Id = _selectedItem.Id;
            projevm.Name = _selectedItem.Name;
            projevm.Customer = _selectedItem.Customer;
            projevm.BatteryType = _selectedItem.BatteryType;
            projevm.Description = _selectedItem.Description;
            projevm.CutoffDischargeVoltage = _selectedItem.CutoffDischargeVoltage;
            projevm.LimitedChargeVoltage = _selectedItem.LimitedChargeVoltage;
            projevm.AbsoluteMaxCapacity = _selectedItem.AbsoluteMaxCapacity;
            projevm.VoltagePoints = _selectedItem.VoltagePoints;

            var ProjectEditViewInstance = new ProjectView();      //实例化一个新的view
            ProjectEditViewInstance.DataContext = projevm;
            ProjectEditViewInstance.ShowDialog();
            if (projevm.IsOK == true)
            {
                _projectService.SuperUpdate(proj);
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private bool CanMakeTable
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            Project proj = new Project();      //实例化一个新的model
            ProjectEditViewModel projevm = new ProjectEditViewModel(proj, _batteryTypeService.Items);      //实例化一个新的view model
            projevm.DisplayName = "Project-Save As";
            projevm.Name = _selectedItem.Name;
            projevm.Customer = _selectedItem.Customer;
            projevm.BatteryType = _selectedItem.BatteryType;
            projevm.Description = _selectedItem.Description;
            projevm.CutoffDischargeVoltage = _selectedItem.CutoffDischargeVoltage;
            projevm.LimitedChargeVoltage = _selectedItem.LimitedChargeVoltage;
            projevm.AbsoluteMaxCapacity = _selectedItem.AbsoluteMaxCapacity;
            projevm.VoltagePoints = _selectedItem.VoltagePoints;

            var ProjectEditViewInstance = new ProjectView();      //实例化一个新的view
            ProjectEditViewInstance.DataContext = projevm;
            ProjectEditViewInstance.ShowDialog();
            if (projevm.IsOK == true)
            {
                _projectService.SuperAdd(proj);
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedItem != null; }
        }
        private void Delete()
        {
            //if (_batteryService.Items.Count(o => o.BatteryType.Id == _selectedItem.Id) != 0)
            //{
            //    MessageBox.Show("Before deleting this battery type, please delete all batteries belong to it.");
            //    return;
            //}
            //if (MessageBox.Show("Are you sure?", "Delete Battery Type", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //{
            //    _batteryTypeService.SuperRemove(_selectedItem.Id);
            //}
        }
        private bool CanDelete
        {
            get { return _selectedItem != null; }
        }
        #endregion //Private Helper

        #region Event Handling Methods

        #endregion // Event Handling Methods
    }
}
