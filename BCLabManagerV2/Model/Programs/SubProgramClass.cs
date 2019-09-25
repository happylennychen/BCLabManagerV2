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
                string ctstr = "";
                string ccstr = "";
                string dtstr = "";
                string dcstr = "";
                if (this.ChargeTemperature == -9999)
                    ctstr = "Room";
                else
                    ctstr = this.ChargeTemperature.ToString() + " deg";

                if (ChargeCurrentType == CurrentTypeEnum.Absolute)
                    ccstr = this.ChargeCurrent.ToString() + "mA";
                else if (ChargeCurrentType == CurrentTypeEnum.Percentage)
                    ccstr = this.ChargeCurrent.ToString() + "C";
                else if (ChargeCurrentType == CurrentTypeEnum.Dynamic)
                    ccstr = "D" + this.ChargeCurrent.ToString();

                if (this.DischargeTemperature == -9999)
                    dtstr = "Room";
                else
                    dtstr = this.DischargeTemperature.ToString() + " deg";

                if (DischargeCurrentType == CurrentTypeEnum.Absolute)
                    dcstr = this.DischargeCurrent.ToString() + "mA";
                else if (DischargeCurrentType == CurrentTypeEnum.Percentage)
                    dcstr = this.DischargeCurrent.ToString() + "C";
                else if (DischargeCurrentType == CurrentTypeEnum.Dynamic)
                    dcstr = "D" + this.DischargeCurrent.ToString();

                return $"{ctstr} {ccstr} charge, {dtstr} {dcstr} discharge";
            }
        }
        public double ChargeTemperature { get; set; }
        public double DischargeTemperature { get; set; }
        public double ChargeCurrent { get; set; }
        public CurrentTypeEnum ChargeCurrentType { get; set; }
        public double DischargeCurrent { get; set; }
        public CurrentTypeEnum DischargeCurrentType { get; set; }
        public TestCountEnum TestCount { get; set; }
        public int Loop { get; set; } = 1;
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
            this.ChargeTemperature = template.ChargeTemperature;
                this.ChargeCurrent = template.ChargeCurrent;
            this.DischargeTemperature = template.DischargeTemperature;
                this.DischargeCurrent = template.DischargeCurrent;
            this.ChargeCurrentType = template.ChargeCurrentType;
            this.DischargeCurrentType = template.DischargeCurrentType;
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
            this.DischargeTemperature = template.DischargeTemperature;
            this.DischargeCurrent = template.DischargeCurrent;
            this.ChargeCurrentType = template.ChargeCurrentType;
            this.DischargeCurrentType = template.DischargeCurrentType;
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
            CurrentTypeEnum chargeCurrentType,
            CurrentTypeEnum dischargeCurrentType,
            TestCountEnum TestCount,
            int Loop) : this()
        {
            //this.Name = Name;
            this.ChargeTemperature = chargeTemperature;
            this.ChargeCurrent = chargeCurrent;
            this.DischargeTemperature = dischargeTemperature;
            this.DischargeCurrent = dischargeCurrent;
            this.ChargeCurrentType = chargeCurrentType;
            this.DischargeCurrentType = dischargeCurrentType;
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
            var newsub = new SubProgramClass(this.ChargeTemperature, this.ChargeCurrent, this.ChargeCurrent, this.DischargeCurrent,this.ChargeCurrentType, this.DischargeCurrentType, this.TestCount, this.Loop);
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
