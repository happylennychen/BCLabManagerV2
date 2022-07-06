using BCLabManager.Model.Chroma17208Auto;
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
    public class Chroma17208AutoProcesser : ITesterProcesser
    {
        Dictionary<Column, double> StepTolerance = new Dictionary<Column, double>();
        Dictionary<Column, double> ContinuityTolerance = new Dictionary<Column, double>();
        public Chroma17208AutoProcesser()
        {
            StepTolerance.Add(Column.CURRENT, 10);        //mA
            StepTolerance.Add(Column.VOLTAGE, 50);          //mV
            StepTolerance.Add(Column.TEMPERATURE, 3.5);
            StepTolerance.Add(Column.TIME_MS, 3);          //S
        }
        public DateTime[] GetTimeFromRawData(ObservableCollection<string> fileList) //只有一个文件，时间放在文件名中 Chroma17208M-Ch1-20220630160748-20220630180629.csv
        {
            DateTime[] output = new DateTime[2];
            var strSections = fileList[0].Split('-', '.');
            if (DateTime.TryParseExact(strSections[2], "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out output[0]))
                if (DateTime.TryParseExact(strSections[3], "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out output[1]))
                    return output;

            return null;
        }

        public bool CheckChannelNumber(string filepath, string channelnumber)
        {
            return GetChannelName(filepath) == channelnumber;
        }

        public string GetChannelName(string filepath)
        {
            var strSections = filepath.Split('-');
            return strSections[1];
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
                string columnLine = sw.ReadLine();
                var columnList = "Index,Time(mS),Mode,Current(mA),Voltage(mV),Temperature(degC),Capacity(mAh),Total Capacity(mAh),Status".Split(',');
                foreach (var column in columnLine.Split(','))
                {
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
            double startTime = 0;
            double timeSpan = 0;
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
            DataPreprocesser.Nodes.Clear();
            DataPreprocesser.StringList.Clear();
            DataPreprocesser.DicList.Clear();
            try
            {
                bool isCOCPoint = false;
                var fullSteps = new List<StepV2>(recipe.RecipeTemplate.GetNormalSteps(recipe.Program.Project).OrderBy(o => o.Index));
                //for (; lineIndex < 10; lineIndex++)     //第十行以后都是数据
                //{
                //    sw.WriteLine(sr.ReadLine());
                //}
                sw.WriteLine(sr.ReadLine());        //第一行是header
                DataPreprocesser.Length = 20;
                for (int i = 0; i < DataPreprocesser.Length; i++)
                {
                    DataPreprocesser.NewLine = sr.ReadLine();
                }
                //dataLine1 = sr.ReadLine();
                //row1 = GetRowFromString(dataLine1);
                //ActionMode am = GetActionMode(row1[Column.STEP_MODE]);
                ActionMode am = DataPreprocesser.Nodes[DataPreprocesser.Index].Mode;
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
                        timeSpan = (DataPreprocesser.Nodes[DataPreprocesser.Index - 1].TimeInMS - startTime);
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
                        step1 = GetNewTargetStep(step0, fullSteps, DataPreprocesser.Nodes[DataPreprocesser.Index - 1], recipe.Temperature, timeSpan);
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

                    //if (DataPreprocesser.Nodes[DataPreprocesser.Index].Status == StepEndString.EndByError)
                    //{
                    //    result = ErrorCode.DP_CHECKSUM;
                    //    if (!DataPreprocesser.ErrorHandler(ref events, ref result, filepath, program, recipe, record, lineIndex))
                    //    {
                    //        return false;
                    //    }
                    //}
                    //if (row1[Column.MODE] == "0")
                    if (DataPreprocesser.IsStepFirstLine)
                    {
                        isCOCPoint = true;
                    }
                    if (isCOCPoint)
                    {
                        #region COC Point Check
                        //timeSpan = (Convert.ToInt32(row0[Column.TIME_MS]) - startTime) / 1000;
                        timeSpan = (DataPreprocesser.Nodes[DataPreprocesser.Index - 1].TimeInMS - startTime);
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
                        step1 = GetNewTargetStep(step0, fullSteps, DataPreprocesser.Nodes[DataPreprocesser.Index - 1], recipe.Temperature, timeSpan);
                        if (step1 == null)
                        {
                            result = ErrorCode.DP_NO_NEXT_STEP;
                            if (!DataPreprocesser.ErrorHandler(ref events, ref result, filepath, program, recipe, record, lineIndex))
                            {
                                return false;
                            }
                        }

                        //result = StepActionModeCheck(step1.Action.Mode, GetActionMode(row1[Column.STEP_MODE]));
                        result = StepActionModeCheck(step1.Action.Mode, DataPreprocesser.Nodes[DataPreprocesser.Index].Mode);
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
                        if (!DataPreprocesser.IsStepFirstLine)
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

        private StepV2 GetNewTargetStep(StepV2 currentStep, List<StepV2> fullSteps, StandardRow row, double temperature, double timeSpan)
        {
            StepV2 nextStep = null;
            CutOffBehavior cob = null;
            switch (currentStep.Action.Mode)
            {
                case ActionMode.REST:// "StepFinishByCut_V":
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME);
                    break;
                case ActionMode.CC_CV_CHARGE://"StepFinishByCut_I":
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME);
                    if (cob != null)
                    {
                        var time = cob.Condition.Value;
                        Console.WriteLine($"time = {time}");
                        Console.WriteLine($"timeSpan = {timeSpan}");
                        if (Math.Abs(timeSpan / 1000 - time) < 1)
                        {
                            //Console.WriteLine($"Meet time condition.");
                            break;
                        }
                        else
                            cob = null;
                    }
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.CURRENT);
                    break;
                case ActionMode.CC_DISCHARGE://"StepFinishByCut_T":
                case ActionMode.CP_DISCHARGE:
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME);
                    if (cob != null)
                    {
                        var time = cob.Condition.Value;
                        //Console.WriteLine($"time = {time}");
                        //Console.WriteLine($"timeSpan = {timeSpan}");
                        if (Math.Abs(timeSpan / 1000 - time) < 1)
                        {
                            //Console.WriteLine($"Meet time condition.");
                            break;
                        }
                        else
                            cob = null;
                    }
                    cob = currentStep.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.VOLTAGE);
                    if (cob != null)
                    {
                        var volt = cob.Condition.Value;
                        //Console.WriteLine($"volt = {volt}");
                        //Console.WriteLine($"row.Voltage = {row.Voltage}");
                        if (Math.Abs(row.Voltage * 1000 - volt) < 15)
                        {
                            //Console.WriteLine($"Meet voltage condition.");
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"Doesn't meet voltage condition.");
                            cob = null;
                        }
                    }
                    break;
            }
            if (cob != null)
                nextStep = Jump(cob, fullSteps, currentStep.Index, row);
            return nextStep;
        }

        private static StepV2 Jump(CutOffBehavior cob, List<StepV2> fullSteps, int currentStepIndex, StandardRow row)
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
                        case Parameter.CURRENT: leftvalue = row.Current; break;
                        case Parameter.POWER: leftvalue = row.Current * row.Voltage; break;
                        case Parameter.TEMPERATURE: leftvalue = row.Temperature; break;
                        case Parameter.TIME: leftvalue = row.TimeInMS / 1000; break;
                        case Parameter.VOLTAGE: leftvalue = row.Voltage; break;
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
            StandardRow row = new StandardRow(rawRow);
            return row;
        }
        public double GetDischargeCapacityFromRawData(ObservableCollection<string> fileList)
        {
            double output = 0;
            List<double> buffer = new List<double>();
            foreach (var file in fileList)
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                var oldNode = DataPreprocesser.GetNodeFromeString(sr.ReadLine());
                while (true)
                {
                    var line = sr.ReadLine();
                    if (line == null)
                    {
                        if (oldNode != null)
                        {
                            if (oldNode.Mode == ActionMode.CC_DISCHARGE || oldNode.Mode == ActionMode.CP_DISCHARGE)
                            {
                                output += oldNode.Capacity;
                            }
                        }
                        break;
                    }
                    var newNode = DataPreprocesser.GetNodeFromeString(line);
                    if (newNode != null)
                    {
                        if (oldNode != null)
                        {
                            if (newNode.Mode != oldNode.Mode)
                            {
                                if (newNode.Mode == ActionMode.CC_CV_CHARGE)
                                {
                                    if (output != 0)
                                    {
                                        buffer.Add(output);
                                        output = 0;
                                    }
                                }
                                if (oldNode.Mode == ActionMode.CC_DISCHARGE || oldNode.Mode == ActionMode.CP_DISCHARGE)
                                    output += oldNode.Capacity;
                            }
                        }
                        oldNode = newNode;
                    }
                }
                sr.Close();
                fs.Close();
            }
            if (output == 0 && buffer.Count > 0)    //充电在放电之后
                output = buffer.Last();
            return output * -1000;
        }
    }
    namespace Chroma17208Auto
    {
        public static class DataPreprocesser
        {
            public static uint Length { get; set; } = 0;
            public static int Index { get; set; } = 0;
            public static List<string> StringList { get; set; } = new List<string>();
            public static List<Dictionary<Column, string>> DicList { get; set; } = new List<Dictionary<Column, string>>();  //尽量淘汰
            public static List<StandardRow> Nodes { get; set; } = new List<StandardRow>();
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
            public static bool IsStepFirstLine
            {
                get
                {
                    if (Index == 0)
                        return true;
                    return Nodes[Index].Status != Nodes[Index - 1].Status && Nodes[Index - 1].Status != Nodes[Index - 2].Status;
                }
            }
            private static byte voltageRaisingErrorCounter = 0;


            public static StandardRow GetNodeFromeString(string value)
            {
                if (value != null)
                {
                    StandardRow node = new StandardRow(value);
                    return node;
                }
                return null;
            }
            private static ActionMode GetActionMode(string v)
            {
                switch (v)
                {
                    case "CC-CV Charge": return ActionMode.CC_CV_CHARGE;
                    case "CC Discharge": return ActionMode.CC_DISCHARGE;
                    case "CP Discharge": return ActionMode.CP_DISCHARGE;
                    case "REST": return ActionMode.REST;
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
                    for (int i = 0; i < 9; i++)
                        output.Add((Column)i, strRow[i]);
                    return output;
                }
                return null;
            }


            public static UInt32 StepStartPointCheck(StepV2 step, double temp, uint options)
            {
                if (!DataPreprocesser.IsStepFirstLine)
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
                        if (timeSpan > 0.95)
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

            public static UInt32 StepCOCPointCheck(StepV2 step, double temp, double timeSpan)
            {
                double voltage = 0;
                double current = 0;
                int targetTimeInMS = 0;
                //double temperature = 0;
                switch (step.Action.Mode)
                {
                    case ActionMode.CC_CV_CHARGE:
                        //if (Nodes[Index].Status == "StepFinishByCut_I")
                        if (Nodes[Index - 1].Status == RowStatus.CUT_OFF_BY_CURRENT)
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
                        else if (Nodes[Index - 1].Status == RowStatus.CUT_OFF_BY_TIME)     //DST测试也会设定充电时间
                        {
                            if (step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME) == null)
                                return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                            targetTimeInMS = step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME).Condition.Value * 1000;
                            if (Math.Abs(timeSpan - targetTimeInMS) > StepTolerance.Time)
                                return ErrorCode.DP_TIME_OUT_OF_RANGE;
                        }
                        else
                        {
                            return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                        }
                        break;
                    case ActionMode.CC_DISCHARGE:
                        if (Nodes[Index - 1].Status == RowStatus.CUT_OFF_BY_VOLTAGE)
                        {
                            if (step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.VOLTAGE) == null)
                                return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                            voltage = Nodes[Index - 1].Voltage * 1000;
                            if (Math.Abs(voltage - step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.VOLTAGE).Condition.Value) > StepTolerance.Voltage)
                                return ErrorCode.DP_VOLTAGE_OUT_OF_RANGE;
                        }
                        else if (Nodes[Index - 1].Status == RowStatus.CUT_OFF_BY_TIME)
                        {
                            if (step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME) == null)
                                return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                            targetTimeInMS = step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME).Condition.Value * 1000;
                            if (Math.Abs(timeSpan - targetTimeInMS) > StepTolerance.Time)
                                return ErrorCode.DP_TIME_OUT_OF_RANGE;
                        }
                        else
                        {
                            return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                        }
                        break;
                    case ActionMode.CP_DISCHARGE:
                        if (Nodes[Index - 1].Status == RowStatus.CUT_OFF_BY_VOLTAGE)
                        {
                            if (step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.VOLTAGE) == null)
                                return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                            voltage = Nodes[Index - 1].Voltage * 1000;
                            if (Math.Abs(voltage - step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.VOLTAGE).Condition.Value) > StepTolerance.Voltage)
                                return ErrorCode.DP_VOLTAGE_OUT_OF_RANGE;
                        }
                        else if (Nodes[Index - 1].Status == RowStatus.CUT_OFF_BY_TIME)
                        {
                            if (step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME) == null)
                                return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                            targetTimeInMS = step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME).Condition.Value * 1000;
                            if (Math.Abs(timeSpan - targetTimeInMS) > StepTolerance.Time)
                                return ErrorCode.DP_TIME_OUT_OF_RANGE;
                        }
                        else
                        {
                            return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                        }
                        break;
                    case ActionMode.REST:
                        if (Nodes[Index - 1].Status == RowStatus.CUT_OFF_BY_TIME)
                        {
                            if (step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME) == null)
                                return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                            targetTimeInMS = step.CutOffBehaviors.SingleOrDefault(o => o.Condition.Parameter == Parameter.TIME).Condition.Value * 1000;
                            if (Math.Abs(timeSpan - targetTimeInMS) > StepTolerance.Time)
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

                if (Nodes[Index].Index == 0)
                    return ErrorCode.DP_STEP_NO_JUMP;

                //diff = (Convert.ToDouble(row1[Column.STEP]) - Convert.ToDouble(row0[Column.STEP]));
                diff = Nodes[Index].Index - Nodes[Index - 1].Index;
                if (diff < 0 || diff > 1)
                    return ErrorCode.DP_STEP_JUMP;

                //diff = (Convert.ToDouble(row1[Column.TIME_MS]) - Convert.ToDouble(row0[Column.TIME_MS]));
                diff = Nodes[Index].TimeInMS - Nodes[Index - 1].TimeInMS;
                if (diff < 0 || diff > 1250)    //给smart tester大一点裕度
                    return ErrorCode.DP_TIME_JUMP;



                if (type.Name == "RC" || type.Name == "OCV")
                {
                    if (step.Action.Mode == ActionMode.CC_DISCHARGE)
                    {
                        if ((Nodes[Index].Voltage - Nodes[Index - 1].Voltage) > 0.005)       //RC或OCV实验的放电过程中，电压回弹超过5mV，则报错
                        {
                            voltageRaisingErrorCounter++;
                            if (voltageRaisingErrorCounter >= 6)
                                return ErrorCode.DP_DISCHARGE_VOLTAGE_JUMP_IN_RC_OCV;
                        }
                        else
                        {
                            voltageRaisingErrorCounter = 0;
                        }
                    }
                }

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
            INDEX,
            TIME_MS,
            MODE,
            CURRENT,
            VOLTAGE,
            TEMPERATURE,
            CAPACITY,
            TOTAL_CAPACITY,
            STATUS,              //0,8
        }
        public static class StepTolerance
        {
            public static double Current { get { return 10; } } //mA
            public static double Temperature { get { return 3.5; } }    //deg
            public static double Voltage { get { return 5; } } //mV
            public static double Power { get { return 100; } }
            public static double Time { get { return 3000; } }     //mS
        }
    }
}
