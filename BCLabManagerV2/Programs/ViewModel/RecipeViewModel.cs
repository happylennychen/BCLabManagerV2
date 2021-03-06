﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;
using Prism.Mvvm;
using System.Windows.Media;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class RecipeViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        public readonly Recipe _recipe;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public RecipeViewModel(Recipe Recipe)
        {
            _recipe = Recipe;
            this.CreateTestRecords();
            _recipe.TestRecordAdded += _Recipe_TestRecordAdded;
            _recipe.PropertyChanged += _Recipe_PropertyChanged;
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

        private void _Recipe_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
            if (e.PropertyName == "EndTime")
                RaisePropertyChanged("EndTimeColor");
            else if (e.PropertyName == "StartTime")
                RaisePropertyChanged("StartTimeColor");
            else if (e.PropertyName == "EET")
                RaisePropertyChanged("EndTime");
            else if (e.PropertyName == "EST")
                RaisePropertyChanged("StartTime");
        }

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
        //public DateTime StartTime
        //{
        //    get { return _recipe.StartTime; }
        //    set
        //    {
        //        if (value == _recipe.StartTime)
        //            return;

        //        _recipe.StartTime = value;

        //        RaisePropertyChanged("StartTime");
        //    }
        //}
        //public DateTime EndTime
        //{
        //    get { return _recipe.EndTime; }
        //    set
        //    {
        //        if (value == _recipe.EndTime)
        //            return;

        //        _recipe.EndTime = value;

        //        RaisePropertyChanged("EndTime");
        //    }
        //}

        public DateTime StartTime
        {
            get
            {
                if (_recipe.StartTime == DateTime.MinValue)
                    return _recipe.EST;
                else
                    return _recipe.StartTime;
            }
        }

        public Brush StartTimeColor
        {
            get
            {
                if (_recipe.StartTime == DateTime.MinValue)
                    return Brushes.DarkGray;
                else
                    return Brushes.Black;
            }
        }

        public DateTime EndTime
        {
            get
            {
                if (_recipe.EndTime == DateTime.MinValue)
                    return _recipe.EET;
                else
                    return _recipe.EndTime;
            }
        }

        public Brush EndTimeColor
        {
            get
            {
                if (_recipe.EndTime == DateTime.MinValue)
                    return Brushes.DarkGray;
                else
                    return Brushes.Black;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                if (EndTime == DateTime.MinValue)
                    return TimeSpan.Zero;
                return EndTime - StartTime;
            }
        }
        public string Name
        {
            get
            {
                return _recipe.Name;
            }
        }
        public double Temperature
        {
            get
            {
                return _recipe.Temperature;
            }
            set
            {
                if (value == _recipe.Temperature)
                    return;

                _recipe.Temperature = value;

                RaisePropertyChanged("Temperature");
            }
        }
        #region Presentation logic
        public string WaitingPercentage
        {
            get
            {
                List<TestRecord> alltr = GetAllTestRecords(_recipe);
                return ((double)alltr.Count(o => o.Status == TestStatus.Waiting) / (double)alltr.Count).ToString() + "*";
            }
        }

        public string ExecutingPercentage
        {
            get
            {
                List<TestRecord> alltr = GetAllTestRecords(_recipe);
                return ((double)alltr.Count(o => o.Status == TestStatus.Executing) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string CompletedPercentage
        {
            get
            {
                List<TestRecord> alltr = GetAllTestRecords(_recipe);
                return ((double)alltr.Count(o => o.Status == TestStatus.Completed) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string InvalidPercentage
        {
            get
            {
                List<TestRecord> alltr = GetAllTestRecords(_recipe);
                return ((double)alltr.Count(o => o.Status == TestStatus.Invalid) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string AbandonedPercentage
        {
            get
            {
                List<TestRecord> alltr = GetAllTestRecords(_recipe);
                return ((double)alltr.Count(o => o.Status == TestStatus.Abandoned) / (double)alltr.Count).ToString() + "*";
            }
        }
        #endregion

        private List<TestRecord> GetAllTestRecords(Recipe sub)
        {
            List<TestRecord> output = new List<TestRecord>();
            foreach (var tr in sub.TestRecords)
                output.Add(tr);
            return output;
        }

        private List<StepRuntime> GetAllStepRuntimes(Recipe sub)
        {
            List<StepRuntime> output = new List<StepRuntime>();
            foreach (var tr in sub.StepRuntimes)
                output.Add(tr);
            return output;
        }

        public ObservableCollection<TestRecordViewModel> TestRecords { get; private set; }        //这个是当前Recipe所拥有的test


        #endregion // Customer Properties
    }
}