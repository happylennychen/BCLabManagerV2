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

namespace BCLabManager.ViewModel
{
    public class DashBoardViewModel : BindBase
    {
        #region Fields

        ObservableCollection<BatteryClass> _batteries;
        ObservableCollection<ChannelClass> _channels;
        ObservableCollection<ChamberClass> _chambers;
        ObservableCollection<ProgramClass> _programs;
        #endregion // Fields

        #region Constructor

        public DashBoardViewModel
            (
            ObservableCollection<ProgramClass> programs,
            ObservableCollection<BatteryClass> batteries,
            ObservableCollection<ChannelClass> channels,
            ObservableCollection<ChamberClass> chambers
            )
        {
            _programs = programs;
            _batteries = batteries;
            _channels = channels;
            _chambers = chambers;
            BookEvents();
        }

        private void BookEvents()
        {
            foreach (var bat in _batteries)
            {
                bat.PropertyChanged += Bat_PropertyChanged;
            }
            _batteries.CollectionChanged += _batteries_CollectionChanged;

            foreach (var cmb in _chambers)
            {
                cmb.PropertyChanged += Cmb_PropertyChanged;
            }
            _chambers.CollectionChanged += _chambers_CollectionChanged;

            foreach (var chn in _channels)
            {
                chn.PropertyChanged += Chn_PropertyChanged;
            }
            _channels.CollectionChanged += _channels_CollectionChanged;

            _programs.CollectionChanged += _programs_CollectionChanged;

            foreach (var pro in _programs)
            {
                pro.Recipes.CollectionChanged += SubPrograms_CollectionChanged;
            }

            foreach (var pro in _programs)
            {
                foreach(var sub in pro.Recipes)
                {
                    sub.FirstTestRecords.CollectionChanged += FirstTestRecords_CollectionChanged;
                    sub.SecondTestRecords.CollectionChanged += SecondTestRecords_CollectionChanged;
                }
            }

            foreach (var pro in _programs)
            {
                foreach (var sub in pro.Recipes)
                {
                    foreach(var tr in sub.FirstTestRecords)
                        tr.StatusChanged += Tr_StatusChanged;
                    foreach (var tr in sub.SecondTestRecords)
                        tr.StatusChanged += Tr_StatusChanged;
                }
            }
        }
        private void UpdateProgramsUI()
        {
            OnPropertyChanged("WaitingAmount");
            OnPropertyChanged("ExecutingAmount");
            OnPropertyChanged("CompletedAmount");
            OnPropertyChanged("InvalidAmount");
            OnPropertyChanged("AbandonedAmount");
            OnPropertyChanged("TotalExeAmount");
            OnPropertyChanged("WaitingPercentage");
            OnPropertyChanged("ExecutingPercentage");
            OnPropertyChanged("CompletedPercentage");
            OnPropertyChanged("InvalidPercentage");
            OnPropertyChanged("AbandonedPercentage"); 
            OnPropertyChanged("CompletedProgramNumber");
            OnPropertyChanged("CompletedSubProgramNumber");
            OnPropertyChanged("CompletedTestNumber");
            OnPropertyChanged("CollectedRawDataNumber");
            OnPropertyChanged("ExecutingTestRecords");
            OnPropertyChanged("WaitingTestRecords");
        }
        private void UpdateAssetsUI()
        {
            OnPropertyChanged("BatteryAmount");
            OnPropertyChanged("UsingBatteryAmount");
            OnPropertyChanged("ChamberAmount");
            OnPropertyChanged("UsingChamberAmount");
            OnPropertyChanged("ChannelAmount");
            OnPropertyChanged("UsingChannelAmount");
            OnPropertyChanged("ChannelUsingPercent");
            OnPropertyChanged("ChamberUsingPercent");
            OnPropertyChanged("BatteryUsingPercent");
        }
        private void Tr_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            UpdateProgramsUI();
        }

