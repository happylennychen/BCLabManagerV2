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
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Windows.Media;
using LiveCharts.Helpers;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class DataAnalyzerViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields

        private BatteryTypeServiceClass _batteryTypeService;
        private ProjectServiceClass _projectService;
        private ProgramServiceClass _programService;
        RelayCommand _cvtCurveCommand;
        RelayCommand _capacityCurveCommand;
        RelayCommand _applySelectionCommand;

        #endregion // Fields

        #region Constructor

        public DataAnalyzerViewModel(BatteryTypeServiceClass batteryTypeService, ProjectServiceClass projectService, ProgramServiceClass programService)
        {
            _batteryTypeService = batteryTypeService;
            _projectService = projectService;
            _programService = programService;
            CTVSeriesCollection = new SeriesCollection();
            CapacitySeriesCollection = new SeriesCollection();
            YFormatter = value => value.ToString("N0");
        }
        #endregion // Constructor
        #region Presentation Properties

        public List<BatteryType> BatteryTypes
        {
            get
            {
                return _batteryTypeService.Items.Where(bt => bt.Projects.Where(prj => prj.Programs.Where(pro => pro.IsInvalid == false).Count() > 0).Count() > 0).ToList();
            }
        }
        private BatteryType _selectedBatteryType;
        public BatteryType SelectedBatteryType
        {
            get
            {
                return _selectedBatteryType;
            }
            set
            {
                _selectedBatteryType = value;
                RaisePropertyChanged("Projects");
                RaisePropertyChanged("Files");

                UpdateTests();
                UpdateCurrentList();
                UpdateTemperatureList();
            }
        }

        private void UpdateTests()
        {
            IEnumerable<TestViewModel> output = null;
            if (SelectedBatteryType != null)
            {
                if (SelectedProject != null)
                {
                    if (SelectedProgram != null)
                    {
                        output = SelectedProgram.Recipes.SelectMany(rec => rec.TestRecords).Where(tr => tr.Current != 0 && tr.DischargeCapacity != 0).Select(tr => new TestViewModel(tr));
                    }
                    else
                        output = SelectedProject.Programs.SelectMany(pro => pro.Recipes).SelectMany(rec => rec.TestRecords).Where(tr => tr.Current != 0 && tr.DischargeCapacity != 0).Select(tr => new TestViewModel(tr));
                }
                else
                    output = SelectedBatteryType.Projects.SelectMany(pro => pro.Programs).SelectMany(pro => pro.Recipes).SelectMany(rec => rec.TestRecords).Where(tr => tr.Current != 0 && tr.DischargeCapacity != 0).Select(tr => new TestViewModel(tr));
            }
            else
                Tests = null;
            output = output.OrderBy(tr => tr.Temperature).ThenBy(tr => tr.Current);
            Tests = new ObservableCollection<TestViewModel>(output);
            RaisePropertyChanged("Tests");
        }
        private void UpdateCurrentList()
        {
            IEnumerable<CurrentViewModel> output = Tests.Select(tr => tr.Current).Distinct().OrderBy(o=>o).Select(cur => new CurrentViewModel(cur));
            CurrentList = new ObservableCollection<CurrentViewModel>(output);
            RaisePropertyChanged("CurrentList");
        }
        private void UpdateTemperatureList()
        {
            IEnumerable<TemperatureViewModel> output = Tests.Select(tr => tr.Temperature).Distinct().OrderBy(o => o).Select(temp=>new TemperatureViewModel(temp));
            TemperatureList = new ObservableCollection<TemperatureViewModel>(output);
            RaisePropertyChanged("TemperatureList");
        }

        public ObservableCollection<Project> Projects
        {
            get
            {
                if (SelectedBatteryType != null)
                {
                    var list = SelectedBatteryType.Projects.Where(prj => prj.Programs.Where(pro => pro.IsInvalid == false).Count() > 0);
                    return new ObservableCollection<Project>(list);
                }
                else return null;
            }
        }
        private Project _selectedProject;
        public Project SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
                RaisePropertyChanged("Programs");
                RaisePropertyChanged("Files");

                UpdateTests();
                UpdateCurrentList();
                UpdateTemperatureList();
            }
        }
        public ObservableCollection<Program> Programs
        {
            get
            {
                if (SelectedProject != null)
                {
                    var list = SelectedProject.Programs.Where(pro => pro.IsInvalid == false);
                    return new ObservableCollection<Program>(list);
                }
                else return null;
            }
        }
        private Program _selectedProgram;
        public Program SelectedProgram
        {
            get { return _selectedProgram; }
            set
            {
                _selectedProgram = value;
                RaisePropertyChanged("Files");

                UpdateTests();
                UpdateCurrentList();
                UpdateTemperatureList();
            }
        }
        public ObservableCollection<String> Files
        {
            get
            {
                IEnumerable<String> output = null;
                if (SelectedBatteryType != null)
                {
                    if (SelectedProject != null)
                    {
                        if (SelectedProgram != null)
                        {
                            output = SelectedProgram.Recipes.SelectMany(rec => rec.TestRecords).Select(tr => tr.StdFilePath);
                        }
                        else
                            output = SelectedProject.Programs.SelectMany(pro => pro.Recipes).SelectMany(rec => rec.TestRecords).Select(tr => tr.StdFilePath);
                    }
                    else
                        output = SelectedBatteryType.Projects.SelectMany(pro => pro.Programs).SelectMany(pro => pro.Recipes).SelectMany(rec => rec.TestRecords).Select(tr => tr.StdFilePath);
                }
                else
                    return null;
                output = output.Where(o => o != string.Empty && o != null);
                return new ObservableCollection<String>(output);
            }
        }
        public ObservableCollection<TestViewModel> Tests
        {
            set;
            get;
        }
        public ObservableCollection<CurrentViewModel> CurrentList
        {
            set;
            get;
        }
        public ObservableCollection<TemperatureViewModel> TemperatureList
        {
            set;
            get;
        }

        public string SelectedFile { get; set; }

        //public ChartValues<ObservableValue> CurrentPoints { get; set; }
        //public ChartValues<ObservableValue> VoltagePoints { get; set; }
        //public ChartValues<ObservableValue> TemperaturePoints { get; set; }
        public SeriesCollection CTVSeriesCollection { get; set; }
        public SeriesCollection CapacitySeriesCollection { get; set; }
        private string[] _labels;
        public string[] Labels 
        {
            get
            {
                return _labels;
            }
            set
            {
                _labels = value;
                RaisePropertyChanged("Labels");
            } 
        }

        public Func<double, string> YFormatter { get; set; }
        public Func<int, string> DateFormatter { get; set; }

        private DateTime _initialDateTime;
        public DateTime InitialDateTime
        {
            get { return _initialDateTime; }
            set
            {
                _initialDateTime = value;
                RaisePropertyChanged("InitialDateTime");
            }
        }


        private PeriodUnits _period = PeriodUnits.Minutes;
        public PeriodUnits Period
        {
            get { return _period; }
            set
            {
                _period = value;
                RaisePropertyChanged("Period");
            }
        }

        private IAxisWindow _selectedWindow;
        public IAxisWindow SelectedWindow
        {
            get { return _selectedWindow; }
            set
            {
                _selectedWindow = value;
                RaisePropertyChanged("SelectedWindow");
            }
        }
        #endregion // Presentation Properties

        #region Public Methods
        public ICommand CVTCurveCommand
        {
            get
            {
                if (_cvtCurveCommand == null)
                {
                    _cvtCurveCommand = new RelayCommand(
                        param => { this.CVTCurve(); },
                        param => this.CanCVTCurve
                        );
                }
                return _cvtCurveCommand;
            }
        }
        public ICommand CapacityCurveCommand
        {
            get
            {
                if (_capacityCurveCommand == null)
                {
                    _capacityCurveCommand = new RelayCommand(
                        param => { this.CapacityCurve(); },
                        param => this.CanCapacityCurve
                        );
                }
                return _capacityCurveCommand;
            }
        }
        public ICommand ApplySelectionCommand
        {
            get
            {
                if (_applySelectionCommand == null)
                {
                    _applySelectionCommand = new RelayCommand(
                        param => { this.ApplySelection(); }
                        );
                }
                return _applySelectionCommand;
            }
        }
        #endregion // Public Methods

        #region Private Helpers

        public bool CanCVTCurve { get { return true; } }
        public bool CanCapacityCurve { get { return true; } }

        private void CVTCurve()
        {
            string filePath = string.Empty;
            var localFilePath = FileTransferHelper.Remote2Local(SelectedFile);
            if (File.Exists(localFilePath))
                filePath = localFilePath;
            else
                filePath = SelectedFile;
            var stdRows = Utilities.LoadSTDFile(filePath);
            int interval = stdRows.Count/2000;
            if (interval == 0)
                interval = 1;
            var currentPoints = new ChartValues<double>(stdRows.Where(o => o.Index % interval == 0).Select(o => o.Current));
            var voltagePoints = new ChartValues<double>(stdRows.Where(o => o.Index % interval == 0).Select(o => o.Voltage));
            var temperaturePoints = new ChartValues<double>(stdRows.Where(o => o.Index % interval == 0).Select(o => o.Temperature));
            var datePoints = stdRows.Where(o => o.Index % interval == 0).Select(o => o.TimeInMS);
            InitialDateTime = DateTime.MinValue;
            CTVSeriesCollection.Clear();
            LineSeries currentLine = new LineSeries()
            {
                LineSmoothness = 0,
                Title = "Current",
                Values = currentPoints,
                Fill = Brushes.Transparent,
                PointGeometrySize = 0,
                ScalesYAt = 0
            };
            CTVSeriesCollection.Add(currentLine);
            LineSeries voltageLine = new LineSeries()
            {
                LineSmoothness = 0,
                Title = "Voltage",
                Values = voltagePoints,
                Fill = Brushes.Transparent,
                PointGeometrySize = 0,
                ScalesYAt = 1
            };
            CTVSeriesCollection.Add(voltageLine);
            LineSeries temperatureLine = new LineSeries()
            {
                LineSmoothness = 0,
                Title = "Temperature",
                Values = temperaturePoints,
                Fill = Brushes.Transparent,
                PointGeometrySize = 0,
                ScalesYAt = 2
            };
            CTVSeriesCollection.Add(temperatureLine);
        }
        private void CapacityCurve()
        {
            CapacitySeriesCollection.Clear();
            var selectedTests = Tests.Where(tr => tr.IsSelected).ToList();
            var temperatures = selectedTests.Select(tr => tr.Temperature).Distinct().ToList();
            Labels = temperatures.Select(t=>t.ToString()).ToArray();
            var currents = selectedTests.Select(tr => tr.Current).Distinct().ToList();
            foreach (var cur in currents)
            {
                var caps = selectedTests.Where(tr => tr.Current == cur).OrderBy(tr => tr.Temperature).Select(tr => tr.DischargeCapacity);
                var points = new ChartValues<double>(caps);
                var line = new LineSeries()
                {
                    LineSmoothness = 0,
                    Title = $"{cur/1000}A",
                    Values = points,
                    Fill = Brushes.Transparent,
                    PointGeometrySize = 0
                };
                CapacitySeriesCollection.Add(line);
            }
        }

        private void ApplySelection()
        {
            var selectedCurrents = CurrentList.Where(c => c.IsSelected).ToList();
            var selectedTemperatures = TemperatureList.Where(t => t.IsSelected).ToList();
            var selectedTests = Tests.Where(test => selectedTemperatures.Select(o => o.Temperature).Contains(test.Temperature) && selectedCurrents.Select(o => o.Current).Contains(test.Current));
            foreach (var test in Tests)
            {
                if (selectedTests.Contains(test))
                    test.IsSelected = true;
                else
                    test.IsSelected = false;
            }
        }
        #endregion // Private Helpers
    }
}