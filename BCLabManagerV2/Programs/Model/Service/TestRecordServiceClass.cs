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
            edittarget.RecipeStr = item.RecipeStr;
            edittarget.StartTime = item.StartTime;
            edittarget.Status = item.Status;
            edittarget.TesterStr = item.TesterStr;
        }
        public void Execute(TestRecordClass testRecord, string batteryTypeStr, string projectName, BatteryClass battery, ChamberClass chamber, string testerStr, ChannelClass channel, double current, double temperature, DateTime startTime, double measurementGain, double measurementOffset, double traceResistance, double capacityDifference, string @operator, string programName, string recipeName)
        {
            testRecord.BatteryTypeStr = batteryTypeStr;
            testRecord.ProjectStr = projectName;
            testRecord.BatteryStr = battery.Name;
            testRecord.ChamberStr = chamber.Name;
            testRecord.TesterStr = testerStr;
            testRecord.ChannelStr = channel.Name;
            testRecord.Current = current;
            testRecord.Temperature = temperature;
            testRecord.StartTime = startTime;
            testRecord.MeasurementGain = measurementGain;
            testRecord.MeasurementOffset = measurementOffset;
            testRecord.TraceResistance = traceResistance;
            testRecord.CapacityDifference = capacityDifference;
            testRecord.Operator = @operator;
            testRecord.ProgramStr = programName;
            testRecord.RecipeStr = recipeName;
            testRecord.AssignedBattery = battery;
            testRecord.AssignedChamber = chamber;
            testRecord.AssignedChannel = channel;
            testRecord.Status = TestStatus.Executing;
            DatabaseUpdate(testRecord);
        }

        internal void Commit(TestRecordClass testRecord, string comment, List<string> rawDataList, bool isRename, string newName, DateTime startTime, DateTime completeTime, string batteryType, string projectName, Header header)
        {
            testRecord.Comment = comment;
            testRecord.StartTime = startTime;
            testRecord.EndTime = completeTime;
            testRecord.AssignedBattery = null;
            testRecord.AssignedChamber = null;
            testRecord.AssignedChannel = null;
            testRecord.Status = TestStatus.Completed;
            string root = $@"{GlobalSettings.RootPath}{batteryType}\{projectName}";
            string temproot = $@"{GlobalSettings.TempraryFolder}{batteryType}\{projectName}";
            string temptestfilepath = string.Empty;
            if (rawDataList.Count > 1)
            {
                temptestfilepath = CreateTestFile(rawDataList, temproot, newName);
                testRecord.TestFilePath = $@"{root}\{GlobalSettings.TestDataFolderName}\{Path.GetFileName(temptestfilepath)}";
                File.Copy(temptestfilepath, testRecord.TestFilePath);
            }
            else
            {
                if (isRename)
                {
                    temptestfilepath = RenameRawDataAndCopyToFolder(rawDataList[0], temproot, newName);
                    testRecord.TestFilePath = $@"{root}\{GlobalSettings.TestDataFolderName}\{Path.GetFileName(temptestfilepath)}";
                    File.Copy(temptestfilepath, testRecord.TestFilePath);
                }
                else
                {
                    temptestfilepath = CopyToFolder(rawDataList[0], temproot);
                    testRecord.TestFilePath = $@"{root}\{GlobalSettings.TestDataFolderName}\{Path.GetFileName(temptestfilepath)}";
                    File.Copy(temptestfilepath, testRecord.TestFilePath);
                }
            }
            SuperUpdate(testRecord);

            if (header.Type == "RC" || header.Type == "OCV")
            {
                string tempheaderFilePath = Path.ChangeExtension($@"{temproot}\{GlobalSettings.HeaderFolderName}\{Path.GetFileName(testRecord.TestFilePath)}", "HDR");
                string headerFilePath = Path.ChangeExtension($@"{root}\{GlobalSettings.HeaderFolderName}\{Path.GetFileName(testRecord.TestFilePath)}", "HDR");
                if (!File.Exists(tempheaderFilePath))
                {
                    CreateHeaderFile(tempheaderFilePath, header);
                    File.Copy(tempheaderFilePath, headerFilePath);
                    var tempSourceFilePath = CreateSourceFile(temproot, tempheaderFilePath, temptestfilepath);
                    string sourceFilePath = $@"{root}\{GlobalSettings.SourceDataFolderName}\{Path.GetFileName(temptestfilepath)}";
                    File.Copy(tempSourceFilePath, sourceFilePath);
                }
            }
        }

        private string CopyToFolder(string filepath, string root)
        {
            var newPath = Path.Combine($@"{root}\{GlobalSettings.TestDataFolderName}", Path.GetFileName(filepath));
            var tempPath = Path.Combine($@"{GlobalSettings.TempraryFolder}\{GlobalSettings.TestDataFolderName}", Path.GetFileName(filepath));
            File.Copy(filepath, tempPath);
            File.Copy(filepath, newPath);
            //rawDataName[0] = newPath;
            return newPath;
        }

        private string RenameRawDataAndCopyToFolder(string rawDataName, string root, string newName)
        {
            var newPath = Path.Combine($@"{root}\{GlobalSettings.TestDataFolderName}", newName + Path.GetExtension(rawDataName));
            File.Copy(rawDataName, newPath);
            //rawDataName[0] = newPath;
            return newPath;
        }

        private string CreateSourceFile(string root, string headerFilePath, string testFilePath)
        {
            string sourceFilePath = $@"{root}\{GlobalSettings.SourceDataFolderName}\{Path.GetFileName(testFilePath)}";
            File.Copy(headerFilePath, sourceFilePath);
            File.AppendAllLines(sourceFilePath, File.ReadAllLines(testFilePath));
            return sourceFilePath;
        }

        private void CreateHeaderFile(string headerFilePath, Header header)
        {
            List<string> lines = new List<string>();
            lines.Add($"Type,{header.Type}");
            lines.Add($"Test Time,{header.TestTime}");
            lines.Add($"Equipment,{header.Equipment}");
            lines.Add($"Manufacture Factory,{header.ManufactureFactory}");
            lines.Add($"Battery Model,{header.BatteryModel}");
            lines.Add($"Cycle Count,{header.CycleCount}");
            lines.Add($"Temperature(DegC),{header.Temperature}");
            lines.Add($"Current(mA),{header.Current}");
            lines.Add($"Measurement Gain,{header.MeasurementGain}");
            lines.Add($"Measurement Offset(mV),{header.MeasurementOffset}");
            lines.Add($"Trace Resistance(mohm),{header.TraceResistance}");
            lines.Add($"Capacity Difference(mAH),{header.CapacityDifference}");
            lines.Add($"Absolute Max Capacity(mAH),{header.AbsoluteMaxCapacity}");
            lines.Add($"Limited Charge Voltage(mV),{header.LimitedChargeVoltage}");
            lines.Add($"Cut-off Discharge Voltage(mV),{header.CutoffDischargeVoltage}");
            lines.Add($"Tester,{header.Tester}");
            for (int i = 0; i < 9; i++)
            {
                lines.Add("");
            }
            File.WriteAllLines(headerFilePath, lines);
        }

        private string CreateTestFile(List<string> rawDataList, string root, string newName)   //默认按顺序导入
        {
            var filepath = $@"{root}\{GlobalSettings.TestDataFolderName}\{newName}";

            bool isFirst = true;
            foreach (var raw in rawDataList)
            {
                if (isFirst)
                {
                    isFirst = false;
                    //File.WriteAllText(filename, File.ReadAllText(raw.FilePath));
                    try
                    {
                        File.Copy(raw, filepath);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                else
                {
                    var lines = File.ReadAllLines(raw).ToList();
                    lines.RemoveRange(0, 10);
                    File.AppendAllLines(filepath, lines);
                }
            }
            return filepath;
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
