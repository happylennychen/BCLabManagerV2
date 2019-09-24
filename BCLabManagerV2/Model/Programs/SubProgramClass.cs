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
    public class SubProgramClass : ModelBase
    {
        public int Id { get; set; }
        public bool IsAbandoned { get; set; }
        public String Name
        {
            get
            {
                string ctstr;
                string ccstr;
                string dtstr;
                string dcstr;
                if (this.ChargeTemperature == -9999)
                    ctstr = "Room";
                else
                    ctstr = this.ChargeTemperature.ToString() + " deg";
                ccstr = this.ChargeCurrent.ToString() + "mA";
                if (this.DischargeTemperature == -9999)
                    dtstr = "Room";
                else
                    dtstr = this.DischargeTemperature.ToString() + " deg";
                dcstr = this.DischargeCurrent.ToString() + "mA";
                return $"{ctstr} {ccstr} charge, {dtstr} {dcstr} discharge";
            }
        }
        public double ChargeTemperature { get; set; }
        public double DischargeTemperature { get; set; }
        public double ChargeCurrent { get; set; }
        public double DischargeCurrent { get; set; }
        public TestCountEnum TestCount { get; set; }
        public int Loop { get; set; }
        public ObservableCollection<TestRecordClass> FirstTestRecords { get; set; }
        public ObservableCollection<TestRecordClass> SecondTestRecords { get; set; }

        public SubProgramClass()
        {
            FirstTestRecords = new ObservableCollection<TestRecordClass>();
            SecondTestRecords = new ObservableCollection<TestRecordClass>();
        }

        public SubProgramClass(SubProgramTemplate template)
        {
            //this.Name = template.Name;
            //this.ChargeTemperature = template.ChargeTemperature;
            //if (template.ChargeCurrentType == CurrentType.Absolute)
            //    this.ChargeCurrent = template.ChargeCurrent;
            //else if (template.ChargeCurrentType == CurrentType.Percentage)
            //    this.ChargeCurrent = template.ChargeCurrent * capacity;
            //this.DischargeTemperature = template.DischargeTemperature;
            //if (template.DischargeCurrentType == CurrentType.Absolute)
            //    this.DischargeCurrent = template.DischargeCurrent;
            //else if (template.DischargeCurrentType == CurrentType.Percentage)
            //    this.DischargeCurrent = template.DischargeCurrent * capacity;
            this.TestCount = template.TestCount;
            this.FirstTestRecords = new ObservableCollection<TestRecordClass>();
            this.SecondTestRecords = new ObservableCollection<TestRecordClass>();

            var tr = new TestRecordClass();
            tr.StatusChanged += this.TestRecord_StatusChanged;
            tr.SubProgramStr = this.Name;
            this.FirstTestRecords.Add(tr);
            if (this.TestCount == TestCountEnum.Two)
            {
                var tr1 = new TestRecordClass();
                tr1.StatusChanged += this.TestRecord_StatusChanged;
                tr1.SubProgramStr = this.Name;
                this.SecondTestRecords.Add(tr1);
            }
        }

        public SubProgramClass(SubProgramTemplate template, string ProgramStr, int loop)  //Only used by populator
        {
            //this.Name = template.Name;
            this.ChargeTemperature = template.ChargeTemperature;
            this.ChargeCurrent = template.ChargeCurrent;
            this.ChargeCurrent = template.DischargeTemperature;
            this.DischargeCurrent = template.DischargeCurrent;
            this.TestCount = template.TestCount;
            this.Loop = loop;
            this.FirstTestRecords = new ObservableCollection<TestRecordClass>();
            this.SecondTestRecords = new ObservableCollection<TestRecordClass>();

            var tr = new TestRecordClass();
            tr.StatusChanged += this.TestRecord_StatusChanged;
            tr.SubProgramStr = this.Name;
            tr.ProgramStr = ProgramStr;
            this.FirstTestRecords.Add(tr);
            if (this.TestCount == TestCountEnum.Two)
            {
                var tr1 = new TestRecordClass();
                tr1.StatusChanged += this.TestRecord_StatusChanged;
                tr1.SubProgramStr = this.Name;
                tr1.ProgramStr = ProgramStr;
                this.SecondTestRecords.Add(tr1);
            }
        }

        public SubProgramClass(
            double chargeTemperature,
            double chargeCurrent,
            double dischargeTemperature,
            double dischargeCurrent, 
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
        public SubProgramClass Clone()  //Clone Name and Test Count, and create testrecords list
        {
            var newsub = new SubProgramClass(this.ChargeTemperature, this.ChargeCurrent, this.ChargeCurrent, this.DischargeCurrent, this.TestCount, this.Loop);
            if (this.TestCount == TestCountEnum.One)
            {
                newsub.FirstTestRecords = new ObservableCollection<TestRecordClass>();
                var tr = new TestRecordClass();
                tr.SubProgramStr = this.Name;
                tr.StatusChanged += newsub.TestRecord_StatusChanged;
                newsub.FirstTestRecords.Add(tr);
            }
            else if (this.TestCount == TestCountEnum.Two)
            {
                newsub.FirstTestRecords = new ObservableCollection<TestRecordClass>();
                var tr = new TestRecordClass();
                tr.SubProgramStr = this.Name;
                tr.StatusChanged += newsub.TestRecord_StatusChanged;
                newsub.FirstTestRecords.Add(tr);
                newsub.SecondTestRecords = new ObservableCollection<TestRecordClass>();
                tr = new TestRecordClass();
                tr.SubProgramStr = this.Name;
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
