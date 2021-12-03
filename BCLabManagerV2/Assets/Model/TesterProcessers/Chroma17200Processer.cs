using BCLabManager.Model.Chroma17200;
using MathNet.Numerics;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BCLabManager.Model
{
    public class Chroma17200Processer : ITesterProcesser
    {
        Dictionary<Column, double> StepTolerance = new Dictionary<Column, double>();
        Dictionary<Column, double> ContinuityTolerance = new Dictionary<Column, double>();
        public Chroma17200Processer()
        {
            StepTolerance.Add(Column.CURRENT, 10);        //mA
            StepTolerance.Add(Column.VOLTAGE, 50);          //mV
            StepTolerance.Add(Column.TEMPERATURE, 3.5);
            StepTolerance.Add(Column.TIME, 3);          //S
        }

        public DateTime[] GetTimeFromRawData(ObservableCollection<string> fileList)
        {
            DateTime[] output = new DateTime[2];
            List<DateTime> StartTimes = new List<DateTime>();
            List<DateTime> EndTimes = new List<DateTime>();
            foreach (var fn in fileList)
            {
                DateTime[] timepair = GetTimesFromFile(fn);
                StartTimes.Add(timepair[0]);
                EndTimes.Add(timepair[1]);
            }
            output[0] = GetEarliest(StartTimes);
            output[1] = GetLatest(EndTimes);
            return output;
        }

        private DateTime[] GetTimesFromFile(string fn)
        {
            DateTime[] output = new DateTime[2];
            FileStream fs = new FileStream(fn, FileMode.Open);
            StreamReader sw = new StreamReader(fs);
            sw.ReadLine();
            sw.ReadLine();
            string startTimeLine = sw.ReadLine();
            string startTimeStr = startTimeLine.Substring(17, 19);
            output[0] = DateTime.Parse(startTimeStr);
            string endTimeLine = sw.ReadLine();
            string endTimeStr = endTimeLine.Substring(15, 19);
            output[1] = DateTime.Parse(endTimeStr);
            sw.Close();
            fs.Close();
            return output;
        }

        private DateTime GetEarliest(List<DateTime> startTimes)
        {
            return startTimes.Min();
        }

        private DateTime GetLatest(List<DateTime> endTimes)
        {
            return endTimes.Max();
        }

        public bool CheckChannelNumber(string filepath, string channelnumber)
        {
            return GetChannelName(filepath) == channelnumber;
        }

        public string GetChannelName(string filepath)
        {
            string output = "Ch";
            FileStream fs = new FileStream(filepath, FileMode.Open);
            StreamReader sw = new StreamReader(fs);
            sw.ReadLine();
            string channelNumberLine = sw.ReadLine();
            string channelNumberStr = channelNumberLine.Substring(15, 1);
            output += channelNumberStr;
            sw.Close();
            fs.Close();
            return output;
        }

        public bool CheckFileFormat(string filepath)
        {
            if (Path.GetExtension(filepath) != ".csv")
                return false;
            try
            {
                FileStream fs = new FileStream(filepath, FileMode.Open);
                StreamReader sw = new StreamReader(fs);
                int i = 0;
                for (; i < 9; i++)
                    sw.ReadLine();
                string columnLine = sw.ReadLine();
                var columnList = "Step No.,Step,DWell Time(ms),TEST TIME,Cycle,Loop,Step Mode,Mode,Current(A),Voltage(V),Capacity(Ah),Total Capacity(Ah),Status".Split(',');
                i = 0;
                foreach (var column in columnLine.Split(','))
                {
                    if (column.StartsWith("Param"))
                        continue;
                    if (column != columnList[i++])
                    {
                        sw.Close();
                        fs.Close();
                        return false;
                    }
                }

                sw.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\n" +
                    $"{e.InnerException}\n");
                return false;
            }
            return true;
        }

        public bool DataPreprocessing(string filepath, Program program, Recipe recipe, TestRecord record, int startStepIndex, uint options)
        {
            UInt32 result = ErrorCode.NORMAL;
            List<Event> events = new List<Event>();
            FileStream fs = new FileStream(filepath, FileMode.Open);
            var tempFilePath = Path.Combine(System.Environment.CurrentDirectory, "temp.txt");
            FileStream tempFileStream = new FileStream(tempFilePath, FileMode.Create);
            StreamReader sr = new StreamReader(fs);
            StreamWriter sw = new StreamWriter(tempFileStream);
            //bool IsFirstDischarge = false;
            //bool IsFirstDischargeChecked = false;
            int lineIndex = 0;
            int startTime = 0;
            int timeSpan = 0;
            //string dataLine0 = string.Empty;
            //string dataLine1 = string.Empty;

            StepV2 step0 = new StepV2();
            StepV2 step1 = new StepV2();

            //Dictionary<Column, string> row0 = new Dictionary<Column, string>();
            //Dictionary<Column, string> row1 = new Dictionary<Column, string>();
            List<Dictionary<Column, string>> buffer = new List<Dictionary<Column, string>>();
            DataPreprocesser.Index = 0;
            DataPreprocesser.IsFirstDischarge = false;
            DataPreprocesser.IsFirstDischargeChecked = false;
            try
            {
                bool isCOCPoint = false;
                var fullSteps = new List<StepV2>(recipe.RecipeTemplate.GetNormalSteps(recipe.Program.Project).OrderBy(o => o.Index));
                for (; lineIndex < 10; lineIndex++)     //第十行以后都是数据
                {
                    sw.WriteLine(sr.ReadLine());
                }
                DataPreprocesser.Length = 20;
                for (int i = 0; i < DataPreprocesser.Length; i++)
                {
                    DataPreprocesser.NewLine = sr.ReadLine();
                }
                //dataLine1 = sr.ReadLine();
                //row1 = GetRowFromString(dataLine1);
                //ActionMode am = GetActionMode(row1[Column.STEP_MODE]);
                ActionMode am = DataPreprocesser.Nodes[DataPreprocesser.Index].StepMode;
                if (am == ActionMode.CC_DISCHARGE || am == ActionMode.CP_DISCHARGE)
                    DataPreprocesser.IsFirstDischarge = true;
                if (startStepIndex != 0)
                {
                    step1 = fullSteps.SingleOrDefault(o => o.Index == startStepIndex);
                }
                else
                    step1 = fullSteps.First(o => o.Action.Mode == am);  //有隐患
                result = DataPreprocesser.StepStartPointCheck(step1, recipe.Temperature, options);
                if (result != ErrorCode.NORMAL)
                {
                    if (!DataPreprocesser.ErrorHandler(ref events, ref result, filepath, program, recipe, record, lineIndex))
                    {
                        return false;
                    }
                }
                sw.WriteLine(DataPreprocesser.StringList[DataPreprocesser.Index]);

                //startTime = Convert.ToInt32(row1[Column.TIME_MS]);
                startTime = DataPreprocesser.Nodes[DataPreprocesser.Index].TimeInMS;
                lineIndex = 11;
                while (true)
                {
                    lineIndex++;
                    //dataLine0 = dataLine1;
                    //row0 = row1;
                    //dataLine1 = sr.ReadLine();
                    if (DataPreprocesser.Index < DataPreprocesser.Length / 2)
                    {
                        DataPreprocesser.Index++;
                    }
                    else
                    {
                        var newLine = sr.ReadLine();
                        if (newLine == null)
                            newLine = null;
                        DataPreprocesser.NewLine = newLine;
                    }
                    if (DataPreprocesser.StringList[DataPreprocesser.Index + 1] == null)  //文件结束
                    {
                        DataPreprocesser.Index++;
                        //timeSpan = (Convert.ToInt32(row0[Column.TIME_MS]) - startTime) / 1000;
                        timeSpan = (DataPreprocesser.Nodes[DataPreprocesser.Index - 1].TimeInMS - startTime) / 1000;
                        //result = StepCOCPointCheck(step1, row0, recipe.Temperature, timeSpan);
                        result = DataPreprocesser.StepCOCPointCheck(step1, recipe.Temperature, timeSpan);      //Index?
                        if (result != ErrorCode.NORMAL)
                        {
                            if (!DataPreprocesser.ErrorHandler(ref events, ref result, filepath, program, recipe, record, lineIndex))
                            {
                                return false;
                            }
                        }
                        step0 = step1;
                        step1 = GetNewTargetStep(step0, fullSteps, DataPreprocesser.Nodes[DataPreprocesser.Index - 1].Status, recipe.Temperature, timeSpan);
                        if (step1 != null)
                        {
                            result = ErrorCode.DP_STEP_MISSING;
                            if (!DataPreprocesser.ErrorHandler(ref events, ref result, filepath, program, recipe, record, lineIndex, step1.Index))
                            {
                                return false;
                            }
                        }
                        sw.WriteLine(DataPreprocesser.StringList[DataPreprocesser.Index - 1]);      //Issue 2655
                        //throw new ProcessException($@"Step {step1.Index} is missing");
                        break;
                    }
                    //row1 = GetRowFromString(dataLine1);
                    if (DataPreprocesser.Nodes[DataPreprocesser.Index].Status == StepEndString.EndByError)
                    {
                        result = ErrorCode.DP_CHECKSUM;
                        if (!DataPreprocesser.ErrorHandler(ref events, ref result, filepath, program, recipe, record, lineIndex))
                        {
                            return false;
                        }
                    }
                    //if (row1[Column.MODE] == "0")
                    if (DataPreprocesser.Nodes[DataPreprocesser.Index].Mode == 0)
                    {
                        isCOCPoint = true;
                    }
                    if (isCOCPoint)
                    {
                        #region COC Point Check
                        //timeSpan = (Convert.ToInt32(row0[Column.TIME_MS]) - startTime) / 1000;
                        timeSpan = (DataPreprocesser.Nodes[DataPreprocesser.Index - 1].TimeInMS - startTime) / 1000;
                        //result = StepCOCPointCheck(step1, row0, recipe.Temperature, timeSpan);
                        result = DataPreprocesser.StepCOCPointCheck(step1, recipe.Temperature, timeSpan);      //Index?
                        if (result != ErrorCode.NORMAL)
                        {
                            if (!DataPreprocesser.ErrorHandler(ref events, ref result, filepath, program, recipe, record, lineIndex))
                            {
                                return false;
                            }
                        }
                        step0 = step1;
                        //step1 = GetNewTargetStep(step0, fullSteps, row0, recipe.Temperature, timeSpan);
                        step1 = GetNewTargetStep(step0, fullSteps, DataPreprocesser.Nodes[DataPreprocesser.Index - 1].Status, recipe.Temperature, timeSpan);
                        if (step1 == null)
                        {
                            result = ErrorCode.DP_NO_NEXT_STEP;
                            if (!DataPreprocesser.ErrorHandler(ref events, ref result, filepath, program, recipe, record, lineIndex))
                            {
                                return false;
                            }
                        }

                        //result = StepActionModeCheck(step1.Action.Mode, GetActionMode(row1[Column.STEP_MODE]));
                        result = StepActionModeCheck(step1.Action.Mode, DataPreprocesser.Nodes[DataPreprocesser.Index].StepMode);
                        if (result != ErrorCode.NORMAL)
                        {
                            if (!DataPreprocesser.ErrorHandler(ref events, ref result, filepath, program, recipe, record, lineIndex))
                            {
                                return false;
                            }
                        }
                        if (!DataPreprocesser.IsFirstDischargeChecked)
                        {
                            if (step1.Action.Mode == ActionMode.CC_DISCHARGE || step1.Action.Mode == ActionMode.CP_DISCHARGE)
                                DataPreprocesser.IsFirstDischarge = true;
                        }
                        isCOCPoint = false;
                        #endregion
                        //isStartPoint = true;
                        #region Start Point Check
                        //result = StepStartPointCheck(step1, row1, recipe.Temperature, IsFirstDischarge, ref IsFirstDischargeChecked);
                        result = DataPreprocesser.StepStartPointCheck(step1, recipe.Temperature, options);
                        if (result != ErrorCode.NORMAL)
                        {
                            if (!DataPreprocesser.ErrorHandler(ref events, ref result, filepath, program, recipe, record, lineIndex))
                            {
                                return false;
                            }
                        }
                        //startTime = Convert.ToInt32(row1[Column.TIME_MS]);
                        startTime = DataPreprocesser.Nodes[DataPreprocesser.Index].TimeInMS;
                        #endregion
                    }
                    else
                    {
                        #region Normal Point Check
                        //result = StepMidPointCheck(step1, row1, recipe.Temperature);
                        result = DataPreprocesser.StepMidPointCheck(step1);
                        if (result != ErrorCode.NORMAL)
                        {
                            if (!DataPreprocesser.ErrorHandler(ref events, ref result, filepath, program, recipe, record, lineIndex))
                            {
                                return false;
                            }
                        }



                        #region Continuity Check

                        //if (row1[Column.MODE] != "0")
                        if (DataPreprocesser.Nodes[DataPreprocesser.Index].Mode != 0)
                        {
                            //Dictionary<Column, bool> rowContinuityMatrix = GetContinuityMatrix(row0, row1);
                            //result = ContinuityCheck(rowContinuityMatrix);
                            result = DataPreprocesser.ContinuityCheck(program.Type, step1);
                            if (result != ErrorCode.NORMAL)
                            {
                                if (!DataPreprocesser.ErrorHandler(ref events, ref result, filepath, program, recipe, record, lineIndex))
                                {
                                    return false;
                                }
                            }
                            //row1 = GetRowFromString(dataLine1); //有可能dataLine1更新了，需要再一次更新row1
                        }
                        #endregion
                        #endregion
                    }

                    sw.WriteLine(DataPreprocesser.StringList[DataPreprocesser.Index]);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                result = ErrorCode.UNDEFINED;
            }
            finally
            {
                sr.Close();
                sw.Close();
                fs.Close();
                if (result == ErrorCode.NORMAL)
                {
                    File.Delete(filepath);

                    try
                    {
                        File.Copy(tempFilePath, filepath, true);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    foreach (var evt in events)
                    {
                        EventService.SuperAdd(evt);
                    }
                }
                else
                {
                    File.Delete(tempFilePath);
                }
            }
            if (result == ErrorCode.NORMAL)
                return true;
            else
                return false;
        }

        private StepV2 GetNewTargetStep(StepV2 currentStep, List<StepV2> fullSteps, string status, double temperature, int timeSpan)
        {
            StepV2 nextStep = null;
            CutOffBehavior cob = null;
            switch (status)
            {
                case "StepFinishByCut_V":
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.VOLTAGE);
                    break;
                case "StepFinishByCut_I":
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.CURRENT);
                    break;
                case "StepFinishByCut_T":
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME);
                    break;
            }
            if (cob != null)
                nextStep = Jump(cob, fullSteps, currentStep.Index);
            return nextStep;
        }

        private static StepV2 Jump(CutOffBehavior cob, List<StepV2> fullSteps, int currentStepIndex)
        {
            StepV2 nextStep = null;
            if (cob.JumpBehaviors.Count == 1)
            {
                var jpb = cob.JumpBehaviors.First();
                switch (jpb.JumpType)
                {
                    case JumpType.INDEX:
                        nextStep = fullSteps.SingleOrDefault(o => o.Index == jpb.Index);
                        break;
                    case JumpType.END:
                        break;
                    case JumpType.NEXT:
                        nextStep = fullSteps.SingleOrDefault(o => o.Index == currentStepIndex + 1);
                        break;
                    case JumpType.LOOP:
                        throw new NotImplementedException();
                }
            }
            else if (cob.JumpBehaviors.Count > 1)
            {
                JumpBehavior validJPB = null;
                foreach (var jpb in cob.JumpBehaviors)
                {
                    bool isConditionMet = false;
                    double leftvalue = 0;
                    double rightvalue = jpb.Condition.Value;
                    switch (jpb.Condition.Parameter)
                    {
                        case Parameter.CURRENT: leftvalue = DataPreprocesser.Nodes[DataPreprocesser.Index].Current; break;
                        case Parameter.POWER: leftvalue = DataPreprocesser.Nodes[DataPreprocesser.Index].Current * DataPreprocesser.Nodes[DataPreprocesser.Index].Voltage; break;
                        case Parameter.TEMPERATURE: leftvalue = DataPreprocesser.Nodes[DataPreprocesser.Index].Temperature; break;
                        case Parameter.TIME: leftvalue = DataPreprocesser.Nodes[DataPreprocesser.Index].TimeInMS / 1000; break;
                        case Parameter.VOLTAGE: leftvalue = DataPreprocesser.Nodes[DataPreprocesser.Index].Voltage; break;
                    }
                    switch (jpb.Condition.Mark)
                    {
                        case CompareMarkEnum.EqualTo:
                            if (leftvalue == rightvalue)
                                isConditionMet = true;
                            break;
                        case CompareMarkEnum.LargerThan:
                            if (leftvalue > rightvalue)
                                isConditionMet = true;
                            break;
                        case CompareMarkEnum.SmallerThan:
                            if (leftvalue < rightvalue)
                                isConditionMet = true;
                            break;
                    }
                    if (isConditionMet)
                    {
                        validJPB = jpb;
                        break;
                    }
                }
                if (validJPB != null)
                {
                    switch (validJPB.JumpType)
                    {
                        case JumpType.INDEX:
                            nextStep = fullSteps.SingleOrDefault(o => o.Index == validJPB.Index);
                            break;
                        case JumpType.END:
                            break;
                        case JumpType.NEXT:
                            nextStep = fullSteps.SingleOrDefault(o => o.Index == currentStepIndex + 1);
                            break;
                        case JumpType.LOOP:
                            throw new NotImplementedException();
                    }
                }
            }
            return nextStep;
        }
