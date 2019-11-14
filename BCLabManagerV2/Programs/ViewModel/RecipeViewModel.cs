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
        public readonly RecipeClass _recipe;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public RecipeViewModel(RecipeClass Recipe)
        {
            _recipe = Recipe;
            this.CreateTestRecords();
            _recipe.TestRecordAdded += _Recipe_TestRecordAdded;
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
                TestRecords.Add(new TestRecordViewModel(e.NewTestRecord));
            }
        }

        void CreateTestRecords()
        {
            List<TestRecordViewModel> all1 =
                (from ft in _recipe.TestRecords
                 select new TestRecordViewModel(ft)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.TestRecords = new ObservableCollection<TestRecordViewModel>(all1);     //再转换成Observable
        }

        #endregion // Constructor

        #region RecipeClass Properties

        public int Id
        {
            get { return _recipe.Id; }
            set
            {
                if (value == _recipe.Id)
                    return;

                _recipe.Id = value;

                RaisePropertyChanged("Id");
            }
        }
        public bool IsAbandoned
        {
            get { return _recipe.IsAbandoned; }
            set
            {
                if (value == _recipe.IsAbandoned)
                    return;

                _recipe.IsAbandoned = value;

                RaisePropertyChanged("IsAbandoned");
            }
        }
        public DateTime StartTime
        {
            get { return _recipe.StartTime; }
            set
            {
                if (value == _recipe.StartTime)
                    return;

                _recipe.StartTime = value;

                RaisePropertyChanged("StartTime");
            }
        }
        public DateTime CompleteTime
        {
            get { return _recipe.CompleteTime; }
            set
            {
                if (value == _recipe.CompleteTime)
                    return;

                _recipe.CompleteTime = value;

                RaisePropertyChanged("CompleteTime");
            }
        }
        public string Name
        {
            get
            {
                return _recipe.Name;
            }
        }
        #region Presentation logic
        public string WaitingPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_recipe);
                return ((double)alltr.Count(o => o.Status == TestStatus.Waiting) / (double)alltr.Count).ToString() + "*";
            }
        }

        public string ExecutingPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_recipe);
                return ((double)alltr.Count(o => o.Status == TestStatus.Executing) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string CompletedPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_recipe);
                return ((double)alltr.Count(o => o.Status == TestStatus.Completed) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string InvalidPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_recipe);
                return ((double)alltr.Count(o => o.Status == TestStatus.Invalid) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string AbandonedPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_recipe);
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
            get { return _recipe.Loop; }
            set
            {
                if (value == _recipe.Loop)
                    return;

                _recipe.Loop = value;

                RaisePropertyChanged("Loop");
            }
        }

        public ObservableCollection<TestRecordViewModel> TestRecords { get; private set; }        //这个是当前Recipe所拥有的test1


        #endregion // Customer Properties
    }
}