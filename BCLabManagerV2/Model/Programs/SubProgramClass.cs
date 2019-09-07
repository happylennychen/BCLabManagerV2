using System;
using System.Collections.Generic;
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
        public String Name { get; set; }
        public TestCountEnum TestCount { get; set; }
        public List<TestRecordClass> FirstTestRecords { get; set; }
        public List<TestRecordClass> SecondTestRecords { get; set; }

        public SubProgramClass()
        {
            FirstTestRecords = new List<TestRecordClass>();
            SecondTestRecords = new List<TestRecordClass>();
        }

        public SubProgramClass(SubProgramTemplate template)
        {
            this.Name = template.Name;
            this.TestCount = template.TestCount;
            this.FirstTestRecords = new List<TestRecordClass>();
            this.SecondTestRecords = new List<TestRecordClass>();

            var tr = new TestRecordClass();
            tr.StatusChanged += this.TestRecord_StatusChanged;
            this.FirstTestRecords.Add(tr);
            if (this.TestCount == TestCountEnum.Two)
            {
                var tr1 = new TestRecordClass();
                tr1.StatusChanged += this.TestRecord_StatusChanged;
                this.SecondTestRecords.Add(tr1);
            }
        }

        public SubProgramClass(String Name, TestCountEnum TestCount) : this()
        {
            this.Name = Name;
            this.TestCount = TestCount;
        }

        public void AssociateEvent(TestRecordClass testRecord)
        {
            testRecord.StatusChanged += this.TestRecord_StatusChanged;
        }

        public void Update(String Name, TestCountEnum TestCount)
        {
            this.Name = Name;
            this.TestCount = TestCount;
        }
        public SubProgramClass Clone()  //Clone Name and Test Count, and create testrecords list
        {
            var newsub = new SubProgramClass(this.Name, this.TestCount);
            if (this.TestCount == TestCountEnum.One)
            {
                newsub.FirstTestRecords = new List<TestRecordClass>();
                var tr = new TestRecordClass();
                tr.StatusChanged += newsub.TestRecord_StatusChanged;
                newsub.FirstTestRecords.Add(tr);
            }
            else if (this.TestCount == TestCountEnum.Two)
            {
                newsub.FirstTestRecords = new List<TestRecordClass>();
                var tr = new TestRecordClass();
                tr.StatusChanged += newsub.TestRecord_StatusChanged;
                newsub.FirstTestRecords.Add(tr);
                newsub.SecondTestRecords = new List<TestRecordClass>();
                tr = new TestRecordClass();
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
        { }
    }
}
