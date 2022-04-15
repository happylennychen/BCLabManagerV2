using BCLabManager.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace BCLabManager
{
    public static class RCTableMaker
    {
        #region RC
        public static void GetRCModel(List<SourceData> SDList, int capacity, List<int> VoltagePoints, ref RCModel rcModel)
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
            CreateRCPoints_TableMini(ref uErr, ref output, SDList, capacity, VoltagePoints);
        }
        private static bool CreateRCPoints_TableMini(ref uint uErr, ref RCModel output, List<SourceData> sdList, int capacity, List<int> VoltagePoints)
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

            List<double> fCTAKeod = new List<double>();        //save Keod value, = Max_Diff_Temp * 10 / Joules_value for each current_temperature
            foreach (double ft in output.listfTemp)		//from low temperature to list
            {
                foreach (double fc in output.listfCurr)		//from low current to list
                {
                    foreach (var sds in sdList)
                    {
                        if ((sds.fTemperature == ft) &&
                            (sds.fCurrent == fc))
                        {
                            {
                                List<Int32> il16tmp = new List<Int32>();
                                //Int32 iCountP = 0;
                                double fCCount = 0;
                                double fMaxDiff = 0, fCalTmp = 0;
                                if (sds.AdjustedExpData.Count < 1)
                                {
                                    uErr = 1;
                                    bReturn &= false;
                                    break;	//foreach (SourceData sds in bdsBatRCSource)
                                }
                                else
                                {
                                    if (!CreateYPoints_CTAV0026(VoltagePoints, ref il16tmp, ref fMaxDiff, ref fCCount, sds.AdjustedExpData, ref uErr, capacity, 0))
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



            List<double> fAvgPerCurrent = new List<double>();     //save Keod'_favg for each temperature, skip Max_Keod_value and Min_Keod_value to calculate average
            List<double> fCTACurrent = new List<double>();     //save Keod'_favg for each temperature, skip Max_Keod_value and Min_Keod_value to calculate average
            if (true)
            {
                double fCalTmp = 0, fCalMax = 0, fCalMin = 10000;
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
                        fCTACurrent.Add(Math.Abs((double)(output.listfCurr[jj] / 1000)));
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
            fCTASlope = (double)((int)(fCTASlope + 0.5));
            fCTABase = (double)((int)(fCTABase * 10000 + 0.5));
            output.fCTABase = fCTABase;
            output.fCTASlope = fCTASlope;

            return bReturn;
        }

        private static bool CreateYPoints_CTAV0026(List<int> TableVoltagePoints, ref List<Int32> ypoints, ref double fRefMaxDiff, ref double fRefCCount, List<TableMakerSourceDataRow> inListRCData, ref UInt32 uErr, double fDesignCapacity, double fCapaciDiff)
        {
            bool bReturn = false;
            int i = 0, j = 0, iCountP = 0;
            double fPreSoC = -99999, fPreVol = -99999, fAvgSoc = -99999, fPreTemp = -9999, fPreCCount = 0;//, fAccCount = 0; ;
            //(A200902)Francis, use the low_cur header setting to calculate
            List<double> flstTmp = new List<double>();
            double fAccCap = 0.0f;

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
                            fRefCCount = (double)Math.Abs(inListRCData[j].fAccMah - fPreCCount);
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
                    TableMakerSourceDataRow rdtmp = null;
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
                //if (fAvgSoc < 0) fAvgSoc = 0;//fAvgSoc -= 1.0F;
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
        private static void CalculateCTASlopeBase(List<double> lstXVals, List<double> lstYVals, int inclusiveStart, int exclusiveEnd,
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

        public static TableMakerProduct GenerateRCTable(Stage stage, Project project, List<int> VoltagePoints, string time, RCModel rcModel)
        {
            var OutFolder = $@"{GlobalSettings.LocalPath}{project.BatteryType.Name}\{project.Name}\{GlobalSettings.ProductFolderName}\{time}";
            if (!Directory.Exists(OutFolder))
            {
                Directory.CreateDirectory(OutFolder);
            }
            string localPath = Path.Combine(OutFolder, rcModel.FileName);//GetRCTableFilePath(project);
            var strRCHeader = GetRCFileHeader(stage, project, rcModel.fCTABase, rcModel.fCTASlope);
            var strRCContent = GetRCFileContent(rcModel.outYValue, VoltagePoints, rcModel.listfTemp, rcModel.listfCurr);
            //UInt32 uErr = 0;
            TableMakerService.CreateFileFromLines(localPath, strRCHeader.Concat(strRCContent).ToList());
            string remotePath, MD5;
            FileTransferHelper.FileUpload(localPath, out remotePath, out MD5);
            TableMakerProduct tmp = new TableMakerProduct();
            tmp.FilePath = remotePath;
            tmp.MD5 = MD5;
            tmp.IsValid = true;
            //tmp.Project = project;
            tmp.Type = TableMakerService.GetFileType("RC", stage);
            return tmp;
        }
        public static string GetRCTableFileName(Project project, Stage stage)
        {
            string description = (stage == Stage.N1) ? "stage1" : "stage2";
            string sFileSeperator = "_";
            //(A170308)Francis, falconly use file output folder
            //string outputFilePath = "OCV" + sFileSeperator + project.BatteryType.Manufacturer +
            //                    sFileSeperator + project.BatteryType.Name +
            //                    sFileSeperator + project.AbsoluteMaxCapacity.ToString() + "mAhr" +
            //                    sFileSeperator + project.LimitedChargeVoltage + "mV" +
            //                    sFileSeperator + project.CutoffDischargeVoltage + "mV" +
            //                    sFileSeperator + TableMakerService.Version +
            //                    sFileSeperator + description +
            //                    "_Arm.txt";
            string outputFilePath = project.BatteryType.Manufacturer + sFileSeperator + project.BatteryType.Name + sFileSeperator + project.AbsoluteMaxCapacity.ToString() + "mAhr" + sFileSeperator + "RC" + sFileSeperator + description + ".txt";
            return outputFilePath;
        }
        private static List<string> GetRCFileHeader(Stage stage, Project project, double fCTABase, double fCTASlope)
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
            int type_id = TableMakerService.GetFileTypeID("RC", stage);
            strRCHeader.Add(string.Format("//type_id = {0}", type_id.ToString()));
            //(A20200826)Francis, for new CTA string, add after header comment
            strRCHeader.Add(string.Format("//CTABase = {0}", fCTABase));
            strRCHeader.Add(string.Format("//CTASlope = {0}", fCTASlope));
            return strRCHeader;
        }

        public static List<SourceData> GetNewSources(List<SourceData> rcStage2Sources, List<SourceData> rcStage1Sources)
        {
            List<SourceData> output = new List<SourceData>();
            List<double> stage1Currents = GetCurrentsFromSources(rcStage1Sources);
            List<double> stage2Currents = GetCurrentsFromSources(rcStage2Sources);
            List<double> stage1Temperatures = GetTemperaturesFromSources(rcStage1Sources);
            List<double> stage2Temperatures = GetTemperaturesFromSources(rcStage2Sources);
            List<double> newCurrents, newTemperatures;
            GetNewPoints(stage1Currents, stage1Temperatures, stage2Currents, stage2Temperatures, out newCurrents, out newTemperatures);
            foreach (var curr in newCurrents)
            {
                foreach (var temp in newTemperatures)
                {
                    var Source = rcStage1Sources.SingleOrDefault(o => o.fCurrent == curr && o.fTemperature == temp);
                    if (Source != null)     //新资料
                    {
                        output.Add(Source);
                    }
                    else
                    {
                        Source = rcStage2Sources.SingleOrDefault(o => o.fCurrent == curr && o.fTemperature == temp);

                        if (Source != null)     //旧资料
                        {
                            output.Add(Source);
                        }
                        else if (Source == null)  //需要填充的资料
                        {
                            if (stage1Currents.Contains(curr))    //新电流
                            {
                                var temps = rcStage1Sources.Where(o => o.fCurrent == curr).Select(o => o.fTemperature);   //找到新电流对应的所有温度
                                var nearestTemp = temps.OrderBy(o => Math.Abs(o - temp)).First();  //再从中找到温度最接近的那一个
                                var stage1Src = rcStage1Sources.SingleOrDefault(o => o.fTemperature == nearestTemp && o.fCurrent == curr);
                                var blendSrc = stage1Src.ShallowCopy();
                                blendSrc.fTemperature = (double)temp;    //修改电流
                                output.Add(blendSrc);
                            }
                            else if (stage1Temperatures.Contains(temp))
                            {
                                var currs = rcStage1Sources.Where(o => o.fTemperature == temp).Select(o => o.fCurrent);
                                var nearestCurr = currs.OrderBy(o => Math.Abs(o - curr)).First();
                                var stage1Rec = rcStage1Sources.SingleOrDefault(o => o.fCurrent == nearestCurr && o.fTemperature == temp);
                                var blendSrc = stage1Rec.ShallowCopy();
                                blendSrc.fCurrent = (double)curr;
                                output.Add(blendSrc);
                            }
                        }
                    }
                }
            }
            return output;
        }
        private static void GetNewPoints(List<double> stage1Currents, List<double> stage1Temperatures, List<double> stage2Currents, List<double> stage2Temperatures, out List<double> newCurrents, out List<double> newTemperatures)
        {
            newCurrents = new List<double>();
            newTemperatures = new List<double>();
            stage1Currents.Sort();
            stage1Temperatures.Sort();
            stage2Currents.Sort();
            stage2Temperatures.Sort();
            foreach (var oldCurr in stage2Currents)
            {
                bool isReplaced = false;
                foreach (var newCurr in stage1Currents)
                {
                    if (Math.Abs(newCurr - oldCurr) < 500)
                    {
                        newCurrents.Add(newCurr);
                        isReplaced = true;
                        break;
                    }
                }
                if (!isReplaced)
                {
                    newCurrents.Add(oldCurr);
                }
            }
            foreach (var newCurr in stage1Currents)
            {
                if (!newCurrents.Contains(newCurr))
                    newCurrents.Add(newCurr);
            }

            foreach (var oldTemp in stage2Temperatures)
            {
                bool isReplaced = false;
                foreach (var newTemp in stage1Temperatures)
                {
                    if (Math.Abs(newTemp - oldTemp) < 5)
                    {
                        newTemperatures.Add(newTemp);
                        isReplaced = true;
                        break;
                    }
                }
                if (!isReplaced)
                {
                    newTemperatures.Add(oldTemp);
                }
            }
            foreach (var newTemp in stage1Temperatures)
            {
                if (!newTemperatures.Contains(newTemp))
                    newTemperatures.Add(newTemp);
            }
        }
        private static List<double> GetTemperaturesFromSources(List<SourceData> sources)
        {
            return sources.Select(o => o.fTemperature).Distinct().OrderBy(o => o).ToList();
        }

        private static List<double> GetCurrentsFromSources(List<SourceData> sources)
        {
            return sources.Select(o => o.fCurrent).Distinct().OrderBy(o => o).ToList();
        }

        private static List<string> GetRCFileContent(List<List<Int32>> rcYval, List<int> TableVoltagePoints, List<double> listfTemp, List<double> listfCurr)
        {
            List<string> strRCContent = new List<string>();
            string strrctmp = "";
            double fTmp;
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
            foreach (double fcur in listfCurr)
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
                foreach (double fcur2 in listfCurr)
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
            foreach (double ftemper in listfTemp)
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