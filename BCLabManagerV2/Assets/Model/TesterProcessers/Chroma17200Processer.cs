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
            StepTolerance.Add(Column.CURRENT, 0.005);
            StepTolerance.Add(Column.VOLTAGE, 0.05);
            StepTolerance.Add(Column.TEMPERATURE, 2.5);

            ContinuityTolerance.Add(Column.STEPNO, 1);
            ContinuityTolerance.Add(Column.STEP, 1);
            ContinuityTolerance.Add(Column.TIME_MS, 1000);
            ContinuityTolerance.Add(Column.TIME, 1);
            ContinuityTolerance.Add(Column.CYCLE, 1);
            ContinuityTolerance.Add(Column.LOOP, 1);
            ContinuityTolerance.Add(Column.MODE, 1);
            ContinuityTolerance.Add(Column.CURRENT, 0.1);
            ContinuityTolerance.Add(Column.VOLTAGE, 0.1);
            ContinuityTolerance.Add(Column.TEMPERATURE, 1);
            ContinuityTolerance.Add(Column.CAPACITY, 0.5);
            ContinuityTolerance.Add(Column.TOTAL_CAPACITY, 0.5);
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
            TOTAL_CAPACITY  //
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

        public bool DataPreprocessing(string filepath, Program program, Recipe recipe, TestRecord record)
        {
            FileStream fs = new FileStream(filepath, FileMode.Open);
            StreamReader sw = new StreamReader(fs);
            bool ret = true;
            bool isFirstDischarge = false;
            bool isFirstDischargeChecked = false;
            int lineIndex = 0;
            int startTime = 0;
            try
            {
                //uint ret = 0;
                bool isStartPoint = false;
                bool isCOCPoint = false;
                List<StepV2> fullSteps = GetFullSteps(recipe.RecipeTemplate.StepV2s);
                for (; lineIndex < 10; lineIndex++)     //第十行以后都是数据
                    sw.ReadLine();
                string dataLine0 = string.Empty;
                string dataLine1 = string.Empty;

                Dictionary<Column, string> row0 = new Dictionary<Column, string>();
                Dictionary<Column, string> row1 = new Dictionary<Column, string>();
                dataLine1 = sw.ReadLine();
                row1 = GetRowFromString(dataLine1);

                StepV2 targetStep;
                ActionMode am = GetActionMode(row1[Column.STEP_MODE]);
                //if (am == ActionMode.CC_CV_CHARGE)
                if (am == ActionMode.CC_DISCHARGE)
                    isFirstDischarge = true;
                targetStep = fullSteps.First(o => o.Action.Mode == am);
                StepStartPointCheck(targetStep, row1, recipe.Temperature, isFirstDischarge, ref isFirstDischargeChecked);
                //if (ret != ErrorCode.NORMAL)
                //{
                //    sw.Close();
                //    fs.Close();
                //    return false;
                //}

                lineIndex = 11;
                while (true)
                {
                    dataLine0 = dataLine1;
                    row0 = row1;
                    dataLine1 = sw.ReadLine();
                    if (dataLine1 == null)
                        break;
                    lineIndex++;
                    row1 = GetRowFromString(dataLine1);

                    if (row1[Column.MODE] == "0")
                    {
                        isCOCPoint = true;
                    }
                    if (isCOCPoint)
                    {
                        #region COC Point Check
                        int timeSpan = (Convert.ToInt32(row0[Column.TIME_MS]) - startTime) / 1000;
                        StepCOCPointCheck(targetStep, row0, recipe.Temperature);
                        targetStep = GetNewTargetStep(targetStep, fullSteps, row1, recipe.Temperature, timeSpan);
                        if (!isFirstDischargeChecked)
                        {
                            if (targetStep.Action.Mode == ActionMode.CC_DISCHARGE)
                                isFirstDischarge = true;
                        }
                        //if (ret != ErrorCode.NORMAL)
                        //{
                        //    sw.Close();
                        //    fs.Close();
                        //    return false;
                        //}
                        isCOCPoint = false;
                        isStartPoint = true;
                        #endregion
                    }

                    else if (isStartPoint)
                    {
                        #region Start Point Check
                        StepStartPointCheck(targetStep, row1, recipe.Temperature, isFirstDischarge, ref isFirstDischargeChecked);
                        //if (ret != ErrorCode.NORMAL)
                        //{
                        //    sw.Close();
                        //    fs.Close();
                        //    return false;
                        //}
                        isStartPoint = false;
                        startTime = Convert.ToInt32(row1[Column.TIME_MS]);
                        #endregion
                    }

                    else
                    {
                        #region Normal Point Check
                        StepMidPointCheck(targetStep, row1, recipe.Temperature);
                        //if (ret != ErrorCode.NORMAL)
                        //{
                        //    sw.Close();
                        //    fs.Close();
                        //    return false;
                        //}



                        #region Continuity Check

                        if (row1[Column.MODE] != "0")
                        {
                            if (lineIndex == 10905)
                                lineIndex = 10905;
                            Dictionary<Column, bool> rowContinuityMatrix = GetContinuityMatrix(row0, row1, ContinuityTolerance);
                            ContinuityCheck(rowContinuityMatrix);
                            //if (ret != ErrorCode.NORMAL)
                            //{
                            //    MessageBox.Show($"Continuity Check Failed! Line {i}");
                            //    sw.Close();
                            //    fs.Close();
                            //    return false;
                            //}
                        }
                        #endregion
                        #endregion
                    }

                }

            }
            catch (StartPointCheckException e)
            {
                ret = false;
            }
            catch (MidPointCheckException e)
            {
                ret = false;
            }
            catch (EndPointCheckException e)
            {
                ret = false;
            }
            catch (ContinuityCheckException e)
            {
                ret = false;
            }
            finally
            {
                sw.Close();
                fs.Close();
            }
            return ret;
        }

        private StepV2 GetNewTargetStep(StepV2 currentStep, List<StepV2> fullSteps, Dictionary<Column, string> row, double temperature, int timeSpan)
        {
            StepV2 nextStep = null;
            foreach (var coc in currentStep.CutOffConditions)
            {
                double value = -999999;
                switch (coc.Parameter)
                {
                    case Parameter.VOLTAGE:
                        value = Convert.ToDouble(row[Column.VOLTAGE]);
                        break;
                    case Parameter.CURRENT:
                        value = Convert.ToDouble(row[Column.CURRENT]);
                        break;
                    case Parameter.TEMPERATURE:
                        value = Convert.ToDouble(row[Column.TEMPERATURE]);
                        break;
                    case Parameter.TIME:
                        value = timeSpan;// Convert.ToDouble(row[Column.TIME_MS]);
                        break;
                    default:
                        break;
                }
                if(value != -999999)
                    nextStep = Compare(coc, value, fullSteps, currentStep.Index);
                if (nextStep != null)
                    return nextStep;
            }
            return nextStep;
        }

        private StepV2 Compare(CutOffCondition coc, double value, List<StepV2> fullSteps, int currentStepIndex)
        {
            StepV2 nextStep = null;
            switch (coc.Mark)
            {
                case CompareMarkEnum.SmallerThan:
                    if (value < coc.Value)
                    {
                        nextStep = Jump(coc, fullSteps, currentStepIndex);
                    }
                    break;
                case CompareMarkEnum.LargerThan:
                    if (value > coc.Value)
                    {
                        nextStep = Jump(coc, fullSteps, currentStepIndex);
                    }
                    break;
            }
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

        private List<StepV2> GetFullSteps(ObservableCollection<StepV2> stepV2s)
        {
            List<StepV2> output = new List<StepV2>();
            int offset = 0;
            foreach (var step in stepV2s)
            {
                if (step.Prerest != 0)
                    output.Add(CreateRestStep(step.Prerest, step.Index, ref offset));
                output.Add(CloneStep(step, ref offset));
                if (step.Rest != 0)
                    output.Add(CreateRestStep(step.Rest, step.Index + offset + 1, ref offset));
            }
            return output;
        }

        private StepV2 CloneStep(StepV2 step, ref int offset)
        {
            StepV2 output = new StepV2();
            output.Index = step.Index + offset;
            output.Loop1Label = step.Loop1Label;
            output.Loop2Label = step.Loop2Label;
            output.Action = step.Action;
            output.CutOffConditions = step.CutOffConditions;
            return output;
        }

        private StepV2 CreateRestStep(int restTime, int index, ref int offset)
        {
            StepV2 output = new StepV2();
            output.Index = index;
            offset++;
            output.Action = new TesterAction() { Mode = ActionMode.REST };
            output.CutOffConditions.Add(new CutOffCondition() { Parameter = Parameter.TIME, Mark = CompareMarkEnum.LargerThan, Value = restTime });

            return output;

        }

        private void StepStartPointCheck(StepV2 step, Dictionary<Column, string> row1, double temp, bool isFirstDischarge, ref bool isFirstDischargeChecked)
        {
            double voltage = 0;
            double current = 0;
            double temperature = 0;
            switch (step.Action.Mode)
            {
                case ActionMode.CC_CV_CHARGE:
                    voltage = Convert.ToDouble(row1[Column.VOLTAGE]);
                    if (Math.Abs(voltage - step.Action.Voltage) > StepTolerance[Column.VOLTAGE])
                        //return ErrorCode.CHARGE_START_VOL;
                        throw new StartPointCheckException("Voltage Out Of Range");
                    current = Convert.ToDouble(row1[Column.CURRENT]);
                    if (Math.Abs(current - step.Action.Current) > StepTolerance[Column.CURRENT])
                        //return ErrorCode.CHARGE_START_CUR;
                        throw new StartPointCheckException("Current Out Of Range");
                    temperature = Convert.ToDouble(row1[Column.TEMPERATURE]);
                    if (Math.Abs(temperature - temp) > StepTolerance[Column.TEMPERATURE])
                        //return ErrorCode.CHARGE_START_TEMP;
                        throw new StartPointCheckException("Temperature Out Of Range");
                    break;
                case ActionMode.CC_DISCHARGE:
                    if (isFirstDischarge && !isFirstDischargeChecked)
                    {
                        current = Convert.ToDouble(row1[Column.CURRENT]);
                        if (Math.Abs(current - step.Action.Current) > StepTolerance[Column.CURRENT])
                            throw new StartPointCheckException("Current Out Of Range");
                        temperature = Convert.ToDouble(row1[Column.TEMPERATURE]);
                        if (Math.Abs(temperature - temp) > StepTolerance[Column.TEMPERATURE])
                            throw new StartPointCheckException("Temperature Out Of Range");

                        isFirstDischargeChecked = true;
                    }
                    break;
                case ActionMode.REST:
                    break;
                default:
                    break;
            }
            //return ErrorCode.NORMAL;
        }

        private void StepMidPointCheck(StepV2 step, Dictionary<Column, string> row1, double temp)
        {
            double voltage = 0;
            //double current = 0;
            double temperature = 0;
            switch (step.Action.Mode)
            {
                case ActionMode.CC_CV_CHARGE:
                    voltage = Convert.ToDouble(row1[Column.VOLTAGE]);
                    if (Math.Abs(voltage - step.Action.Voltage) > StepTolerance[Column.VOLTAGE])
                        throw new MidPointCheckException("Voltage Out Of Range");
                    //return ErrorCode.CHARGE_MID_VOL;
                    //current = Convert.ToDouble(row1[Column.CURRENT]);
                    //if (Math.Abs(current - step.Action.Current) > StepTolerance[Column.CURRENT])
                    //    return ret;
                    temperature = Convert.ToDouble(row1[Column.TEMPERATURE]);
                    if (Math.Abs(temperature - temp) > StepTolerance[Column.TEMPERATURE])
                        throw new MidPointCheckException("Temperature Out Of Range");
                    //return ErrorCode.CHARGE_MID_TEMP;
                    break;
                case ActionMode.CC_DISCHARGE:
                    break;
                case ActionMode.REST:
                    break;
                default:
                    break;
            }
            //return ErrorCode.NORMAL;
        }

        private void StepCOCPointCheck(StepV2 step, Dictionary<Column, string> row1, double temp)
        {
            double voltage = 0;
            double current = 0;
            double temperature = 0;
            switch (step.Action.Mode)
            {
                case ActionMode.CC_CV_CHARGE:
                    voltage = Convert.ToDouble(row1[Column.VOLTAGE]);
                    if (Math.Abs(voltage - step.Action.Voltage) > StepTolerance[Column.VOLTAGE])
                        //return ErrorCode.CHARGE_END_VOL;
                        throw new EndPointCheckException("Voltage Out Of Range");
                    current = Convert.ToDouble(row1[Column.CURRENT]);
                    if (Math.Abs(current - step.Action.Current) > StepTolerance[Column.CURRENT])
                        //return ErrorCode.CHARGE_END_CUR;
                        throw new EndPointCheckException("Current Out Of Range");
                    temperature = Convert.ToDouble(row1[Column.TEMPERATURE]);
                    if (Math.Abs(temperature - temp) > StepTolerance[Column.TEMPERATURE])
                        //return ErrorCode.CHARGE_END_TEMP;
                        throw new EndPointCheckException("Temperature Out Of Range");
                    break;
                case ActionMode.CC_DISCHARGE:
                    break;
                case ActionMode.REST:
                    break;
                default:
                    break;
            }
            //return ErrorCode.NORMAL;
        }

        private ActionMode GetActionMode(string v)
        {
            switch (v)
            {
                case "CC_CV_Charge": return ActionMode.CC_CV_CHARGE;
                case "CC_Discharge": return ActionMode.CC_DISCHARGE;
                case "Rest": return ActionMode.REST;
                default: return ActionMode.NA;
            }
        }

        private void ContinuityCheck(Dictionary<Column, bool> rowContinuityMatrix)
        {
            int result = 0;
            foreach (var key in rowContinuityMatrix.Keys)
            {
                if (rowContinuityMatrix[key] == false)
                    result |= (0x0001 << (int)key);
            }
            if (result == 0x00)
            {
                //return ErrorCode.NORMAL;
                return;
            }
            else if ((result | 0b111110000000) == 0b111110000000)   //只有物理参数超范围，不管
            {
                return; //ErrorCode.NORMAL;
            }
            //if ((result & 0x0080) == 0x0000)    //step change
            //{
            //    return true;
            //}
            else if ((result | 0x000e) == 0x000e)       //Step 和 Time错了
            {

                //return ErrorCode.STEP_JUMP;
                throw new ContinuityCheckException("STEP Jump.");
            }
            else
            {
                //return ErrorCode.UNDEFINDED;
                throw new ContinuityCheckException("Undefined.");
            }
        }

        private Dictionary<Column, bool> GetContinuityMatrix(Dictionary<Column, string> row0, Dictionary<Column, string> row1, Dictionary<Column, double> tolerance)
        {
            Dictionary<Column, bool> output = new Dictionary<Column, bool>();
            foreach (var key in tolerance.Keys)
            {
                bool result = false;
                if (key == Column.TIME)
                {
                    DateTime t0 = DateTime.Parse(row0[key]);
                    DateTime t1 = DateTime.Parse(row1[key]);
                    if ((t1 - t0) > TimeSpan.FromSeconds(tolerance[key]))
                        result = false;
                    else
                        result = true;
                }
                else
                {
                    if (Math.Abs(Convert.ToDouble(row1[key]) - Convert.ToDouble(row0[key])) > tolerance[key])
                        result = false;
                    else
                        result = true;
                }
                output.Add(key, result);
            }
            return output;
        }

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
        //    TOTAL_CAPACITY  //
        private Dictionary<Column, string> GetRowFromString(string dataLine)
        {
            Dictionary<Column, string> output = new Dictionary<Column, string>();
            var strRow = dataLine.Split(',');
            for (int i = 0; i < 13; i++)
                output.Add((Column)i, strRow[i]);

            return output;
        }
    }
}
