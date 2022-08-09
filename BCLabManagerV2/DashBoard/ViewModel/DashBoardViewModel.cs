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
using Prism.Mvvm;
using System.Windows;
using LiveCharts;
using LiveCharts.Defaults;

namespace BCLabManager.ViewModel
{
    public class DashBoardViewModel : BindableBase
    {
        #region Fields

        //ObservableCollection<BatteryClass> _batteryService.Items;
        //ObservableCollection<Channel> _channelService.Items;
        //ObservableCollection<ChamberClass> _chamberService.Items;
        //ObservableCollection<ProgramClass> _programService.Items;
        private BatteryTypeServiceClass _batteryTypeService;
        private ProjectServiceClass _projectService;
        private BatteryServiceClass _batteryService;
        private ChannelServiceClass _channelService;
        private ChamberServiceClass _chamberService;
        private ProgramServiceClass _programService;
        private RelayCommand _updateCommand;
        private RelayCommand _timeCommand;
        #endregion // Fields

        #region Constructor

        public DashBoardViewModel
            (
            BatteryTypeServiceClass batteryTypeService,
            ProjectServiceClass projectService,
            //ObservableCollection<ProgramClass> programs,
            //ObservableCollection<BatteryClass> batteries,
            //ObservableCollection<Channel> channels,
            //ObservableCollection<ChamberClass> chambers        
            BatteryServiceClass batteryService,
         ChannelServiceClass channelService,
         ChamberServiceClass chamberService,
         ProgramServiceClass programService
            )
        {
            _batteryTypeService = batteryTypeService;
            _projectService = projectService;
            _batteryService = batteryService;
            _channelService = channelService;
            _chamberService = chamberService;
            _programService = programService;
            BookEvents();
            var trs = _programService.Items.SelectMany(pro => pro.Recipes.SelectMany(rec => rec.TestRecords));
            var tr = trs.Single(o => o.EndTime == trs.Max(j => j.EndTime));
            BatteryType = tr.Recipe.Program.Project.BatteryType;
            Project = tr.Recipe.Program.Project;
            UpdateButtoneName();

            MonthFormatter = x => new DateTime((long)x).ToString("yyyy MMM");
            MonthStep = TimeSpan.FromDays(30).Ticks;

            DayFormatter = x => new DateTime((long)x).ToString("MMM dd");
            DayStep = TimeSpan.FromDays(30).Ticks;
        }
        public ChartValues<DateTimePoint> TestDataValues { get; set; }
        public ChartValues<DateTimePoint> ORValues { get; set; }
        public ChartValues<DateTimePoint> ProductValues { get; set; }
        public ChartValues<DateTimePoint> EventValues { get; set; }
        public ChartValues<double> ProjectDays { get; set; }
        public string[] ProjectNames { get; set; }
        public Func<double, string> MonthFormatter { get; set; }
        public double MonthStep { get; set; }
        public Func<double, string> DayFormatter { get; set; }
        public double DayStep { get; set; }

        private void UpdateButtoneName()
        {
            if (Project.StartTimes.Count == Project.StopTimes.Count)
                ButtonName = "Start";
            else
                ButtonName = "Stop";
        }

