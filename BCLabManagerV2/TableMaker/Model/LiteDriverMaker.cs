﻿using BCLabManager.Model;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace BCLabManager
{
    public static class LiteDriverMaker
    {
        public static bool GetLiteModel(UInt32 uEoDVoltage, List<SourceData> ocvSource, List<SourceData> rcSource, OCVModel ocvModel, RCModel rcModel, Project project, List<int> VoltagePoints, ref LiteModel liteModel)
        {
            List<double> fLstTblM_OCV;
            List<int> iLstTblM_SOC1;
            GetLstTblM_OCV(ocvSource, out fLstTblM_OCV, out iLstTblM_SOC1);
            List<double> flstTblOCVCof;
            if(!GetLstTblOCVCof(fLstTblM_OCV, iLstTblM_SOC1, out flstTblOCVCof))
                return false;
            liteModel.flstTblOCVCof = flstTblOCVCof;
            liteModel.ilistCurr = rcModel.listfCurr.Select(o => (short)o).ToList();
            List<double> flstKeodCont;
            List<double> flstAccAtEoD;
            GetLstKeodCont(uEoDVoltage, ref rcModel, project, VoltagePoints, out flstKeodCont, out flstAccAtEoD);
            List<double[]> flstdbDCapCof;
            List<double[]> flstdbKeodCof;
            var ilstTemp = rcModel.listfTemp.Select(o => (short)(o * 10)).ToList();
            GetDCapCof(ilstTemp, liteModel.ilistCurr, flstKeodCont, flstAccAtEoD, project.AbsoluteMaxCapacity, out flstdbDCapCof, out flstdbKeodCof);
            liteModel.flstdbDCapCof = flstdbDCapCof;
            liteModel.flstdbKeodCof = flstdbKeodCof;
            return true;
        }
        private static bool CreateRCPoints_TableMini(UInt32 uEoDVoltage, ref uint uErr, ref RCModel rcModel, List<SourceData> sdList, Project project, List<int> VoltagePoints, out List<double> flstKeodContent, out List<double> flstAccAtEoD)
        {
            bool bReturn = true;	//cause below will use bReturn &= xxxx
            bool bDoMiniCal = false;
            flstAccAtEoD = new List<double>();
            flstKeodContent = new List<double>();

            foreach (List<Int32> il in rcModel.outYValue)
            {
                il.Clear();
            }
            rcModel.outYValue.Clear();
            rcModel.listfTemp.Sort();
            rcModel.listfCurr.Sort();
            rcModel.listfCurr.Reverse();
            //fYPointACCall.Clear();
            List<double> fCTADiffMax = new List<double>();     //debug use, saving Max_Diff_Temp for each current_temperature
            List<Int32> iCTACountP = new List<Int32>();      //debug use, saving count number of valid calculated RawDataNode for each current_temperature
            List<double> fCTAJoules = new List<double>();      //saving Joules value, = Count_Number / 3600 for each current_temperature
            List<double> fCTAKeod = new List<double>();        //save Keod value, = Max_Diff_Temp * 10 / Joules_value for each current_temperature
            List<double> fAvgPerCurrent = new List<double>();     //save Keod'_favg for each temperature, skip Max_Keod_value and Min_Keod_value to calculate average
            List<double> fCTACurrent = new List<double>();     //save Keod'_favg for each temperature, skip Max_Keod_value and Min_Keod_value to calculate average
            List<double> fDCapCof = new List<double>();
            List<Int32> iLstRCM_SOC1 = new List<Int32>();

            fCTADiffMax.Clear();
            iCTACountP.Clear();
            fCTAJoules.Clear();
            fCTAKeod.Clear();
            fDCapCof.Clear();
            fAvgPerCurrent.Clear();
            iLstRCM_SOC1.Clear();
            //flstDCapAtEoD.Clear();
            //for (int iRC = 0; iRC < fLstRCM_Volt.Count; iRC++)
            //{
            //    fLstRCM_Volt[iRC].Clear();
            //}
            //fLstRCM_Volt.Clear();
            for (int jj = 0; jj < (TableMakerService.iNumOfMiniPoints + 1); jj++)
            {
                iLstRCM_SOC1.Add(((OCVTableMaker.iMaxPercent - OCVTableMaker.iMinPercent) / TableMakerService.iNumOfMiniPoints) * (TableMakerService.iNumOfMiniPoints - jj));
            }
            foreach (double ft in rcModel.listfTemp)		//from low temperature to list
            {
                foreach (double fc in rcModel.listfCurr)		//from low current to list
                {
                    foreach (var sds in sdList)
                    {
                        if ((sds.fTemperature == ft) &&
                            (sds.fCurrent == fc))
                        {
                            List<Int32> il16tmp = new List<Int32>();
                            //Int32 iCountP = 0;
                            double fCCount = 0;
                            double fMaxDiff = 0, fCalTmp = 0;
                            //double fDCapAtEoD = 0, 
                            double fKeodAtEach = 0;
                            if (sds.AdjustedExpData.Count < 1)
                            {
                                uErr = 1;
                                bReturn &= false;
                                break;	//foreach (SourceData sds in bdsBatRCSource)
                            }
                            else
                            {
                                if (fc == rcModel.listfCurr[0])
                                    bDoMiniCal = true;
                                else
                                    bDoMiniCal = false;
                                //if (!CreateYPoints_CTAV0026(ref il16tmp, ref fMaxDiff, ref fCCount, sds.AdjustedExpData, ref uErr, bDoMiniCal))
                                if (!CreateYPoints_CTAV26_TBLLite(uEoDVoltage, VoltagePoints, ref il16tmp, ref fMaxDiff, ref fCCount, sds.AdjustedExpData, ref uErr, project.AbsoluteMaxCapacity, rcModel.MinVoltage, ref fKeodAtEach, ref flstAccAtEoD, bDoMiniCal))
                                {
                                    bReturn &= false;
                                }
                                else
                                {
                                }
                                rcModel.outYValue.Add(il16tmp); //if error, still add into RCYvalue
                                fCTADiffMax.Add(fMaxDiff);
                                //iCTACountP.Add(iCountP);
                                //fCalTmp = ((double)(iCountP) / 3600) * Math.Abs(fc);
                                fCalTmp = fCCount;
                                fCTAJoules.Add(fCalTmp);
                                fMaxDiff *= 10;
                                fMaxDiff /= fCalTmp;
                                fCTAKeod.Add(fMaxDiff);
                                //ilstCNumOfRCLine.Add(sds.AdjustedExpData.Count);
                                fKeodAtEach *= 10;
                                fKeodAtEach /= ((double)sds.AdjustedExpData.Count / 3600.0f) * (Math.Abs(sds.fCurrent) / 1000);
                                flstKeodContent.Add(fKeodAtEach);
                                //fDCapTmp.Add(fDCapAtEoD);
                                //fKeodTmp.Add(fKeodAtEach);
                                break;  //foreach (SourceDataSample sds in bdsBatRCSource)
                            }
                        }
                    }
                }
            }
            //bReturn &= true;

            //(M20200813) for CTA calculation
            //calculate Keod'_favg for each temperature
            if (bReturn)
            {
                double fCalTmp = 0, fCalMax = 0, fCalMin = 10000;
                int idxKeod = 0;
                for (int jj = 0; jj < rcModel.listfCurr.Count(); jj++)
                {
                    fCalTmp = 0;
                    fCalMax = 0;
                    fCalMin = 10000;
                    for (int ii = 0; ii < rcModel.listfTemp.Count(); ii++)
                    {
                        idxKeod = ((ii * rcModel.listfCurr.Count()) + jj);
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
                        fCalTmp /= (rcModel.listfTemp.Count() - 2);
                        fAvgPerCurrent.Add(fCalTmp);
                        fCTACurrent.Add(Math.Abs(rcModel.listfCurr[jj] / 1000));
                    }
                }
                if (fAvgPerCurrent.Count() != rcModel.listfCurr.Count())
                {
                    uErr = 1;
                    bReturn = false;
                }
            }

            //if (bReturn)
            //{
            //    fCTABase = 0;
            //    fCTASlope = 1;
            //    fSquare = 1;
            //    CalculateCTASlopeBase(fCTACurrent, fAvgPerCurrent, 0, fAvgPerCurrent.Count(), out fSquare, out fCTABase, out fCTASlope);
            //    fCTASlope *= 10000;
            //    fCTASlope = (double)((int)(fCTASlope + 0.5));
            //    fCTABase = (double)((int)(fCTABase * 10000 + 0.5));
            //    GenerateTableMiniTempFile(ref uErr);
            //    GenerateTableLiteTempFile(ref uErr);
            //}


            return bReturn;
        }

        private static bool CreateYPoints_CTAV26_TBLLite(UInt32 uEoDVoltage, List<int> TableVoltagePoints, ref List<Int32> ypoints, ref double fRefMaxDiff, ref double fRefCCount, List<TableMakerSourceDataRow> inListRCData, ref UInt32 uErr, double fDesignCapacity, int iMinVoltage, ref double _fKeodAtEach, ref List<double> flstDCapAtEoD, bool bDoMini = false)
        {
            double fCapaciDiff = 0;
            bool bReturn = false;
            int i = 0, j = 0, iCountP = 0;
            double fPreSoC = -99999, fPreVol = -99999, fAvgSoc = -99999, fPreTemp = -9999, fPreCCount = 0;//, fAccCount = 0; ;
            String strDbg;
            //(A200902)Francis, use the low_cur header setting to calculate
            double fFullCapacity = 0, fACCMiniStep = 0;// = OCVSample.fMiniTableSteps * 0.01F * (TableSourceData[0].myHeader.fFullCapacity - TableSourceData[0].myHeader.fCapacityDiff);
            List<double> flstTmp = new List<double>();
            double fAccCap = 0.0f, fDCapEoD = 0.0f, fKeodCal = 0.0f;

            //if (bDoMini)
            {
                fFullCapacity = fDesignCapacity;
                fACCMiniStep = TableMakerService.fMiniTableSteps * 0.01F * (fFullCapacity);
            }

            TableVoltagePoints.Sort();
            TableVoltagePoints.Reverse();
            ypoints.Clear();
            fRefMaxDiff = 0;
            fRefCCount = 0;

            //(M140718)Francis,
            i = 0; j = 0; _fKeodAtEach = 0;
            for (i = 0; i < TableVoltagePoints.Count; i++)
            {
                for (; j < inListRCData.Count; j++)
                {
                    //(A20201015), memorize maximum accumulated capacity
                    //if (Math.Abs(inListRCData[j].fAccMah) > fAccCap)
                    //    fAccCap = Math.Abs(inListRCData[j].fAccMah);
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
                            fRefCCount = Math.Abs(inListRCData[j].fAccMah - fPreCCount);
                        }
                    }
                    //(A201208) for table_lite
                    if ((inListRCData[j].fVoltage <= uEoDVoltage) && (fDCapEoD < 0.1f))
                    {
                        if (inListRCData[j].fVoltage == uEoDVoltage)
                        {
                            fDCapEoD = inListRCData[j].fAccMah;
                        }
                        else
                        {
                            double fBase = Math.Abs(inListRCData[j - 1].fVoltage - inListRCData[j].fVoltage);
                            double fTmp = Math.Abs(inListRCData[j - 1].fAccMah - inListRCData[j].fAccMah);
                            fTmp *= Math.Abs(uEoDVoltage - inListRCData[j].fVoltage) / fBase;
                            fDCapEoD = inListRCData[j].fAccMah - fTmp;
                        }
                    }
                    if (j != 0)
                    {
                        fKeodCal = inListRCData[j].fTemperature - inListRCData[0].fTemperature;
                        _fKeodAtEach = Math.Max(_fKeodAtEach, fKeodCal);
                    }
                    //(E201208)

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

            //(A200902)francis, table_mini
            //fYPointACCall.Add(fAccCap);
            //(A201208)for table lite
            if (fDCapEoD < 0.1f)
                fDCapEoD = inListRCData[inListRCData.Count - 1].fAccMah;
            flstDCapAtEoD.Add(fDCapEoD);
            for (; j < inListRCData.Count; j++)
            {
                if (j != 0)
                {
                    fKeodCal = inListRCData[j].fTemperature - inListRCData[0].fTemperature;
                    _fKeodAtEach = Math.Max(_fKeodAtEach, fKeodCal);
                }
            }
            //if (bDoMini)
            {
                i = 0;
                fPreVol = inListRCData[0].fVoltage;
                flstTmp.Clear();
                for (j = 0; j < inListRCData.Count; j++)
                {
                    if (i < TableMakerService.iNumOfMiniPoints)
                    {
                        if (inListRCData[j].fAccMah < (i * fACCMiniStep))
                        {
                            fPreVol = inListRCData[j].fVoltage;
                        }
                        else
                        {
                            flstTmp.Add(fPreVol);
                            i++;
                        }
                    }
                }
                for (int ij = flstTmp.Count; ij <= TableMakerService.iNumOfMiniPoints; ij++)
                {
                    flstTmp.Add(iMinVoltage);
                }
                //if (bDoMini)
                //    fLstRCM_Volt.Add(flstTmp);
                //fLstRCL_Volt.Add(flstTmp);
            }

            return bReturn;
        }

        private static void GetLstKeodCont(UInt32 uEoDVoltage, ref RCModel rcModel, Project project, List<int> VoltagePoints, out List<double> flstKeodCont, out List<double> flstAccAtEoD)
        {
            uint uErr = 0;
            CreateRCPoints_TableMini(uEoDVoltage, ref uErr, ref rcModel, rcModel.SourceList, project, VoltagePoints, out flstKeodCont, out flstAccAtEoD);
        }

        private static void GetLstTblM_OCV(List<SourceData> ocvSource, out List<double> fLstTblM_OCV, out List<int> iLstTblM_SOC1)
        {
            fLstTblM_OCV = new List<double>();
            iLstTblM_SOC1 = new List<int>();
            SourceData lowcurSample, higcurSample;
            if (Math.Abs(ocvSource[0].fCurrent) < Math.Abs(ocvSource[1].fCurrent))
            {
                lowcurSample = ocvSource[0];
                higcurSample = ocvSource[1];
            }
            else
            {
                lowcurSample = ocvSource[1];
                higcurSample = ocvSource[0];
            }

            double fTmpVolt1;
            int indexLow = 0, indexHigh = 0;
            for (int fi = OCVTableMaker.iMaxPercent; fi >= OCVTableMaker.iMinPercent;
                fi -= ((OCVTableMaker.iMaxPercent - OCVTableMaker.iMinPercent) / TableMakerService.iNumOfMiniPoints))
            {
                double fSoCVoltLow = 0; double fSoCVoltHigh = 0;  //default value
                #region find <10000, Volt>, <9900, Volt> from low current data
                double fTmpSoC1 = 0; //fTmpSoC2 = 0; //default value
                double fSoCbk = 0; double fVoltbk = 0;    //default value
                for (; indexLow < lowcurSample.AdjustedExpData.Count; indexLow++)
                {
                    fTmpVolt1 = lowcurSample.AdjustedExpData[indexLow].fVoltage;
                    fTmpSoC1 = lowcurSample.AdjustedExpData[indexLow].fSoCAdj;
                    if (fi >= fTmpSoC1)
                    {
                        if (indexLow == 0)  //first record
                        {
                            fSoCVoltLow = fTmpVolt1;
                            fSoCbk = fTmpSoC1;
                            fVoltbk = fTmpVolt1;
                        }
                        else
                        {
                            if (fi == fTmpSoC1)
                            {
                                fSoCVoltLow = fTmpVolt1;
                                fSoCbk = fTmpSoC1;
                                fVoltbk = fTmpVolt1;
                            }
                            else
                            {
                                fSoCVoltLow = fTmpVolt1;    //no modify fSoCbk, fVoltbk
                            }
                        }
                        break;
                    }
                    else
                    {
                        fSoCbk = fTmpSoC1;
                        fVoltbk = fTmpVolt1;
                    }
                }   //for(;
                if (indexLow < lowcurSample.AdjustedExpData.Count)      //in experiment range
                {
                    if ((Math.Abs(fSoCbk - fi) > 5) ||
                        (Math.Abs(fi - fTmpSoC1) > 5))//SoC difference is bigger than 5, error
                    {
                        MessageBox.Show("Wrong");
                    }
                    else
                    {
                        //if in 5 (10000C unit) range, use linear interpolation
                        if (fSoCbk != fTmpSoC1)     //must use this!!
                        {
                            fSoCVoltLow += (fVoltbk - fSoCVoltLow) * (fi - fTmpSoC1) / (fSoCbk - fTmpSoC1);
                        }
                    }
                }
                else        //out of experiment range
                {
                    //use linear extrapolation, use last one point copy currently and temporarily 
                    if (fVoltbk != 0)
                    {
                        fSoCVoltLow = fVoltbk;
                    }
                    else
                    {
                        if (lowcurSample.AdjustedExpData.Count > 1)
                            fSoCVoltLow = lowcurSample.AdjustedExpData[lowcurSample.AdjustedExpData.Count - 1].fVoltage;
                    }
                }
                #endregion

                #region find <10000, Volt>, <9900, Volt> from high current data
                fTmpSoC1 = 0; //fTmpSoC2 = 0; //default value
                fSoCbk = 0; fVoltbk = 0;    //default value
                for (; indexHigh < higcurSample.AdjustedExpData.Count; indexHigh++)
                {
                    fTmpVolt1 = higcurSample.AdjustedExpData[indexHigh].fVoltage;
                    fTmpSoC1 = higcurSample.AdjustedExpData[indexHigh].fSoCAdj;
                    if (fi >= fTmpSoC1)
                    {
                        if (indexHigh == 0) //first record
                        {
                            fSoCVoltHigh = fTmpVolt1;
                            fSoCbk = fTmpSoC1;
                            fVoltbk = fTmpVolt1;
                        }
                        else
                        {
                            if (fi == fTmpSoC1)
                            {
                                fSoCVoltHigh = fTmpVolt1;
                                fSoCbk = fTmpSoC1;
                                fVoltbk = fTmpVolt1;
                            }
                            else
                            {
                                fSoCVoltHigh = fTmpVolt1;   //no modify fSoCbk, fVoltbk
                            }
                        }
                        break;
                    }
                    else
                    {
                        fSoCbk = fTmpSoC1;
                        fVoltbk = fTmpVolt1;
                    }
                }   //for(;
                if (indexHigh < higcurSample.AdjustedExpData.Count)     //in experiment range
                {
                    if ((Math.Abs(fSoCbk - fi) > 5) ||
                        (Math.Abs(fi - fTmpSoC1) > 5))//SoC difference is bigger than 5, error
                    {
                        MessageBox.Show("Wrong");
                    }
                    else
                    {
                        //if in 5 (10000C unit) range, use linear interpolation
                        if (fSoCbk != fTmpSoC1)     //must use this!!
                        {
                            fSoCVoltHigh += (fVoltbk - fSoCVoltHigh) * (fi - fTmpSoC1) / (fSoCbk - fTmpSoC1);
                        }
                    }
                }
                else        //out of experiment range
                {
                    //use linear extrapolation, use last one point copy currently and temporarily 
                    if (fVoltbk != 0)
                    {
                        fSoCVoltHigh = fVoltbk;
                    }
                    else
                    {
                        if (higcurSample.AdjustedExpData.Count > 1)
                            fSoCVoltHigh = higcurSample.AdjustedExpData[higcurSample.AdjustedExpData.Count - 1].fVoltage;
                    }
                }
                #endregion

                double fRsocn = (fSoCVoltLow - fSoCVoltHigh) / (Math.Abs(higcurSample.fCurrent) - Math.Abs(lowcurSample.fCurrent));
                fTmpVolt1 = fSoCVoltLow + Math.Abs(lowcurSample.fCurrent) * fRsocn;
                if (fTmpVolt1 > lowcurSample.fLimitChgVolt) fTmpVolt1 = lowcurSample.fLimitChgVolt;
                if (fTmpVolt1 < lowcurSample.fCutoffDsgVolt) fTmpVolt1 = lowcurSample.fCutoffDsgVolt;
                if (fTmpVolt1 < 0) MessageBox.Show("Minus Voltage Got");    //Fran debug
                iLstTblM_SOC1.Add(fi);
                fLstTblM_OCV.Add(fTmpVolt1 + 0.5F);
            }
        }

        private static bool GetLstTblOCVCof(List<double> fLstTblM_OCV, List<int> iLstTblM_SOC1, out List<double> flstTblOCVCof)
        {
            flstTblOCVCof = new List<double>();
            CreatePolyDataFile(fLstTblM_OCV, iLstTblM_SOC1);
            if (!RunExcel())
                return false;
            flstTblOCVCof = LoadFromFile();
            DeleteTmpFile();
            return true;
        }

        private static void DeleteTmpFile()
        {
            if (File.Exists("poly_cof.txt"))
                File.Delete("poly_cof.txt");
            if (File.Exists("Poly_data.txt"))
                File.Delete("Poly_data.txt");
        }

        private static List<double> LoadFromFile()
        {
            List<double> flstTblOCVCof = new List<double>();

            var strtmpfu = "poly_cof.txt";
            if (File.Exists(strtmpfu))
            {
                var stmRead = new StreamReader(strtmpfu);

                if (stmRead != null)
                {
                    flstTblOCVCof.Clear();
                    string strcont;
                    double fTmp;
                    while ((strcont = stmRead.ReadLine()) != null)
                    {
                        if (double.TryParse(strcont, out fTmp))
                        {
                            flstTblOCVCof.Add(fTmp);
                        }
                    }
                }
                stmRead.Close();
            }
            return flstTblOCVCof;
        }

        private static bool RunExcel()
        {
            ProcessStartInfo psInfo = new ProcessStartInfo();
            psInfo.CreateNoWindow = false;
            psInfo.UseShellExecute = true;
            psInfo.FileName = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "tableVBARunner.vbs");
            psInfo.WindowStyle = ProcessWindowStyle.Hidden;
            psInfo.Arguments = " \"" + Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "table_poly_coeff_Generator.xlsm") + "\" "
                                + "TableMaker_ExcelGen" + " " + "MacroMain";

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(psInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch(Exception e)
            {
                MessageBox.Show($"Cannot run excel file while making lite model, please check if the excel related file exist or not.");
                return false;
            }
            return true;
        }

        private static void CreatePolyDataFile(List<double> fLstTblM_OCV, List<int> iLstTblM_SOC1)
        {
            try
            {
                FileStream fs = File.Open("Poly_data.txt", FileMode.Create, FileAccess.Write, FileShare.None);
                StreamWriter sw = new StreamWriter(fs, new UTF8Encoding(false));// Encoding.Default);
                string strWriteTo = string.Format("LINEST, 7, false, true");
                sw.WriteLine(strWriteTo);
                for (int j = 0; j < iLstTblM_SOC1.Count; j++)
                {
                    strWriteTo = string.Format("{0}, {1},", Convert.ToInt16(iLstTblM_SOC1[j]), fLstTblM_OCV[j]);
                    sw.WriteLine(strWriteTo);
                }

                sw.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Create Poly_data.txt failed.");
            }
        }

        private static void GetDCapCof(List<short> ilstTemp, List<short> ilstCurr, List<double> flstKeodCont, List<double> flstAccAtEoD, double fullCapacity, out List<double[]> flstdbDCapCof, out List<double[]> flstdbKeodCof)
        {
            flstdbDCapCof = new List<double[]>();
            flstdbKeodCof = new List<double[]>();
            int i = 0;
            double[] xdata = new double[ilstTemp.Count];
            //double[] yxdata = new double[ilstTemp.Count];
            double[] ydata = new double[ilstTemp.Count];
            double[] zdata = new double[ilstTemp.Count];
            double[] poly2Dcap, poly2Keod;
            double dbTmp;
            for (i = 0; i < ilstTemp.Count; i++)
            {
                dbTmp = Convert.ToDouble(ilstTemp[i]);
                xdata[i] = dbTmp / 1000;
                //yxdata[i] = dbTmp;
            }
            for (int ki = 0; ki < ilstCurr.Count; ki++)
            {
                //object objMini = wsfMini.LinEst(ilstOutIR.ToArray(), ilstTemp.ToArray(), true, true);
                for (i = 0; i < ilstTemp.Count; i++)
                {
                    //dbTmp = ilstTemp[i]
                    //xdata[i] = Convert.ToDouble(ilstTemp[i]);
                    //dbTmp = Convert.ToDouble(flstKTmp[i * ilstCurr.Count + ki]);
                    dbTmp = Convert.ToDouble(flstKeodCont[i * ilstCurr.Count + ki]);
                    ydata[i] = Convert.ToDouble(dbTmp);
                    dbTmp = Convert.ToDouble(flstAccAtEoD[i * ilstCurr.Count + ki]);
                    dbTmp /= /*Convert.ToDouble(flstTotalAcc[i * ilstCurr.Count + ki]);//*/fullCapacity;
                    zdata[i] = 1.0f - dbTmp;
                }
                poly2Dcap = Fit.Polynomial(xdata, zdata, 3);        //2-order polynomial
                double[] dbDcapTmp = new double[poly2Dcap.Length];
                for (int pj = 0; pj < poly2Dcap.Length; pj++)
                {
                    dbDcapTmp[pj] = poly2Dcap[poly2Dcap.Length - pj - 1];
                }
                flstdbDCapCof.Add(dbDcapTmp);
                poly2Keod = Fit.Polynomial(xdata, ydata, 3);        //2-order polynomial
                double[] dbKeodTmp = new double[poly2Keod.Length];
                for (int pj = 0; pj < poly2Keod.Length; pj++)
                {
                    dbKeodTmp[pj] = poly2Keod[poly2Keod.Length - pj - 1];
                }
                flstdbKeodCof.Add(dbKeodTmp);
            }
        }

        public static List<TableMakerProduct> GenerateLiteDriver(Stage stage, LiteModel liteModel, string time, Project project)
        {
            var rootPath = string.Empty;
            //if (isRemoteOutput)
            //{
            //    rootPath = GlobalSettings.RemotePath;
            //}
            //else
            //{
            rootPath = GlobalSettings.LocalPath;
            //}
            var OutFolder = $@"{rootPath}{project.BatteryType.Name}\{project.Name}\{GlobalSettings.ProductFolderName}\{time}";
            if (!Directory.Exists(OutFolder))
            {
                Directory.CreateDirectory(OutFolder);
            }
            List<string> strFilePaths = liteModel.FileNames;
            List<string> strHHeaderComments, strCHeaderComments;
            UInt32 uErr = 0;
            int type_id = TableMakerService.GetFileTypeID("LiteH", stage);
            TableMakerService.InitializeHeaderInfor(ref uErr, project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), project.LimitedChargeVoltage.ToString(), project.CutoffDischargeVoltage.ToString(), type_id.ToString(), out strHHeaderComments);
            type_id = TableMakerService.GetFileTypeID("LiteC", stage);
            TableMakerService.InitializeHeaderInfor(ref uErr, project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), project.LimitedChargeVoltage.ToString(), project.CutoffDischargeVoltage.ToString(), type_id.ToString(), out strCHeaderComments);
            return GenerateCHFiles(stage, OutFolder, strFilePaths[0], strFilePaths[1], strHHeaderComments, strCHeaderComments, liteModel);
        }
        private static List<TableMakerProduct> GenerateCHFiles(Stage stage, string OutFolder, string strCFileStandardName, string strHFileStandardName, List<string> strHHeaderComments, List<string> strCHeaderComments, LiteModel liteModel)
        {
            string localCFilePath = System.IO.Path.Combine(OutFolder, strCFileStandardName);
            string localHFilePath = System.IO.Path.Combine(OutFolder, strHFileStandardName);

            List<string> hFileContent = GetHFileContent(strHFileStandardName, strHHeaderComments, liteModel.ilistCurr.Count);
            TableMakerService.CreateFileFromLines(localHFilePath, hFileContent);
            List<string> cFileContent = GetCFileContent(strCFileStandardName, strHFileStandardName, strCHeaderComments, liteModel.ilistCurr, liteModel.flstdbDCapCof, liteModel.flstdbKeodCof, liteModel.flstTblOCVCof);
            TableMakerService.CreateFileFromLines(localCFilePath, cFileContent);


            //string targetPath = FileTransferHelper.Local2Universal(localCFilePath);
            //var MD5 = FileTransferHelper.FileCopyWithMD5Check(localCFilePath, targetPath);
            //targetPath = FileTransferHelper.Mapping2Remote(targetPath);
            string targetPath, MD5;
            FileTransferHelper.FileUpload(localCFilePath, out targetPath, out MD5);
            List<TableMakerProduct> output = new List<TableMakerProduct>();
            TableMakerProduct ctmp = new TableMakerProduct();
            ctmp.FilePath = targetPath;
            ctmp.MD5 = MD5;
            ctmp.IsValid = true;
            ctmp.Type = TableMakerService.GetFileType("LiteC", stage);
            output.Add(ctmp);


            //targetPath = FileTransferHelper.Local2Universal(localHFilePath);
            //FileTransferHelper.FileCopyWithMD5Check(localHFilePath, targetPath);
            //targetPath = FileTransferHelper.Mapping2Remote(targetPath);
            FileTransferHelper.FileUpload(localHFilePath, out targetPath, out MD5);
            TableMakerProduct htmp = new TableMakerProduct();
            htmp.FilePath = targetPath;
            htmp.MD5 = MD5;
            htmp.IsValid = true;
            htmp.Type = TableMakerService.GetFileType("LiteH", stage);
            output.Add(htmp);

            return output;
        }

        private static List<string> GetHFileContent(string strStandardH, List<string> strHHeaderComments, int currNum)
        {
            int iLineCmtHCFile = 4;
            int i = 0;
            List<string> output = new List<string>();
            var ilstCellTempData = TableMakerService.GenerateSampleCellTempData();
            foreach (string shc in strHHeaderComments)
            {
                if (i == iLineCmtHCFile)
                {
                    output.Add(shc + strStandardH);
                }
                else
                {
                    output.Add(shc);
                }
                i++;
            }
            output.Add(string.Format("#ifndef _TABLE_LITE_H_"));
            output.Add(string.Format("#define _TABLE_LITE_H_"));
            output.Add(string.Format("\n"));
            //(A210602)as discussed with Jon, resume temperature table
            output.Add(string.Format("#define FGL_TEMPERATURE_NUM\t\t{0}", ilstCellTempData.Count / 2));
            output.Add(string.Format("#define YNUM\t\t{0}\t//poly func table Y axis number", currNum));
            output.Add(string.Format("#define FNUM7D\t\t8\t//7th order function coefficient number"));
            output.Add(string.Format("#define FNUM5D\t\t6\t//5th order function coefficient number"));
            output.Add(string.Format("#define FNUM3D\t\t4\t//3th order function coefficient number"));
            output.Add(string.Format(""));
            output.Add(string.Format("/****************************************************************************"));
            output.Add(string.Format("* Struct section"));
            output.Add(string.Format("*  add struct #define here if any"));
            output.Add(string.Format("***************************************************************************/"));
            output.Add(string.Format("typedef struct tag_one_latitude_data {{"));
            output.Add(string.Format("\tshort\t\t\t\t\tx;//"));
            output.Add(string.Format("\tshort\t\t\t\t\ty;//"));
            output.Add(string.Format("}} one_latitude_data_t;"));
            output.Add(string.Format(""));
            output.Add(string.Format("/****************************************************************************"));
            output.Add(string.Format("* extern variable declaration section"));
            output.Add(string.Format("***************************************************************************/"));
            output.Add(string.Format("extern int y_data[YNUM];"));
            output.Add(string.Format("extern float ocv_cof[FNUM7D];"));
            output.Add(string.Format("extern float keod_cof[YNUM][FNUM3D];"));
            output.Add(string.Format("extern float dcap_cof[YNUM][FNUM3D];"));
            output.Add(string.Format("\n"));
            output.Add(string.Format("#endif	//_TABLE_LITE_H_"));
            return output;
        }

        private static List<string> GetCFileContent(string strCFileName, string strStandardH, List<string> strHHeaderComments, List<short> ilstCurr, List<double[]> flstdbDCapCof, List<double[]> flstdbKeodCof, List<double> flstTblOCVCof)
        {
            int iLineCmtHCFile = 4;
            string strCTmp = string.Empty;
            int i = 0;
            List<string> output = new List<string>();
            //var ilstCellTempData = TableMakerService.GenerateSampleCellTempData();
            foreach (string shc in strHHeaderComments)
            {
                if (i == iLineCmtHCFile)
                {
                    output.Add(shc + strCFileName);
                }
                else
                {
                    output.Add(shc);
                }
                i++;
            }
            output.Add(string.Format("#include \"{0}\"", strStandardH));
            output.Add(string.Format("\n"));
            output.Add(string.Format("/*****************************************************************************"));
            output.Add(string.Format("* OCV Coefficient "));
            output.Add(string.Format("****************************************************************************/"));
            output.Add(string.Format("float ocv_cof[FNUM7D] = "));
            output.Add(string.Format("{{"));
            strCTmp = "";
            for (i = 0; i < flstTblOCVCof.Count; i++)
            {
                if (i < flstTblOCVCof.Count - 1)
                    strCTmp += string.Format("\t{0},", (float)flstTblOCVCof[i]);
                else
                    strCTmp += string.Format("\t{0}", (float)flstTblOCVCof[i]);
            }
            output.Add(strCTmp);
            output.Add(string.Format("}};"));

            //output.Add(string.Format("/*****************************************************************************"));
            //output.Add(string.Format("* Cell temperature table"));
            //output.Add(string.Format("* x_dat:	cell mini-voltage"));
            //output.Add(string.Format("* y_dat:	temperature in 0.1degC format"));
            //output.Add(string.Format("****************************************************************************/"));
            //output.Add(string.Format("one_latitude_data_t fgl_cell_temp_data[FGL_TEMPERATURE_NUM] = "));
            //output.Add(string.Format("{{"));
            //strCTmp = string.Empty;
            //for (i = 0; i < ilstCellTempData.Count; i++)
            //{
            //    var tmp = string.Format("\t{{{0}, \t{1}", ilstCellTempData[i], ilstCellTempData[++i]) + "},";
            //    if ((i > 2) && (i % 10 == 9))
            //    {
            //        strCTmp += tmp;
            //        output.Add(strCTmp);
            //        strCTmp = string.Empty;
            //    }
            //    else
            //    {
            //        strCTmp += tmp;
            //    }
            //}
            //output.Add(strCTmp);
            //output.Add("};");

            output.Add("\n");
            output.Add("/*****************************************************************************");
            output.Add("* RCAP and KEOD Function Table Y-Axis Data");
            output.Add("****************************************************************************/");
            strCTmp = "int\ty_data[YNUM] = {";
            for (int ic = 0; ic < ilstCurr.Count; ic++)
            {
                short shtmp = ilstCurr[ic];
                if (shtmp < 0) shtmp *= -1;
                if (ic != ilstCurr.Count - 1)
                    strCTmp += (string.Format("{0}, ", shtmp));
                else
                    strCTmp += (string.Format("{0}", shtmp));
            }
            output.Add(strCTmp + "};\n");
            output.Add("/*****************************************************************************");
            output.Add("* RCAP Function Table Coefficient Data");
            output.Add("****************************************************************************/");
            output.Add("float dcap_cof[YNUM][FNUM3D] = {");
            for (int id = 0; id < flstdbDCapCof.Count; id++)
            {
                strCTmp = ("\t{");
                double[] dbtmp = flstdbDCapCof[id];
                for (int idd = 0; idd < dbtmp.Length; idd++)
                {
                    if (idd < dbtmp.Length - 1)
                        strCTmp += (string.Format("{0:F10}, ", (float)dbtmp[idd]));
                    else
                        strCTmp += (string.Format("{0:F10}", (float)dbtmp[idd]));
                }
                output.Add(strCTmp += "},");
            }
            output.Add("};\n");
            output.Add("/*****************************************************************************");
            output.Add("* KEOD Function Table Coefficient Data");
            output.Add("****************************************************************************/");
            output.Add("float keod_cof[YNUM][FNUM3D] = {");
            for (int id = 0; id < flstdbKeodCof.Count; id++)
            {
                strCTmp = ("\t{");
                double[] dbtmp = flstdbKeodCof[id];
                for (int idd = 0; idd < dbtmp.Length; idd++)
                {
                    if (idd < dbtmp.Length - 1)
                        strCTmp += (string.Format("{0:F10}, ", (float)dbtmp[idd]));
                    else
                        strCTmp += (string.Format("{0:F10}", (float)dbtmp[idd]));
                }
                output.Add(strCTmp + "},");
            }
            output.Add("};\n");
            return output;
        }
    }
}