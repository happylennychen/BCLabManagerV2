﻿//#define Test
using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BCLabManager.Model
{
    public class TestRecordServiceClass
    {
        public ObservableCollection<TestRecord> Items { get; set; }
        public void SuperAdd(TestRecord item)
        {
            DatabaseAdd(item);
            DomainAdd(item);
        }
        public void DatabaseAdd(TestRecord item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.TestRecords.Insert(item);
                uow.Commit();
            }
        }
        public void DomainAdd(TestRecord item)
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
        public void SuperUpdate(TestRecord item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(TestRecord item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.TestRecords.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(TestRecord item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.BatteryStr = item.BatteryStr;
            edittarget.BatteryTypeStr = item.BatteryTypeStr;
            edittarget.ChamberStr = item.ChamberStr;
            edittarget.ChannelStr = item.ChannelStr;
            edittarget.Comment = item.Comment;
            edittarget.EndTime = item.EndTime;
            edittarget.LastCycle = item.LastCycle;
            edittarget.NewCycle = item.NewCycle;
            edittarget.ProgramStr = item.ProgramStr;
            edittarget.RecipeStr = item.RecipeStr;
            edittarget.StartTime = item.StartTime;
            edittarget.Status = item.Status;
            edittarget.TesterStr = item.TesterStr;
        }
        public void Execute(TestRecord testRecord, string batteryTypeStr, string projectName, Battery battery, Chamber chamber, string testerStr, Channel channel, double current, double temperature, DateTime startTime, double measurementGain, double measurementOffset, double traceResistance, double capacityDifference, string @operator, string programName, string recipeName)
        {
            testRecord.BatteryTypeStr = batteryTypeStr;
            testRecord.ProjectStr = projectName;
            testRecord.BatteryStr = battery.Name;
            testRecord.LastCycle = battery.CycleCount;
            if (chamber != null)
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

        internal void Commit(TestRecord testRecord, string comment, List<string> rawDataList, bool isRename, string newName, DateTime startTime, DateTime completeTime, string batteryType, string projectName, Header header)
        {
            testRecord.Comment = comment;
            testRecord.StartTime = startTime;
            testRecord.EndTime = completeTime;
            testRecord.AssignedBattery = null;
            testRecord.AssignedChamber = null;
            testRecord.AssignedChannel = null;
            testRecord.Status = TestStatus.Completed;
            string root = $@"{GlobalSettings.RootPath}{batteryType}\{projectName}";
            string temproot = $@"{GlobalSettings.LocalFolder}{batteryType}\{projectName}";
            string temptestfilepath = string.Empty;
            if (rawDataList.Count > 1)
            {
                temptestfilepath = CreateTestFile(rawDataList, temproot, newName);
                testRecord.TestFilePath = $@"{root}\{GlobalSettings.TestDataFolderName}\{Path.GetFileName(temptestfilepath)}";
                CopyToServerWithRetry(temptestfilepath, testRecord.TestFilePath);
            }
            else
            {
                if (isRename)
                {
                    temptestfilepath = RenameRawDataAndCopyToFolder(rawDataList[0], $@"{temproot}\{GlobalSettings.TestDataFolderName}", newName);
                    testRecord.TestFilePath = $@"{root}\{GlobalSettings.TestDataFolderName}\{Path.GetFileName(temptestfilepath)}";
                    CopyToServerWithRetry(temptestfilepath, testRecord.TestFilePath);
                }
                else
                {
                    temptestfilepath = CopyToFolder(rawDataList[0], temproot);
                    testRecord.TestFilePath = $@"{root}\{GlobalSettings.TestDataFolderName}\{Path.GetFileName(temptestfilepath)}";
                    CopyToServerWithRetry(temptestfilepath, testRecord.TestFilePath);
                }
            }
            if (testRecord.TestFilePath == "")
            {
                MessageBox.Show("Test File Path Empty!");
                return;
            }
            SuperUpdate(testRecord);

            if (header.Type == "RC" || header.Type == "OCV")
            {
                string tempheaderFilePath = Path.ChangeExtension($@"{temproot}\{GlobalSettings.HeaderFolderName}\{Path.GetFileName(testRecord.TestFilePath)}", "HDR");
                string headerFilePath = Path.ChangeExtension($@"{root}\{GlobalSettings.HeaderFolderName}\{Path.GetFileName(testRecord.TestFilePath)}", "HDR");
                if (!File.Exists(tempheaderFilePath))
                {
                    CreateHeaderFile(tempheaderFilePath, header);
                    CopyToServerWithRetry(tempheaderFilePath, headerFilePath);
                    var tempSourceFilePath = CreateSourceFile(temproot, tempheaderFilePath, temptestfilepath);
                    string sourceFilePath = $@"{root}\{GlobalSettings.SourceDataFolderName}\{Path.GetFileName(temptestfilepath)}";
                    CopyToServerWithRetry(tempSourceFilePath, sourceFilePath);
                }
            }
        }

        private void CopyToServerWithRetry(string tempPath, string serverPath)
        {
#if! Test
            var thread = new Thread(() =>
            {
                int i;
                for (i = 0; i < 5; i++)
                {
                    if (CopyToServer(tempPath, serverPath))
                    {
                        break;
                    }
                }
                if (i == 5)
                {
                    if (!File.Exists(serverPath))
                    {
                        Event evt = new Event();
                        evt.Module = "NAS file transfer";
                        evt.Timestamp = DateTime.Now;
                        evt.Type = EventType.Error;
                        evt.Description = $"Test File Missing!. File Name: {Path.GetFileName(tempPath)}";
                        EventService.SuperAdd(evt);
                        MessageBox.Show(evt.Description);
                        return;
                    }

                    FileInfo fi1 = new FileInfo(tempPath);
                    FileInfo fi2 = new FileInfo(serverPath);
                    if (fi1.Length != fi2.Length)
                    {
                        Event evt = new Event();
                        evt.Module = "NAS file transfer";
                        evt.Timestamp = DateTime.Now;
                        evt.Type = EventType.Error;
                        evt.Description = $"Original file length is {fi1.Length}B, server file length is {fi2.Length}B.";
                        EventService.SuperAdd(evt);
                        MessageBox.Show(evt.Description);
                    }
                }
            });
            thread.Start();
#endif
        }

        private bool CopyToServer(string tempPath, string serverPath)
        {
            File.Copy(tempPath, serverPath, true);
            if (!File.Exists(serverPath))
                return false;

            FileInfo fi1 = new FileInfo(tempPath);
            FileInfo fi2 = new FileInfo(serverPath);
            if (fi1.Length != fi2.Length)
                return false;

            return true;
        }

        private string CopyToFolder(string filepath, string root)
        {
            var newPath = Path.Combine($@"{root}\{GlobalSettings.TestDataFolderName}", Path.GetFileName(filepath));
            var tempPath = Path.Combine($@"{GlobalSettings.LocalFolder}\{GlobalSettings.TestDataFolderName}", Path.GetFileName(filepath));
            File.Copy(filepath, tempPath, true);
            File.Copy(filepath, newPath, true);
            //rawDataName[0] = newPath;
            return newPath;
        }

        private string RenameRawDataAndCopyToFolder(string rawDataName, string root, string newName)
        {
            var newPath = Path.Combine(root, newName + Path.GetExtension(rawDataName));
            File.Copy(rawDataName, newPath, true);
            //rawDataName[0] = newPath;
            return newPath;
        }

        private string CreateSourceFile(string root, string headerFilePath, string testFilePath)
        {
            string sourceFilePath = $@"{root}\{GlobalSettings.SourceDataFolderName}\{Path.GetFileName(testFilePath)}";
            File.Copy(headerFilePath, sourceFilePath, true);
            File.AppendAllLines(sourceFilePath, File.ReadAllLines(testFilePath));
            return sourceFilePath;
        }

        private void CreateHeaderFile(string headerFilePath, Header header)
        {
            List<string> lines = new List<string>();
            lines.Add($"Type: ,{header.Type}");
            lines.Add($"Test Time: ,{header.TestTime}");
            lines.Add($"Equipment: ,{header.Equipment}");
            lines.Add($"Manufacture Factory: ,{header.ManufactureFactory}");
            lines.Add($"Battery Model: ,{header.BatteryModel}");
            lines.Add($"Cycle Count: ,{header.CycleCount}");
            lines.Add($"Temperature(DegC): ,{header.Temperature}");
            lines.Add($"Current(mA): ,{header.Current}");
            lines.Add($"Measurement Gain: ,{header.MeasurementGain}");
            lines.Add($"Measurement Offset(mV): ,{header.MeasurementOffset}");
            lines.Add($"Trace Resistance(mohm): ,{header.TraceResistance}");
            lines.Add($"Capacity Difference(mAH): ,{header.CapacityDifference}");
            lines.Add($"Absolute Max Capacity(mAH): ,{header.AbsoluteMaxCapacity}");
            lines.Add($"Limited Charge Voltage(mV): ,{header.LimitedChargeVoltage}");
            lines.Add($"Cut-off Discharge Voltage(mV): ,{header.CutoffDischargeVoltage}");
            //lines.Add($"Tester: ,{header.Tester}");
            lines.Add($"Tester: ,Leon");
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
                        File.Copy(raw, filepath, true);
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

        internal void Invalidate(TestRecord testRecord, string comment)
        {
            testRecord.Comment += "\r\n" + comment;
            testRecord.Status = TestStatus.Invalid;
            try
            {
                File.Move(testRecord.TestFilePath, testRecord.TestFilePath + "_INVALID");
            }
            catch (Exception e)
            {
            }
            testRecord.TestFilePath += "_INVALID";
            DatabaseUpdate(testRecord);
        }

        internal void Abandon(TestRecord testRecord)
        {
            testRecord.Status = TestStatus.Abandoned;
            if (testRecord.TestFilePath != string.Empty)
            {
                File.Move(testRecord.TestFilePath, testRecord.TestFilePath + "_ABANDONED");
                testRecord.TestFilePath += "_ABANDONED";
            }
            DatabaseUpdate(testRecord);
        }

        internal void ExecuteFree(TestRecord testRecord, Battery battery, Chamber chamber, string testerStr, Channel channel, DateTime startTime, double measurementGain, double measurementOffset, double traceResistance, double capacityDifference, string @operator)
        {
            testRecord.BatteryTypeStr = battery.BatteryType.Name;
            testRecord.BatteryStr = battery.Name;
            if (chamber != null)
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

        internal void CommitFree(TestRecord testRecord, string comment, List<string> rawDataList, bool isRename, string newName, DateTime startTime, DateTime completeTime)
        {
            testRecord.Comment = comment;
            testRecord.StartTime = startTime;
            testRecord.EndTime = completeTime;
            testRecord.AssignedBattery = null;
            testRecord.AssignedChamber = null;
            testRecord.AssignedChannel = null;
            testRecord.Status = TestStatus.Completed;
            string root = $@"{GlobalSettings.RootPath}{GlobalSettings.TempDataFolderName}";
            string temproot = $@"{GlobalSettings.LocalFolder}{GlobalSettings.TempDataFolderName}";
            string temptestfilepath = string.Empty;
            if (rawDataList.Count > 1)
            {
                temptestfilepath = CreateTestFile(rawDataList, temproot, newName);
                testRecord.TestFilePath = $@"{root}\{GlobalSettings.TestDataFolderName}\{Path.GetFileName(temptestfilepath)}";
                CopyToServerWithRetry(temptestfilepath, testRecord.TestFilePath);
            }
            else
            {
                if (isRename)
                {
                    temptestfilepath = RenameRawDataAndCopyToFolder(rawDataList[0], temproot, newName);
                    testRecord.TestFilePath = $@"{root}\{Path.GetFileName(temptestfilepath)}";
                    CopyToServerWithRetry(temptestfilepath, testRecord.TestFilePath);
                }
                else
                {
                    temptestfilepath = CopyToFolder(rawDataList[0], temproot);
                    testRecord.TestFilePath = $@"{root}\{GlobalSettings.TestDataFolderName}\{Path.GetFileName(temptestfilepath)}";
                    CopyToServerWithRetry(temptestfilepath, testRecord.TestFilePath);
                }
            }
            if (testRecord.TestFilePath == "")
            {
                MessageBox.Show("Test File Path Empty!");
                return;
            }
            SuperUpdate(testRecord);
        }

        internal void Detach(TestRecord record)
        {
            Items.Remove(record);
        }

        internal void UpdateFreeTestRecord(TestRecord testRecord, bool isRename, string newName, string batteryType, string projectName, string programName, string recipeName)
        {
            testRecord.ProjectStr = projectName;
            testRecord.ProgramStr = programName;
            testRecord.RecipeStr = recipeName;
            string root = $@"{GlobalSettings.RootPath}{batteryType}\{projectName}";
            string temproot = $@"{GlobalSettings.LocalFolder}{batteryType}\{projectName}";
            string temptestfilepath = string.Empty;
            if (isRename)
            {
                var oldPath = Path.Combine($@"{GlobalSettings.LocalFolder}{GlobalSettings.TempDataFolderName}", Path.GetFileName(testRecord.TestFilePath));
                temptestfilepath = RenameRawDataAndCopyToFolder(oldPath, $@"{temproot}\{GlobalSettings.TestDataFolderName}", Path.GetFileNameWithoutExtension(newName));
                testRecord.TestFilePath = $@"{root}\{GlobalSettings.TestDataFolderName}\{Path.GetFileName(temptestfilepath)}";
                CopyToServerWithRetry(temptestfilepath, testRecord.TestFilePath);
            }
            else
            {
                temptestfilepath = CopyToFolder(testRecord.TestFilePath, temproot);
                testRecord.TestFilePath = $@"{root}\{GlobalSettings.TestDataFolderName}\{Path.GetFileName(temptestfilepath)}";
                CopyToServerWithRetry(temptestfilepath, testRecord.TestFilePath);
            }
            if (testRecord.TestFilePath == "")
            {
                MessageBox.Show("Test File Path Empty!");
                return;
            }
            SuperUpdate(testRecord);
        }

        internal string DataPreProcess(TestRecord record, List<string> rawDataList, bool isRename, string newName, int startIndex, DateTime st, DateTime et, string batteryType, string projectName, Program program, Recipe recipe, ITesterProcesser processer, bool isSkipDP)
        {
            string root = $@"{GlobalSettings.RootPath}{batteryType}\{projectName}";
            string temproot = $@"{GlobalSettings.LocalFolder}{batteryType}\{projectName}";
            string temptestfilepath = string.Empty;
            if (rawDataList.Count > 1)
            {
                temptestfilepath = CreateTestFile(rawDataList, temproot, newName);
            }
            else
            {
                if (isRename)
                {
                    temptestfilepath = RenameRawDataAndCopyToFolder(rawDataList[0], $@"{temproot}\{GlobalSettings.TestDataFolderName}", newName);
                }
                else
                {
                    temptestfilepath = CopyToFolder(rawDataList[0], temproot);
                }
            }
            TesterServiceClass ts = new TesterServiceClass();
            //#if !Test

            if (!isSkipDP)
            {
                if (!ts.DataPreprocessing(processer, temptestfilepath, program, recipe, record, startIndex))
                {
                    File.Delete(temptestfilepath);
                    return string.Empty;
                }
            }
            //#endif
            var TestFilePath = $@"{root}\{GlobalSettings.TestDataFolderName}\{Path.GetFileName(temptestfilepath)}";
            CopyToServerWithRetry(temptestfilepath, TestFilePath);
            if (TestFilePath == "")
            {
                MessageBox.Show("Test File Path Empty!");
                return string.Empty;
            }
            return TestFilePath;
        }

        internal void CommitV2(TestRecord testRecord, string comment, string filePath, DateTime startTime, DateTime completeTime)
        {
            testRecord.Comment = comment;
            testRecord.StartTime = startTime;
            testRecord.EndTime = completeTime;
            testRecord.AssignedBattery = null;
            testRecord.AssignedChamber = null;
            testRecord.AssignedChannel = null;
            testRecord.Status = TestStatus.Completed;
            testRecord.TestFilePath = filePath;
            SuperUpdate(testRecord);
        }
    }
}
