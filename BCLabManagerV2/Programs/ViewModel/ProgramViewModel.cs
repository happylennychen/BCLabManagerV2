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
using System.Windows;
using Prism.Mvvm;
using System.Windows.Media;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Program object.
    /// </summary>
    public class ProgramViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        public ProgramClass _program;            //为了AllProgramsViewModel中的Edit，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public ProgramViewModel(ProgramClass program)
        {

            _program = program;
            this.CreateRecipes();
            _program.PropertyChanged += _program_PropertyChanged;
            var trlist = GetAllTestRecords(program);
            foreach(var tr in trlist)
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

        private void _program_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

        void CreateRecipes()
        {
            List<RecipeViewModel> all =
                (from sub in _program.Recipes
                 select new RecipeViewModel(sub)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //foreach (RecipeModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnRecipeModelViewModelPropertyChanged;

            this.Recipes = new ObservableCollection<RecipeViewModel>(all);     //再转换成Observable
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }
        #endregion // Constructor

        #region ProgramClass Properties

        public int Id
        {
            get { return _program.Id; }
            set
            {
                if (value == _program.Id)
                    return;

                _program.Id = value;

                RaisePropertyChanged("Id");
            }
        }

        public string Name
        {
            get { return _program.Name; }
            set
            {
                if (value == _program.Name)
                    return;

                _program.Name = value;

                RaisePropertyChanged("Name");
            }
        }

        public ProjectClass Project
        {
            get { return _program.Project; }
            set
            {
                if (value == _program.Project)
                    return;

                _program.Project = value;

                RaisePropertyChanged("Project");
            }
        }

        public string Requester
        {
            get { return _program.Requester; }
            set
            {
                if (value == _program.Requester)
                    return;

                _program.Requester = value;

                RaisePropertyChanged("Requester");
            }
        }

        public string Description
        {
            get { return _program.Description; }
            set
            {
                if (value == _program.Description)
                    return;

                _program.Description = value;

                RaisePropertyChanged("Description");
            }
        }

        public DateTime RequestTime
        {
            get { return _program.RequestTime; }
            set
            {
                if (value == _program.RequestTime)
                    return;

                _program.RequestTime = value;

                RaisePropertyChanged("RequestTime");
            }
        }
        public DateTime StartTime
        {
            get 
            {
                if (_program.StartTime != DateTime.MinValue)
                    return _program.StartTime;
                else
                    return _program.EST;
            }
            set
            {
                if (value == _program.StartTime)
                    return;

                _program.StartTime = value;


                RaisePropertyChanged();
                //RaisePropertyChanged("StartTimeColor");
            }
        }

        public Brush StartTimeColor
        {
            get
            {
                if (_program.StartTime == DateTime.MinValue)
                    return Brushes.DarkGray;
                else
                    return Brushes.Black;
            }
        }
        public DateTime EndTime
        {
            get
            {
                if (_program.EndTime != DateTime.MinValue)
                    return _program.EndTime;
                else
                    return _program.EET;
            }
            set
            {
                if (value == _program.EndTime)
                    return;

                _program.EndTime = value;

                RaisePropertyChanged();
            }
        }

        public Brush EndTimeColor
        {
            get
            {
                if (_program.EndTime == DateTime.MinValue)
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
        //public TimeSpan EstimatedTime
        //{
        //    get
        //    {
        //        List<ProgramClass> pros = GetAllCompletedProgramByGroup();
        //        if (pros.Count == 0 /*|| EndTime != DateTime.MinValue*/)
        //            return TimeSpan.Zero;
        //        TimeSpan total = GetTotalTime(pros);
        //        var est = TimeSpan.FromSeconds((total.TotalSeconds) / pros.Count);
        //        return est;
        //    }
        //}

        //private TimeSpan GetTotalTime(List<ProgramClass> pros)
        //{
        //    TimeSpan total = TimeSpan.Zero;
        //    foreach (var pro in pros)
        //    {
        //        total += (pro.EndTime - pro.StartTime);
        //    }
        //    return total;
        //}

        //private List<ProgramClass> GetAllCompletedProgramByGroup()
        //{
        //    using (var dbContext = new AppDbContext())
        //    {
        //        return dbContext.Programs
        //            .Where(o => o.Group.Id == this._program.Group.Id && o.EndTime != DateTime.MinValue)
        //            .ToList();
        //    }
        //}
        private TimeSpan _estimatedTime;

        public TimeSpan EstimatedTime
        {
            get { return _estimatedTime; }
            set { _estimatedTime = value;
                RaisePropertyChanged("EstimatedTime");
            }
        }

        private TimeSpan GetTotalTime(List<ProgramViewModel> pros)
        {
            TimeSpan total = TimeSpan.Zero;
            foreach (var pro in pros)
            {
                total += (pro.EndTime - pro.StartTime);
            }
            return total;
        }


        public ObservableCollection<RecipeViewModel> Recipes { get; set; }        //这个是当前program所拥有的Recipes
        #endregion // Customer Properties
        #region Presentation logic
        public string WaitingPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_program);
                return ((double)alltr.Count(o => o.Status == TestStatus.Waiting) / (double)alltr.Count).ToString() + "*";
            }
        }

        public string ExecutingPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_program);
                return ((double)alltr.Count(o => o.Status == TestStatus.Executing) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string CompletedPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_program);
                return ((double)alltr.Count(o => o.Status == TestStatus.Completed) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string InvalidPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_program);
                return ((double)alltr.Count(o => o.Status == TestStatus.Invalid) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string AbandonedPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_program);
                return ((double)alltr.Count(o => o.Status == TestStatus.Abandoned) / (double)alltr.Count).ToString() + "*";
            }
        }
        #endregion

        private List<TestRecordClass> GetAllTestRecords(ProgramClass program)
        {
            List<TestRecordClass> output = new List<TestRecordClass>();
            foreach (var sub in program.Recipes)
            {
                foreach (var tr in sub.TestRecords)
                    output.Add(tr);
            }
            return output;
        }
    }
}