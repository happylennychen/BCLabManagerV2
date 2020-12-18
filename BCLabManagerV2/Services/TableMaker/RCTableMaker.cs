using BCLabManager.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BCLabManager
{
    public static class RCTableMaker
    {
        #region RC
        public static void GetRCSource(Project project, List<Program> programs, List<Tester> testers, out List<SourceData> SDList)
        {
            var trs = programs.Select(o => o.Recipes.Select(i => i.TestRecords.Where(j => j.Status == TestStatus.Completed).ToList()).ToList()).ToList();
            List<TestRecord> testRecords = new List<TestRecord>();
            foreach (var tr in trs)
            {
                foreach (var t in tr)
                    testRecords = testRecords.Concat(t).ToList();
            }
            testRecords = testRecords.Where(o => o.Status == TestStatus.Completed).ToList();
            SDList = new List<SourceData>();
            foreach (var tr in testRecords)
            {
                SourceData sd = new SourceData();
                sd.fAbsMaxCap = project.AbsoluteMaxCapacity;
                sd.fCapacityDiff = (float)tr.CapacityDifference;
                sd.fCurrent = (float)tr.Current * (-1);
                sd.fCutoffDsgVolt = project.CutoffDischargeVoltage;
                sd.fLimitChgVolt = project.LimitedChargeVoltage;
                sd.fMeasureGain = (float)tr.MeasurementGain;
                sd.fMeasureOffset = (float)tr.MeasurementOffset;
                sd.fTemperature = (float)tr.Temperature;
                sd.fTraceResis = (float)tr.TraceResistance;
                var tester = testers.SingleOrDefault(o => o.Name == tr.TesterStr);
                UInt32 result = tester.ITesterProcesser.LoadRawToSource(tr.TestFilePath, ref sd);
                if (result == ErrorCode.NORMAL)
                {
                    SDList.Add(sd);
                }
                //string filePath = Path.Combine("D:\\Issues\\Open\\BC_Lab\\Table Maker\\Francis 30Q Source Data", Path.GetFileName(tr.TestFilePath));
                //if (File.Exists(filePath))
                //{
                //    UInt32 result = tester.ITesterProcesser.LoadRawToSource(filePath, ref sd);
                //    if (result == ErrorCode.NORMAL)
                //    {
                //        SDList.Add(sd);
                //    }
                //}
            }
        }
        public static void GetRCModel(List<SourceData> SDList, Project project, ref RCModel rcModel)
        {
            RCModel output = rcModel;
            output.SourceList = SDList;
            Int32 iMinVoltage;
            Int32 iMaxVoltage;
            TableMakerService.GetVoltageBondary(SDList, out iMinVoltage, out iMaxVoltage);
            output.MinVoltage = iMinVoltage;
            output.MaxVoltage = iMaxVoltage;
            foreach (var sd in SDList)
            {
                if (!output.listfTemp.Contains(sd.fTemperature))  //not exist in list add it
                {
                    output.listfTemp.Add(sd.fTemperature);
                }
                if (!output.listfCurr.Contains(sd.fCurrent))          //not exis in list add it
                {
                    output.listfCurr.Add(sd.fCurrent);
                }
            }
            UInt32 uErr = 0;
            CreateRCPoints_TableMini(ref uErr, ref output, SDList, project);
        }
        private static bool CreateRCPoints_TableMini(ref uint uErr, ref RCModel output, List<SourceData> sdList, Project project)
        {
            bool bReturn = true;	//cause below will use bReturn &= xxxx

            foreach (List<Int32> il in output.outYValue)
            {
                il.Clear();
            }
            output.outYValue.Clear();
            output.listfTemp.Sort();
            output.listfCurr.Sort();
            output.listfCurr.Reverse();

            List<float> fCTAKeod = new List<float>();        //save Keod value, = Max_Diff_Temp * 10 / Joules_value for each current_temperature
            foreach (float ft in output.listfTemp)		//from low temperature to list
            {
                foreach (float fc in output.listfCurr)		//from low current to list
                {
                    foreach (var sds in sdList)
                    {
                        if ((sds.fTemperature == ft) &&
                            (sds.fCurrent == fc))
                        {
                            {
                                List<Int32> il16tmp = new List<Int32>();
                                //Int32 iCountP = 0;
                                float fCCount = 0;
                                float fMaxDiff = 0, fCalTmp = 0;
                                if (sds.AdjustedExpData.Count < 1)
                                {
                                    uErr = 1;
                                    bReturn &= false;
                                    break;	//foreach (SourceData sds in bdsBatRCSource)
                                }
                                else
                                {
                                    if (!CreateYPoints_CTAV0026(project.VoltagePoints, ref il16tmp, ref fMaxDiff, ref fCCount, sds.AdjustedExpData, ref uErr, project.AbsoluteMaxCapacity, 0))
                                    {
                                        bReturn &= false;
                                    }
                                    else
                                    {
                                    }
                                    output.outYValue.Add(il16tmp);	//if error, still add into RCYvalue
                                    fCalTmp = fCCount;
                                    fMaxDiff *= 10;
                                    fMaxDiff /= fCalTmp;
                                    fCTAKeod.Add(fMaxDiff);
                                    break;
                                }
                            }
                        }
                    }
                }
            }



            List<float> fAvgPerCurrent = new List<float>();     //save Keod'_favg for each temperature, skip Max_Keod_value and Min_Keod_value to calculate average
            List<float> fCTACurrent = new List<float>();     //save Keod'_favg for each temperature, skip Max_Keod_value and Min_Keod_value to calculate average
            if (true)
            {
                float fCalTmp = 0, fCalMax = 0, fCalMin = 10000;
                int idxKeod = 0;
                for (int jj = 0; jj < output.listfCurr.Count(); jj++)
                {
                    fCalTmp = 0;
                    fCalMax = 0;
                    fCalMin = 10000;
                    for (int ii = 0; ii < output.listfTemp.Count(); ii++)
                    {
                        idxKeod = ((ii * output.listfCurr.Count()) + jj);
                        if (idxKeod < fCTAKeod.Count())
                        {
                            fCalTmp += fCTAKeod[idxKeod];
                            if (fCTAKeod[idxKeod] > fCalMax)
                                fCalMax = fCTAKeod[idxKeod];
                            if (fCTAKeod[idxKeod] < fCalMin)
                                fCalMin = fCTAKeod[idxKeod];
                        }
                        else
                        {
                            bReturn = false;
                            break;  //for (int jj = 0; jj < listfCurr.Count(); jj++)
                        }
                    }
                    if (!bReturn)
                    {
                        uErr = 1;
                        break;      //for (int ii = 0; ii < listfTemp.Count(); ii++)
                    }
                    else
                    {
                        fCalTmp -= fCalMax;
                        fCalTmp -= fCalMin;
                        fCalTmp /= (output.listfTemp.Count() - 2);
                        fAvgPerCurrent.Add(fCalTmp);
                        fCTACurrent.Add(Math.Abs((float)(output.listfCurr[jj] / 1000)));
                    }
                }
                if (fAvgPerCurrent.Count() != output.listfCurr.Count())
                {
                    uErr = 1;
                    bReturn = false;
                }
            }

            double fSquare, fCTABase, fCTASlope;
            CalculateCTASlopeBase(fCTACurrent, fAvgPerCurrent, 0, fAvgPerCurrent.Count(), out fSquare, out fCTABase, out fCTASlope);
            fCTASlope *= 10000;
            fCTASlope = (float)((int)(fCTASlope + 0.5));
            fCTABase = (float)((int)(fCTABase * 10000 + 0.5));
            output.fCTABase = fCTABase;
            output.fCTASlope = fCTASlope;

            return bReturn;
        }

        private static bool CreateYPoints_CTAV0026(List<int> TableVoltagePoints, ref List<Int32> ypoints, ref float fRefMaxDiff, ref float fRefCCount, List<DataRow> inListRCData, ref UInt32 uErr, float fDesignCapacity, float fCapaciDiff)
        {
            bool bReturn = false;
            int i = 0, j = 0, iCountP = 0;
            float fPreSoC = -99999, fPreVol = -99999, fAvgSoc = -99999, fPreTemp = -9999, fPreCCount = 0;//, fAccCount = 0; ;
            //(A200902)Francis, use the low_cur header setting to calculate
            List<float> flstTmp = new List<float>();
            float fAccCap = 0.0f;

            TableVoltagePoints.Sort();
            TableVoltagePoints.Reverse();
            ypoints.Clear();
            fRefMaxDiff = 0;
            fRefCCount = 0;

            //(M140718)Francis,
            i = 0; j = 0;
            for (i = 0; i < TableVoltagePoints.Count; i++)
            {
                for (; j < inListRCData.Count; j++)
                {
                    //(A20201015), memorize maximum accumulated capacity
                    if (Math.Abs(inListRCData[j].fAccMah) > fAccCap)
                        fAccCap = Math.Abs(inListRCData[j].fAccMah);
                    //(A20200813)
                    if (j >= iCountP)
                    {
                        if (fPreTemp == -9999)
                        {
                            iCountP += 1;
                            fPreTemp = inListRCData[j].fTemperature;
                            fPreCCount = inListRCData[j].fAccMah;
                        }
                        else
                        {
                            //strDbg = string.Format("j={0:d}, diff= {1:f}, sum befor ={2:f}", j, (inListRCData[j].fTemperature - fPreTemp), fRefSumDiff);
                            //Debug.WriteLine(strDbg);
                            if (fRefMaxDiff < (inListRCData[j].fTemperature - fPreTemp))
                                fRefMaxDiff = (inListRCData[j].fTemperature - fPreTemp);
                            iCountP += 1;
                            /*fAccCount*/
                            fRefCCount = (float)Math.Abs(inListRCData[j].fAccMah - fPreCCount);
                        }
                    }
                    //

                    if (TableVoltagePoints[i] < inListRCData[j].fVoltage)
                    {
                        fPreSoC = inListRCData[j].fAccMah;
                        fPreVol = inListRCData[j].fVoltage;
                    }
                    else
                    {
                        if ((fPreSoC != -99999) && (fPreVol != -99999))
                        {
                            fAvgSoc = (fPreVol - TableVoltagePoints[i]) / (fPreVol - inListRCData[j].fVoltage);
                            fAvgSoc *= (fPreSoC - inListRCData[j].fAccMah);
                            fAvgSoc += inListRCData[j].fAccMah;
                            if ((i + 1) < TableVoltagePoints.Count)
                            {
                                if (TableVoltagePoints[i + 1] > inListRCData[j].fVoltage)
                                {
                                }
                                else
                                {
                                    fPreSoC = -99999;
                                    fPreVol = -99999;
                                }
                            }
                            else
                            {
                                fPreSoC = -99999;
                                fPreVol = -99999;
                            }
                        }
                        else
                        {
                            if (j == 0)
                            {
                                fAvgSoc = 0F;
                            }
                            else
                            {
                                j += 1;
                            }
                        }
                        break; //for(; j<)
                    }
                }   //for (; j < inListRCData.Count; j++)

                if (j < inListRCData.Count)
                {
                    if (fAvgSoc != -99999)
                    {
                        fAvgSoc = (fDesignCapacity - fCapaciDiff - fAvgSoc);	//convert to remaining
                        fAvgSoc *= (10000 / fDesignCapacity);		//convert to 10000C
                    }
                }
                else
                {
                    DataRow rdtmp = null;
                    for (int ij = inListRCData.Count - 1; ij >= 0; ij--)
                    {
                        if (Math.Abs(inListRCData[ij].fCurrent - 0) > 5)
                        {
                            rdtmp = inListRCData[ij];
                            break;
                        }
                    }
                    if (rdtmp != null)
                    {
                        if (Math.Abs(TableVoltagePoints[i] - rdtmp.fVoltage) < 10)
                        {
                            fAvgSoc = (fDesignCapacity - fCapaciDiff - inListRCData[inListRCData.Count - 1].fAccMah);	//convert to remaining
                            fAvgSoc *= (10000 / fDesignCapacity);			//convert to 10000C
                        }
                        else
                        {
                            uErr = 1;
                            //because leakage of information, CreateNewErrorLog() need to be done outside of this funciton
                            return bReturn;
                        }
                    }
                    else
                    {
                        uErr = 1;
                        //because leakage of information, CreateNewErrorLog() need to be done outside of this funciton
                        return bReturn;
                    }
                }
                if (fAvgSoc > 10000) fAvgSoc = 10000.0F;
                //if (fAvgSoc < 0) fAvgSoc = -9999.0F;
                if (fAvgSoc < 0) fAvgSoc = 0;//fAvgSoc -= 1.0F;
                ypoints.Add(Convert.ToInt32(Math.Round(fAvgSoc, 0)));
                //i += 1;
                if (i >= TableVoltagePoints.Count)
                {
                    break;
                }

            }   //for (i = 0; i < TableVoltagePoints.Count; i++)

            if (ypoints.Count == TableVoltagePoints.Count - 1)
            {
                fAvgSoc = inListRCData[inListRCData.Count - 1].fAccMah;
                fAvgSoc = (fDesignCapacity - fCapaciDiff - fAvgSoc);	//convert to remaining
                fAvgSoc *= (10000 / fDesignCapacity);		//convert to 10000C
                if (fAvgSoc > 10000) fAvgSoc = 10000.0F;
                if (fAvgSoc < 0) fAvgSoc = -9999.0F;
                ypoints.Add(Convert.ToInt32(Math.Round(fAvgSoc, 0)));
                i += 1;
            }

            if (ypoints.Count == TableVoltagePoints.Count)
            {
                ypoints.Sort();
                bReturn = true;
            }
            else
            {
                for (int ii = ypoints.Count; ii < TableVoltagePoints.Count; ii++)
                {
                    ypoints.Add(0);
                }
                ypoints.Sort();
                uErr = 1;
                //because leakage of information, CreateNewErrorLog() need to be done outside of this funciton
            }
            TableVoltagePoints.Sort();
            return bReturn;
        }
        private static void CalculateCTASlopeBase(List<float> lstXVals, List<float> lstYVals, int inclusiveStart, int exclusiveEnd,
                                   out double rsquared, out double yintercept,
                                   out double slope)
        {
            Debug.Assert(lstXVals.Count() == lstYVals.Count());
            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            //double meanOfX = 0;
            //double meanOfY = 0;
            double ssX = 0;
            double ssY = 0;
            double sumCodeviates = 0;
            double sCo = 0;
            double count = exclusiveEnd - inclusiveStart;

            for (int ctr = inclusiveStart; ctr < exclusiveEnd; ctr++)
            {
                double x = lstXVals[ctr];
                double y = lstYVals[ctr];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }
            ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            ssY = sumOfYSq - ((sumOfY * sumOfY) / count);
            double RNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            double RDenom = (count * sumOfXSq - (sumOfX * sumOfX))
             * (count * sumOfYSq - (sumOfY * sumOfY));
            sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            double meanX = sumOfX / count;
            double meanY = sumOfY / count;
            double dblR = RNumerator / Math.Sqrt(RDenom);
            rsquared = dblR * dblR;
            yintercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;
        }

        public static TableMakerProduct GenerateRCTable(Project project, RCModel rcModel)
        {
            var OutFolder = $@"{GlobalSettings.RootPath}{project.BatteryType.Name}\{project.Name}\{GlobalSettings.ProductFolderName}";
            string filePath = Path.Combine(OutFolder, rcModel.FilePath);//GetRCTableFilePath(project);
            var strRCHeader = GetRCFileHeader(project, rcModel.fCTABase, rcModel.fCTASlope);
            var strRCContent = GetRCFileContent(rcModel.outYValue, project.VoltagePoints, rcModel.listfTemp, rcModel.listfCurr);
            //UInt32 uErr = 0;
            TableMakerService.CreateFile(filePath, strRCHeader.Concat(strRCContent).ToList());
            TableMakerProduct tmp = new TableMakerProduct();
            tmp.FilePath = filePath;
            tmp.IsValid = true;
            tmp.Project = project;
            return tmp;
        }
        public static string GetRCTableFilePath(Project project)
        {
            string sFileSeperator = "_";
            //(A170308)Francis, falconly use file output folder
            string outputFilePath = "RC" + sFileSeperator + project.BatteryType.Manufacturer +
                                sFileSeperator + project.BatteryType.Name +
                                sFileSeperator + project.AbsoluteMaxCapacity + "mAhr" +
                                sFileSeperator + project.LimitedChargeVoltage + "mV" +
                                sFileSeperator + project.CutoffDischargeVoltage + "mV" +
                                sFileSeperator + TableMakerService.Version +
                                sFileSeperator + DateTime.Now.ToString("yyyyMMddHHmmss") +
                                ".txt";
            return outputFilePath;
        }
        private static List<string> GetRCFileHeader(Project project, double fCTABase, double fCTASlope)
        {
            List<string> strRCHeader = new List<string>();

            strRCHeader.Clear();
            strRCHeader.Add(string.Format("//[Description]"));
            strRCHeader.Add(string.Format("// Remaining Capacity as a function of cell capacity"));
            strRCHeader.Add(string.Format("// This table is used during discharging mode, to determine remaining cell capacity in "));
            strRCHeader.Add(string.Format("// particular condition, based on cell voltage, discharging current and cell temperature. "));
            strRCHeader.Add(string.Format(""));
            strRCHeader.Add(string.Format("// Please note that the cell must in discharging mode and  voltage is under table "));
            strRCHeader.Add(string.Format("// definition range, otherwise it will be considerable in error. "));
            strRCHeader.Add(string.Format(""));
            strRCHeader.Add(string.Format("//Table Header Information:"));
            strRCHeader.Add(string.Format(""));
            strRCHeader.Add(string.Format("//Manufacturer = {0}", project.BatteryType.Manufacturer));
            strRCHeader.Add(string.Format("//Battery Type = {0}", project.BatteryType.Name));
            strRCHeader.Add(string.Format("//Equipment = "));
            strRCHeader.Add(string.Format("//Built Date = {0} {1} {2}", DateTime.Now.Year.ToString("D4"), DateTime.Now.Month.ToString("D2"), DateTime.Now.Day.ToString("D2")));
            strRCHeader.Add(string.Format("//MinimalVoltage = {0}", project.CutoffDischargeVoltage));
            strRCHeader.Add(string.Format("//MaximalVoltage = {0}", project.LimitedChargeVoltage));
            strRCHeader.Add(string.Format("//FullAbsoluteCapacity = {0}", project.AbsoluteMaxCapacity));
            strRCHeader.Add(string.Format("//Age = {0}", "1"));
            strRCHeader.Add(string.Format("//Tester = "));
            strRCHeader.Add(string.Format("//Battery ID = "));
            strRCHeader.Add(string.Format("//Version = "));
            strRCHeader.Add(string.Format("//Date = "));
            strRCHeader.Add(string.Format("//Comment = "));
            strRCHeader.Add(string.Format(""));
            //(A20200826)Francis, for new CTA string, add after header comment
            strRCHeader.Add(string.Format("//CTABase = {0}", fCTABase));
            strRCHeader.Add(string.Format("//CTASlope = {0}", fCTASlope));
            return strRCHeader;
        }

        private static List<string> GetRCFileContent(List<List<Int32>> rcYval, List<int> TableVoltagePoints, List<float> listfTemp, List<float> listfCurr)
        {
            List<string> strRCContent = new List<string>();
            string strrctmp = "";
            float fTmp;
            bool bCRate = false;

            strRCContent.Clear();
            strRCContent.Add(string.Format(""));
            strRCContent.Add(string.Format("//Table 1 header: residual capacity lookup"));
            strRCContent.Add(string.Format(""));
            strRCContent.Add(string.Format("7 \t\t //DO NOT CHANGE: word length of header (including this length)"));
            strRCContent.Add(string.Format("1 \t\t //control word"));
            strRCContent.Add(string.Format("3 \t\t //number of axii"));
            strRCContent.Add(string.Format("{0} \t\t //'x' voltage axis entries", TableVoltagePoints.Count));
            strRCContent.Add(string.Format("{0} \t\t //'w' current axis entries", listfCurr.Count));
            strRCContent.Add(string.Format("{0} \t\t //'v' temp axis entries", listfTemp.Count));
            strRCContent.Add(string.Format("1 \t\t //'y' axis entries per 'x' axis entries"));
            strRCContent.Add(string.Format(""));
            strRCContent.Add(string.Format("//[Data]"));
            strRCContent.Add(string.Format("//'x' axis: voltage in mV "));
            TableVoltagePoints.Sort();
            strrctmp = "";
            foreach (UInt32 uvol in TableVoltagePoints)
            {
                strrctmp += string.Format("{0}, ", uvol);
            }
            //(A140917)Francis, bugid=15206, delete last comma ','
            strrctmp = strrctmp.Substring(0, strrctmp.Length - 2);
            //(E140917)
            strRCContent.Add(strrctmp);
            strRCContent.Add(string.Format(""));
            strrctmp = "";
            listfCurr.Sort();
            listfCurr.Reverse();
            foreach (float fcur in listfCurr)
            {
                //fTmp = ((fcur * (-1)) / fDesignCapacity) * iCRate;
                fTmp = (fcur * (-1));
                if (fTmp > 32767)
                {
                    bCRate = true;
                    break;
                }
                strrctmp += string.Format("{0}, ", (int)(fTmp + 0.5));
            }
            if (bCRate) //need to change to 100C
            {
                strrctmp = "";
                foreach (float fcur2 in listfCurr)
                {
                    //fTmp = ((fcur2 * (-1)) / fDesignCapacity) * iCRate;
                    fTmp = (fcur2 * (-1));
                    strrctmp += string.Format("{0}, ", (int)(fTmp + 0.5));
                }
            }
            //(A140917)Francis, bugid=15206, delete last comma ','
            strrctmp = strrctmp.Substring(0, strrctmp.Length - 2);
            //(E140917)
            strRCContent.Add(string.Format("//'w' axis: current in in mA format (minor axis of 2d  lookup)"));//{0}*C (minor axis of 2d  lookup)", iCRate));
            strRCContent.Add(strrctmp);
            strRCContent.Add(string.Format(""));
            strRCContent.Add(string.Format("//'v' axis: temperature (major axis of 2d lookup) in .1 degrees C"));
            listfTemp.Sort();       //no need, just for case
            strrctmp = "";
            foreach (float ftemper in listfTemp)
            {
                if (ftemper >= 0)
                    strrctmp += string.Format("{0}, ", Convert.ToInt32(Math.Round(ftemper * 10, 0)));
                else
                    strrctmp += string.Format("{0}, ", Convert.ToInt32(Math.Round(ftemper * 10, 0)));
            }
            //(A140917)Francis, bugid=15206, delete last comma ','
            strrctmp = strrctmp.Substring(0, strrctmp.Length - 2);
            //(E140917)
            strRCContent.Add(strrctmp);
            strRCContent.Add(string.Format(""));
            strRCContent.Add(string.Format("//capacity in 10000*C"));
            strRCContent.Add(string.Format(""));

            for (int it = 0; it < listfTemp.Count; it++)
            {
                strRCContent.Add(string.Format("//temp = {0} ^C", listfTemp[it]));
                for (int ic = 0; ic < listfCurr.Count; ic++)
                {
                    strrctmp = "";
                    ConvertRCDataToString(ref strrctmp, rcYval[it * listfCurr.Count + ic]);
                    strRCContent.Add(strrctmp);
                }
                strRCContent.Add(string.Format(""));
            }
            return strRCContent;
        }
        private static void ConvertRCDataToString(ref string strOutput, List<Int32> inVal)
        {
            strOutput = "";

            foreach (Int32 iy in inVal)
            {
                strOutput += iy.ToString() + ",";
            }
            //(A140917)Francis, bugid=15206, delete last comma ','
            strOutput = strOutput.Substring(0, strOutput.Length - 1);
            //(E140917)
        }
        #endregion
    }
}