using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.DataAccess;

namespace BCLabManager.Model
{
    public enum TestCountEnum
    {
        One,
        Two,
    }
    public class RecipeClass : BindBase
    {
        private DateTime startTime;
        private DateTime completeTime;

        public int Id { get; set; }
        public bool IsAbandoned { get; set; }
        //public String Name { get; set; }
        public ChargeTemperatureClass ChargeTemperature { get; set; }
        public ChargeCurrentClass ChargeCurrent { get; set; }
        public DischargeTemperatureClass DischargeTemperature { get; set; }
        public DischargeCurrentClass DischargeCurrent { get; set; }
        public TestCountEnum TestCount { get; set; }
        public int Loop { get; set; } = 1;
        public DateTime StartTime
        {
            get => startTime;
            set
            {
                startTime = value;
                OnPropertyChanged("StartTime");
            }
        }
        public DateTime CompleteTime
        {
            get => completeTime;
            set
            {
                completeTime = value;
                OnPropertyChanged("CompleteTime");
            }
        }
        public ObservableCollection<TestRecordClass> FirstTestRecords { get; set; }
        public ObservableCollection<TestRecordClass> SecondTestRecords { get; set; }

        public RecipeClass()
        {
            FirstTestRecords = new ObservableCollection<TestRecordClass>();
            SecondTestRecords = new ObservableCollection<TestRecordClass>();
        }

        public RecipeClass(SubProgramTemplate template)
        {
            //this.Name = template.Name;
            this.ChargeTemperature = template.ChargeTemperature;
            this.ChargeCurrent = template.ChargeCurrent;
            this.DischargeTemperature = template.DischargeTemperature;
            this.DischargeCurrent = template.DischargeCurrent;
            this.TestCount = template.TestCount;
            this.FirstTestRecords = new ObservableCollection<TestRecordClass>();
            this.SecondTestRecords = new ObservableCollection<TestRecordClass>();

            var tr = new TestRecordClass();
            tr.StatusChanged += this.TestRecord_StatusChanged;
            tr.SubProgramStr = $"{this.ChargeTemperature.Name} {this.ChargeCurrent} charge, {this.DischargeTemperature} {this.DischargeCurrent} discharge";
            this.FirstTestRecords.Add(tr);
            if (this.TestCount == TestCountEnum.Two)
            {
                var tr1 = new TestRecordClass();
                tr1.StatusChanged += this.TestRecord_StatusChanged;
                tr1.SubProgramStr = $"{this.ChargeTemperature.Name} {this.ChargeCurrent} charge, {this.DischargeTemperature} {this.DischargeCurrent} discharge";
                this.SecondTestRecords.Add(tr1);
            }
        }

        public RecipeClass(SubProgramTemplate template, string ProgramStr, int loop)  //Only used by populator
        {
            //this.Name = template.Name;
            this.ChargeTemperature = template.ChargeTemperature;
            this.ChargeCurrent = template.ChargeCurrent;
            this.DischargeTemperature = template.DischargeTemperature;
            this.DischargeCurrent = template.DischargeCurrent;
            this.TestCount = template.TestCount;
            this.Loop = loop;
            this.FirstTestRecords = new ObservableCollection<TestRecordClass>();
            this.SecondTestRecords = new ObservableCollection<TestRecordClass>();

            var tr = new TestRecordClass();
            tr.StatusChanged += this.TestRecord_StatusChanged;
            tr.SubProgramStr = $"{this.ChargeTemperature.Name} {this.ChargeCurrent} charge, {this.DischargeTemperature} {this.DischargeCurrent} discharge";
            tr.ProgramStr = ProgramStr;
            this.FirstTestRecords.Add(tr);
            if (this.TestCount == TestCountEnum.Two)
            {
                var tr1 = new TestRecordClass();
                tr1.StatusChanged += this.TestRecord_StatusChanged;
                tr1.SubProgramStr = $"{this.ChargeTemperature.Name} {this.ChargeCurrent} charge, {this.DischargeTemperature} {this.DischargeCurrent} discharge";
                tr1.ProgramStr = ProgramStr;
                this.SecondTestRecords.Add(tr1);
            }
        }

