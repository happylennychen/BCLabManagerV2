using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
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

        enum Column
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
            return true;
        }

        public bool DataPreprocessing(string filepath, Program program, Recipe recipe, TestRecord record, int startIndex)
        {
            UInt32 result = ErrorCode.NORMAL;
            List<Event> events = new List<Event>();
            FileStream fs = new FileStream(filepath, FileMode.Open);
            var tempFilePath = Path.Combine(System.Environment.CurrentDirectory, "temp.txt");
            FileStream tempFileStream = new FileStream(tempFilePath, FileMode.Create);
            StreamReader sr = new StreamReader(fs);
            StreamWriter sw = new StreamWriter(tempFileStream);
            bool isFirstDischarge = false;
            bool isFirstDischargeChecked = false;
            int lineIndex = 0;
            int startTime = 0;
            int timeSpan = 0;
            string dataLine0 = string.Empty;
            string dataLine1 = string.Empty;

            StepV2 step0 = new StepV2();
            StepV2 step1 = new StepV2();

            Dictionary<Column, string> row0 = new Dictionary<Column, string>();
            Dictionary<Column, string> row1 = new Dictionary<Column, string>();
            try
            {
                bool isCOCPoint = false;
                var fullSteps = new List<StepV2>(recipe.RecipeTemplate.StepV2s.OrderBy(o => o.Index));
                for (; lineIndex < 10; lineIndex++)     //第十行以后都是数据
                {
                    sw.WriteLine(sr.ReadLine());
                }
                dataLine1 = sr.ReadLine();
                row1 = GetRowFromString(dataLine1);
                ActionMode am = GetActionMode(row1[Column.STEP_MODE]);
                if (am == ActionMode.CC_DISCHARGE || am == ActionMode.CP_DISCHARGE)
                    isFirstDischarge = true;
                if (startIndex != 0)
                {
                    step1 = fullSteps.SingleOrDefault(o => o.Index == startIndex);
                }
                else
                    step1 = fullSteps.First(o => o.Action.Mode == am);  //有隐患
                result = StepStartPointCheck(step1, row1, recipe.Temperature, isFirstDischarge, ref isFirstDischargeChecked);
                if (result != ErrorCode.NORMAL)
                {
                    if (!ErrorHandler(ref events, result, filepath, program, recipe, record, lineIndex, dataLine0, ref dataLine1))
                    {
                        return false;
                    }
                }
                sw.WriteLine(dataLine1);

                startTime = Convert.ToInt32(row1[Column.TIME_MS]);
                lineIndex = 11;
                while (true)
                {
                    lineIndex++;
                    dataLine0 = dataLine1;
                    row0 = row1;
                    dataLine1 = sr.ReadLine();
                    if (dataLine1 == null)
                    {
                        timeSpan = (Convert.ToInt32(row0[Column.TIME_MS]) - startTime) / 1000;
                        result = StepCOCPointCheck(step1, row0, recipe.Temperature, timeSpan);
                        if (result != ErrorCode.NORMAL)
                        {
                            if (!ErrorHandler(ref events, result, filepath, program, recipe, record, lineIndex, dataLine0, ref dataLine1))
                            {
                                return false;
                            }
                        }
                        step0 = step1;
                        step1 = GetNewTargetStep(step0, fullSteps, row0, recipe.Temperature, timeSpan);
                        if (step1 != null)
                        {
                            result = ErrorCode.DP_STEP_MISSING;
                            if (result != ErrorCode.NORMAL)
                            {
                                if (!ErrorHandler(ref events, result, filepath, program, recipe, record, lineIndex, dataLine0, ref dataLine1, step1.Index))
                                {
                                    return false;
                                }
                            }
                        }
                        //throw new ProcessException($@"Step {step1.Index} is missing");
                        break;
                    }
                    row1 = GetRowFromString(dataLine1);

                    if (row1[Column.MODE] == "0")
                    {
                        isCOCPoint = true;
                    }
                    if (isCOCPoint)
                    {
                        #region COC Point Check
                        timeSpan = (Convert.ToInt32(row0[Column.TIME_MS]) - startTime) / 1000;
                        result = StepCOCPointCheck(step1, row0, recipe.Temperature, timeSpan);
                        if (result != ErrorCode.NORMAL)
                        {
                            if (!ErrorHandler(ref events, result, filepath, program, recipe, record, lineIndex, dataLine0, ref dataLine1))
                            {
                                return false;
                            }
                        }
                        step0 = step1;
                        step1 = GetNewTargetStep(step0, fullSteps, row0, recipe.Temperature, timeSpan);
                        if (step1 == null)
                        //throw new ProcessException("Cannot Get Next Step.");
                        {
                            result = ErrorCode.DP_NO_NEXT_STEP;
                            if (result != ErrorCode.NORMAL)
                            {
                                if (!ErrorHandler(ref events, result, filepath, program, recipe, record, lineIndex, dataLine0, ref dataLine1))
                                {
                                    return false;
                                }
                            }
                        }
                        result = StepActionModeCheck(step1.Action.Mode, GetActionMode(row1[Column.STEP_MODE]));
                        if (result != ErrorCode.NORMAL)
                        {
                            if (!ErrorHandler(ref events, result, filepath, program, recipe, record, lineIndex, dataLine0, ref dataLine1))
                            {
                                return false;
                            }
                        }
                        if (!isFirstDischargeChecked)
                        {
                            if (step1.Action.Mode == ActionMode.CC_DISCHARGE || step1.Action.Mode == ActionMode.CP_DISCHARGE)
                                isFirstDischarge = true;
                        }
                        isCOCPoint = false;
                        #endregion
                        //isStartPoint = true;
                        #region Start Point Check
                        result = StepStartPointCheck(step1, row1, recipe.Temperature, isFirstDischarge, ref isFirstDischargeChecked);
                        if (result != ErrorCode.NORMAL)
                        {
                            if (!ErrorHandler(ref events, result, filepath, program, recipe, record, lineIndex, dataLine0, ref dataLine1))
                            {
                                return false;
                            }
                        }
                        startTime = Convert.ToInt32(row1[Column.TIME_MS]);
                        #endregion
                    }

                    //else if (isStartPoint)
                    //{
                    //    #region Start Point Check
                    //    StepStartPointCheck(step1, row1, recipe.Temperature, isFirstDischarge, ref isFirstDischargeChecked);
                    //    isStartPoint = false;
                    //    startTime = Convert.ToInt32(row1[Column.TIME_MS]);
                    //    #endregion
                    //}

                    else
                    {
                        #region Normal Point Check
                        result = StepMidPointCheck(step1, row1, recipe.Temperature);
                        if (result != ErrorCode.NORMAL)
                        {
                            if (!ErrorHandler(ref events, result, filepath, program, recipe, record, lineIndex, dataLine0, ref dataLine1))
                            {
                                return false;
                            }
                        }



                        #region Continuity Check

                        if (row1[Column.MODE] != "0")
                        {
                            Dictionary<Column, bool> rowContinuityMatrix = GetContinuityMatrix(row0, row1);
                            result = ContinuityCheck(rowContinuityMatrix);
                            if (result != ErrorCode.NORMAL)
                            {
                                if (!ErrorHandler(ref events, result, filepath, program, recipe, record, lineIndex, dataLine0, ref dataLine1))
                                {
                                    return false;
                                }
                            }
                            row1 = GetRowFromString(dataLine1); //有可能dataLine1更新了，需要再一次更新row1
                        }
                        #endregion
                        #endregion
                    }

                    sw.WriteLine(dataLine1);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                sr.Close();
                sw.Close();
                fs.Close();
                if (result == ErrorCode.NORMAL)
                {
                    File.Delete(filepath);
                    File.Move(tempFilePath, filepath);
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
            return true;
        }

        private bool ErrorHandler(ref List<Event> events, uint result, string filepath, Program program, Recipe recipe, TestRecord record, int lineIndex, string dataLine0, ref string dataLine1, params object[] args)
        {
            Event evt = new Event();
            string info = string.Empty;
            string solution = string.Empty;
            string oldLine1 = dataLine1;
            string newLine1 = string.Empty;
            bool ret;
            Dictionary<Column, string> row0 = new Dictionary<Column, string>();
            Dictionary<Column, string> row1 = new Dictionary<Column, string>();
            switch (result)
            {
                //case ErrorCode.DP_CHECKSUM_ABNORMAL_STEP_AND_TIME_JUMP:       //只有step和time不对，直接修复
                //    row0 = GetRowFromString(dataLine0);
                //    row1 = GetRowFromString(dataLine1);
                //    row1[Column.STEP] = row0[Column.STEP];
                //    row1[Column.TIME_MS] = (Convert.ToInt32(row0[Column.TIME_MS]) + 1000).ToString();
                //    row1[Column.TIME] = DateTime.Parse(row0[Column.TIME]).Add(TimeSpan.FromSeconds(1)).ToString("yyyy-MM-dd HH:mm:ss");
                //    row1[Column.STATUS] = row0[Column.STATUS];
                //    dataLine1 = BuildStringFromRow(row1);   //更新dataLine1
                //    newLine1 = dataLine1;
                //    solution = "Auto Fixed";
                //    ret = true;
                //    break;
                //case ErrorCode.DP_CHECKSUM_ABNORMAL_STEP_JUMP:
                //    row0 = GetRowFromString(dataLine0);
                //    row1 = GetRowFromString(dataLine1);
                //    row1[Column.STEP] = row0[Column.STEP];
                //    row1[Column.STATUS] = row0[Column.STATUS];
                //    dataLine1 = BuildStringFromRow(row1);   //更新dataLine1
                //    newLine1 = dataLine1;
                //    solution = "Auto Fixed";
                //    ret = true;
                //    break;
                default:
                    var str = $"{ErrorCode.GetDescription(result, args)}\n" +
                               $"-----------------------------" +
                               $"Line:{lineIndex - 1}\n" +
                               $"{dataLine0}\n" +
                               $"-----------------------------" +
                               $"Line:{lineIndex}\n" +
                               $"{dataLine1}\n" +
                               $"-----------------------------" +
                               $"Continue to commit?";
                    var userRet = MessageBox.Show(str, "Data Preprocessing Error", MessageBoxButton.YesNo);
                    if (userRet == MessageBoxResult.Yes)
                    {
                        solution = "Ignored";
                        ret = true;
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
                info = GetChromaEventInfo(lineIndex, ErrorCode.GetDescription(result, args), solution, dataLine0, oldLine1, newLine1);
                evt.Description = EventDescriptor(filepath, program, recipe, record, info);
                events.Add(evt);
            }
            return ret;
        }

        private string GetChromaEventInfo(int lineIndex, string message, string solution = "", string dataLine0 = "", string dataLine1 = "", string newLine1 = "", string userComment = "")
        {
            return $"Line: {lineIndex}\nProblem: {message}\nSolution: {solution}\nLine0: {dataLine0}\nLine1: {dataLine1}\nNew Line: {newLine1}\nUser Commnet: {userComment}";
        }

        private UInt32 StepActionModeCheck(ActionMode mode, ActionMode actionMode)
        {
            if (mode != actionMode)
                //throw new ProcessException("Action mode mismatch");
                return ErrorCode.DP_ACTION_MODE_MISMATCH;
            else
                return ErrorCode.NORMAL;
        }

        private StepV2 GetNewTargetStep(StepV2 currentStep, List<StepV2> fullSteps, Dictionary<Column, string> row, double temperature, int timeSpan)
        {
            StepV2 nextStep = null;
            //foreach (var coc in currentStep.CutOffConditions)
            //{
            //    double value = -999999;
            //    double tolerance = 0;
            //    switch (coc.Parameter)
            //    {
            //        case Parameter.VOLTAGE:
            //            value = Convert.ToDouble(row[Column.VOLTAGE]) * 1000;
            //            tolerance = StepTolerance[Column.VOLTAGE];
            //            break;
            //        case Parameter.CURRENT:
            //            value = Convert.ToDouble(row[Column.CURRENT]) * 1000;
            //            tolerance = StepTolerance[Column.CURRENT];
            //            break;
            //        case Parameter.TEMPERATURE:
            //            value = Convert.ToDouble(row[Column.TEMPERATURE]);
            //            tolerance = StepTolerance[Column.TEMPERATURE];
            //            break;
            //        case Parameter.TIME:
            //            value = timeSpan;
            //            tolerance = StepTolerance[Column.TIME];
            //            break;
            //        default:
            //            break;
            //    }
            //    if (value != -999999)
            //        nextStep = Compare(coc, value, tolerance, fullSteps, currentStep.Index);
            //    if (nextStep != null)
            //        return nextStep;
            //}
            CutOffCondition coc = null;
            switch (row[Column.STATUS])
            {
                case "StepFinishByCut_V":
                    coc = currentStep.CutOffConditions.SingleOrDefault(o => o.Parameter == Parameter.VOLTAGE);
                    break;
                case "StepFinishByCut_I":
                    coc = currentStep.CutOffConditions.SingleOrDefault(o => o.Parameter == Parameter.CURRENT);
                    break;
                case "StepFinishByCut_T":
                    coc = currentStep.CutOffConditions.SingleOrDefault(o => o.Parameter == Parameter.TIME);
                    break;
            }
            if (coc != null)
                nextStep = Jump(coc, fullSteps, currentStep.Index);
            return nextStep;
        }

        private StepV2 Compare(CutOffCondition coc, double value, double tolerance, List<StepV2> fullSteps, int currentStepIndex)
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
            //        if (Math.Abs(value - coc.Value) <= StepTolerance[Column.CURRENT])
            //        {
            //            nextStep = Jump(coc, fullSteps, currentStepIndex);
            //        }
            //        break;
            //}
            return nextStep;
        }

        private StepV2 Jump(CutOffCondition coc, List<StepV2> fullSteps, int currentStepIndex)
        {
            StepV2 nextStep = null;
            switch (coc.JumpType)
            {
                case JumpType.INDEX:
                    nextStep = fullSteps.SingleOrDefault(o => o.Index == coc.Index);
                    break;
                case JumpType.END:
                    break;
                case JumpType.NEXT:
                    nextStep = fullSteps.SingleOrDefault(o => o.Index == currentStepIndex + 1);
                    break;
                case JumpType.LOOP:
                    throw new NotImplementedException();
            }
            return nextStep;
        }

        private UInt32 StepStartPointCheck(StepV2 step, Dictionary<Column, string> row1, double temp, bool isFirstDischarge, ref bool isFirstDischargeChecked)
        {
            double voltage = 0;
            double current = 0;
            double power = 0;
            double temperature = 0;
            switch (step.Action.Mode)
            {
                case ActionMode.CC_CV_CHARGE:
                    voltage = Convert.ToDouble(row1[Column.VOLTAGE]) * 1000;
                    if (Math.Abs(voltage - step.Action.Voltage) > StepTolerance[Column.VOLTAGE])
                    {
                        current = GetCurrentFromRow(row1) * -1;
                        if (Math.Abs(current - step.Action.Current) > StepTolerance[Column.CURRENT])
                            return ErrorCode.DP_CURRENT_OUT_OF_RANGE;
                    }
                    break;
                case ActionMode.CC_DISCHARGE:

                    current = GetCurrentFromRow(row1);
                    if (Math.Abs(current - step.Action.Current) > StepTolerance[Column.CURRENT])
                        return ErrorCode.DP_CURRENT_OUT_OF_RANGE;

                    if (isFirstDischarge && !isFirstDischargeChecked)
                    {
                        temperature = Convert.ToDouble(row1[Column.TEMPERATURE]);
                        if (Math.Abs(temperature - temp) > StepTolerance[Column.TEMPERATURE])
                            return ErrorCode.DP_TEMPERATURE_OUT_OF_RANGE;

                        isFirstDischargeChecked = true;
                    }
                    break;
                case ActionMode.CP_DISCHARGE:

                    power = GetPowerFromRow(row1) * 1000;             //mW
                    if (Math.Abs(power - step.Action.Power) > 200)
                        return ErrorCode.DP_POWER_OUT_OF_RANGE;

                    if (isFirstDischarge && !isFirstDischargeChecked)
                    {
                        temperature = Convert.ToDouble(row1[Column.TEMPERATURE]);
                        if (Math.Abs(temperature - temp) > StepTolerance[Column.TEMPERATURE])
                            return ErrorCode.DP_TEMPERATURE_OUT_OF_RANGE;

                        isFirstDischargeChecked = true;
                    }
                    break;
                case ActionMode.REST:
                    break;
                default:
                    break;
            }
            return ErrorCode.NORMAL;
        }

        private double GetCurrentFromRow(Dictionary<Column, string> row1)
        {
            return Convert.ToDouble(row1[Column.CURRENT]) * -1000.0;
        }

        private double GetPowerFromRow(Dictionary<Column, string> row1)
        {
            var current = Convert.ToDouble(row1[Column.CURRENT]) * -1.0;
            var voltage = Convert.ToDouble(row1[Column.VOLTAGE]);
            return current * voltage;
        }

        private UInt32 StepMidPointCheck(StepV2 step, Dictionary<Column, string> row1, double temp)
        {
            double current = 0;
            switch (step.Action.Mode)
            {
                case ActionMode.CC_CV_CHARGE:
                    break;
                case ActionMode.CC_DISCHARGE:
                    current = GetCurrentFromRow(row1);
                    if (Math.Abs(current - step.Action.Current) > StepTolerance[Column.CURRENT])
                        //throw new ProcessException("Current Out Of Range");
                        return ErrorCode.DP_CURRENT_OUT_OF_RANGE;
                    break;
                case ActionMode.CP_DISCHARGE:
                    var power = GetPowerFromRow(row1) * 1000;             //mW
                    if (Math.Abs(power - step.Action.Power) > 200)
                        return ErrorCode.DP_POWER_OUT_OF_RANGE;
                    break;
                case ActionMode.REST:
                    break;
                default:
                    break;
            }
            return ErrorCode.NORMAL;
        }

        private UInt32 StepCOCPointCheck(StepV2 step, Dictionary<Column, string> row, double temp, int timeSpan)
        {
            double voltage = 0;
            double current = 0;
            double temperature = 0;
            switch (step.Action.Mode)
            {
                case ActionMode.CC_CV_CHARGE:
                    if (row[Column.STATUS] == "StepFinishByCut_I")
                    {
                        current = GetCurrentFromRow(row) * -1;
                        if (Math.Abs(current - step.CutOffConditions.SingleOrDefault(o => o.Parameter == Parameter.CURRENT).Value) > StepTolerance[Column.CURRENT])
                            return ErrorCode.DP_CURRENT_OUT_OF_RANGE;
                        voltage = Convert.ToDouble(row[Column.VOLTAGE]) * 1000;
                        if (Math.Abs(voltage - step.Action.Voltage) > StepTolerance[Column.VOLTAGE])
                            return ErrorCode.DP_VOLTAGE_OUT_OF_RANGE;
                    }
                    else if (row[Column.STATUS] == "StepFinishByCut_T")     //DST测试也会设定充电时间
                    {
                        if (Math.Abs(timeSpan - step.CutOffConditions.SingleOrDefault(o => o.Parameter == Parameter.TIME).Value) > StepTolerance[Column.TIME])
                            return ErrorCode.DP_TIME_OUT_OF_RANGE;
                    }
                    else
                    {
                        return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                    }
                    break;
                case ActionMode.CC_DISCHARGE:
                    if (row[Column.STATUS] == "StepFinishByCut_V")
                    {
                        voltage = Convert.ToDouble(row[Column.VOLTAGE]) * 1000;
                        if (Math.Abs(voltage - step.CutOffConditions.SingleOrDefault(o => o.Parameter == Parameter.VOLTAGE).Value) > StepTolerance[Column.VOLTAGE])
                            return ErrorCode.DP_VOLTAGE_OUT_OF_RANGE;
                    }
                    else if (row[Column.STATUS] == "StepFinishByCut_T")
                    {
                        if (Math.Abs(timeSpan - step.CutOffConditions.SingleOrDefault(o => o.Parameter == Parameter.TIME).Value) > StepTolerance[Column.TIME])
                            return ErrorCode.DP_TIME_OUT_OF_RANGE;
                    }
                    else
                    {
                        return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                    }
                    break;
                case ActionMode.CP_DISCHARGE:
                    if (row[Column.STATUS] == "StepFinishByCut_V")
                    {
                        voltage = Convert.ToDouble(row[Column.VOLTAGE]) * 1000;
                        if (Math.Abs(voltage - step.CutOffConditions.SingleOrDefault(o => o.Parameter == Parameter.VOLTAGE).Value) > StepTolerance[Column.VOLTAGE])
                            return ErrorCode.DP_VOLTAGE_OUT_OF_RANGE;
                    }
                    else if (row[Column.STATUS] == "StepFinishByCut_T")
                    {
                        if (Math.Abs(timeSpan - step.CutOffConditions.SingleOrDefault(o => o.Parameter == Parameter.TIME).Value) > StepTolerance[Column.TIME])
                            return ErrorCode.DP_TIME_OUT_OF_RANGE;
                    }
                    else
                    {
                        return ErrorCode.DP_ABNORMAL_STEP_CUTOFF;
                    }
                    break;
                case ActionMode.REST:
                    if (row[Column.STATUS] == "StepFinishByCut_T")
                    {
                        if (Math.Abs(timeSpan - step.CutOffConditions.SingleOrDefault(o => o.Parameter == Parameter.TIME).Value) > StepTolerance[Column.TIME])
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

        private ActionMode GetActionMode(string v)
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


        private UInt32 ContinuityCheck(Dictionary<Column, bool> rowContinuityMatrix)
        {
            int result = 0;
            int i = 0;
            foreach (var key in rowContinuityMatrix.Keys)
            {
                if (rowContinuityMatrix[key] == false)
                    result |= (0x0001 << i);
                i++;
            }
            //     0,    1,       2,    3,     4,    5,    6,       7,       8,           9,       10,            11,      12,        13
            //STEPNO, STEP, TIME_MS, TIME, CYCLE, LOOP, MODE, ,STEP_MODE, CURRENT, VOLTAGE, TEMPERATURE, CAPACITY, TOTAL_CAPACITY, STATUS
            if (result == 0x00)
            {
                return ErrorCode.NORMAL;
            }
            else if ((result | 0x1f00) == 0x1f00)   //只有物理参数超范围，不管
            {
                return ErrorCode.NORMAL;
            }
            else if (result == 0x2002)       //Step, Status错了
            {
                return ErrorCode.DP_CHECKSUM_ABNORMAL_STEP_JUMP;
            }
            else if (result == 0x200e)       //Step, Time, Time_mS, Status错了
            {
                return ErrorCode.DP_CHECKSUM_ABNORMAL_STEP_AND_TIME_JUMP;
            }
            else
            {
                //throw new ProcessException("Undefined.");
                return ErrorCode.DP_UNDEFINED;
            }
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
        private Dictionary<Column, bool> GetContinuityMatrix(Dictionary<Column, string> row0, Dictionary<Column, string> row1)
        {
            Dictionary<Column, bool> output = new Dictionary<Column, bool>();
            double diff;
            //foreach (var key in tolerance.Keys)
            //{
            //    bool result = false;
            //    if (key == Column.TIME)
            //    {
            //        DateTime t0 = DateTime.Parse(row0[key]);
            //        DateTime t1 = DateTime.Parse(row1[key]);
            //        if (Math.Abs((t1 - t0).TotalSeconds) > tolerance[key])
            //            result = false;
            //        else
            //            result = true;
            //    }
            //    else if (key == Column.STEP)
            //    {
            //        var diff = (Convert.ToDouble(row1[key]) - Convert.ToDouble(row0[key]));
            //        if (diff < 0 || diff > tolerance[key])
            //            result = false;
            //        else
            //            result = true;
            //    }
            //    else
            //    {
            //        if (Math.Abs(Convert.ToDouble(row1[key]) - Convert.ToDouble(row0[key])) > tolerance[key])
            //            result = false;
            //        else
            //            result = true;
            //    }
            //    output.Add(key, result);
            //}
            if (row1[Column.STEPNO] == "0")
                output.Add(Column.STEPNO, false);
            else
                output.Add(Column.STEPNO, true);

            diff = (Convert.ToDouble(row1[Column.STEP]) - Convert.ToDouble(row0[Column.STEP]));
            if (diff < 0 || diff > 1)
                output.Add(Column.STEP, false);
            else
                output.Add(Column.STEP, true);

            diff = (Convert.ToDouble(row1[Column.TIME_MS]) - Convert.ToDouble(row0[Column.TIME_MS]));
            if (diff < 0 || diff > 1000)
                output.Add(Column.TIME_MS, false);
            else
                output.Add(Column.TIME_MS, true);

            DateTime t0 = DateTime.Parse(row0[Column.TIME]);
            DateTime t1 = DateTime.Parse(row1[Column.TIME]);
            diff = (t1 - t0).TotalSeconds;
            if (diff < 0 || diff > 1)
                output.Add(Column.TIME, false);
            else
                output.Add(Column.TIME, true);

            if (Math.Abs(Convert.ToDouble(row1[Column.CYCLE]) - Convert.ToDouble(row0[Column.CYCLE])) > 1)
                output.Add(Column.CYCLE, false);
            else
                output.Add(Column.CYCLE, true);

            if (Math.Abs(Convert.ToDouble(row1[Column.LOOP]) - Convert.ToDouble(row0[Column.LOOP])) > 1)
                output.Add(Column.LOOP, false);
            else
                output.Add(Column.LOOP, true);

            output.Add(Column.STEP_MODE, true);

            if (Math.Abs(Convert.ToDouble(row1[Column.MODE]) - Convert.ToDouble(row0[Column.MODE])) > 1)
                output.Add(Column.MODE, false);
            else
                output.Add(Column.MODE, true);

            if (Math.Abs(Convert.ToDouble(row1[Column.CURRENT]) - Convert.ToDouble(row0[Column.CURRENT])) > 0.1)
                output.Add(Column.CURRENT, false);
            else
                output.Add(Column.CURRENT, true);

            if (Math.Abs(Convert.ToDouble(row1[Column.VOLTAGE]) - Convert.ToDouble(row0[Column.VOLTAGE])) > 0.1)
                output.Add(Column.VOLTAGE, false);
            else
                output.Add(Column.VOLTAGE, true);

            if (Math.Abs(Convert.ToDouble(row1[Column.TEMPERATURE]) - Convert.ToDouble(row0[Column.TEMPERATURE])) > 1)
                output.Add(Column.TEMPERATURE, false);
            else
                output.Add(Column.TEMPERATURE, true);

            if (Math.Abs(Convert.ToDouble(row1[Column.CAPACITY]) - Convert.ToDouble(row0[Column.CAPACITY])) > 0.5)
                output.Add(Column.CAPACITY, false);
            else
                output.Add(Column.CAPACITY, true);

            if (Math.Abs(Convert.ToDouble(row1[Column.TOTAL_CAPACITY]) - Convert.ToDouble(row0[Column.TOTAL_CAPACITY])) > 0.5)
                output.Add(Column.TOTAL_CAPACITY, false);
            else
                output.Add(Column.TOTAL_CAPACITY, true);


            if (row1[Column.STATUS] == "Warring_CheckSum")
                output.Add(Column.STATUS, false);
            else
                output.Add(Column.STATUS, true);
            return output;
        }
        private Dictionary<Column, string> GetRowFromString(string dataLine)
        {
            Dictionary<Column, string> output = new Dictionary<Column, string>();
            var strRow = dataLine.Split(',');
            for (int i = 0; i < 14; i++)
                output.Add((Column)i, strRow[i]);

            return output;
        }
        private string BuildStringFromRow(Dictionary<Column, string> row)
        {
            string output = string.Empty;
            foreach (var key in row.Keys)
            {
                output += row[key] + ",";
            }
            return output.Substring(0, output.Length - 1);
        }

        public string EventDescriptor(string filepath, Program program, Recipe recipe, TestRecord record, string info)
        {
            return $"Battery Type: {program.Project.BatteryType.Name}\n" +
                $"Project: {program.Project.Name}\n" +
                $"Program: {program.Name}\n" +
                $"Recipe: {recipe.Name}\n" +
                $"Test Record ID: {record.Id}\n" +
                //$"Battery: {record.BatteryStr}\n" +
                //$"Tester: {record.TesterStr}\n" +
                //$"Channel: {record.ChannelStr}\n" +
                //$"Chamber: {record.ChamberStr}\n" +
                //$"File Path: {record.TestFilePath}\n" +
                //$"Operator: {record.Operator}\n" +
                $"{info}";
        }
    }
}
