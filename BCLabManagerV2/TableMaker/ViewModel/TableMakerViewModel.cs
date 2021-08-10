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
using System.Threading;
using System.Diagnostics;
using System.Windows;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TableMakerViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields

        private TableMakerModel _tableMakerModel;
        private ProjectServiceClass _projectService;
        private TableMakerRecordServiceClass _tableMakerRecordService;
        private TableMakerProductServiceClass _tableMakerProductService;
        private ProgramServiceClass _programService;
        RelayCommand _generateCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public TableMakerViewModel(/*TableMakerModel tableMakerModel*/ProjectServiceClass projectService, TableMakerRecordServiceClass tableMakerRecordService, TableMakerProductServiceClass tableMakerProductService, ProgramServiceClass programService)
        {
            //_tableMakerModel = tableMakerModel;
            _projectService = projectService;
            _tableMakerRecordService = tableMakerRecordService;
            _tableMakerProductService = tableMakerProductService;
            _programService = programService;
        }
        #endregion // Constructor

        #region Presentation Properties
        public string Version { get { return TableMakerService.Version; } }
        public ObservableCollection<Project> Projects
        {
            get
            {
                return _projectService.Items;
            }
        }
        private Project _project;
        public Project Project
        {
            get { return _project; }
            set
            {
                SetProperty(ref _project, value);
                string strVP = string.Empty;
                foreach (var vp in _project.VoltagePoints)
                {
                    if (vp == _project.VoltagePoints.Last())
                        strVP += vp;
                    else
                        strVP += vp + ", ";
                }
                VoltagePoints = strVP;


                var programs = _programService.Items.Select(o => o).Where(o => o.Project.Id == _project.Id && o.IsInvalid == false).ToList();
                var records = GetRecordsFromPrograms(programs.Select(o => o).Where(o => o.Type.Name == "OCV" || o.Type.Name == "RC").ToList());
                SourceList = records.Select(o => o.TestFilePath).ToList();
            }
        }
        private Project _stage1Project;
        public Project Stage1Project
        {
            get { return _stage1Project; }
            set
            {
                SetProperty(ref _stage1Project, value);


                var programs = _programService.Items.Select(o => o).Where(o => o.Project.Id == _stage1Project.Id && o.IsInvalid == false).ToList();
                var records = GetRecordsFromPrograms(programs.Select(o => o).Where(o => o.Type.Name == "OCV" || o.Type.Name == "RC").ToList());
                Stage1SourceList = records.Select(o => o.TestFilePath).ToList();
            }
        }
        private uint _eod;
        public uint EOD
        {
            get { return _eod; }
            set { SetProperty(ref _eod, value); }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        private string _voltagePoints;
        //[NotMapped]
        public string VoltagePoints
        {
            get { return _voltagePoints; }
            set { SetProperty(ref _voltagePoints, value); }
        }
        private List<string> _sourceList;
        //[NotMapped]
        public List<string> SourceList
        {
            get { return _sourceList; }
            set { SetProperty(ref _sourceList, value); }
        }
        private List<string> _stage1SourceList;
        //[NotMapped]
        public List<string> Stage1SourceList
        {
            get { return _stage1SourceList; }
            set { SetProperty(ref _stage1SourceList, value); }
        }

        public ObservableCollection<TableMakerRecord> Records
        {
            get
            {
                return _tableMakerRecordService.Items;
            }
        }
        private TableMakerRecord _selectedRecord;
        public TableMakerRecord SelectedRecord
        {
            get { return _selectedRecord; }
            set
            {
                SetProperty(ref _selectedRecord, value);
                //Products = new ObservableCollection<TableMakerProduct>(_selectedRecord.Products);
                RaisePropertyChanged("Products");
            }
        }

        public List<TableMakerProduct> Products { get { return _selectedRecord.Products; } }
        #endregion // Presentation Properties

        #region Public Methods


        #endregion // Public Methods

        #region Private Helpers
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
        #endregion // Private Helpers
    }
}