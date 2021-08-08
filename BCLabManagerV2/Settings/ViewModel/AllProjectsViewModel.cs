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
        private TableMakerRecordServiceClass _tableMakerRecordService;
        private TesterServiceClass _testerService;
        private BatteryTypeServiceClass _batteryTypeService;

        #endregion // Fields

        #region Constructor

        public AllProjectsViewModel(TesterServiceClass testerService, ProjectServiceClass projectService, BatteryTypeServiceClass batteryTypeServie, ProgramServiceClass programService, ProjectSettingServiceClass projectSettingService, TableMakerProductServiceClass tableMakerProductService, TableMakerRecordServiceClass tableMakerRecordService)
        {
            _testerService = testerService;
            _projectService = projectService;
            _batteryTypeService = batteryTypeServie;
            _programService = programService;
            _projectSettingService = projectSettingService;
            _tableMakerProductService = tableMakerProductService;
            _tableMakerRecordService = tableMakerRecordService;
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
            TableMakerModel tableMakerModel = new TableMakerModel();
            tableMakerModel.Project = _selectedItem._project;
            tableMakerModel.Testers = _testerService.Items.ToList();
            var programs = _programService.Items.Select(o => o).Where(o => o.Project.Id == tableMakerModel.Project.Id && o.IsInvalid == false).ToList();
            tableMakerModel.OCVRecords = GetRecordsFromPrograms(programs.Select(o => o).Where(o => o.Type.Name == "OCV").ToList());
            tableMakerModel.RCRecords = GetRecordsFromPrograms(programs.Select(o => o).Where(o => o.Type.Name == "RC").ToList());
            tableMakerModel.Stage1OCVPrograms = programs.Select(o => o).Where(o => o.Type.Name == "Stage1OCV").ToList();
            tableMakerModel.Stage1RCPrograms = programs.Select(o => o).Where(o => o.Type.Name == "Stage1RC").ToList();
            tableMakerModel.Stage2OCVPrograms = programs.Select(o => o).Where(o => o.Type.Name == "Stage1OCV" || o.Type.Name == "Stage2OCV").ToList();
            tableMakerModel.Stage2RCPrograms = programs.Select(o => o).Where(o => o.Type.Name == "Stage1RC"|| o.Type.Name == "Stage2RC").ToList();
            TableMakerViewModel tableMakerViewModel = new TableMakerViewModel(tableMakerModel, _tableMakerRecordService);
            TableMakerView tableMakerView = new TableMakerView();
            tableMakerView.DataContext = tableMakerViewModel;
            tableMakerView.ShowDialog();
        }

        private List<TestRecord> GetRecordsFromPrograms(List<Program> programs)
        {
            var trs = programs.Select(o => o.Recipes.Select(i => i.TestRecords.Where(j => j.Status == TestStatus.Completed).ToList()).ToList()).ToList();
            List<TestRecord> testRecords = new List<TestRecord>();
            foreach (var tr in trs)
            {
                foreach (var t in tr)
                    testRecords = testRecords.Concat(t).ToList();
            }
            testRecords = testRecords.Where(o => o.Status == TestStatus.Completed).ToList();
            return testRecords;
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
            get
            {
                if (_selectedItem == null)
                    return false;
                var project = _selectedItem._project;
                if (project.VoltagePoints == null)
                    return false;
                if (project.VoltagePoints.Count == 0)
                    return false;

                var ocvPrograms = _programService.Items.Select(o => o).Where(o => o.Project.Id == project.Id && o.IsCompleted == true && (o.Type.Name == "OCV")).ToList();
                var rcPrograms = _programService.Items.Select(o => o).Where(o => o.Project.Id == project.Id && o.IsCompleted == true && (o.Type.Name == "RC")).ToList();

                if (ocvPrograms.Count >= 1)
                {
                    List<string> strTemplates = new List<string>();
                    foreach (var p in ocvPrograms)
                    {
                        if (p.RecipeTemplates != null)
                            strTemplates = strTemplates.Concat(p.RecipeTemplates).ToList();
                    }
                    strTemplates = strTemplates.Distinct().ToList();
                    if (strTemplates.Count == 2)
                        return true;
                }

                if (rcPrograms.Count == 1)
                {
                    return true;
                }
                return false;
            }
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
