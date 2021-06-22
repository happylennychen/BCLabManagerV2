using MathNet.Numerics;
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

        enum O2ChromaRecord : int
        {
            ChSerial = 2,
            ChTime = 3,
            ChChgDsg = 6,
            ChCurrent = 8,
            ChVoltage = 9,
            ChTemperature = 10,
            ChAccMah = 11,
            ChStatus = 13,
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

        public bool DataPreprocessing(string filepath, Program program, Recipe recipe, TestRecord record, int startStepIndex)
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
                var fullSteps = new List<StepV2>(recipe.RecipeTemplate.StepV2s.OrderBy(o => o.Index));
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
                result = DataPreprocesser.StepStartPointCheck(step1, recipe.Temperature);
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
                        result = DataPreprocesser.StepStartPointCheck(step1, recipe.Temperature);
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


        public uint LoadRawToSource(string filePath, ref SourceData output)
        {
            //SourceData output = new SourceData(current, temperature);
            float fErrorRatio = 0.1F;       //90% * header.current
            float fErrorStep = 4.0F;
            char AcuTSeperater = ',';
            bool ret = false;
            UInt32 result = 0;
            Stream stmCSV = null;
            StreamReader stmContent = null;
            string strTemp;
            string[] strToken;
            char[] chSeperate = new char[] { AcuTSeperater };
            //int iAction;
            string sVoltage = "", sCurrent = "", sTemp = "", sAccM = "", sDate = "";
            float fVoltn = -1F, fCurrn = -1F, fTempn = -1F, fAccmn = -1F, fVoltOld = -1, fVoltAdj;
            bool bReachHighVolt = false, bStartExpData = false, bReachLowVolt = false, bStopExpData = true;
            float ftmp;
            UInt32 iNumColCnt, iNumSrlNow, iNumSrlStart, iNumSrlEnd, iNumSrlLast, iNumZerotmp, iNumLostCount;
            UInt32 iElipseTime;
            float fVoltageDiff = 10F;
            UInt16 iHighVoltDiff = 100;
            float fAccmnStart = -1F;            //use to record the faccmn value when reaching the first discharge current

            //var tempFilePath = Path.Combine(GlobalSettings.LocalFolder, Path.GetFileName(filePath));

            try
            {
                //File.Copy(filePath, tempFilePath);
                stmCSV = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                stmContent = new StreamReader(stmCSV);
            }
            catch (Exception e)
            {
                result = 1;
                if (!ErrorHandler(result)) return ErrorCode.UNDEFINED;
            }

            //initialization
            result = 0;
            ret = true;
            iNumColCnt = 0; iNumSrlNow = 0; iNumSrlStart = 0;
            iNumSrlLast = iNumSrlStart;
            iNumSrlEnd = iNumSrlStart;
            iNumZerotmp = 0; iNumLostCount = 0;
            iElipseTime = 0;

            while ((strTemp = stmContent.ReadLine()) != null)
            {
                sVoltage.Remove(0, sVoltage.Length);
                sCurrent.Remove(0, sCurrent.Length);
                sTemp.Remove(0, sTemp.Length);
                sAccM.Remove(0, sAccM.Length);
                sDate.Remove(0, sDate.Length);
                if (iElipseTime != 0)
                {
                    //reserve last one value
                    iNumSrlLast = iNumSrlNow;
                }
                if (iNumSrlLast == 5400)
                    fVoltOld = fVoltn;
                fVoltOld = fVoltn;

                #region skip other except log data line
                strToken = strTemp.Split(chSeperate, StringSplitOptions.None);  //split column by ',' character
                //if (strToken.Length < (int)O2TXTRecord.TxtAccMah)
                if (strToken.Length < 13)   //Leon: 被逗号分出的份数小于13，意味着不是数据行
                {
                    if ((strTemp.IndexOf(":") != -1) || (strTemp.Length == 0))
                    {   //header file or empty line
                        continue;
                    }
                    else
                    {
                        if (strToken.Length < 2)
                        {
                            result = 1;
                            if (!ErrorHandler(result)) return ErrorCode.UNDEFINED;
                            break;
                        }
                    }
                }
                else
                {
                    if (!float.TryParse(strToken[8], out ftmp)) //Leon:如果不是数据，代表不是数据行，继续读下一行，否则可执行后面的操作
                    {
                        //raw data start for next line
                        continue;
                    }
                }
                #endregion

                #region call format parsing, to get correct value for log line

                //iNumSrlNow = 0;
                ftmp = 1000F;      //Chroma is in A/V/Ahr format
                                   //ParseChroFormat(strToken, out sVoltage, out sCurrent, out sTemp, out sAccM, out sDate, out iNumSrlNow);
                if (!ParseChroFormaV2(strToken, out sVoltage, out sCurrent, out sTemp, out sAccM, out sDate, out iNumSrlNow))
                {
                    iNumSrlNow = iNumSrlLast;   //restore previous value
                    continue;                   //skip if any error happened
                }
                if (iNumSrlNow != 0)
                    iElipseTime = 1;
                else
                    iElipseTime = 0;
                #endregion

                if ((sVoltage.Length != 0) && (sCurrent.Length != 0) &&
                    (sTemp.Length != 0) && (sAccM.Length != 0) && (sDate.Length != 0) && iNumSrlNow != 0)
                {
                    #region check volt/curr/temp/accm conveting to float
                    if (!float.TryParse(sVoltage, out fVoltn))
                    {
                        result = 1;
                        break;
                    }
                    if (!float.TryParse(sCurrent, out fCurrn))
                    {
                        result = 1;
                        break;
                    }
                    if (!float.TryParse(sTemp, out fTempn))
                    {
                        result = 1;
                        break;
                    }
                    if (!float.TryParse(sAccM, out fAccmn))
                    {
                        result = 1;
                        break;
                    }
                    fVoltn *= ftmp;
                    fCurrn *= ftmp;
                    fAccmn *= ftmp;
                    #endregion

                    if (fVoltn > output.fMaxExpVolt) output.fMaxExpVolt = fVoltn;//set maximum voltage value from raw data
                    if (fVoltn < output.fMinExpVolt) output.fMinExpVolt = fVoltn;


                    //bReachHighVolt = true;  //(20201109)for leon miss
                    #region check all log data about voltage/current, to get bReachHighVolt, bStartExpData, bStopExpData, and, bReachLowVolt value
                    if (!bReachHighVolt)
                    {       //initially, first time must go here, suppose not reach high voltage
                        if ((Math.Abs(fVoltn - output.fLimitChgVolt) < iHighVoltDiff))    //maybe log will only be idle stage after charge_to_full, set higher hysteresis
                        {   //check voltage is reached high voltage, no matter charge/discharge/or idle mode
                            bReachHighVolt = true;
                        }
                        //(20201109)for leon miss
                        else if ((Math.Abs(fCurrn) > 0))
                        {
                            bReachHighVolt = true;
                        }
                    }
                    //if reached high voltage, wait for get experiment data
                    //first one log may be the started discharge log, so skip else
                    /*else*/
                    if ((bReachHighVolt) && (!bStartExpData))
                    {
                        if (fCurrn < 0)
                        {
                            //if (Math.Abs(fCurrn - header.fCurrent) < fErrorStep)
                            if (Math.Abs(fCurrn - output.fCurrent) < Math.Abs(fErrorRatio * output.fCurrent))
                            {   //current is familiar with header setting
                                bStartExpData = true;
                                bStopExpData = false;
                                fAccmnStart = fAccmn;
                            }
                            else
                            {
                                //not experiment current value
                            }
                        }
                        //else wait for discharging value
                    }
                    //else if((bReachHighVolt) && (fCurrn > 0))	//still in charging learning cycle
                    else if ((bStartExpData) && (!bReachLowVolt))
                    {
                        //if (Math.Abs(fCurrn - header.fCurrent) < fErrorStep)
                        if (Math.Abs(fCurrn - output.fCurrent) < Math.Abs(fErrorRatio * output.fCurrent))
                        {
                            if ((Math.Abs(fVoltn - output.fCutoffDsgVolt) < fErrorStep))  //stop discharging to idle
                            {
                                bReachLowVolt = true;
                            }
                        }
                        else
                        {
                            if ((Math.Abs(fCurrn - 0) < 10) && (Math.Abs(fVoltn - output.fCutoffDsgVolt) < 500))
                            {
                                bReachLowVolt = true;
                            }
                        }
                        //else voltage still in range of High_Low voltage
                    }
                    /*else*/
                    if ((bReachLowVolt) && (!bStopExpData))
                    {
                        if ((Math.Abs(fCurrn - 0) < 10) || (fCurrn >= 0) ||
                            ((Math.Abs(fVoltn - output.fCutoffDsgVolt) < fErrorStep) && (Math.Abs(fCurrn - output.fCurrent) < Math.Abs(fErrorRatio * output.fCurrent))))
                        {
                            bStopExpData = true;
                            output.fAccmAhrCap = Math.Abs(fAccmn);
                        }
                    }
                    #endregion

                    if ((bStartExpData) && (!bStopExpData))     //experiment data starts but not stop
                    {
                        if (iNumSrlNow == iNumSrlLast)
                        {
                            //(M141201)Francis, as Guoyan request, skip this error and not log it
                            //ret = LibErrorCode.IDS_ERR_TMK_SD_SERIAL_SAME;
                            //CreateNewError(iNumSrlNow, fVoltn, ret);
                            //(E141201)
                        }
                        #region check raw data is resonable or not
                        if (iNumColCnt == 0)        //first one record
                        {
                            iNumSrlStart = iNumSrlNow;
                            iNumSrlEnd = iNumSrlNow;


                            fVoltageDiff = -1.0F;   //negitively changing
                        }
                        else        //after 2nd one
                        {
                            if ((iNumSrlNow - iNumSrlLast) > 1) //jump more than 1
                            {
                                if ((iNumSrlNow - iNumSrlLast) > 5) //jump more than 5
                                {
                                    result = 1;
                                }
                                else
                                {
                                    //jump less than 5 in 2 seconds
                                    if ((Math.Abs(fVoltn - fVoltOld) > fErrorStep) &&
                                        (iElipseTime < 2))
                                    {
                                        result = 1;
                                    }
                                    else
                                    { //voltage is not jumping more 5mV
                                        iNumLostCount += (iNumSrlNow - iNumSrlLast - 1); //record not continuous serial 
                                    }
                                }
                            }
                            else //jumping only 1 serail number, reasonable 
                            {
                                if (Math.Abs(fCurrn - 0) < fErrorStep)  //nearly zero current value found
                                {
                                    iNumZerotmp += 1;
                                    if (iNumZerotmp > fErrorStep)   //continue more than 5 times
                                    {
                                        result = 1;
                                    }
                                }
                                else
                                {
                                    if (iNumZerotmp != 0)
                                    {
                                        iNumZerotmp = 0;
                                    }
                                    //if (Math.Abs(fVoltn - fVoltOld) > fVoltageDiff )
                                    //if (Math.Abs(fVoltn - fVoltOld) > 5)
                                    if (((fVoltn - fVoltOld) * fVoltageDiff) < 0)       //check voltage changing direction, in charging, voltage should increase, vice versa
                                    {
                                        if ((Math.Abs(fVoltn - fVoltOld) > 10.0F) &&    //if changing tolence is bigger than 10mV in 2 seconds
                                            (iElipseTime < 2))
                                        {
                                            result = 1;
                                        }
                                    }
                                }
                            }
                            //if(Math.Abs(fVoltn - fVoltOld) != 0)
                            //fVoltageDiff = Math.Abs(fVoltn - fVoltOld);
                        }   //if (iNumColCnt == 0)		else	(first one)
                        #endregion

                        if (result != 0)
                        {
                            //(M141117)Francis, if parsing error occurred, just let it keeps going parsing, no to break while loop
                            ret = false;
                            //break;
                            //(E141117)
                        }
                        //if every check is Ok add into <List>
                        iNumColCnt += 1;
                        //(M141117)Francis, make sure bReturn is true; to make sure parsing is OK then to add inot ReservedExpData
                        //if (bRecord)
                        if (ret)
                        {
                            //RawDataNode nodeN = new RawDataNode(iNumSrlNow, sVoltage, sCurrent, sTemp, sAccM, sDate, ftmp);
                            DataRow nodeN = new DataRow(iNumSrlNow, fVoltn, fCurrn, fTempn, fAccmn, sDate, 1);  //(M170628)Francis, did multiple before
                                                                                                                //TableRawData.Add(nodeN);
                            output.ReservedExpData.Add(nodeN);
                            fVoltAdj = (fVoltn - output.fMeasureOffset) / output.fMeasureGain
                                - (output.fCurrent * output.fTraceResis * 0.001F);
                            DataRow rdnAdjust = new DataRow(iNumSrlNow, fVoltAdj, fCurrn, fTempn, fAccmn, sDate);
                            output.AdjustedExpData.Add(rdnAdjust);
                        }
                    }   //if ((bStartExpData) && (!bStopExpData))
                    else if ((bStartExpData) && (bStopExpData))     //experiment data start and stop detect
                    {
                        if (iNumSrlEnd == iNumSrlStart)
                        {
                            iNumSrlEnd = iNumSrlNow;
                            iNumColCnt += 1;        //force to add last one record
                                                    //(M141117)Francis, make sure bReturn is true; to make sure parsing is OK then to add inot ReservedExpData
                                                    //if (bRecord)
                            if (ret)
                            {
                                //(A150806)Francis, if AccMah is jumping too much, skip last one record
                                if (Math.Abs(fAccmn - output.fAbsMaxCap) < (output.fAbsMaxCap * 0.05))
                                {
                                    //RawDataNode nodeN = new RawDataNode(iNumSrlNow, sVoltage, sCurrent, sTemp, sAccM, sDate, ftmp);
                                    DataRow nodeN = new DataRow(iNumSrlNow, fVoltn, fCurrn, fTempn, fAccmn, sDate, ftmp);
                                    //TableRawData.Add(nodeN);
                                    output.ReservedExpData.Add(nodeN);
                                    fVoltAdj = (fVoltn - output.fMeasureOffset) / output.fMeasureGain
                                        - (output.fCurrent * output.fTraceResis * 0.001F);
                                    DataRow rdnAdjust = new DataRow(iNumSrlNow, fVoltAdj, fCurrn, fTempn, fAccmn, sDate);
                                    output.AdjustedExpData.Add(rdnAdjust);
                                }
                            }
                            break;      //after last one ignore it
                        }
                    }
                }   //if ((sVoltage.Length != 0) && (sCurrent.Length != 0) &&
            }   //while ((strTemp = stmContent.ReadLine()) != null)
            stmContent.Close();

            float fSoCA;
            float fUsedMaxCap = output.fAccmAhrCap;



            int iCount;
            //foreach (RawDataNode rdnT in AdjustedExpData)	//(M141107)Francis
            for (iCount = 1; iCount <= output.AdjustedExpData.Count; iCount++) //(M141107)Francis
            {
                fSoCA = ((fUsedMaxCap - output.fCapacityDiff - output.AdjustedExpData[iCount - 1].fAccMah) *       //(M141107)Francis
                                    (10000 / fUsedMaxCap));

                output.AdjustedExpData[iCount - 1].fSoCAdj = fSoCA;
            }
            //File.Delete(tempFilePath);
            return ErrorCode.NORMAL;
        }

        private bool ErrorHandler(uint result)
        {
            throw new NotImplementedException();
        }

        private bool ParseChroFormaV2(string[] inToken, out string outVolt, out string outCurr, out string outTemp, out string outAccm, out string outDate, out UInt32 outSerial)
        {
            string sStepCtlg = "";
            int iMinus = 0;

            outVolt = "";
            outCurr = "";
            outTemp = "";
            outAccm = "";
            outDate = "";
            outSerial = 0;

            if (inToken.Length > (int)O2ChromaRecord.ChStatus)
            {
                sStepCtlg = inToken[(int)O2ChromaRecord.ChStatus];
                if (sStepCtlg.ToLower().IndexOf("checksum") != -1)
                {
                    //skip if status = Warring_CheckSum
                    return false;
                }
            }

            sStepCtlg = inToken[(int)O2ChromaRecord.ChChgDsg];
            if (sStepCtlg.ToLower().IndexOf("rest") == -1)
            {
                if (sStepCtlg.ToLower().IndexOf("discharge") != -1)
                {
                    iMinus = -1;
                }
                else
                {
                    iMinus = 1;
                }
            }
            outDate = new string(inToken[(int)O2ChromaRecord.ChTime].ToCharArray());
            outCurr = inToken[(int)O2ChromaRecord.ChCurrent];
            if (iMinus == -1)
            {
                if (outCurr.IndexOf("-") == -1)
                {
                    outCurr = "";
                    return false;
                }
            }
            else if (iMinus == 1)    //if (iMinus == 1)
            {
                if (outCurr.IndexOf("-") != -1)
                {
                    outCurr = "";
                    return false;
                }
            }
            else { }                //if (iMinus == 0)
            outVolt = new string(inToken[(int)O2ChromaRecord.ChVoltage].ToCharArray());
            outTemp = new string(inToken[(int)O2ChromaRecord.ChTemperature].ToCharArray());
            outAccm = new string(inToken[(int)O2ChromaRecord.ChAccMah].ToCharArray());
            if (!UInt32.TryParse(inToken[(int)O2ChromaRecord.ChSerial], out outSerial))
            {
                outSerial = 0;          //(A140702)Francis, as guoyan request, check total number
            }
            else
            {
                outSerial = outSerial / 1000;
                if (outSerial > (UInt32.MaxValue / 2 - 1))
                    outSerial = 1;
            }

            return true;
        }
    }

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


        private static ChromaNode GetNodeFromeString(string value)
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
                    MessageBox.Show($"Row Parsing Failed. Error Message is:\n{e.Message}");
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


        public static UInt32 StepStartPointCheck(StepV2 step, double temp)
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
                        temperature = Nodes[Index].Temperature;
                        if (Math.Abs(temperature - temp) > StepTolerance.Temperature)
                            return ErrorCode.DP_TEMPERATURE_OUT_OF_RANGE;

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
                        temperature = Nodes[Index].Temperature;
                        if (Math.Abs(temperature - temp) > StepTolerance.Temperature)
                            return ErrorCode.DP_TEMPERATURE_OUT_OF_RANGE;

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
        public static string EndByCurrent { get { return "StepFinishByCut_I"; } }
        public static string EndByVoltage { get { return "StepFinishByCut_V"; } }
        public static string EndByPower { get { return ""; } }
        public static string EndByTemperature { get { return ""; } }
        public static string EndByTime { get { return "StepFinishByCut_T"; } }
        public static string EndByError { get { return "Warring_CheckSum"; } }
    }
}
