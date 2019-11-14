using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.DataAccess;
using Prism.Mvvm;

namespace BCLabManager.Model
{
    public class RecipeClass : BindableBase
    {
        private DateTime _startTime;
        private DateTime _completeTime;

        public int Id { get; set; }
        public bool IsAbandoned { get; set; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        public int Loop { get; set; } = 1;
        public DateTime StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value;
                RaisePropertyChanged("StartTime");
            }
        }
        public DateTime CompleteTime
        {
            get => _completeTime;
            set
            {
                _completeTime = value;
                RaisePropertyChanged("CompleteTime");
            }
        }
        //正常来说一个Recipe应该只包含一个TestRecord。但是考虑到有时候测试会无效，所以这里需要用一个List来处理。
        public ObservableCollection<TestRecordClass> TestRecords { get; set; } = new ObservableCollection<TestRecordClass>();   

        public RecipeClass()
        {
        }

        public RecipeClass(RecipeTemplate template)
        {

            var tr = new TestRecordClass();
            tr.StatusChanged += this.TestRecord_StatusChanged;
            //tr.RecipeStr = $"{this.ChargeTemperature.Name} {this.ChargeCurrent} charge, {this.DischargeTemperature} {this.DischargeCurrent} discharge";
            this.TestRecords.Add(tr);
        }

        public RecipeClass(RecipeTemplate template, string ProgramStr, int loop)  //Only used by populator
        {
            this.Name = template.Name;
            this.Loop = loop;

            var tr = new TestRecordClass();
            tr.StatusChanged += this.TestRecord_StatusChanged;
            //tr.RecipeStr = template.Name;
            //tr.ProgramStr = ProgramStr;
            this.TestRecords.Add(tr);
        }

        public RecipeClass(
            int Loop) : this()
        {
            //this.Name = Name;
            this.Loop = Loop;
        }

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
    }
}
