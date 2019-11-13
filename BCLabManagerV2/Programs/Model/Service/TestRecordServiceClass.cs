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
        public void Add(TestRecordClass item)
        {
            //using (var uow = new UnitOfWork(new AppDbContext()))
            //{
            //    item.BatteryType = uow.BatteryTypes.GetById(item.BatteryType.Id);
            //    uow.Batteries.Insert(item);
            //    uow.Commit();
            //}
            //Items.Add(item);
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
        public void Update(TestRecordClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.TestRecords.Update(item);
                uow.Commit();
            }
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.BatteryStr = item.BatteryStr;
            edittarget.BatteryTypeStr = item.BatteryTypeStr;
            edittarget.ChamberStr = item.ChamberStr;
            edittarget.ChannelStr = item.ChannelStr;
            edittarget.Comment = item.Comment;
            edittarget.CompleteTime = item.CompleteTime;
            edittarget.NewCycle = item.NewCycle;
            edittarget.ProgramStr = item.ProgramStr;
            edittarget.RawDataList = item.RawDataList;
            edittarget.RecipeStr = item.RecipeStr;
            edittarget.StartTime = item.StartTime;
            edittarget.Status = item.Status;
            edittarget.TesterStr = item.TesterStr;
        }
        public void Execute(TestRecordClass testRecord, string batteryTypeStr, string batteryStr, string chamberStr, string testerStr, string channelStr, DateTime startTime)
        {
            testRecord.BatteryTypeStr = batteryTypeStr;
            testRecord.BatteryStr = batteryStr;
            testRecord.ChamberStr = chamberStr;
            testRecord.TesterStr = testerStr;
            testRecord.ChannelStr = channelStr;
            testRecord.StartTime = startTime;
            testRecord.Status = TestStatus.Executing;
            Update(testRecord);
        }
    }
}