#if false
        private static StepV2 Compare(CutOffCondition coc, double value, double tolerance, List<StepV2> fullSteps, int currentStepIndex)
        {
            StepV2 nextStep = null;
            //switch (coc.Mark)
            //{
            //    case CompareMarkEnum.SmallerThan:
            if (Math.Abs(value - coc.Value) <= tolerance)
            {
                nextStep = Jump(coc, fullSteps, currentStepIndex);
            }
            //        break;
            //    case CompareMarkEnum.LargerThan:
            //        if (Math.Abs(value - coc.Value) <= StepTolerance.Current)
            //        {
            //            nextStep = Jump(coc, fullSteps, currentStepIndex);
            //        }
            //        break;
            //}
            return nextStep;
        }
#endif
        private UInt32 StepActionModeCheck(ActionMode mode, ActionMode actionMode)
        {
            if (mode != actionMode)
                //throw new ProcessException("Action mode mismatch");
                return ErrorCode.DP_ACTION_MODE_MISMATCH;
            else
                return ErrorCode.NORMAL;
        }

        public StandardRow ConvertToStdRow(uint index, string rawRow)
        {
            var node = DataPreprocesser.GetNodeFromeString(rawRow);
            if (node == null)
                return null;
            StandardRow row = new StandardRow();
            row.Index = index;
            row.TimeInMS = (uint)node.TimeInMS;
            row.Mode = node.StepMode;
            row.Current = node.Current * 1000;
            row.Voltage = node.Voltage * 1000;
            row.Temperature = node.Temperature;
            row.Capacity = node.Capacity * 1000;
            row.TotalCapacity = node.TotalCapacity * 1000;
            row.Status = ConvertStatus(node.Status);
            return row;
        }

        private RowStatus ConvertStatus(string status)
        {
            switch (status)
            {
                case StepEndString.Running: return RowStatus.RUNNING;
                case StepEndString.EndByCurrent: return RowStatus.CUT_OFF_BY_CURRENT;
                case StepEndString.EndByTime: return RowStatus.CUT_OFF_BY_TIME;
                case StepEndString.EndByVoltage: return RowStatus.CUT_OFF_BY_VOLTAGE;
                case StepEndString.EndByError: return RowStatus.CUT_OFF_BY_ERROR;
                default: return RowStatus.UNKNOWN;
            }
        }
    }
    namespace Chroma17200
    {
        public static class DataPreprocesser
        {
            public static uint Length { get; set; } = 0;
            public static int Index { get; set; } = 0;
            public static List<string> StringList { get; set; } = new List<string>();
            public static List<Dictionary<Column, string>> DicList { get; set; } = new List<Dictionary<Column, string>>();  //尽量淘汰
            public static List<ChromaNode> Nodes { get; set; } = new List<ChromaNode>();
            public static string NewLine
            {
                set
                {
                    if (Length == 0)
                    {
                        MessageBox.Show("Please Set Length first.");
                        return;
                    }
                    StringList.Add(value);
                    if (StringList.Count > Length)
                    {
                        StringList.RemoveAt(0);
                    }
                    var dic = GetRowFromString(value);
                    DicList.Add(dic);
                    if (DicList.Count > Length)
                    {
                        DicList.RemoveAt(0);
                    }
                    var node = GetNodeFromeString(value);
                    Nodes.Add(node);
                    if (Nodes.Count > Length)
                    {
                        Nodes.RemoveAt(0);
                    }
                }
            }
            public static bool IsFirstDischarge { get; set; }
            public static bool IsFirstDischargeChecked { get; set; }


            public static ChromaNode GetNodeFromeString(string value)
            {
                if (value != null)
                {
                    ChromaNode node = new ChromaNode();
                    var strRow = value.Split(',');
                    if (strRow.Length != 14)
                        return null;
                    try
                    {
                        node.StepNo = Convert.ToInt32(strRow[(int)Column.STEPNO]);
                        node.Step = Convert.ToInt32(strRow[(int)Column.STEP]);
                        node.TimeInMS = Convert.ToInt32(strRow[(int)Column.TIME_MS]);
                        node.Time = DateTime.Parse(strRow[(int)Column.TIME]);
                        node.Cycle = Convert.ToByte(strRow[(int)Column.CYCLE]);
                        node.Loop = Convert.ToByte(strRow[(int)Column.LOOP]);
                        node.StepMode = GetActionMode(strRow[(int)Column.STEP_MODE]);
                        node.Mode = Convert.ToByte(strRow[(int)Column.MODE]);
                        node.Current = Convert.ToDouble(strRow[(int)Column.CURRENT]);
                        node.Voltage = Convert.ToDouble(strRow[(int)Column.VOLTAGE]);
                        node.Temperature = Convert.ToDouble(strRow[(int)Column.TEMPERATURE]);
                        node.Capacity = Convert.ToDouble(strRow[(int)Column.CAPACITY]);
                        node.TotalCapacity = Convert.ToDouble(strRow[(int)Column.TOTAL_CAPACITY]);
                        node.Status = strRow[(int)Column.STATUS];
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show($"Row Parsing Failed. Error Message is:\n{e.Message}");
                        return null;
                    }
                    return node;
                }
                return null;
            }
            private static ActionMode GetActionMode(string v)
            {
                switch (v)
                {
                    case "CC_CV_Charge": return ActionMode.CC_CV_CHARGE;
                    case "CC_Discharge": return ActionMode.CC_DISCHARGE;
                    case "CP_Discharge": return ActionMode.CP_DISCHARGE;
                    case "Rest": return ActionMode.REST;
                    default: return ActionMode.NA;
                }
            }
            private static string ConvertActionModeToString(ActionMode v)
            {
                switch (v)
                {
                    case ActionMode.CC_CV_CHARGE: return "CC_CV_Charge";
                    case ActionMode.CC_DISCHARGE: return "CC_Discharge";
                    case ActionMode.CP_DISCHARGE: return "CP_Discharge";
                    case ActionMode.REST: return "Rest";
                    default: return string.Empty;
                }
            }

            private static Dictionary<Column, string> GetRowFromString(string dataLine)
            {
                if (dataLine != null)
                {
                    Dictionary<Column, string> output = new Dictionary<Column, string>();
                    var strRow = dataLine.Split(',');
                    for (int i = 0; i < 14; i++)
                        output.Add((Column)i, strRow[i]);
                    return output;
                }
                return null;
            }


            public static UInt32 StepStartPointCheck(StepV2 step, double temp, uint options)
            {
                if (DataPreprocesser.Nodes[DataPreprocesser.Index].Mode != 0)
                {
                    return ErrorCode.DP_UNDEFINED;
                }
                double voltage = 0;
                double current = 0;
                double power = 0;
                double temperature = 0;
                switch (step.Action.Mode)
                {
                    case ActionMode.CC_CV_CHARGE:
                        //voltage = Convert.ToDouble(row1[Column.VOLTAGE]) * 1000;
                        voltage = Nodes[Index].Voltage * 1000;
                        if (Math.Abs(voltage - step.Action.Voltage) > StepTolerance.Voltage)
                        {
                            //current = GetCurrentFromRow(row1) * -1;
                            current = Nodes[Index].Current * 1000;
                            if (Math.Abs(current - step.Action.Current) > StepTolerance.Current)
                                return ErrorCode.DP_CURRENT_OUT_OF_RANGE;
                        }
                        break;
                    case ActionMode.CC_DISCHARGE:

                        current = Nodes[Index].Current * -1000;
                        if (Math.Abs(current - step.Action.Current) > StepTolerance.Current)
                            return ErrorCode.DP_CURRENT_OUT_OF_RANGE;

                        if (IsFirstDischarge && !IsFirstDischargeChecked)
                        {
                            if ((options & TesterProcesserOptions.SkipTemperatureCheck) == 0)
                            {
                                temperature = Nodes[Index].Temperature;
                                if (Math.Abs(temperature - temp) > StepTolerance.Temperature)
                                    return ErrorCode.DP_TEMPERATURE_OUT_OF_RANGE;
                            }

                            IsFirstDischargeChecked = true;
                        }
                        break;
                    case ActionMode.CP_DISCHARGE:

                        //power = GetPowerFromRow(row1) * 1000;             //mW
                        var timeSpan = (Nodes[Index].TimeInMS - Nodes[Index - 1].TimeInMS);
                        if (timeSpan > 950)
                        {
                            power = Math.Abs(Nodes[Index].Current * Nodes[Index].Voltage) * 1000;   //mW
                        }
                        else
                        {
                            power = Math.Abs(Nodes[Index + 1].Current * Nodes[Index + 1].Voltage) * 1000;   //mW
                        }
                        if (Math.Abs(power - step.Action.Power) > StepTolerance.Power)
                            return ErrorCode.DP_POWER_OUT_OF_RANGE;

                        if (IsFirstDischarge && !IsFirstDischargeChecked)
                        {
                            if ((options & TesterProcesserOptions.SkipTemperatureCheck) == 0)
                            {
                                temperature = Nodes[Index].Temperature;
                                if (Math.Abs(temperature - temp) > StepTolerance.Temperature)
                                    return ErrorCode.DP_TEMPERATURE_OUT_OF_RANGE;
                            }

                            IsFirstDischargeChecked = true;
                        }
                        break;
                    case ActionMode.REST:
                        break;
                    default:
                        break;
                }
                return ErrorCode.NORMAL;
            }
            private static string GetChromaEventInfo(int lineIndex, string message, string solution = "", string oldLine = "", string userComment = "")
            {
                string str = $"Line: {lineIndex}\nProblem: {message}\nSolution: {solution}\n";
                int i = 0;
                foreach (string s in StringList)
                {
                    if (i++ == Index)
                        str += $"{s}\t*\n";
                    else
                        str += $"{s}\n";
                }
                str += $"Old Line: {oldLine}\nUser Commnet: {userComment}";
                return str;
            }

            private static double GetCurrentFromRow(Dictionary<Column, string> row1)
            {
                return Convert.ToDouble(row1[Column.CURRENT]) * -1000.0;
            }

            private static double GetPowerFromRow(Dictionary<Column, string> row1)
            {
                var current = Convert.ToDouble(row1[Column.CURRENT]) * -1.0;
                var voltage = Convert.ToDouble(row1[Column.VOLTAGE]);
                return current * voltage;
            }

            public static UInt32 StepMidPointCheck(StepV2 step)
            {
                double current = 0;
                switch (step.Action.Mode)
                {
                    case ActionMode.CC_CV_CHARGE:
                        break;
                    case ActionMode.CC_DISCHARGE:
                        current = Nodes[Index].Current * -1000;
                        if (Math.Abs(current - step.Action.Current) > StepTolerance.Current)
                            //throw new ProcessException("Current Out Of Range");
                            return ErrorCode.DP_CURRENT_OUT_OF_RANGE;
                        break;
                    case ActionMode.CP_DISCHARGE:
                        //var power = GetPowerFromRow(row1) * 1000;             //mW
                        var power = Math.Abs(Nodes[Index].Current * Nodes[Index].Voltage) * 1000;       //mW
                        if (Math.Abs(power - step.Action.Power) > StepTolerance.Power)
                            return ErrorCode.DP_POWER_OUT_OF_RANGE;
                        break;
                    case ActionMode.REST:
                        break;
                    default:
                        break;
                }
                return ErrorCode.NORMAL;
            }

            public static UInt32 StepCOCPointCheck(StepV2 step, double temp, int timeSpan)
            {
                double voltage = 0;
                double current = 0;
                //double temperature = 0;
                switch (step.Action.Mode)
                {
                    case ActionMode.CC_CV_CHARGE:
                        //if (Nodes[Index].Status == "StepFinishByCut_I")
                        if (Nodes[Index - 1].Status == StepEndString.EndByCurrent)
                        {
                            if (!step.CutOffBehaviors.Any(o => o.Condition.Parameter == Parameter.CURRENT))
                                return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                            current = Nodes[Index - 1].Current * 1000;
                            if (Math.Abs(current - step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.CURRENT).Condition.Value) > StepTolerance.Current)
                                return ErrorCode.DP_CURRENT_OUT_OF_RANGE;
                            voltage = Nodes[Index - 1].Voltage * 1000;
                            if (Math.Abs(voltage - step.Action.Voltage) > StepTolerance.Voltage)
                                return ErrorCode.DP_VOLTAGE_OUT_OF_RANGE;
                        }
                        else if (Nodes[Index - 1].Status == StepEndString.EndByTime)     //DST测试也会设定充电时间
                        {
                            if (step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME) == null)
                                return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                            if (Math.Abs(timeSpan - step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME).Condition.Value) > StepTolerance.Time)
                                return ErrorCode.DP_TIME_OUT_OF_RANGE;
                        }
                        else
                        {
                            return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                        }
                        break;
                    case ActionMode.CC_DISCHARGE:
                        if (Nodes[Index - 1].Status == StepEndString.EndByVoltage)
                        {
                            if (step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.VOLTAGE) == null)
                                return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                            voltage = Nodes[Index - 1].Voltage * 1000;
                            if (Math.Abs(voltage - step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.VOLTAGE).Condition.Value) > StepTolerance.Voltage)
                                return ErrorCode.DP_VOLTAGE_OUT_OF_RANGE;
                        }
                        else if (Nodes[Index - 1].Status == StepEndString.EndByTime)
                        {
                            if (step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME) == null)
                                return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                            if (Math.Abs(timeSpan - step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME).Condition.Value) > StepTolerance.Time)
                                return ErrorCode.DP_TIME_OUT_OF_RANGE;
                        }
                        else
                        {
                            return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                        }
                        break;
                    case ActionMode.CP_DISCHARGE:
                        if (Nodes[Index - 1].Status == StepEndString.EndByVoltage)
                        {
                            if (step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.VOLTAGE) == null)
                                return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                            voltage = Nodes[Index - 1].Voltage * 1000;
                            if (Math.Abs(voltage - step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.VOLTAGE).Condition.Value) > StepTolerance.Voltage)
                                return ErrorCode.DP_VOLTAGE_OUT_OF_RANGE;
                        }
                        else if (Nodes[Index - 1].Status == StepEndString.EndByTime)
                        {
                            if (step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME) == null)
                                return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                            if (Math.Abs(timeSpan - step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME).Condition.Value) > StepTolerance.Time)
                                return ErrorCode.DP_TIME_OUT_OF_RANGE;
                        }
                        else
                        {
                            return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                        }
                        break;
                    case ActionMode.REST:
                        if (Nodes[Index - 1].Status == StepEndString.EndByTime)
                        {
                            if (step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME) == null)
                                return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                            if (Math.Abs(timeSpan - step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME).Condition.Value) > StepTolerance.Time)
                                return ErrorCode.DP_TIME_OUT_OF_RANGE;
                        }
                        else
                        {
                            return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                        }
                        break;
                    default:
                        break;
                }
                return ErrorCode.NORMAL;
            }

            //    ContinuityTolerance.Add(Column.STEPNO, 1);
            //    ContinuityTolerance.Add(Column.STEP, 1);
            //    ContinuityTolerance.Add(Column.TIME_MS, 1000);
            //    ContinuityTolerance.Add(Column.TIME, 1);
            //    ContinuityTolerance.Add(Column.CYCLE, 1);
            //    ContinuityTolerance.Add(Column.LOOP, 1);
            //    ContinuityTolerance.Add(Column.MODE, 1);
            //    ContinuityTolerance.Add(Column.CURRENT, 0.1);   //A
            //    ContinuityTolerance.Add(Column.VOLTAGE, 0.1);
            //    ContinuityTolerance.Add(Column.TEMPERATURE, 1);
            //    ContinuityTolerance.Add(Column.CAPACITY, 0.5);
            //    ContinuityTolerance.Add(Column.TOTAL_CAPACITY, 0.5);        
            //enum Column
            //{
            //    STEPNO,     //配方的Index，1234234234
            //    STEP,       //真实的顺序，1234...n
            //    TIME_MS,    //100，200，300
            //    TIME,       //2020-07-24 08:55:31
            //    CYCLE,      //外层循环？
            //    LOOP,       //内层循环？
            //    STEP_MODE,       //Rest, CC_CV_Charge, CC_Discharge
            //    MODE,            //Step改变时为0，否则为1
            //    CURRENT,
            //    VOLTAGE,
            //    TEMPERATURE,
            //    CAPACITY,       //单个Step的电量
            //    TOTAL_CAPACITY,  //
            //    STATUS              //StepFinishByCut_V,StepFinishByCut_T,StepFinishByCut_I
            //}
            public static UInt32 ContinuityCheck(ProgramType type, StepV2 step)
            {
                double diff;

                if (Nodes[Index].StepNo == 0)
                    return ErrorCode.DP_STEP_NO_JUMP;

                //diff = (Convert.ToDouble(row1[Column.STEP]) - Convert.ToDouble(row0[Column.STEP]));
                diff = Nodes[Index].Step - Nodes[Index - 1].Step;
                if (diff < 0 || diff > 1)
                    return ErrorCode.DP_STEP_JUMP;

                //diff = (Convert.ToDouble(row1[Column.TIME_MS]) - Convert.ToDouble(row0[Column.TIME_MS]));
                diff = Nodes[Index].TimeInMS - Nodes[Index - 1].TimeInMS;
                if (diff < 0 || diff > 1000)
                    return ErrorCode.DP_TIME_JUMP;

                //DateTime t0 = DateTime.Parse(row0[Column.TIME]);
                //DateTime t1 = DateTime.Parse(row1[Column.TIME]);
                //diff = (t1 - t0).TotalSeconds;
                diff = (Nodes[Index].Time - Nodes[Index - 1].Time).TotalSeconds;
                if (diff < 0 || diff > 1)
                    return ErrorCode.DP_TIME_JUMP;

                //if (Math.Abs(Convert.ToDouble(row1[Column.CYCLE]) - Convert.ToDouble(row0[Column.CYCLE])) > 1)
                if (Math.Abs(Nodes[Index].Cycle - Nodes[Index - 1].Cycle) > 1)
                    return ErrorCode.DP_CYCLE_JUMP;

                //if (Math.Abs(Convert.ToDouble(row1[Column.LOOP]) - Convert.ToDouble(row0[Column.LOOP])) > 1)
                if (Math.Abs(Nodes[Index].Loop - Nodes[Index - 1].Loop) > 1)
                    return ErrorCode.DP_LOOP_JUMP;

                //output.Add(Column.STEP_MODE, true);

                //if (Math.Abs(Convert.ToDouble(row1[Column.MODE]) - Convert.ToDouble(row0[Column.MODE])) > 1)
                if (Math.Abs(Nodes[Index].Mode - Nodes[Index - 1].Mode) > 1)
                    return ErrorCode.DP_MODE_JUMP;


                //output.Add(Column.CURRENT, true);       //目前不挑错

                //if (Math.Abs(Convert.ToDouble(row1[Column.VOLTAGE]) - Convert.ToDouble(row0[Column.VOLTAGE])) > 0.1)
                //    output.Add(Column.VOLTAGE, false);
                //else
                //    output.Add(Column.VOLTAGE, true);
                if (type.Name == "RC" || type.Name == "OCV")
                {
                    if (step.Action.Mode == ActionMode.CC_DISCHARGE)
                    {
                        if ((Nodes[Index].Voltage - Nodes[Index - 1].Voltage) > 0.005)       //RC或OCV实验的放电过程中，电压回弹超过5mV，则报错
                            return ErrorCode.DP_DISCHARGE_VOLTAGE_JUMP_IN_RC_OCV;
                    }
                }

                //output.Add(Column.TEMPERATURE, true);       //目前不挑错

                //output.Add(Column.CAPACITY, true);       //目前不挑错

                //output.Add(Column.TOTAL_CAPACITY, true);       //目前不挑错

                if (Nodes[Index].Status == StepEndString.EndByError)
                    return ErrorCode.DP_CHECKSUM;

                return ErrorCode.NORMAL;
            }
            private static string BuildStringFromRow(Dictionary<Column, string> row)
            {
                string output = string.Empty;
                foreach (var key in row.Keys)
                {
                    output += row[key] + ",";
                }
                return output.Substring(0, output.Length - 1);
            }
            public static bool ErrorHandler(ref List<Event> events, ref uint result, string filepath, Program program, Recipe recipe, TestRecord record, int lineIndex, params object[] args)
            {
                Event evt = new Event();
                string info = string.Empty;
                string solution = string.Empty;
                string oldLine1 = null;
                if (StringList[Index] != null)
                    oldLine1 = StringList[Index];
                //string newLine1 = string.Empty;
                bool ret;
                //Dictionary<Column, string> row0 = new Dictionary<Column, string>();
                //Dictionary<Column, string> row1 = new Dictionary<Column, string>();
                switch (result)
                {
                    case ErrorCode.DP_CHECKSUM:
                        if (Nodes.Where(o => o != null).Where(o => o.Status == StepEndString.EndByError).Count() > 1)
                        {
                            MessageBox.Show("Unhandled!");
                            ret = false;
                            break;
                        }
                        RestoreCheckSumError();
                        //MessageBox.Show("Unfinished yet.");
                        solution = "Auto Fixed";
                        ret = true;
                        result = ErrorCode.NORMAL;
                        break;
                    default:
                        var str = $"{ErrorCode.GetDescription(result, args)}\n";
                        foreach (var s in StringList)
                        {
                            str += $"{s}\n";
                        }
                        str += $"Continue to commit?";
                        var userRet = MessageBox.Show(str, "Data Preprocessing Error", MessageBoxButton.YesNo);
                        if (userRet == MessageBoxResult.Yes)
                        {
                            solution = "Ignored";
                            ret = true;
                            result = ErrorCode.NORMAL;
                        }
                        else
                            ret = false;
                        break;
                }
                if (ret)
                {
                    evt.Module = Module.DataPreprocessor;
                    evt.Type = EventType.Warning;
                    evt.Timestamp = DateTime.Now;
                    info = GetChromaEventInfo(lineIndex, ErrorCode.GetDescription(result, args), solution, oldLine1);
                    evt.Description = EventDescriptor(filepath, program, recipe, record, info);
                    events.Add(evt);
                }
                return ret;
            }

            private static void RestoreCheckSumError()
            {
                //throw new NotImplementedException();
                RestoreTimeInMs();
                RestoreTime();
                RestorePhysicalColumns();
                RestoreUnchangedColumns();
                UpdateStringList();
            }

            private static void UpdateStringList()
            {
                StringList[Index] = $"{Nodes[Index].StepNo},{Nodes[Index].Step},{Nodes[Index].TimeInMS},{Nodes[Index].Time.ToString("yyyy-MM-dd HH:mm:ss")},{Nodes[Index].Cycle},{Nodes[Index].Loop},{ConvertActionModeToString(Nodes[Index].StepMode)},{Nodes[Index].Mode},{Nodes[Index].Current},{Nodes[Index].Voltage},{Nodes[Index].Temperature},{Nodes[Index].Capacity},{Nodes[Index].TotalCapacity},{Nodes[Index].Status}";
            }

            private static void RestorePhysicalColumns()
            {
                double[] xdata;
                double[] ydata;
                Tuple<double, double> p;
                double b, a;

                var nodes = Nodes.Select(o => o).Where(o => o != null).ToList();

                int startTime = nodes.First().TimeInMS;
                int endTime = nodes.Last().TimeInMS;
                for (int i = nodes.Count() - 1; i >= 0; i--)
                {
                    if (nodes[i].StepMode != nodes[Index].StepMode)
                    {
                        if (i > Index)
                            endTime = nodes[i - 1].TimeInMS;
                        else if (i < Index)
                        {
                            startTime = nodes[i + 1].TimeInMS;
                            break;
                        }
                    }
                }
                var nodesClip = nodes.Where(o => o.Status != StepEndString.EndByError && o.TimeInMS >= startTime && o.TimeInMS <= endTime);
                xdata = nodesClip.Select(o => Convert.ToDouble(o.TimeInMS)).ToArray();
                ydata = nodesClip.Select(o => o.Current).ToArray();
                p = Fit.Line(xdata, ydata);
                b = p.Item1; // == 10; intercept
                a = p.Item2; // == 0.5; slope
                double newCurrent = a * nodes[Index].TimeInMS + b;
                nodes[Index].Current = Math.Round(newCurrent, 4);


                ydata = nodesClip.Select(o => o.Voltage).ToArray();
                p = Fit.Line(xdata, ydata);
                b = p.Item1; // == 10; intercept
                a = p.Item2; // == 0.5; slope
                double newVoltage = a * nodes[Index].TimeInMS + b;
                nodes[Index].Voltage = Math.Round(newVoltage, 4);

                ydata = nodesClip.Select(o => o.Temperature).ToArray();
                p = Fit.Line(xdata, ydata);
                b = p.Item1; // == 10; intercept
                a = p.Item2; // == 0.5; slope
                double newTemperature = a * nodes[Index].TimeInMS + b;
                nodes[Index].Temperature = Math.Round(newTemperature, 4);

                ydata = nodesClip.Select(o => o.Capacity).ToArray();
                p = Fit.Line(xdata, ydata);
                b = p.Item1; // == 10; intercept
                a = p.Item2; // == 0.5; slope
                double newCapacity = a * nodes[Index].TimeInMS + b;
                nodes[Index].Capacity = Math.Round(newCapacity, 4);

                ydata = nodesClip.Select(o => o.TotalCapacity).ToArray();
                p = Fit.Line(xdata, ydata);
                b = p.Item1; // == 10; intercept
                a = p.Item2; // == 0.5; slope
                double newTotalCapacity = a * nodes[Index].TimeInMS + b;
                nodes[Index].TotalCapacity = Math.Round(newTotalCapacity, 4);
            }

            private static void RestoreUnchangedColumns()
            {
                if (Nodes[Index - 1].StepNo == Nodes[Index + 1].StepNo)
                {
                    if (Nodes[Index - 1].StepNo != Nodes[Index].StepNo)
                        Nodes[Index].StepNo = Nodes[Index - 1].StepNo;
                }
                else
                {
                    ;//未处理
                }
                if (Nodes[Index - 1].Step == Nodes[Index + 1].Step)
                {
                    if (Nodes[Index - 1].Step != Nodes[Index].Step)
                        Nodes[Index].Step = Nodes[Index - 1].Step;
                }
                else
                {
                    ;//未处理
                }
                if (Nodes[Index - 1].Cycle == Nodes[Index + 1].Cycle)
                {
                    if (Nodes[Index - 1].Cycle != Nodes[Index].Cycle)
                        Nodes[Index].Cycle = Nodes[Index - 1].Cycle;
                }
                else
                {
                    ;//未处理
                }
                if (Nodes[Index - 1].Loop == Nodes[Index + 1].Loop)
                {
                    if (Nodes[Index - 1].Loop != Nodes[Index].Loop)
                        Nodes[Index].Loop = Nodes[Index - 1].Loop;
                }
                else
                {
                    ;//未处理
                }
                if (Nodes[Index - 1].StepMode == Nodes[Index + 1].StepMode)
                {
                    if (Nodes[Index - 1].StepMode != Nodes[Index].StepMode)
                        Nodes[Index].StepMode = Nodes[Index - 1].StepMode;
                }
                else
                {
                    ;//未处理
                }
                if (Nodes[Index - 1].Mode == Nodes[Index + 1].Mode)
                {
                    if (Nodes[Index - 1].Mode != Nodes[Index].Mode)
                        Nodes[Index].Mode = Nodes[Index - 1].Mode;
                }
                else
                {
                    ;//未处理
                }
                if (Nodes[Index - 1].Status == Nodes[Index + 1].Status)
                {
                    if (Nodes[Index - 1].Status != Nodes[Index].Status)
                        Nodes[Index].Status = Nodes[Index - 1].Status;
                }
                else
                {
                    ;//未处理
                }
            }

            private static void RestoreTimeInMs()
            {
                List<int> Diffs = new List<int>();
                var nodes = Nodes.Select(o => o).Where(o => o != null).ToList();
                for (int i = 1; i < nodes.Count; i++)
                {
                    //if (nodes[i - 1] == null)
                    //    break;
                    if (i == Index || i == Index + 1)
                        continue;
                    Diffs.Add(nodes[i].TimeInMS - nodes[i - 1].TimeInMS);
                }
                var diff = Diffs.GroupBy(o => o).Max(o => o.Key);
                nodes[Index].TimeInMS = nodes[Index - 1].TimeInMS + diff;
            }

            private static void RestoreTime()
            {
                Nodes[Index].Time = Nodes[Index - 1].Time + TimeSpan.FromMilliseconds(Nodes[Index].TimeInMS - Nodes[Index - 1].TimeInMS);
            }

            private static string EventDescriptor(string filepath, Program program, Recipe recipe, TestRecord record, string info)
            {
                return $"Battery Type: {program.Project.BatteryType.Name}\n" +
                    $"Project: {program.Project.Name}\n" +
                    $"Program: {program.Name}\n" +
                    $"Recipe: {recipe.Name}\n" +
                    //$"Test Record ID: {record.Id}\n" +
                    //$"Battery: {record.BatteryStr}\n" +
                    //$"Tester: {record.TesterStr}\n" +
                    //$"Channel: {record.ChannelStr}\n" +
                    //$"Chamber: {record.ChamberStr}\n" +
                    $"File Path: {record.TestFilePath}\n" +
                    //$"Operator: {record.Operator}\n" +
                    $"{info}";
            }
        }

        public enum Column
        {
            STEPNO,     //配方的Index，1234234234
            STEP,       //真实的顺序，1234...n
            TIME_MS,    //100，200，300
            TIME,       //2020-07-24 08:55:31
            CYCLE,      //外层循环？
            LOOP,       //内层循环？
            STEP_MODE,       //Rest, CC_CV_Charge, CC_Discharge
            MODE,            //Step改变时为0，否则为1
            CURRENT,
            VOLTAGE,
            TEMPERATURE,
            CAPACITY,       //单个Step的电量
            TOTAL_CAPACITY,  //
            STATUS              //StepFinishByCut_V,StepFinishByCut_T,StepFinishByCut_I
        }
        public class ChromaNode
        {
            public Int32 StepNo { get; set; }
            public Int32 Step { get; set; }
            public int TimeInMS { get; set; }
            public DateTime Time { get; set; }
            public byte Cycle { get; set; }
            public byte Loop { get; set; }
            public ActionMode StepMode { get; set; }
            public byte Mode { get; set; }
            public double Current { get; set; }
            public double Voltage { get; set; }
            public double Temperature { get; set; }
            public double Capacity { get; set; }
            public double TotalCapacity { get; set; }
            public string Status { get; set; }
        }
        public static class StepTolerance
        {
            public static double Current { get { return 10; } } //mA
            public static double Temperature { get { return 3.5; } }    //deg
            public static double Voltage { get { return 5; } } //mV
            public static double Power { get { return 100; } }
            public static double Time { get { return 3; } }     //S
        }
        public static class StepEndString
        {
            public const string EndByCurrent = "StepFinishByCut_I";
            public const string EndByVoltage = "StepFinishByCut_V";
            public const string EndByPower = "";
            public const string EndByTemperature = "";
            public const string EndByTime = "StepFinishByCut_T";
            public const string EndByError = "Warring_CheckSum";
            public const string Running = "StepRunning";
        }
    }
}
