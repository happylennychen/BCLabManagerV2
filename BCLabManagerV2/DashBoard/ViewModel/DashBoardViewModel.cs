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

namespace BCLabManager.ViewModel
{
    public class DashBoardViewModel : BindableBase
    {
        #region Fields

        //ObservableCollection<BatteryClass> _batteryService.Items;
        //ObservableCollection<ChannelClass> _channelService.Items;
        //ObservableCollection<ChamberClass> _chamberService.Items;
        //ObservableCollection<ProgramClass> _programService.Items;
        private BatteryServieClass _batteryService;
        private ChannelServieClass _channelService;
        private ChamberServieClass _chamberService;
        private ProgramServiceClass _programService;
        #endregion // Fields

        #region Constructor

        public DashBoardViewModel
            (
            //ObservableCollection<ProgramClass> programs,
            //ObservableCollection<BatteryClass> batteries,
            //ObservableCollection<ChannelClass> channels,
            //ObservableCollection<ChamberClass> chambers        
            BatteryServieClass batteryService,
         ChannelServieClass channelService,
         ChamberServieClass chamberService,
         ProgramServiceClass programService
            )
        {
            _batteryService = batteryService;
            _channelService = channelService;
            _chamberService = chamberService;
            _programService = programService;
            BookEvents();
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
        private int GetTestRecordAmountByStatus(TestStatus testStatus)
        {
            return
            (
                from pro in _programService.Items
                from sub in pro.Recipes
                from ftr in sub.TestRecords
                where ftr.Status == testStatus
                select ftr
            ).Count();
        }
        public Int32 WaitingAmount
        {
            get
            {
                return GetTestRecordAmountByStatus(TestStatus.Waiting);
            }
        }
        public Int32 ExecutingAmount
        {
            get
            {
                return GetTestRecordAmountByStatus(TestStatus.Executing);
            }
        }
        public Int32 CompletedAmount
        {
            get
            {
                return GetTestRecordAmountByStatus(TestStatus.Completed);
            }
        }
        public Int32 InvalidAmount
        {
            get
            {
                return GetTestRecordAmountByStatus(TestStatus.Invalid);
            }
        }
        public Int32 AbandonedAmount
        {
            get
            {
                return GetTestRecordAmountByStatus(TestStatus.Abandoned);
            }
        }
        public Int32 TotalExeAmount
        {
            get
            {
                return
                (
                    from pro in _programService.Items
                    from sub in pro.Recipes
                    from ftr in sub.TestRecords
                    select ftr
                ).Count();
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
        public double ExecutingPercentage
        {
            get
            {
                if (TotalExeAmount == 0)
                    return 0;
                else
                    return (double)ExecutingAmount / (double)TotalExeAmount;
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
        public ObservableCollection<TestRecordViewModel> ExecutingTestRecords
        {
            get
            {
                List<TestRecordViewModel> all = new List<TestRecordViewModel>();
                foreach (var pro in _programService.Items)
                {
                    foreach (var sub in pro.Recipes)
                    {
                        foreach (var tr in sub.TestRecords)
                        {
                            if (tr.Status == TestStatus.Executing)
                                all.Add(new TestRecordViewModel(tr));
                        }
                    }
                }
                return new ObservableCollection<TestRecordViewModel>(all);
            }
        }
        public ObservableCollection<TestRecordViewModel> WaitingTestRecords
        {
            get
            {
                List<TestRecordViewModel> all = new List<TestRecordViewModel>();
                foreach (var pro in _programService.Items)
                {
                    foreach (var sub in pro.Recipes)
                    {
                        foreach (var tr in sub.TestRecords)
                        {
                            if (tr.Status == TestStatus.Waiting)
                                all.Add(new TestRecordViewModel(tr));
                        }
                    }
                }
                return new ObservableCollection<TestRecordViewModel>(all);
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

        private bool IsProgramCompleted(ProgramClass pro)
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

        private bool IsRecipeCompleted(RecipeClass sub)
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

        private bool IsTestCompleted(TestRecordClass tr)
        {
            return tr.Status == TestStatus.Completed;
        }
        public int CollectedRawDataNumber
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
                                i += tr.RawDataList.Count;
                    }
                }
                return i;
            }
        }
        #endregion
        #endregion // Public Interface
    }
}
