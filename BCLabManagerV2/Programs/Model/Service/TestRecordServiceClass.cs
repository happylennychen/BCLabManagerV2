using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        public void Execute(TestRecordClass testRecord, string batteryTypeStr, BatteryClass battery, ChamberClass chamber, string testerStr, ChannelClass channel, DateTime startTime, double measurementGain, double measurementOffset, double traceResistance, double capacityDifference, string @operator)
        {
            testRecord.BatteryTypeStr = batteryTypeStr;
            testRecord.BatteryStr = battery.Name;
            testRecord.ChamberStr = chamber.Name;
            testRecord.TesterStr = testerStr;
            testRecord.ChannelStr = channel.Name;
            testRecord.StartTime = startTime;
            testRecord.MeasurementGain = measurementGain;
            testRecord.MeasurementOffset = measurementOffset;
            testRecord.TraceResistance = traceResistance;
            testRecord.CapacityDifference = capacityDifference;
            testRecord.Operator = @operator;
            testRecord.AssignedBattery = battery;
            testRecord.AssignedChamber = chamber;
            testRecord.AssignedChannel = channel;
            testRecord.Status = TestStatus.Executing;
            DatabaseUpdate(testRecord);
        }

        internal void Commit(TestRecordClass testRecord, string comment, List<RawDataClass> rawDataList, DateTime startTime, DateTime completeTime, string batteryType, string projectName, string programName, string recipeName)
        {
            testRecord.Comment = comment;
            testRecord.RawDataList = rawDataList;
            testRecord.StartTime = startTime;
            testRecord.EndTime = completeTime;
            testRecord.AssignedBattery = null;
            testRecord.AssignedChamber = null;
            testRecord.AssignedChannel = null;
            testRecord.ProjectStr = projectName;
            testRecord.ProgramStr = programName;
            testRecord.RecipeStr = recipeName;
            testRecord.Status = TestStatus.Completed;
            testRecord.TestFilePath = CreateTestFile(rawDataList, batteryType, projectName);
            SuperUpdate(testRecord);
        }

        private string CreateTestFile(List<RawDataClass> rawDataList, string batteryType, string projectName)   //默认按顺序导入
        {
            if (rawDataList.Count == 1)
            {
                return rawDataList[0].FilePath;
            }
            else
            {
                string filename = string.Empty ;
                StringBuilder sb = new StringBuilder();
                foreach (var raw in rawDataList)
                {
                    filename += Path.GetFileName(raw.FilePath) +"__";
                }
                filename = filename.Substring(0, filename.Length - 2);
                var filepath = $@"Q:\807\Software\WH BC Lab\Data\{batteryType}\{projectName}\Test Data\{filename}";

                bool isFirst = true;
                foreach (var raw in rawDataList)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        //File.WriteAllText(filename, File.ReadAllText(raw.FilePath));
                        try
                        {
                            File.Copy(raw.FilePath, filepath);
                        }
                        catch(Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    }
                    else
                    {
                        var lines = File.ReadAllLines(raw.FilePath).ToList();
                        lines.RemoveRange(0, 10);
                        File.AppendAllLines(filepath, lines);
                    }
                }
                return filepath;
            }
        }

        internal TestRecordClass Invalidate(TestRecordClass testRecord, string comment)
        {
            testRecord.Comment += "\r\n" + comment;
            testRecord.Status = TestStatus.Invalid;
            DatabaseUpdate(testRecord);

            var newTestRecord = new TestRecordClass();
            SuperAdd(newTestRecord);
            return newTestRecord;
        }

        internal void Abandon(TestRecordClass testRecord)
        {
            testRecord.Status = TestStatus.Abandoned;
            DatabaseUpdate(testRecord);
        }
    }
}
