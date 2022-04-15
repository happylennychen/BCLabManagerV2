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
using Microsoft.Win32;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TableMakerViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields

        private ProjectServiceClass _projectService;
        private TableMakerRecordServiceClass _tableMakerRecordService;
        private ProgramServiceClass _programService;
        private TesterServiceClass _testerService;
        RelayCommand _buildStage2TableCommand;
        RelayCommand _buildStage1TableCommand;
        RelayCommand _voltagePointsLoadCommand;
        RelayCommand _deleteRecordCommand;

        #endregion // Fields

        #region Constructor

        public TableMakerViewModel(/*TableMakerModel tableMakerModel*/ProjectServiceClass projectService, TableMakerRecordServiceClass tableMakerRecordService, ProgramServiceClass programService, TesterServiceClass testerService)
        {
            //_tableMakerModel = tableMakerModel;
            _projectService = projectService;
            _tableMakerRecordService = tableMakerRecordService;
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
                Stage2SourceList = records.Select(o => GetSourceFile(o)).ToList();
                EOD = _stage2Project.DefaultEOD;
            }
        }

        private string GetSourceFile(TestRecord o)
        {
            if (string.IsNullOrEmpty(o.StdFilePath))
                return o.TestFilePath;
            else
                return o.StdFilePath;
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
                EOD = _stage1Project.DefaultEOD;
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
                RaisePropertyChanged("Sources");
            }
        }

        public List<TableMakerProduct> Products
        {
            get
            {
                if (_selectedRecord == null)
                    return null;
                return _selectedRecord.Products;
            }
        }
        public List<string> Sources
        {
            get
            {
                if (_selectedRecord == null)
                    return null;
                return _selectedRecord.OCVSources.Concat(_selectedRecord.RCSources).ToList();
            }
        }
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
        public ICommand VoltagePointsLoadCommand
        {
            get
            {
                if (_voltagePointsLoadCommand == null)
                {
                    _voltagePointsLoadCommand = new RelayCommand(
                        param => { this.Load(); }
                        );
                }
                return _voltagePointsLoadCommand;
            }
        }
        public ICommand DeleteRecordCommand
        {
            get
            {
                if (_deleteRecordCommand == null)
                {
                    _deleteRecordCommand = new RelayCommand(
                        param => { this.DeleteRecord(); },
                        param => this.CanDeleteRecord
                        );
                }
                return _deleteRecordCommand;
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
                if (EOD == 0)
                    return false;
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

        public bool CanDeleteRecord { get { if (_selectedRecord != null && _selectedRecord.IsValid) return true; else return false; } }

        private void BuildStage2Table()
        {
            //string stage2 = "stage2";
            Stage stage = Stage.N2;
            BuildTables(stage, Stage2Project);
        }

        private void BuildTables(Stage stage, Project stage2Project, Project stage1Project = null)
        {
            OCVModel ocvModel = new OCVModel();
            RCModel rcModel = new RCModel();
            MiniModel miniModel = new MiniModel();
            StandardModel standardModel = new StandardModel();
            AndroidModel androidModel = new AndroidModel();
            LiteModel liteModel = new LiteModel();
            List<string> fileNames;
            Project baseProject;
            if (stage1Project == null)
                baseProject = stage2Project;
            else
                baseProject = stage1Project;
            fileNames = GetFileNames(baseProject, stage);
            ocvModel.FileName = fileNames[0];
            rcModel.FileName = fileNames[1];
            miniModel.FileNames = new List<string>() { fileNames[2], fileNames[3] };
            standardModel.FileNames = new List<string>() { fileNames[4], fileNames[5] };
            androidModel.FileNames = new List<string>() { fileNames[6], fileNames[7] };
            liteModel.FileNames = new List<string>() { fileNames[8], fileNames[9] };

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
                var timestamp = DateTime.Now;
                string time = timestamp.ToString("yyyyMMddHHmmss");
                var OutFolder = $@"{GlobalSettings.UniversalPath}{baseProject.BatteryType.Name}\{baseProject.Name}\{GlobalSettings.ProductFolderName}\{time}";
                try
                {
                    if (MessageBox.Show("It will take a while to get the work done, Continue?", "Generate Tables and Drivers.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                        stopwatch.Start();
                        if (!Directory.Exists(OutFolder))
                        {
                            Directory.CreateDirectory(OutFolder);
                        }
                        var programs = _programService.Items.Select(o => o).Where(o => o.Project.Id == stage2Project.Id && o.IsInvalid == false).ToList();
                        var ocvRecords = GetCompletedRecordsFromPrograms(programs.Select(o => o).Where(o => o.Type.Name == "OCV").ToList());
                        List<TableMakerProduct> products = new List<TableMakerProduct>();

                        List<SourceData> ocvSource = null;
                        List<string> ocvSourceFiles = null;
                        if (ocvRecords != null && ocvRecords.Count != 0)
                        {
                            if (!TableMakerService.GetSourceV2(stage2Project, ocvRecords, testers, out ocvSource, out ocvSourceFiles))
                            {
                                Directory.Delete(OutFolder, true);
                                return;
                            }
                            if (ocvSource == null)
                            {
                                MessageBox.Show("Get OCV source failed.");
                                Directory.Delete(OutFolder, true);
                                return;
                            }
                            if (ocvSource.Count != 2)
                            {
                                MessageBox.Show("OCV source number is not 2.");
                                Directory.Delete(OutFolder, true);
                                return;
                            }
                            OCVTableMaker.GetOCVModel(ocvSource, ref ocvModel);
                            products.Add(OCVTableMaker.GenerateOCVTable(stage, baseProject, time, ocvModel));
                        }
                        List<string> rcSourceFiles = null;
                        if (stage1Project != null)
                        {
                            var rcStage2Records = GetCompletedRecordsFromPrograms(programs.Select(o => o).Where(o => o.Type.Name == "RC").ToList());

                            var rcStage1Files = Stage1SourceList.Where(o => o.IsCheck == true).Select(o => o.FilePath).ToList();
                            var rcStage1programs = _programService.Items.Select(o => o).Where(o => o.Project.Id == _stage1Project.Id && o.IsInvalid == false && o.Type.Name == "RC").ToList();
                            var rcStage1Records = GetCompletedRecordsFromPrograms(rcStage1programs);
                            rcStage1Records = rcStage1Records.Where(o => rcStage1Files.Contains(o.TestFilePath)).ToList();
                            //rcSources = rcStage2Records.Select(o => o.TestFilePath).ToList().Concat(rcStage1Files).ToList();

                            List<SourceData> rcStage2Source;
                            List<SourceData> rcStage1Source;
                            List<string> stage1RcSourceFiles = null;
                            List<string> stage2RcSourceFiles = null;
                            if (!TableMakerService.GetSourceV2(stage1Project, rcStage2Records, testers, out rcStage2Source, out stage2RcSourceFiles))
                                //if (!TableMakerService.GetSource(stage1Project, rcStage2Records, testers, out rcStage2Source, out stage2RcSourceFiles))
                                    return;
                            if (rcStage2Source == null)
                            {
                                MessageBox.Show("Get Stage 2 source failed.");
                                Directory.Delete(OutFolder, true);
                                return;
                            }
                            if (!TableMakerService.GetSourceV2(stage1Project, rcStage1Records, testers, out rcStage1Source, out stage1RcSourceFiles))
                                //if (!TableMakerService.GetSource(stage1Project, rcStage1Records, testers, out rcStage1Source, out stage1RcSourceFiles))
                                    return;
                            if (rcStage1Source == null)
                            {
                                MessageBox.Show("Get Stage 1 source failed.");
                                Directory.Delete(OutFolder, true);
                                return;
                            }
                            rcSourceFiles = stage1RcSourceFiles.Concat(stage2RcSourceFiles).ToList();
                            List<SourceData> rcSource = RCTableMaker.GetNewSources(rcStage2Source, rcStage1Source);
                            RCTableMaker.GetRCModel(rcSource, stage1Project.AbsoluteMaxCapacity, _voltagePoints, ref rcModel); //做出中间table
                            MiniDriverMaker.GetMiniModel(ocvSource, rcSource, ocvModel, rcModel, _voltagePoints, ref miniModel);
                            StandardDriverMaker.GetStandardModel(ocvModel, rcModel, ref standardModel);
                            AndroidDriverMaker.GetAndroidModel(ocvModel, rcModel, ref androidModel);
                            if (!LiteDriverMaker.GetLiteModel(EOD, ocvSource, rcSource, ocvModel, rcModel, stage1Project, _voltagePoints, ref liteModel))
                            {
                                MessageBox.Show("Get lite model failed.");
                                Directory.Delete(OutFolder, true);
                                return;
                            }
                        }
                        else
                        {
                            var rcRecords = GetCompletedRecordsFromPrograms(programs.Select(o => o).Where(o => o.Type.Name == "RC").ToList());
                            //rcSources = rcRecords.Select(o => o.TestFilePath).ToList();

                            List<SourceData> rcSource;
                            if (!TableMakerService.GetSourceV2(stage2Project, rcRecords, testers, out rcSource, out rcSourceFiles))
                            {
                                //if (!TableMakerService.GetSource(stage2Project, rcRecords, testers, out rcSource, out rcSourceFiles))
                                Directory.Delete(OutFolder, true);
                                return;
                            }
                            if (rcSource == null)
                            {
                                MessageBox.Show("Get RC source failed.");
                                Directory.Delete(OutFolder, true);
                                return;
                            }
                            RCTableMaker.GetRCModel(rcSource, stage2Project.AbsoluteMaxCapacity, _voltagePoints, ref rcModel);
                            MiniDriverMaker.GetMiniModel(ocvSource, rcSource, ocvModel, rcModel, _voltagePoints, ref miniModel);
                            StandardDriverMaker.GetStandardModel(ocvModel, rcModel, ref standardModel);
                            AndroidDriverMaker.GetAndroidModel(ocvModel, rcModel, ref androidModel);
                            if (!LiteDriverMaker.GetLiteModel(EOD, ocvSource, rcSource, ocvModel, rcModel, stage2Project, _voltagePoints, ref liteModel))
                            {
                                MessageBox.Show("Get lite model failed.");
                                Directory.Delete(OutFolder, true);
                                return;
                            }
                        }

                        products.Add(RCTableMaker.GenerateRCTable(stage, baseProject, _voltagePoints, time, rcModel));
                        products.AddRange(MiniDriverMaker.GenerateMiniDriver(stage, miniModel, time, baseProject));
                        products.AddRange(StandardDriverMaker.GenerateStandardDriver(stage, standardModel, time, baseProject, _voltagePoints));
                        products.AddRange(AndroidDriverMaker.GenerateAndroidDriver(stage, androidModel, time, baseProject, _voltagePoints));
                        products.AddRange(LiteDriverMaker.GenerateLiteDriver(stage, liteModel, time, baseProject));

                        var tmrs = _tableMakerRecordService;
                        TableMakerRecord tmr = new TableMakerRecord();
                        tmr.EOD = EOD;
                        tmr.TableMakerVersion = TableMakerService.Version;
                        tmr.Description = Description;
                        tmr.IsValid = true;
                        tmr.OCVSources = ocvSourceFiles;
                        tmr.RCSources = rcSourceFiles;
                        tmr.Project = baseProject;
                        tmr.TableMakerVersion = Version;
                        tmr.VoltagePoints = _voltagePoints;
                        tmr.Timestamp = timestamp;
                        tmr.Products = products;
                        tmrs.SuperAdd(tmr);
                        RaisePropertyChanged("TableMakerRecords");

                        var folder = $@"{GlobalSettings.UniversalPath}{baseProject.BatteryType.Name}\{baseProject.Name}\{GlobalSettings.ProductFolderName}\{time}";
                        string timespan = Math.Round(stopwatch.Elapsed.TotalSeconds, 0).ToString() + "S";
                        MessageBox.Show($"Completed. It took {timespan} to get the job done.");
                        Process.Start(folder);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    Directory.Delete(OutFolder, true);
                }
                //);
                //t.Start();
            }
        }

        private List<string> GetFileNames(Project baseProject, Stage stage)
        {
            List<string> output = new List<string>();
            output.Add(OCVTableMaker.GetOCVTableFileName(baseProject, stage));
            output.Add(RCTableMaker.GetRCTableFileName(baseProject, stage));
            List<string> strFileNames;
            TableMakerService.GetDriverFileNames(baseProject.BatteryType.Manufacturer, baseProject.BatteryType.Name, baseProject.AbsoluteMaxCapacity.ToString(), "mini", stage, out strFileNames);
            output.AddRange(strFileNames);
            TableMakerService.GetDriverFileNames(baseProject.BatteryType.Manufacturer, baseProject.BatteryType.Name, baseProject.AbsoluteMaxCapacity.ToString(), "standard", stage, out strFileNames);
            output.AddRange(strFileNames);
            TableMakerService.GetDriverFileNames(baseProject.BatteryType.Manufacturer, baseProject.BatteryType.Name, baseProject.AbsoluteMaxCapacity.ToString(), "android", stage, out strFileNames);
            output.AddRange(strFileNames);
            TableMakerService.GetDriverFileNames(baseProject.BatteryType.Manufacturer, baseProject.BatteryType.Name, baseProject.AbsoluteMaxCapacity.ToString(), "lite", stage, out strFileNames);
            output.AddRange(strFileNames);
            return output;
        }

        private void BuildStage1Table()
        {
            Stage stage = Stage.N1;
            BuildTables(stage, Stage2Project, Stage1Project);
        }

        private void Load()
        {
            var dialog = new OpenFileDialog();
            string voltagepoints = string.Empty;
            dialog.DefaultExt = ".vcfg";
            dialog.Filter = "Voltage Points File|*.vcfg";
            dialog.Title = "Load Voltage Points";
            if (dialog.ShowDialog() == true)
            {
                _voltagePoints = Utilities.LoadVCFGFile(dialog.FileName);
                RaisePropertyChanged("VoltagePoints");
            }
        }

        private void DeleteRecord()
        {
            if (MessageBoxResult.OK == MessageBox.Show("Are you sure to remove this record?", "Deleting Record", MessageBoxButton.OKCancel))
            {
                _selectedRecord.IsValid = false; 
                using (var uow = new UnitOfWork(new AppDbContext()))
                {
                    foreach (var tmp in _selectedRecord.Products)
                    {
                        tmp.IsValid = false;
                        uow.TableMakerProducts.Update(tmp);
                        uow.Commit();
                    }
                }
                _tableMakerRecordService.SuperUpdate(_selectedRecord);
            }
        }
        #endregion // Private Helpers
    }
}