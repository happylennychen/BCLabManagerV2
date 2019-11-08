using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class SubProgramViewModel : BindBase//, IDataErrorInfo
    {
        #region Fields
        public readonly RecipeClass _subprogram;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public SubProgramViewModel(RecipeClass subprogram)
        {
            _subprogram = subprogram;
            this.CreateTestRecords();
            _subprogram.TestRecordAdded += _subprogram_TestRecordAdded;
            //_subprogram.PropertyChanged += _subprogram_PropertyChanged;
            var trlist = GetAllTestRecords(subprogram);
            foreach (var tr in trlist)
                tr.StatusChanged += Tr_StatusChanged;
        }

        private void Tr_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            OnPropertyChanged("WaitingPercentage");
            OnPropertyChanged("ExecutingPercentage");
            OnPropertyChanged("CompletedPercentage");
            OnPropertyChanged("InvalidPercentage");
            OnPropertyChanged("AbandonedPercentage");
        }

        //private void _subprogram_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    OnPropertyChanged(e.PropertyName);
        //}

        private void _subprogram_TestRecordAdded(object sender, TestRecordAddedEventArgs e)
        {
            if (e.IsFirst)
            {
                Test1Records.Add(new TestRecordViewModel(e.NewTestRecord));
            }
            else
            {
                Test2Records.Add(new TestRecordViewModel(e.NewTestRecord));
            }
        }

        void CreateTestRecords()
        {
            List<TestRecordViewModel> all1 =
                (from ft in _subprogram.TestRecords
                 select new TestRecordViewModel(ft)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.Test1Records = new ObservableCollection<TestRecordViewModel>(all1);     //再转换成Observable
        }

        #endregion // Constructor

        #region SubProgramClass Properties

        public int Id
        {
            get { return _subprogram.Id; }
            set
            {
                if (value == _subprogram.Id)
                    return;

                _subprogram.Id = value;

                base.OnPropertyChanged("Id");
            }
        }
        public bool IsAbandoned
        {
            get { return _subprogram.IsAbandoned; }
            set
            {
                if (value == _subprogram.IsAbandoned)
                    return;

                _subprogram.IsAbandoned = value;

                base.OnPropertyChanged("IsAbandoned");
            }
        }
        public DateTime StartTime
        {
            get { return _subprogram.StartTime; }
            set
            {
                if (value == _subprogram.StartTime)
                    return;

                _subprogram.StartTime = value;

                OnPropertyChanged("StartTime");
            }
        }
        public DateTime CompleteTime
        {
            get { return _subprogram.CompleteTime; }
            set
            {
                if (value == _subprogram.CompleteTime)
                    return;

                _subprogram.CompleteTime = value;

                OnPropertyChanged("CompleteTime");
            }
        }
        public string Name
        {
            get
            {
                return $"{_subprogram.ChargeTemperature.Name} {_subprogram.ChargeCurrent} charge, {_subprogram.DischargeTemperature} {_subprogram.DischargeCurrent} discharge";
            }
        }
        #region Presentation logic
        public string WaitingPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_subprogram);
                return ((double)alltr.Count(o => o.Status == TestStatus.Waiting) / (double)alltr.Count).ToString() + "*";
            }
        }

        public string ExecutingPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_subprogram);
                return ((double)alltr.Count(o => o.Status == TestStatus.Executing) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string CompletedPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_subprogram);
                return ((double)alltr.Count(o => o.Status == TestStatus.Completed) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string InvalidPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_subprogram);
                return ((double)alltr.Count(o => o.Status == TestStatus.Invalid) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string AbandonedPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_subprogram);
                return ((double)alltr.Count(o => o.Status == TestStatus.Abandoned) / (double)alltr.Count).ToString() + "*";
            }
        }
        #endregion

        private List<TestRecordClass> GetAllTestRecords(RecipeClass sub)
        {
            List<TestRecordClass> output = new List<TestRecordClass>();
            foreach (var tr in sub.TestRecords)
                output.Add(tr);
            return output;
        }

        public int Loop
        {
            get { return _subprogram.Loop; }
            set
            {
                if (value == _subprogram.Loop)
                    return;

                _subprogram.Loop = value;

                base.OnPropertyChanged("Loop");
            }
        }

        public ObservableCollection<TestRecordViewModel> Test1Records { get; private set; }        //这个是当前sub program所拥有的test1

        public ObservableCollection<TestRecordViewModel> Test2Records { get; private set; }        //这个是当前sub program所拥有的test2

        #endregion // Customer Properties

        public void Abandon()
        {
            IsAbandoned = true;
            foreach (var tr in Test1Records)
            {
                tr.Abandon();
            }
            foreach (var tr in Test2Records)
            {
                tr.Abandon();
            }
        }
    }
}