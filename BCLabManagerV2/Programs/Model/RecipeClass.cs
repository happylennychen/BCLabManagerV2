using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BCLabManager.DataAccess;
using Prism.Mvvm;

namespace BCLabManager.Model
{
    public class RecipeClass : BindableBase
    {

        public int Id { get; set; }
        public bool IsAbandoned { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private DateTime _startTime;
        public DateTime StartTime
        {
            get => _startTime;
            set { SetProperty(ref _startTime, value); }
        }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get => _endTime;
            set { SetProperty(ref _endTime, value); }
        }
        private DateTime _est;
        public DateTime EST
        {
            get { return _est; }
            set { SetProperty(ref _est, value); }
        }
        private DateTime _eet;
        public DateTime EET
        {
            get { return _eet; }
            set { SetProperty(ref _eet, value); }
        }
        private TimeSpan _ed;
        public TimeSpan ED
        {
            get { return _ed; }
            set { SetProperty(ref _ed, value); }
        }
        //正常来说一个Recipe应该只包含一个TestRecord。但是考虑到有时候测试会无效，所以这里需要用一个List来处理。
        public ObservableCollection<TestRecordClass> TestRecords { get; set; } = new ObservableCollection<TestRecordClass>();

        public ObservableCollection<StepRuntimeClass> StepRuntimes { get; set; } = new ObservableCollection<StepRuntimeClass>();
        public RecipeClass()
        {
        }

        public RecipeClass(RecipeTemplate template, BatteryTypeClass batteryType)
        {
            Name = template.Name;
            //foreach (var step in template.Steps)
            //{
            //    StepRuntimeClass sr = new StepRuntimeClass();
            //    //sr.Step = step;
            //    sr.StepTemplate = step.StepTemplate;
            //    sr.DesignCapacityInmAH = batteryType.TypicalCapacity;
            //    this.StepRuntimes.Add(sr);
            //}
            BuildStepRuntimesBasedOnSteps(template.Steps.ToList(), batteryType);
            var tr = new TestRecordClass();
            //tr.ProgramStr = ProgramStr;
            tr.RecipeStr = this.Name;
            tr.StatusChanged += this.TestRecord_StatusChanged;
            this.TestRecords.Add(tr);
        }

        private void BuildStepRuntimesBasedOnSteps(List<StepClass> steps, BatteryTypeClass batteryType)
        {
            int targetIndex;
            List<int> numbers = new List<int>();
            Dictionary<int, int> LoopCounts = new Dictionary<int, int>();

            for (int i = 0; i < steps.Count; i++)
            {
                if (steps[i].LoopLabel != null)
                {
                    LoopCounts.Add(i, 1);
                }
            }
            for (int i = 0; i < steps.Count; i = targetIndex)
            {
                numbers.Add(i);
                if (steps[i].LoopTarget != null)
                {
                    int ti = FindTargetIndex(steps, steps[i].LoopTarget);
                    if (ti == -1)
                    {
                        MessageBox.Show("Wrong loop settings!");
                        return;
                    }
                    if (LoopCounts[ti] < steps[i].LoopCount)
                    {
                        LoopCounts[ti]++;
                        targetIndex = ti;
                    }
                    else
                    {
                        LoopCounts[ti] = 1;
                        targetIndex = i + 1;
                    }
                }
                else
                {
                    targetIndex = i + 1;
                }
            }

            foreach (var index in numbers)
            {
                StepRuntimeClass sr = new StepRuntimeClass();
                sr.StepTemplate = steps[index].StepTemplate;
                sr.DesignCapacityInmAH = batteryType.TypicalCapacity;
                this.StepRuntimes.Add(sr);
            }
        }

        private int FindTargetIndex(List<StepClass> steps, string loopTarget)
        {
            foreach (var step in steps)
            {
                if (step.LoopLabel == loopTarget)
                    return steps.IndexOf(step);
            }
            return -1;
        }

        public RecipeClass(RecipeTemplate template, string ProgramStr, BatteryTypeClass batteryType)  //Only used by populator
        {
            this.Name = template.Name;
            foreach (var step in template.Steps)
            {
                StepRuntimeClass sr = new StepRuntimeClass();
                sr.StepTemplate = step.StepTemplate;
                sr.DesignCapacityInmAH = batteryType.TypicalCapacity;
                this.StepRuntimes.Add(sr);
            }

            var tr = new TestRecordClass();
            tr.ProgramStr = ProgramStr;
            tr.RecipeStr = this.Name;
            tr.StatusChanged += this.TestRecord_StatusChanged;
            this.TestRecords.Add(tr);
        }

        //public RecipeClass(
        //    int Loop) : this()
        //{
        //    //this.Name = Name;
        //    this.Loop = Loop;
        //}

        public void AssociateEvent(TestRecordClass testRecord)
        {
            testRecord.StatusChanged += this.TestRecord_StatusChanged;
        }

        private void TestRecord_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            TestRecordClass tr = sender as TestRecordClass; //被改变的Test Record
            if (e.Status == TestStatus.Invalid)
            {
                var dbContext = new AppDbContext();
                var sub = dbContext.Recipes.SingleOrDefault(i => i.Id == this.Id);
                dbContext.Entry(sub)
                    .Collection(i => i.TestRecords)
                    .Load();
                dbContext.Entry(sub)
                    .Collection(i => i.TestRecords)
                    .Load();
                var newTestRecord = new TestRecordClass();
                newTestRecord.StatusChanged += this.TestRecord_StatusChanged;
                TestRecords.Add(newTestRecord);
                OnRasieTestRecordAddedEvent(newTestRecord, true);
                sub.TestRecords.Add(newTestRecord);
                dbContext.SaveChanges();
            }

        }
        #region event   //Used by viewmodel
        public event EventHandler<TestRecordAddedEventArgs> TestRecordAdded;

        protected virtual void OnRasieTestRecordAddedEvent(TestRecordClass tr, bool isFirst)
        {
            EventHandler<TestRecordAddedEventArgs> handler = TestRecordAdded;
            TestRecordAddedEventArgs arg = new TestRecordAddedEventArgs(tr, isFirst);

            if (handler != null)
            {
                handler(this, arg);
            }
        }
        #endregion //event

        public void Abandon(String Description = "")
        {
            IsAbandoned = true;
            foreach (var tr in TestRecords)
            {
                tr.Abandon();
            }
        }
        public RecipeClass Clone()  //Clone Name and Test Count, and create testrecords list
        {
            var newsub = new RecipeClass();
            newsub.Name = this.Name;
            newsub.TestRecords = new ObservableCollection<TestRecordClass>();
            var tr = new TestRecordClass();
            tr.StatusChanged += newsub.TestRecord_StatusChanged;
            newsub.TestRecords.Add(tr);

            foreach (var stepRuntime in this.StepRuntimes)
            {
                StepRuntimeClass sr = new StepRuntimeClass();
                sr.StepTemplate = stepRuntime.StepTemplate;
                sr.DesignCapacityInmAH = stepRuntime.DesignCapacityInmAH;
                newsub.StepRuntimes.Add(sr);
            }
            return newsub;
        }
    }
}
