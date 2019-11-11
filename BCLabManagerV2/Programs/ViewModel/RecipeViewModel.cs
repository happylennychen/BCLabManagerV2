using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class RecipeViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        public readonly RecipeClass _Recipe;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public RecipeViewModel(RecipeClass Recipe)
        {
            _Recipe = Recipe;
            this.CreateTestRecords();
            _Recipe.TestRecordAdded += _Recipe_TestRecordAdded;
            //_Recipe.PropertyChanged += _Recipe_PropertyChanged;
            var trlist = GetAllTestRecords(Recipe);
            foreach (var tr in trlist)
                tr.StatusChanged += Tr_StatusChanged;
        }

        private void Tr_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            RaisePropertyChanged("WaitingPercentage");
            RaisePropertyChanged("ExecutingPercentage");
            RaisePropertyChanged("CompletedPercentage");
            RaisePropertyChanged("InvalidPercentage");
            RaisePropertyChanged("AbandonedPercentage");
        }

        //private void _Recipe_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    RaisePropertyChanged(e.PropertyName);
        //}

        private void _Recipe_TestRecordAdded(object sender, TestRecordAddedEventArgs e)
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
                (from ft in _Recipe.TestRecords
                 select new TestRecordViewModel(ft)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.Test1Records = new ObservableCollection<TestRecordViewModel>(all1);     //再转换成Observable
        }

        #endregion // Constructor

        #region RecipeClass Properties

        public int Id
        {
            get { return _Recipe.Id; }
            set
            {
                if (value == _Recipe.Id)
                    return;

                _Recipe.Id = value;

                RaisePropertyChanged("Id");
            }
        }
        public bool IsAbandoned
        {
            get { return _Recipe.IsAbandoned; }
            set
            {
                if (value == _Recipe.IsAbandoned)
                    return;

                _Recipe.IsAbandoned = value;

                RaisePropertyChanged("IsAbandoned");
            }
        }
        public DateTime StartTime
        {
            get { return _Recipe.StartTime; }
            set
            {
                if (value == _Recipe.StartTime)
                    return;

                _Recipe.StartTime = value;

                RaisePropertyChanged("StartTime");
            }
        }
        public DateTime CompleteTime
        {
            get { return _Recipe.CompleteTime; }
            set
            {
                if (value == _Recipe.CompleteTime)
                    return;

                _Recipe.CompleteTime = value;

                RaisePropertyChanged("CompleteTime");
            }
        }
        public string Name
        {
            get
            {
                //return $"{_Recipe.ChargeTemperature.Name} {_Recipe.ChargeCurrent} charge, {_Recipe.DischargeTemperature} {_Recipe.DischargeCurrent} discharge";
                return "";
            }
        }
        #region Presentation logic
        public string WaitingPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_Recipe);
                return ((double)alltr.Count(o => o.Status == TestStatus.Waiting) / (double)alltr.Count).ToString() + "*";
            }
        }

        public string ExecutingPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_Recipe);
                return ((double)alltr.Count(o => o.Status == TestStatus.Executing) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string CompletedPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_Recipe);
                return ((double)alltr.Count(o => o.Status == TestStatus.Completed) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string InvalidPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_Recipe);
                return ((double)alltr.Count(o => o.Status == TestStatus.Invalid) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string AbandonedPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_Recipe);
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
            get { return _Recipe.Loop; }
            set
            {
                if (value == _Recipe.Loop)
                    return;

                _Recipe.Loop = value;

                RaisePropertyChanged("Loop");
            }
        }

        public ObservableCollection<TestRecordViewModel> Test1Records { get; private set; }        //这个是当前Recipe所拥有的test1

        public ObservableCollection<TestRecordViewModel> Test2Records { get; private set; }        //这个是当前Recipe所拥有的test2

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