        private void BookEvents()
        {
            foreach (var bat in _batteryService.Items)
            {
                bat.PropertyChanged += Bat_PropertyChanged;
            }
            _batteryService.Items.CollectionChanged += _batteries_CollectionChanged;

            foreach (var cmb in _chamberService.Items)
            {
                cmb.PropertyChanged += Cmb_PropertyChanged;
            }
            _chamberService.Items.CollectionChanged += _chambers_CollectionChanged;

            foreach (var chn in _channelService.Items)
            {
                chn.PropertyChanged += Chn_PropertyChanged;
            }
            _channelService.Items.CollectionChanged += _channels_CollectionChanged;

            _programService.Items.CollectionChanged += _programs_CollectionChanged;

            foreach (var pro in _programService.Items)
            {
                pro.Recipes.CollectionChanged += Recipes_CollectionChanged;
            }

            foreach (var pro in _programService.Items)
            {
                foreach (var sub in pro.Recipes)
                {
                    sub.TestRecords.CollectionChanged += TestRecords_CollectionChanged;
                }
            }

            foreach (var pro in _programService.Items)
            {
                foreach (var sub in pro.Recipes)
                {
                    foreach (var tr in sub.TestRecords)
                        tr.StatusChanged += Tr_StatusChanged;
                }
            }
        }
        private void UpdateProgramsUI()
        {
            RaisePropertyChanged("WaitingAmount");
            RaisePropertyChanged("ExecutingAmount");
            RaisePropertyChanged("CompletedAmount");
            RaisePropertyChanged("InvalidAmount");
            RaisePropertyChanged("AbandonedAmount");
            RaisePropertyChanged("TotalExeAmount");
            RaisePropertyChanged("WaitingPercentage");
            RaisePropertyChanged("ExecutingPercentage");
            RaisePropertyChanged("CompletedPercentage");
            RaisePropertyChanged("InvalidPercentage");
            RaisePropertyChanged("AbandonedPercentage");
            RaisePropertyChanged("CompletedProgramNumber");
            RaisePropertyChanged("CompletedRecipeNumber");
            RaisePropertyChanged("CompletedTestNumber");
            RaisePropertyChanged("CollectedRawDataNumber");
            RaisePropertyChanged("ExecutingTestRecords");
            RaisePropertyChanged("WaitingTestRecords");
        }
        private void UpdateAssetsUI()
        {
            RaisePropertyChanged("BatteryAmount");
            RaisePropertyChanged("UsingBatteryAmount");
            RaisePropertyChanged("ChamberAmount");
            RaisePropertyChanged("UsingChamberAmount");
            RaisePropertyChanged("ChannelAmount");
            RaisePropertyChanged("UsingChannelAmount");
            RaisePropertyChanged("ChannelUsingPercent");
            RaisePropertyChanged("ChamberUsingPercent");
            RaisePropertyChanged("BatteryUsingPercent");
        }
        private void Tr_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            UpdateProgramsUI();
        }

