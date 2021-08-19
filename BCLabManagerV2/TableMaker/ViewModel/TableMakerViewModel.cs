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
using BCLabManager.View;
using System.IO;

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
        private TesterServiceClass _testerService;
        RelayCommand _buildStage2TableCommand;
        RelayCommand _buildStage1TableCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public TableMakerViewModel(/*TableMakerModel tableMakerModel*/ProjectServiceClass projectService, TableMakerRecordServiceClass tableMakerRecordService, TableMakerProductServiceClass tableMakerProductService, ProgramServiceClass programService, TesterServiceClass testerService)
        {
            //_tableMakerModel = tableMakerModel;
            _projectService = projectService;
            _tableMakerRecordService = tableMakerRecordService;
            _tableMakerProductService = tableMakerProductService;
            _programService = programService;
            _testerService = testerService;
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
        private Project _stage2Project;
        public Project Stage2Project
        {
            get { return _stage2Project; }
            set
            {
                SetProperty(ref _stage2Project, value);
                _voltagePoints = _stage2Project.VoltagePoints;
                RaisePropertyChanged("VoltagePoints");


                var programs = _programService.Items.Select(o => o).Where(o => o.Project.Id == _stage2Project.Id && o.IsInvalid == false).ToList();
                var records = GetCompletedRecordsFromPrograms(programs.Select(o => o).Where(o => o.Type.Name == "OCV" || o.Type.Name == "RC").ToList());
                Stage2SourceList = records.Select(o => o.TestFilePath).ToList();
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
                var records = GetCompletedRecordsFromPrograms(programs.Select(o => o).Where(o => o.Type.Name == "RC").ToList());
                Stage1SourceList = records.OrderBy(o => o.Temperature).ThenBy(o => o.Current).Select(o => new Stage1Source(o.TestFilePath, false)).ToList();
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
        private List<int> _voltagePoints;
        //[NotMapped]
        public string VoltagePoints
        {
            get
            {
                string strVP = string.Empty;
                if (_voltagePoints != null)
                {
                    foreach (var vp in _voltagePoints)
                    {
                        if (vp == _voltagePoints.Last())
                            strVP += vp;
                        else
                            strVP += vp + ", ";
                    }
                }
                return strVP;
            }
        }
        private List<string> _stage2sourceList;
        //[NotMapped]
        public List<string> Stage2SourceList
        {
            get { return _stage2sourceList; }
            set { SetProperty(ref _stage2sourceList, value); }
        }
        private List<Stage1Source> _stage1SourceList;
        //[NotMapped]
        public List<Stage1Source> Stage1SourceList
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
        public ICommand BuildStage2TableCommand
        {
            get
            {
                if (_buildStage2TableCommand == null)
                {
                    _buildStage2TableCommand = new RelayCommand(
                        param => { this.BuildStage2Table(); },
                        param => this.CanBuildStage2Table
                        );
                }
                return _buildStage2TableCommand;
            }
        }
        public ICommand BuildStage1TableCommand
        {
            get
            {
                if (_buildStage1TableCommand == null)
                {
                    _buildStage1TableCommand = new RelayCommand(
                        param => { this.BuildStage1Table(); },
                        param => this.CanBuildStage1Table
                        );
                }
                return _buildStage1TableCommand;
            }
        }

        #endregion // Public Methods

        #region Private Helpers
        private List<TestRecord> GetCompletedRecordsFromPrograms(List<Program> programs)
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

        public bool CanBuildStage2Table
        {
            get
            {
                if (Stage2SourceList == null)
                    return false;
                if (VoltagePoints == null)
                    return false;
                if (VoltagePoints.Count() == 0)
                    return false;

                var ocvPrograms = _programService.Items.Select(o => o).Where(o => o.Project.Id == Stage2Project.Id && o.IsInvalid == false && (o.Type.Name == "OCV")).ToList();
                var rcPrograms = _programService.Items.Select(o => o).Where(o => o.Project.Id == Stage2Project.Id && o.IsInvalid == false && (o.Type.Name == "RC")).ToList();

                if (ocvPrograms.Count >= 1)
                {
                    var ocvRecords = GetCompletedRecordsFromPrograms(ocvPrograms);
                    if (ocvRecords.Count < 2)
                        return false;
                }

                if (rcPrograms.Count < 1)
                {
                    //var rcRecords = GetRecordsFromPrograms(rcPrograms);
                    //if (rcRecords.Count != rcPrograms.Sum(p => p.Recipes.Count * p.Temperatures.Count))
                    return false;
                }
                return true;
            }
        }

        public bool CanBuildStage1Table
        {
            get
            {
                if (CanBuildStage2Table == false)
                    return false;
                if (Stage1SourceList == null)
                    return false;
                return true;
            }
        }

        private void BuildStage2Table()
        {
            string stage2 = "stage2";
            OCVModel ocvModel = new OCVModel();
            RCModel rcModel = new RCModel();
            MiniModel miniModel = new MiniModel();
            StandardModel standardModel = new StandardModel();
            AndroidModel androidModel = new AndroidModel();
            LiteModel liteModel = new LiteModel();

            //tableMakerModel.OCVModel = ocvModel;
            //tableMakerModel.RCModel = rcModel;
            //tableMakerModel.MiniModel = miniModel;
            //tableMakerModel.StandardModel = standardModel;
            //tableMakerModel.AndroidModel = androidModel;
            //tableMakerModel.LiteModel = liteModel;
            var project = Stage2Project;
            ocvModel.FileName = OCVTableMaker.GetOCVTableFileName(project, stage2);
            rcModel.FileName = RCTableMaker.GetRCTableFileName(project, stage2);
            List<string> strFileNames;
            TableMakerService.GetDriverFileNames(project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), "mini", stage2, out strFileNames);
            miniModel.FileNames = strFileNames;
            TableMakerService.GetDriverFileNames(project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), "standard", stage2, out strFileNames);
            standardModel.FileNames = strFileNames;
            TableMakerService.GetDriverFileNames(project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), "android", stage2, out strFileNames);
            androidModel.FileNames = strFileNames;
            TableMakerService.GetDriverFileNames(project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), "lite", stage2, out strFileNames);
            liteModel.FileNames = strFileNames;

            var testers = _testerService.Items.ToList();
            TableMakerConfirmationViewModel tmcvm = new TableMakerConfirmationViewModel();
            tmcvm.AndroidDriverCFileName = androidModel.FileNames[0];
            tmcvm.AndroidDriverHFileName = androidModel.FileNames[1];
            tmcvm.MiniDriverCFileName = miniModel.FileNames[0];
            tmcvm.MiniDriverHFileName = miniModel.FileNames[1];
            tmcvm.StandardDriverCFileName = standardModel.FileNames[0];
            tmcvm.StandardDriverHFileName = standardModel.FileNames[1];
            tmcvm.LiteDriverCFileName = liteModel.FileNames[0];
            tmcvm.LiteDriverHFileName = liteModel.FileNames[1];
            tmcvm.OCVFileName = ocvModel.FileName;
            tmcvm.RCFileName = rcModel.FileName;
            tmcvm.OCVReady = true;
            tmcvm.RCReady = true;
            TableMakerConfirmationWindow tmcw = new TableMakerConfirmationWindow();
            tmcw.DataContext = tmcvm;
            tmcw.ShowDialog();
            if (tmcvm.IsOK)
            {
                //Thread t = new Thread(() =>
                {
                    if (MessageBox.Show("It will take a while to get the work done, Continue?", "Generate Tables and Drivers.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                        stopwatch.Start();
                        string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                        var OutFolder = $@"{GlobalSettings.RemotePath}{project.BatteryType.Name}\{project.Name}\{GlobalSettings.ProductFolderName}\{time}";
                        if (!Directory.Exists(OutFolder))
                        {
                            Directory.CreateDirectory(OutFolder);
                        }
                        //TableMakerService.Build(ref _tableMakerModel, 800, "Some description", _tableMakerRecordService);
                        //var project = _tableMakerModel.Project;
                        var tmrs = _tableMakerRecordService;
                        TableMakerRecord tmr = new TableMakerRecord();
                        tmr.EOD = EOD;
                        tmr.Description = Description;
                        tmr.IsValid = true;
                        var programs = _programService.Items.Select(o => o).Where(o => o.Project.Id == _stage2Project.Id && o.IsInvalid == false).ToList();
                        var ocvRecords = GetCompletedRecordsFromPrograms(programs.Select(o => o).Where(o => o.Type.Name == "OCV").ToList());
                        var rcRecords = GetCompletedRecordsFromPrograms(programs.Select(o => o).Where(o => o.Type.Name == "RC").ToList());
                        tmr.OCVSources = ocvRecords.Select(o => o.TestFilePath).ToList();
                        tmr.RCSources = rcRecords.Select(o => o.TestFilePath).ToList();
                        tmr.Project = project;
                        tmr.TableMakerVersion = Version;
                        tmr.VoltagePoints = _voltagePoints;
                        tmr.Timestamp = DateTime.Now;
                        //var products = GenerateFilePackage(tmr.Timestamp, ocvRecords, rcRecords, project, testers, uEodVoltage, ref ocvModel, ref rcModel, ref miniModel, ref standardModel, ref androidModel, ref liteModel);

                        List<TableMakerProduct> products = new List<TableMakerProduct>();
                        List<SourceData> ocvSource = null;
                        if (ocvRecords != null && ocvRecords.Count != 0)
                        {
                            OCVTableMaker.GetOCVSource(project, ocvRecords, testers, out ocvSource);
                            OCVTableMaker.GetOCVModel(ocvSource, ref ocvModel);
                            products.Add(OCVTableMaker.GenerateOCVTable(project, time, ocvModel));
                        }

                        if (rcRecords != null && rcRecords.Count != 0 && ocvSource != null)
                        {
                            List<SourceData> rcSource;
                            RCTableMaker.GetRCSource(project, rcRecords, testers, out rcSource);
                            RCTableMaker.GetRCModel(rcSource, project.AbsoluteMaxCapacity, project.VoltagePoints, ref rcModel);
                            MiniDriverMaker.GetMiniModel(ocvSource, rcSource, ocvModel, rcModel, project.VoltagePoints, ref miniModel);

                            StandardDriverMaker.GetStandardModel(ocvModel, rcModel, ref standardModel);

                            AndroidDriverMaker.GetAndroidModel(ocvModel, rcModel, ref androidModel);
                            products.Add(RCTableMaker.GenerateRCTable(project, _voltagePoints, time, rcModel));
                            products.AddRange(MiniDriverMaker.GenerateMiniDriver(miniModel, time, project));
                            products.AddRange(StandardDriverMaker.GenerateStandardDriver(standardModel, time, project, _voltagePoints));
                            products.AddRange(AndroidDriverMaker.GenerateAndroidDriver(androidModel, time, project, _voltagePoints));

                            LiteDriverMaker.GetLiteModel(EOD, ocvSource, rcSource, ocvModel, rcModel, project, _voltagePoints, ref liteModel);
                            products.AddRange(LiteDriverMaker.GenerateLiteDriver(liteModel, time, project));
                        }

                        tmr.Products = products;
                        tmrs.SuperAdd(tmr);

                        var folder = $@"{GlobalSettings.LocalFolder}{project.BatteryType.Name}\{project.Name}\{GlobalSettings.ProductFolderName}\{time}";
                        string timespan = Math.Round(stopwatch.Elapsed.TotalSeconds, 0).ToString() + "S";
                        MessageBox.Show($"Completed. It took {timespan} to get the job done.");
                        Process.Start(folder);
                    }
                }//);
                //t.Start();
            }
        }
        private void BuildStage1Table()
        {
            string stage1 = "stage1";
            OCVModel ocvModel = new OCVModel();
            RCModel rcModel = new RCModel();
            MiniModel miniModel = new MiniModel();
            StandardModel standardModel = new StandardModel();
            AndroidModel androidModel = new AndroidModel();
            LiteModel liteModel = new LiteModel();

            //tableMakerModel.OCVModel = ocvModel;
            //tableMakerModel.RCModel = rcModel;
            //tableMakerModel.MiniModel = miniModel;
            //tableMakerModel.StandardModel = standardModel;
            //tableMakerModel.AndroidModel = androidModel;
            //tableMakerModel.LiteModel = liteModel;

            var stage2project = Stage2Project;
            var stage1project = Stage1Project;
            ocvModel.FileName = OCVTableMaker.GetOCVTableFileName(stage1project, stage1);
            rcModel.FileName = RCTableMaker.GetRCTableFileName(stage1project, stage1);
            List<string> strFileNames;
            TableMakerService.GetDriverFileNames(stage1project.BatteryType.Manufacturer, stage1project.BatteryType.Name, stage1project.AbsoluteMaxCapacity.ToString(), "mini", stage1, out strFileNames);
            miniModel.FileNames = strFileNames;
            TableMakerService.GetDriverFileNames(stage1project.BatteryType.Manufacturer, stage1project.BatteryType.Name, stage1project.AbsoluteMaxCapacity.ToString(), "standard", stage1, out strFileNames);
            standardModel.FileNames = strFileNames;
            TableMakerService.GetDriverFileNames(stage1project.BatteryType.Manufacturer, stage1project.BatteryType.Name, stage1project.AbsoluteMaxCapacity.ToString(), "android", stage1, out strFileNames);
            androidModel.FileNames = strFileNames;
            TableMakerService.GetDriverFileNames(stage1project.BatteryType.Manufacturer, stage1project.BatteryType.Name, stage1project.AbsoluteMaxCapacity.ToString(), "lite", stage1, out strFileNames);
            liteModel.FileNames = strFileNames;

            var testers = _testerService.Items.ToList();
            TableMakerConfirmationViewModel tmcvm = new TableMakerConfirmationViewModel();
            tmcvm.AndroidDriverCFileName = androidModel.FileNames[0];
            tmcvm.AndroidDriverHFileName = androidModel.FileNames[1];
            tmcvm.MiniDriverCFileName = miniModel.FileNames[0];
            tmcvm.MiniDriverHFileName = miniModel.FileNames[1];
            tmcvm.StandardDriverCFileName = standardModel.FileNames[0];
            tmcvm.StandardDriverHFileName = standardModel.FileNames[1];
            tmcvm.LiteDriverCFileName = liteModel.FileNames[0];
            tmcvm.LiteDriverHFileName = liteModel.FileNames[1];
            tmcvm.OCVFileName = ocvModel.FileName;
            tmcvm.RCFileName = rcModel.FileName;
            tmcvm.OCVReady = true;
            tmcvm.RCReady = true;
            TableMakerConfirmationWindow tmcw = new TableMakerConfirmationWindow();
            tmcw.DataContext = tmcvm;
            tmcw.ShowDialog();
            if (tmcvm.IsOK)
            {
                //Thread t = new Thread(() =>
                {
                    if (MessageBox.Show("It will take a while to get the work done, Continue?", "Generate Tables and Drivers.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                        stopwatch.Start();
                        string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                        //var OutFolder = $@"{GlobalSettings.RemotePath}{stage1project.BatteryType.Name}\{stage1project.Name}\{GlobalSettings.ProductFolderName}\{time}";
                        //if (!Directory.Exists(OutFolder))
                        //{
                        //    Directory.CreateDirectory(OutFolder);
                        //}

                        var stage2programs = _programService.Items.Select(o => o).Where(o => o.Project.Id == _stage2Project.Id && o.IsInvalid == false).ToList();
                        var ocvStage2Records = GetCompletedRecordsFromPrograms(stage2programs.Select(o => o).Where(o => o.Type.Name == "OCV").ToList());
                        var rcStage2Records = GetCompletedRecordsFromPrograms(stage2programs.Select(o => o).Where(o => o.Type.Name == "RC").ToList());

                        var rcStage1Files = Stage1SourceList.Where(o => o.IsCheck == true).Select(o => o.FilePath).ToList();
                        var rcStage1programs = _programService.Items.Select(o => o).Where(o => o.Project.Id == _stage1Project.Id && o.IsInvalid == false && o.Type.Name == "RC").ToList();
                        var rcStage1Records = GetCompletedRecordsFromPrograms(rcStage1programs);
                        rcStage1Records = rcStage1Records.Where(o => rcStage1Files.Contains(o.TestFilePath)).ToList();
                        List<TestRecord> newRecords = RCTableMaker.GetNewRecords(rcStage2Records, rcStage1Records);

                        var tmrs = _tableMakerRecordService;
                        TableMakerRecord tmr = new TableMakerRecord();
                        tmr.EOD = EOD;
                        tmr.Description = Description;
                        tmr.IsValid = true;

                        tmr.OCVSources = ocvStage2Records.Select(o => o.TestFilePath).ToList();
                        tmr.RCSources = rcStage2Records.Select(o => o.TestFilePath).ToList().Concat(rcStage1Files).ToList();
                        tmr.Project = stage1project;
                        tmr.TableMakerVersion = Version;
                        tmr.VoltagePoints = _voltagePoints;
                        tmr.Timestamp = DateTime.Now;

                        List<TableMakerProduct> products = new List<TableMakerProduct>();
                        List<SourceData> ocvSource = null;
                        if (ocvStage2Records != null && ocvStage2Records.Count != 0)
                        {
                            OCVTableMaker.GetOCVSource(stage2project, ocvStage2Records, testers, out ocvSource);
                            OCVTableMaker.GetOCVModel(ocvSource, ref ocvModel);
                            products.Add(OCVTableMaker.GenerateOCVTable(stage1project, time, ocvModel));
                        }

                        if (rcStage2Records != null && rcStage2Records.Count != 0 && ocvSource != null)
                        {
                            List<SourceData> rcStage2Source;
                            List<SourceData> rcStage1Source;
                            RCTableMaker.GetRCSource(stage1project, rcStage2Records, testers, out rcStage2Source);
                            RCTableMaker.GetRCSource(stage1project, rcStage1Records, testers, out rcStage1Source);
                            List<SourceData> rcSource = RCTableMaker.GetNewSources(rcStage2Source, rcStage1Source);
                            RCTableMaker.GetRCModel(rcSource, stage1project.AbsoluteMaxCapacity, _voltagePoints, ref rcModel); //做出中间table
                            MiniDriverMaker.GetMiniModel(ocvSource, rcSource, ocvModel, rcModel, _voltagePoints, ref miniModel);

                            StandardDriverMaker.GetStandardModel(ocvModel, rcModel, ref standardModel);

                            AndroidDriverMaker.GetAndroidModel(ocvModel, rcModel, ref androidModel);
                            products.Add(RCTableMaker.GenerateRCTable(stage1project, _voltagePoints, time, rcModel));
                            products.AddRange(MiniDriverMaker.GenerateMiniDriver(miniModel, time, stage1project));
                            products.AddRange(StandardDriverMaker.GenerateStandardDriver(standardModel, time, stage1project, _voltagePoints));
                            products.AddRange(AndroidDriverMaker.GenerateAndroidDriver(androidModel, time, stage1project, _voltagePoints));

                            LiteDriverMaker.GetLiteModel(EOD, ocvSource, rcSource, ocvModel, rcModel, stage1project, _voltagePoints, ref liteModel);
                            products.AddRange(LiteDriverMaker.GenerateLiteDriver(liteModel, time, stage1project));
                        }

                        tmr.Products = products;
                        tmrs.SuperAdd(tmr);

                        var folder = $@"{GlobalSettings.LocalFolder}{stage1project.BatteryType.Name}\{stage1project.Name}\{GlobalSettings.ProductFolderName}\{time}";
                        string timespan = Math.Round(stopwatch.Elapsed.TotalSeconds, 0).ToString() + "S";
                        MessageBox.Show($"Completed. It took {timespan} to get the job done.");
                        Process.Start(folder);
                    }
                }//);
                //t.Start();
            }
        }
        #endregion // Private Helpers
    }
}