        public RecipeClass(
            ChargeTemperatureClass chargeTemperature,
            ChargeCurrentClass chargeCurrent,
            DischargeTemperatureClass dischargeTemperature,
            DischargeCurrentClass dischargeCurrent,
            TestCountEnum TestCount,
            int Loop) : this()
        {
            //this.Name = Name;
            this.ChargeTemperature = chargeTemperature;
            this.ChargeCurrent = chargeCurrent;
            this.DischargeTemperature = dischargeTemperature;
            this.DischargeCurrent = dischargeCurrent;
            this.TestCount = TestCount;
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
        public RecipeClass Clone()  //Clone Name and Test Count, and create testrecords list
        {
            var newsub = new RecipeClass(this.ChargeTemperature, this.ChargeCurrent, this.DischargeTemperature, this.DischargeCurrent, this.TestCount, this.Loop);
            if (this.TestCount == TestCountEnum.One)
            {
                newsub.FirstTestRecords = new ObservableCollection<TestRecordClass>();
                var tr = new TestRecordClass();
                tr.SubProgramStr = $"{this.ChargeTemperature.Name} {this.ChargeCurrent} charge, {this.DischargeTemperature} {this.DischargeCurrent} discharge";
                tr.StatusChanged += newsub.TestRecord_StatusChanged;
                newsub.FirstTestRecords.Add(tr);
            }
            else if (this.TestCount == TestCountEnum.Two)
            {
                newsub.FirstTestRecords = new ObservableCollection<TestRecordClass>();
                var tr = new TestRecordClass();
                tr.SubProgramStr = $"{this.ChargeTemperature.Name} {this.ChargeCurrent} charge, {this.DischargeTemperature} {this.DischargeCurrent} discharge";
                tr.StatusChanged += newsub.TestRecord_StatusChanged;
                newsub.FirstTestRecords.Add(tr);
                newsub.SecondTestRecords = new ObservableCollection<TestRecordClass>();
                tr = new TestRecordClass();
                tr.SubProgramStr = $"{this.ChargeTemperature.Name} {this.ChargeCurrent} charge, {this.DischargeTemperature} {this.DischargeCurrent} discharge";
                tr.StatusChanged += newsub.TestRecord_StatusChanged;
                newsub.SecondTestRecords.Add(tr);
            }
            return newsub;
        }

        private void TestRecord_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            TestRecordClass tr = sender as TestRecordClass; //被改变的Test Record
            if (e.Status == TestStatus.Invalid)
            {
                var dbContext = new AppDbContext();
                var sub = dbContext.SubPrograms.SingleOrDefault(i => i.Id == this.Id);
                dbContext.Entry(sub)
                    .Collection(i => i.FirstTestRecords)
                    .Load();
                dbContext.Entry(sub)
                    .Collection(i => i.SecondTestRecords)
                    .Load();
                if (FirstTestRecords.Contains(tr))
                {
                    var newTestRecord = new TestRecordClass();
                    newTestRecord.StatusChanged += this.TestRecord_StatusChanged;
                    FirstTestRecords.Add(newTestRecord);
                    OnRasieTestRecordAddedEvent(newTestRecord, true);
                    sub.FirstTestRecords.Add(newTestRecord);
                }
                else if (SecondTestRecords.Contains(tr))
                {
                    var newTestRecord = new TestRecordClass();
                    newTestRecord.StatusChanged += this.TestRecord_StatusChanged;
                    SecondTestRecords.Add(newTestRecord);
                    OnRasieTestRecordAddedEvent(newTestRecord, false);
                    sub.SecondTestRecords.Add(newTestRecord);
                }
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
            foreach (var tr in FirstTestRecords)
            {
                tr.Abandon();
            }
            foreach (var tr in SecondTestRecords)
            {
                tr.Abandon();
            }
        }
    }
}