        private void SecondTestRecords_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //RaisePropertyChanged("CompletedAmount");
            //RaisePropertyChanged("InvalidAmount");
            //RaisePropertyChanged("WaitingAmount");
            //RaisePropertyChanged("TotalExeAmount");
            UpdateProgramsUI();
        }

        private void TestRecords_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //RaisePropertyChanged("CompletedAmount");
            //RaisePropertyChanged("InvalidAmount");
            //RaisePropertyChanged("WaitingAmount");
            //RaisePropertyChanged("TotalExeAmount");
            UpdateProgramsUI();
        }

        private void Recipes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //RaisePropertyChanged("WaitingAmount");
            //RaisePropertyChanged("TotalExeAmount");
            UpdateProgramsUI();
        }

        private void _programs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //RaisePropertyChanged("WaitingAmount");
            //RaisePropertyChanged("TotalExeAmount");
            UpdateProgramsUI();
        }

        private void _channels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //RaisePropertyChanged("ChannelAmount");
            UpdateAssetsUI();
        }

        private void Chn_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "Status")
            //    RaisePropertyChanged("UsingChannelAmount");
            UpdateAssetsUI();
        }

        private void _chambers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //RaisePropertyChanged("ChamberAmount");
            UpdateAssetsUI();
        }

        private void Cmb_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "Status")
            //    RaisePropertyChanged("UsingChamberAmount");
            UpdateAssetsUI();
        }

        private void Bat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "Status")
            //    RaisePropertyChanged("UsingBatteryAmount");
            UpdateAssetsUI();
        }

        private void _batteries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //RaisePropertyChanged("BatteryAmount");
            UpdateAssetsUI();
        }

        //void CreateAllPrograms(List<ProgramClass> programClasses)
        //{
        //    List<ProgramViewModel> all =
        //        (from program in programClasses
        //         select new ProgramViewModel(program)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

        //    this.AllPrograms = new ObservableCollection<ProgramViewModel>(all);     //再转换成Observable
        //}

        #endregion // Constructor


        #region Public Interface
        public ICommand UpdateCommand
        {
            get
            {
                if (_updateCommand == null)
                {
                    _updateCommand = new RelayCommand(
                        param => { this.UpdatePlanedDays(); },
                        param => this.CanUpdate
                        );
                }
                return _updateCommand;
            }
        }

        private void UpdatePlanedDays()
        {
            //Project.PlanedDays = PlanedDays;
            if (MessageBoxResult.OK == MessageBox.Show("Are you sure to update Project Planed Days?", "Confirmation", MessageBoxButton.OKCancel))
            {
                _projectService.DatabaseUpdate(Project);
                RaisePropertyChanged("ExpectedProgress");
            }
        }

        public ICommand TimeCommand
        {
            get
            {
                if (_timeCommand == null)
                {
                    _timeCommand = new RelayCommand(
                        param => { this.UpdateTime(); },
                        param => this.CanTime
                        );
                }
                return _timeCommand;
            }
        }

        private void UpdateTime()
        {
            string action = ButtonName.ToLower();

            if (MessageBoxResult.OK == MessageBox.Show($"Are you sure to {action} this project?", "Confirmation", MessageBoxButton.OKCancel))
            {
                if (Project.StartTimes.Count == Project.StopTimes.Count)
                    Project.StartTimes.Add(Time);
                else
                    Project.StopTimes.Add(Time);
                _projectService.DatabaseUpdate(Project);
                UpdateButtoneName();
                RaisePropertyChanged("ExpectedProgress");
            }
        }

        public List<BatteryType> BatteryTypes
        {
            get { return _batteryTypeService.Items.Where(bt=>bt.Projects.Count!=0 && bt.Projects.Any(prj=>prj.Programs.Any(pro=>!pro.IsInvalid))).ToList(); }
        }

        private BatteryType _batteryType;
        public BatteryType BatteryType
        {
            get { return _batteryType; }
            set
            {
                SetProperty(ref _batteryType, value);
                RaisePropertyChanged("Projects");
                if (Projects.Count != 0)
                    Project = Projects[0];
            }
        }
        public List<Project> Projects
        {
            get
            {
                return _projectService.Items.Where(o => o.BatteryType == BatteryType && o.Programs.Any(pro=>!pro.IsInvalid)).ToList();
            }
        }

        private Project _project;
        public Project Project
        {
            get { return _project; }
            set
            {
                SetProperty(ref _project, value);
                if (value == null)
                    return;
                RaisePropertyChanged("WaitingAmount");
                RaisePropertyChanged("ExecutingAmount");
                RaisePropertyChanged("CompletedAmount");
                RaisePropertyChanged("InvalidAmount");
                RaisePropertyChanged("AbandonedAmount");
                RaisePropertyChanged("WaitingPercentage");
                RaisePropertyChanged("ExecutingPercentage");
                RaisePropertyChanged("CompletedPercentage");
                RaisePropertyChanged("InvalidPercentage");
                RaisePropertyChanged("AbandonedPercentage");
                RaisePropertyChanged("WaitingTestRecords");
                RaisePropertyChanged("CompletedTestRecords");
                RaisePropertyChanged("Products");
                RaisePropertyChanged("RealProgress");
                RaisePropertyChanged("ExpectedProgress");
                RaisePropertyChanged("PlanedDays");
                UpdateButtoneName();
            }
        }

        #region Assets Usage
        public int BatteryAmount
        {
            get { return _batteryService.Items.Count; }
        }

        public int UsingBatteryAmount
        {
            get
            {
                return (from bat in _batteryService.Items
                        where bat.AssetUseCount > 0
                        select bat).Count();
            }
        }

        public double BatteryUsingPercent
        {
            get { return (double)UsingBatteryAmount / (double)BatteryAmount; }
        }

        public int ChamberAmount
        {
            get { return _chamberService.Items.Count; }
        }

        public int UsingChamberAmount
        {
            get
            {
                return (from cmb in _chamberService.Items
                        where cmb.AssetUseCount > 0
                        select cmb).Count();
            }
        }

        public double ChamberUsingPercent
        {
            get { return (double)UsingChamberAmount / (double)ChamberAmount; }
        }

        public int ChannelAmount
        {
            get
            {
                return _channelService.Items.Count();
            }
        }

        public int UsingChannelAmount
        {
            get
            {
                return (from channel in _channelService.Items
                        where channel.AssetUseCount > 0
                        select channel).Count();
            }
        }

        public double ChannelUsingPercent
        {
            get { return (double)UsingChannelAmount / (double)ChannelAmount; }
        }
        #endregion
        #region Test Summary
        public Int32 WaitingAmount
        {
            get
            {
                if (Project == null)
                    return 0;
                return Project.Programs.SelectMany(pro => pro.Recipes.SelectMany(rec => rec.TestRecords)).Count(tr => tr.Status == TestStatus.Waiting);
            }
        }
        public Int32 CompletedAmount
        {
            get
            {
                if (Project == null)
                    return 0;
                return Project.Programs.SelectMany(pro => pro.Recipes.SelectMany(rec => rec.TestRecords)).Count(tr => tr.Status == TestStatus.Completed);
            }
        }
        public Int32 InvalidAmount
        {
            get
            {
                if (Project == null)
                    return 0;
                return Project.Programs.SelectMany(pro => pro.Recipes.SelectMany(rec => rec.TestRecords)).Count(tr => tr.Status == TestStatus.Invalid);
            }
        }
        public Int32 AbandonedAmount
        {
            get
            {
                if (Project == null)
                    return 0;
                return Project.Programs.SelectMany(pro => pro.Recipes.SelectMany(rec => rec.TestRecords)).Count(tr => tr.Status == TestStatus.Abandoned);
            }
        }
        public Int32 TotalExeAmount
        {
            get
            {
                if (Project == null)
                    return 1;
                return Project.Programs.SelectMany(pro => pro.Recipes.SelectMany(rec => rec.TestRecords)).Count();
            }
        }
        public double WaitingPercentage
        {
            get
            {
                if (TotalExeAmount == 0)
                    return 0;
                else
                    return (double)WaitingAmount / (double)TotalExeAmount;
            }
        }
        public double CompletedPercentage
        {
            get
            {
                if (TotalExeAmount == 0)
                    return 0;
                else
                    return (double)CompletedAmount / (double)TotalExeAmount;
            }
        }
        public double InvalidPercentage
        {
            get
            {
                if (TotalExeAmount == 0)
                    return 0;
                else
                    return (double)InvalidAmount / (double)TotalExeAmount;
            }
        }
        public double AbandonedPercentage
        {
            get
            {
                if (TotalExeAmount == 0)
                    return 0;
                else
                    return (double)AbandonedAmount / (double)TotalExeAmount;
            }
        }
        #endregion
        #region Ongoing Tests
        public ObservableCollection<TestRecordViewModel> WaitingTestRecords
        {
            get
            {
                if (Project == null)
                    return null;
                var all = Project.Programs.SelectMany(pro => pro.Recipes.SelectMany(rec => rec.TestRecords.Where(tr => tr.Status == TestStatus.Waiting))).Select(tr => new TestRecordViewModel(tr)).ToList();
                return new ObservableCollection<TestRecordViewModel>(all);
            }
        }
        public ObservableCollection<TestRecordViewModel> CompletedTestRecords
        {
            get
            {
                if (Project == null)
                    return null;
                var all = Project.Programs.SelectMany(pro => pro.Recipes.SelectMany(rec => rec.TestRecords.Where(tr => tr.Status == TestStatus.Completed))).Select(tr => new TestRecordViewModel(tr));
                return new ObservableCollection<TestRecordViewModel>(all);
            }
        }
        #endregion
        #region Products
        public ObservableCollection<TableMakerProductViewModel> Products
        {
            get
            {
                if (Project == null)
                    return null;
                var all = Project.TableMakerRecords.Where(tmr => tmr.IsValid).SelectMany(tmr => tmr.Products.Where(prd => prd.IsValid)).Select(o => new TableMakerProductViewModel(o)).ToList();
                return new ObservableCollection<TableMakerProductViewModel>(all);
            }
        }
        #endregion
        #region Statistics
        public int CompletedProgramNumber
        {
            get
            {
                int i = 0;
                foreach (var pro in _programService.Items)
                {
                    if (IsProgramCompleted(pro))
                        i++;
                }
                return i;
            }
        }

        private bool IsProgramCompleted(Program pro)
        {
            foreach (var sub in pro.Recipes)
            {
                if (sub.IsAbandoned == true)
                    continue;
                if (IsRecipeCompleted(sub) == false)
                    return false;
            }
            return true;
        }
        public int CompletedRecipeNumber
        {
            get
            {
                int i = 0;
                foreach (var pro in _programService.Items)
                {
                    foreach (var sub in pro.Recipes)
                        if (IsRecipeCompleted(sub))
                            i++;
                }
                return i;
            }
        }

        private bool IsRecipeCompleted(Recipe sub)
        {
            foreach (var tr in sub.TestRecords)
                if (IsTestCompleted(tr) == false)
                    return false;
            return true;
        }
        public int CompletedTestNumber
        {
            get
            {
                int i = 0;
                foreach (var pro in _programService.Items)
                {
                    foreach (var sub in pro.Recipes)
                    {
                        foreach (var tr in sub.TestRecords)
                            if (IsTestCompleted(tr))
                                i++;
                    }
                }
                return i;
            }
        }

        private bool IsTestCompleted(TestRecord tr)
        {
            return tr.Status == TestStatus.Completed;
        }
        #endregion
        #region Progress
        private string _buttonName;
        public string ButtonName
        {
            get { return _buttonName; }
            set { SetProperty(ref _buttonName, value); }
        }
        //private uint _planedDays;
        public uint PlanedDays
        {
            get 
            {
                if (Project == null)
                    return 0;
                return Project.PlanedDays; 
            }
            set
            {
                Project.PlanedDays = value;
            }
        }
        private DateTime _time;
        public DateTime Time
        {
            get { return _time; }
            set { SetProperty(ref _time, value); }
        }

        public bool CanUpdate { get { return true; } }
        public bool CanTime { get { return true; } }
        public double ExpectedProgress
        {
            get
            {
                if (Project == null)
                    return 0;
                if (PlanedDays == 0)
                    return 0;

                double elapsed = 0;
                for (int i = 0; i < Project.StartTimes.Count; i++)
                {
                    if (Project.StopTimes.Count > i)
                    {
                        elapsed += (Project.StopTimes[i] - Project.StartTimes[i]).TotalDays;
                    }
                    else
                    {
                        elapsed += (DateTime.Now - Project.StartTimes[i]).TotalDays;
                    }
                }
                return elapsed / (double)PlanedDays;
            }
        }
        public double RealProgress
        {
            get
            {
                if (Project == null)
                    return 0;
                var total = Project.Programs.SelectMany(pro => pro.Recipes.Where(rec => !rec.IsAbandoned)).Count();
                if (total == 0)
                    return 0;
                var completed = Project.Programs.SelectMany(pro => pro.Recipes.Where(rec => rec.IsCompleted)).Count();
                return (double)completed / (double)total;
            }
        }
        #endregion
        #endregion // Public Interface
    }
}
