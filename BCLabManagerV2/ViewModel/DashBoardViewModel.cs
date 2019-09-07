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
    public class DashBoardViewModel : ViewModelBase
    {
        #region Fields

        ObservableCollection<BatteryClass> _batteries;
        ObservableCollection<ChannelClass> _channels;
        ObservableCollection<ChamberClass> _chambers;
        List<ProgramClass> _programs;
        #endregion // Fields

        #region Constructor

        public DashBoardViewModel
            (
            List<ProgramClass> programs,
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
                cmb.PropertyChanged += Cmb_PropertyChanged; ;
            }
            _chambers.CollectionChanged += _chambers_CollectionChanged; ;

            foreach (var chn in _channels)
            {
                chn.PropertyChanged += Chn_PropertyChanged; ;
            }
            _channels.CollectionChanged += _channels_CollectionChanged; ;
        }

        private void _channels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("ChannelAmount");
        }

        private void Chn_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")
                OnPropertyChanged("UsingChannelAmount");
        }

        private void _chambers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("ChamberAmount");
        }

        private void Cmb_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")
                OnPropertyChanged("UsingChamberAmount");
        }

        private void Bat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")
                OnPropertyChanged("UsingBatteryAmount");
        }

        private void _batteries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("BatteryAmount");
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
        #region Assets
        public double BatteryAmount
        {
            get { return _batteries.Count; }
        }

        public double UsingBatteryAmount
        {
            get
            {
                return (from bat in _batteries
                        where bat.Status == AssetStatusEnum.USING
                        select bat).Count();
            }
        }

        public double BatteryUsingPercent
        {
            get { return (double)UsingBatteryAmount / (double)BatteryAmount; }
        }

        public double ChamberAmount
        {
            get { return _chambers.Count; }
        }

        public double UsingChamberAmount
        {
            get
            {
                return (from cmb in _chambers
                        where cmb.Status == AssetStatusEnum.USING
                        select cmb).Count();
            }
        }

        public double ChamberUsingPercent
        {
            get { return (double)UsingChamberAmount / (double)ChamberAmount; }
        }

        public double ChannelAmount
        {
            get
            {
                return _channels.Count();
            }
        }

        public double UsingChannelAmount
        {
            get
            {
                return (from channel in _channels
                        where channel.Status == AssetStatusEnum.USING
                        select channel).Count();
            }
        }

        public double ChannelUsingPercent
        {
            get { return (double)UsingChannelAmount / (double)ChannelAmount; }
        }
        #endregion
        #region Legend
        private int GetTestRecordAmountByStatus(TestStatus testStatus)
        {
            return
            (
                from pro in _programs
                from sub in pro.SubPrograms
                from ftr in sub.FirstTestRecords
                where ftr.Status == testStatus
                select ftr
            ).Count()
            +
            (
                from pro in _programs
                from sub in pro.SubPrograms
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
                    from sub in pro.SubPrograms
                    from ftr in sub.FirstTestRecords
                    select ftr
                ).Count()
                +
                (
                    from pro in _programs
                    from sub in pro.SubPrograms
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

        #endregion // Public Interface

        #region  Base Class Overrides

        protected override void OnDispose()
        {
            //foreach (ProgramViewModel viewmodel in this.AllPrograms)
            //    viewmodel.Dispose();

            //this.AllPrograms.Clear();
        }

        #endregion // Base Class Overrides
    }
}