        private void SecondTestRecords_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //OnPropertyChanged("CompletedAmount");
            //OnPropertyChanged("InvalidAmount");
            //OnPropertyChanged("WaitingAmount");
            //OnPropertyChanged("TotalExeAmount");
            UpdateProgramsUI();
        }

        private void FirstTestRecords_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //OnPropertyChanged("CompletedAmount");
            //OnPropertyChanged("InvalidAmount");
            //OnPropertyChanged("WaitingAmount");
            //OnPropertyChanged("TotalExeAmount");
            UpdateProgramsUI();
        }

        private void SubPrograms_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //OnPropertyChanged("WaitingAmount");
            //OnPropertyChanged("TotalExeAmount");
            UpdateProgramsUI();
        }

        private void _programs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //OnPropertyChanged("WaitingAmount");
            //OnPropertyChanged("TotalExeAmount");
            UpdateProgramsUI();
        }

        private void _channels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //OnPropertyChanged("ChannelAmount");
            UpdateAssetsUI();
        }

        private void Chn_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "Status")
            //    OnPropertyChanged("UsingChannelAmount");
            UpdateAssetsUI();
        }

        private void _chambers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //OnPropertyChanged("ChamberAmount");
            UpdateAssetsUI();
        }

        private void Cmb_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "Status")
            //    OnPropertyChanged("UsingChamberAmount");
            UpdateAssetsUI();
        }

        private void Bat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == "Status")
            //    OnPropertyChanged("UsingBatteryAmount");
            UpdateAssetsUI();
        }

        private void _batteries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //OnPropertyChanged("BatteryAmount");
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
            get { return _batteries.Count; }
        }

        public int UsingBatteryAmount
        {
            get
            {
                return (from bat in _batteries
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
            get { return _chambers.Count; }
        }

        public int UsingChamberAmount
        {
            get
            {
                return (from cmb in _chambers
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
                return _channels.Count();
            }
        }

        public int UsingChannelAmount
        {
            get
            {
                return (from channel in _channels
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
                from pro in _programs
                from sub in pro.Recipes
                from ftr in sub.FirstTestRecords
                where ftr.Status == testStatus
                select ftr
            ).Count()
            +
            (
                from pro in _programs
                from sub in pro.Recipes
                from str in sub.SecondTestRecords
                where str.Status == testStatus
                select str
            ).Count()
            ;
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
                    from pro in _programs
                    from sub in pro.Recipes
                    from ftr in sub.FirstTestRecords
                    select ftr
                ).Count()
                +
                (
                    from pro in _programs
                    from sub in pro.Recipes
                    from str in sub.SecondTestRecords
                    select str
                ).Count()
                ;
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
                foreach (var pro in _programs)
                {
                    foreach (var sub in pro.Recipes)
                    {
                        foreach (var tr in sub.FirstTestRecords)
                        {
                            if (tr.Status == TestStatus.Executing)
                                all.Add(new TestRecordViewModel(tr));
                        }
                        foreach (var tr in sub.SecondTestRecords)
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
                foreach (var pro in _programs)
                {
                    foreach (var sub in pro.Recipes)
                    {
                        foreach (var tr in sub.FirstTestRecords)
                        {
                            if (tr.Status == TestStatus.Waiting)
                                all.Add(new TestRecordViewModel(tr));
                        }
                        foreach (var tr in sub.SecondTestRecords)
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
                foreach (var pro in _programs)
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
                if (IsSubProgramCompleted(sub) == false)
                    return false;
            }
            return true;
        }
        public int CompletedSubProgramNumber
        {
            get
            {
                int i = 0;
                foreach (var pro in _programs)
                {
                    foreach(var sub in pro.Recipes)
                        if (IsSubProgramCompleted(sub))
                            i++;
                }
                return i;
            }
        }

        private bool IsSubProgramCompleted(RecipeClass sub)
        {
            foreach (var tr in sub.FirstTestRecords)
                if (IsTestCompleted(tr) == false)
                    return false;
            foreach (var tr in sub.SecondTestRecords)
                if (IsTestCompleted(tr) == false)
                    return false;
            return true;
        }
        public int CompletedTestNumber
        {
            get
            {
                int i = 0;
                foreach (var pro in _programs)
                {
                    foreach (var sub in pro.Recipes)
                    {
                        foreach (var tr in sub.FirstTestRecords)
                            if (IsTestCompleted(tr))
                                i++;
                        foreach (var tr in sub.SecondTestRecords)
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
                foreach (var pro in _programs)
                {
                    foreach (var sub in pro.Recipes)
                    {
                        foreach (var tr in sub.FirstTestRecords)
                            if (IsTestCompleted(tr))
                                i+= tr.RawDataList.Count;
                        foreach (var tr in sub.SecondTestRecords)
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
