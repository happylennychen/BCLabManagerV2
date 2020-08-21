using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            ContinuityTolerance.Add(Column.CURRENT, 0.003);
            ContinuityTolerance.Add(Column.VOLTAGE, 0.0005);
            ContinuityTolerance.Add(Column.TEMPERATURE, 0.5);
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

        public bool DataPreprocessing(string filepath, Program program, Recipe recipe)
        {
            bool ret = false;
            bool isStartPoint = false;
            bool isCOCPoint = false;
            List<StepV2> fullSteps = GetFullSteps(recipe.RecipeTemplate.StepV2s);
            FileStream fs = new FileStream(filepath, FileMode.Open);
            StreamReader sw = new StreamReader(fs);

            int i = 0;
            for (; i < 10; i++)     //第十行以后都是数据
                sw.ReadLine();
            string dataLine0 = string.Empty;
            string dataLine1 = string.Empty;

            Dictionary<Column, string> row0 = new Dictionary<Column, string>();
            Dictionary<Column, string> row1 = new Dictionary<Column, string>();
            dataLine1 = sw.ReadLine();
            row1 = GetRowFromString(dataLine1);

            StepV2 step;
            ActionMode am = GetActionMode(row1[Column.STEP_MODE]);
            //if (am == ActionMode.CC_CV_CHARGE)
            step = fullSteps.First(o => o.Action.Mode == am);
            ret = StepStartPointCheck(step, row1, recipe.Temperature);
            if (!ret)
            {
                sw.Close();
                fs.Close();
                return false;
            }
            while (true)
            {
                dataLine0 = dataLine1;
                row0 = row1;
                dataLine1 = sw.ReadLine();
                if (dataLine1 == null)
                    break;

                row1 = GetRowFromString(dataLine1);

                #region Continuity Check
                Dictionary<Column, bool> rowContinuityMatrix = GetContinuityMatrix(row0, row1, ContinuityTolerance);
                ret = ContinuityCheck(rowContinuityMatrix);
                if (!ret)
                {
                    sw.Close();
                    fs.Close();
                    return false;
                }
                #endregion

                if (row1[Column.MODE] == "0")
                {
                    isCOCPoint = true;
                }

                #region COC Point Check
                if (isCOCPoint)
                {
                    ret = StepCOCPointCheck(step, row0, recipe.Temperature);
                    if (!ret)
                    {
                        sw.Close();
                        fs.Close();
                        return false;
                    }
                    isCOCPoint = false;
                    isStartPoint = true;
                }
                #endregion

                #region Start Point Check
                if (isStartPoint)
                {
                    ret = StepStartPointCheck(step, row1, recipe.Temperature);
                    if (!ret)
                    {
                        sw.Close();
                        fs.Close();
                        return false;
                    }
                    isStartPoint = false;
                }
                #endregion

                #region Normal Point Check
                ret = StepNormalPointCheck(step, row1, recipe.Temperature);
                if (!ret)
                {
                    sw.Close();
                    fs.Close();
                    return false;
                }
                #endregion
            }


            sw.Close();
            fs.Close();
            return true;
        }

        private List<StepV2> GetFullSteps(ObservableCollection<StepV2> stepV2s)
        {
            throw new NotImplementedException();
        }

        private bool StepStartPointCheck(StepV2 step, Dictionary<Column, string> row1, double temp)
        {
            double voltage = 0;
            double current = 0;
            double temperature = 0;
            bool ret = false;
            switch (step.Action.Mode)
            {
                case ActionMode.CC_CV_CHARGE:
                    voltage = Convert.ToDouble(row1[Column.VOLTAGE]);
                    if (Math.Abs(voltage - step.Action.Voltage) > StepTolerance[Column.VOLTAGE])
                        return ret;
                    current = Convert.ToDouble(row1[Column.CURRENT]);
                    if (Math.Abs(current - step.Action.Current) > StepTolerance[Column.CURRENT])
                        return ret;
                    temperature = Convert.ToDouble(row1[Column.TEMPERATURE]);
                    if (Math.Abs(temperature - temp) > StepTolerance[Column.TEMPERATURE])
                        return ret;
                    break;
                case ActionMode.CC_DISCHARGE:
                    break;
                default:
                    break;
            }
            return ret;
        }

        private bool StepNormalPointCheck(StepV2 step, Dictionary<Column, string> row1, double temp)
        {
            double voltage = 0;
            //double current = 0;
            double temperature = 0;
            bool ret = false;
            switch (step.Action.Mode)
            {
                case ActionMode.CC_CV_CHARGE:
                    voltage = Convert.ToDouble(row1[Column.VOLTAGE]);
                    if (Math.Abs(voltage - step.Action.Voltage) > StepTolerance[Column.VOLTAGE])
                        return ret;
                    //current = Convert.ToDouble(row1[Column.CURRENT]);
                    //if (Math.Abs(current - step.Action.Current) > StepTolerance[Column.CURRENT])
                    //    return ret;
                    temperature = Convert.ToDouble(row1[Column.TEMPERATURE]);
                    if (Math.Abs(temperature - temp) > StepTolerance[Column.TEMPERATURE])
                        return ret;
                    break;
                case ActionMode.CC_DISCHARGE:
                    break;
                default:
                    break;
            }
            return ret;
        }

        private bool StepCOCPointCheck(StepV2 step, Dictionary<Column, string> row1, double temp)
        {
            double voltage = 0;
            double current = 0;
            double temperature = 0;
            bool ret = false;
            switch (step.Action.Mode)
            {
                case ActionMode.CC_CV_CHARGE:
                    voltage = Convert.ToDouble(row1[Column.VOLTAGE]);
                    if (Math.Abs(voltage - step.Action.Voltage) > StepTolerance[Column.VOLTAGE])
                        return ret;
                    current = Convert.ToDouble(row1[Column.CURRENT]);
                    if (Math.Abs(current - step.Action.Current) > StepTolerance[Column.CURRENT])
                        return ret;
                    temperature = Convert.ToDouble(row1[Column.TEMPERATURE]);
                    if (Math.Abs(temperature - temp) > StepTolerance[Column.TEMPERATURE])
                        return ret;
                    break;
                case ActionMode.CC_DISCHARGE:
                    break;
                default:
                    break;
            }
            return ret;
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

        private bool ContinuityCheck(Dictionary<Column, bool> rowContinuityMatrix)
        {
            throw new NotImplementedException();
        }

        private Dictionary<Column, bool> GetContinuityMatrix(Dictionary<Column, string> row0, Dictionary<Column, string> row1, Dictionary<Column, double> tolerance)
        {
            Dictionary<Column, bool> output = new Dictionary<Column, bool>();
            foreach (var key in tolerance.Keys)
            {
                bool result = false;
                if (Convert.ToDouble(row1[key]) - Convert.ToDouble(row0[key]) > tolerance[key])
                    result = false;
                else
                    result = true;
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
