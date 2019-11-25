using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class TestRecordServiceClass
    {
        public ObservableCollection<TestRecordClass> Items { get; set; }
        public RawDataServiceClass RawDataService { get; set; } = new RawDataServiceClass();
        public void SuperAdd(TestRecordClass item)
        {
            DatabaseAdd(item);
            DomainAdd(item);
        }
        public void DatabaseAdd(TestRecordClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.TestRecords.Insert(item);
                uow.Commit();
            }
        }
        public void DomainAdd(TestRecordClass item)
        {
            Items.Add(item);
        }
        public void Remove(int id)
        {
            //using (var uow = new UnitOfWork(new AppDbContext()))
            //{
            //    uow.Batteries.Delete(id);
            //    uow.Commit();
            //}

            //var item = Items.SingleOrDefault(o => o.Id == id);
            //Items.Remove(item);
        }
        public void SuperUpdate(TestRecordClass item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(TestRecordClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.TestRecords.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(TestRecordClass item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.BatteryStr = item.BatteryStr;
            edittarget.BatteryTypeStr = item.BatteryTypeStr;
            edittarget.ChamberStr = item.ChamberStr;
            edittarget.ChannelStr = item.ChannelStr;
            edittarget.Comment = item.Comment;
            edittarget.EndTime = item.EndTime;
            edittarget.NewCycle = item.NewCycle;
            edittarget.ProgramStr = item.ProgramStr;
            edittarget.RawDataList = item.RawDataList;
            edittarget.RecipeStr = item.RecipeStr;
            edittarget.StartTime = item.StartTime;
            edittarget.Status = item.Status;
            edittarget.TesterStr = item.TesterStr;
        }
        public void Execute(TestRecordClass testRecord, string batteryTypeStr, BatteryClass battery, ChamberClass chamber, string testerStr, ChannelClass channel, DateTime startTime)
        {
            testRecord.BatteryTypeStr = batteryTypeStr;
            testRecord.BatteryStr = battery.Name;
            testRecord.ChamberStr = chamber.Name;
            testRecord.TesterStr = testerStr;
            testRecord.ChannelStr = channel.Name;
            testRecord.StartTime = startTime;
            testRecord.AssignedBattery = battery;
            testRecord.AssignedChamber = chamber;
            testRecord.AssignedChannel = channel;
            testRecord.Status = TestStatus.Executing;
            DatabaseUpdate(testRecord);
        }

        internal void Commit(TestRecordClass testRecord, string comment, List<RawDataClass> rawDataList, DateTime completeTime, string programName, string recipeName)
        {
            testRecord.Comment = comment;
            testRecord.RawDataList = rawDataList;
            testRecord.EndTime = completeTime;
            testRecord.AssignedBattery = null;
            testRecord.AssignedChamber = null;
            testRecord.AssignedChannel = null;
            testRecord.ProgramStr = programName;
            testRecord.RecipeStr = recipeName;
            testRecord.Status = TestStatus.Completed;
            SuperUpdate(testRecord);
        }

        internal void Invalidate(TestRecordClass testRecord, string comment)
        {
            testRecord.Comment += "\r\n" + comment;
            testRecord.Status = TestStatus.Invalid;
            DatabaseUpdate(testRecord);
        }

        internal void Abandon(TestRecordClass testRecord)
        {
            testRecord.Status = TestStatus.Abandoned;
            DatabaseUpdate(testRecord);
        }
    }
}
