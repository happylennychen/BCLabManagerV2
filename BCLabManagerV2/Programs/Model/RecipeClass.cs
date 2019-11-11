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
        //public String Name { get; set; }
        //public ChargeTemperatureClass ChargeTemperature { get; set; }
        //public ChargeCurrentClass ChargeCurrent { get; set; }
        //public DischargeTemperatureClass DischargeTemperature { get; set; }
        //public DischargeCurrentClass DischargeCurrent { get; set; }
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
        public ObservableCollection<TestRecordClass> TestRecords { get; set; } = new ObservableCollection<TestRecordClass>();

        public RecipeClass()
        {
        }

        public RecipeClass(RecipeTemplate template)
        {
            //this.Name = template.Name;
            //this.ChargeTemperature = template.ChargeTemperature;
            //this.ChargeCurrent = template.ChargeCurrent;
            //this.DischargeTemperature = template.DischargeTemperature;
            //this.DischargeCurrent = template.DischargeCurrent;

            var tr = new TestRecordClass();
            tr.StatusChanged += this.TestRecord_StatusChanged;
            //tr.RecipeStr = $"{this.ChargeTemperature.Name} {this.ChargeCurrent} charge, {this.DischargeTemperature} {this.DischargeCurrent} discharge";
            this.TestRecords.Add(tr);
        }

        public RecipeClass(RecipeTemplate template, string ProgramStr, int loop)  //Only used by populator
        {
            //this.Name = template.Name;
            //this.ChargeTemperature = template.ChargeTemperature;
            //this.ChargeCurrent = template.ChargeCurrent;
            //this.DischargeTemperature = template.DischargeTemperature;
            //this.DischargeCurrent = template.DischargeCurrent;
            this.Loop = loop;

            var tr = new TestRecordClass();
            tr.StatusChanged += this.TestRecord_StatusChanged;
            //tr.RecipeStr = $"{this.ChargeTemperature.Name} {this.ChargeCurrent} charge, {this.DischargeTemperature} {this.DischargeCurrent} discharge";
            tr.ProgramStr = ProgramStr;
            this.TestRecords.Add(tr);
        }

        public RecipeClass(
            //ChargeTemperatureClass chargeTemperature,
            //ChargeCurrentClass chargeCurrent,
            //DischargeTemperatureClass dischargeTemperature,
            //DischargeCurrentClass dischargeCurrent,
            int Loop) : this()
        {
            //this.Name = Name;
            //this.ChargeTemperature = chargeTemperature;
            //this.ChargeCurrent = chargeCurrent;
            //this.DischargeTemperature = dischargeTemperature;
            //this.DischargeCurrent = dischargeCurrent;
            this.Loop = Loop;
        }

        public void AssociateEvent(TestRecordClass testRecord)
        {
            testRecord.StatusChanged += this.TestRecord_StatusChanged;
        }

        //public void Update(String Name, TestCountEnum TestCount)
        //{
        //    this.Name = Name;
        //    this.TestCount = TestCount;
        //}
        //public RecipeClass Clone()  //Clone Name and Test Count, and create testrecords list
        //{
            //var newsub = new RecipeClass(this.ChargeTemperature, this.ChargeCurrent, this.DischargeTemperature, this.DischargeCurrent, this.Loop);
            //newsub.TestRecords = new ObservableCollection<TestRecordClass>();
            //var tr = new TestRecordClass();
            //tr.RecipeStr = $"{this.ChargeTemperature.Name} {this.ChargeCurrent} charge, {this.DischargeTemperature} {this.DischargeCurrent} discharge";
            //tr.StatusChanged += newsub.TestRecord_StatusChanged;
            //newsub.TestRecords.Add(tr);
            //return newsub;
        //}

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
