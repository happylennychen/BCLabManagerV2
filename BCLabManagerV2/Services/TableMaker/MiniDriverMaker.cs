using BCLabManager.Model;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BCLabManager
{
    public static class MiniDriverMaker
    {
        static public int iNumOfMiniPoints { get; set; } = 100;           //(A200831)francis, for table_mini
        static public float fMiniTableSteps { get; set; } = 1;        //(A200831)francis, for table_mini
        #region Mini
        public static void GetMiniModel(List<SourceData> ocvSource, List<SourceData> rcSource, OCVModel ocvModel, RCModel rcModel, Project project, ref MiniModel miniModel)
        {
            MiniModel output = miniModel;
            output.iOCVVolt = ocvModel.iOCVVolt;
            output.fCTABase = rcModel.fCTABase;
            output.fCTASlope = rcModel.fCTASlope;
            List<short> ilstOutIR;
            var num = (iNumOfMiniPoints + 1) * rcModel.listfTemp.Count;
            List<float> flstRC_T;

            List<List<float>> fLstRCM_Volt;
            GetLstRCM_Volt(ocvSource, rcSource, rcModel, out fLstRCM_Volt);
            GetLstRC_T(fLstRCM_Volt, out flstRC_T);
            List<float> flstOCV;
            GetLstTblM_OCV(ocvSource, out flstOCV);
            GetLstOutIR(num, iNumOfMiniPoints + 1, rcModel.listfCurr, flstOCV, flstRC_T, out ilstOutIR);
            List<float> flstTotalAcc;
            GetLstTotalAcc(out flstTotalAcc, project, rcSource, rcModel);
            List<double> poly2EstFACC;
            List<double> poly2EstIR;
            GetPoly(rcModel.listfTemp, ilstOutIR, flstTotalAcc, out poly2EstFACC, out poly2EstIR);
            output.poly2EstFACC = poly2EstFACC;
            output.poly2EstIR = poly2EstIR;
        }
        private static bool GetLstTblM_OCV(List<SourceData> lstSample2, out List<float> fLstTblM_OCV)
        {
            Int32 iMinVoltage;
            Int32 iMaxVoltage;
            TableMakerService.GetVoltageBondary(lstSample2, out iMinVoltage, out iMaxVoltage);
            fLstTblM_OCV = new List<float>();
            bool bReturn = false;
            int indexLow, indexHigh;
            SourceData lowcurSample, higcurSample;
            float fTmpSoC1, fTmpSoC2, fTmpVolt1;
            int fmulti = (int)(((float)(iMaxVoltage - iMinVoltage)) * 10F / TableMakerService.iSOCStepmV);
            int ileft = (int)fmulti % 10;
            int ii, jj;
            //(200902)francis, for table_mini
            float fFullCapacity = 0, fACCMiniStep = 0;// = fMiniTableSteps * 0.01F * (TableSourceData[0].fFullCapacity - TableSourceData[0].fCapacityDiff);
            List<float> flstLoCurVoltPoints = new List<float>();
            List<float> flstHiCurVoltPoints = new List<float>();

            #region assign low/high current sample, and assign SoC points (as default or input)
            //lstSample2.Count definitely is 2
            if (Math.Abs(lstSample2[0].fCurrent) < Math.Abs(lstSample2[1].fCurrent))
            {
                lowcurSample = lstSample2[0];
                higcurSample = lstSample2[1];
            }
            else
            {
                lowcurSample = lstSample2[1];
                higcurSample = lstSample2[0];
            }
            //fHdAbsMaxCap = lstSample2[0].fAbsMaxCap;
            //fHdCapacityDiff = lstSample2[0].fCapacityDiff;

            //(A200902)Francis, use the low_cur header setting to calculate
            fFullCapacity = lowcurSample.fAbsMaxCap - lowcurSample.fCapacityDiff;
            fACCMiniStep = fMiniTableSteps * 0.01F * (lowcurSample.fAbsMaxCap - lowcurSample.fCapacityDiff);
            #endregion

            //calculate TSOCbyOCV, high/low voltage is coming from user input
            fmulti /= 10;
            fTmpVolt1 = fmulti * TableMakerService.iSOCStepmV + iMinVoltage;

            if ((ileft != 0) || (fTmpVolt1 != iMaxVoltage))
            {
                if (fTmpVolt1 < iMaxVoltage)	//should not bigger than iMaxVoltage
                {
                    iMaxVoltage = (int)(fTmpVolt1 + 0.5);
                }
            }



            #region (A200902)francis, for table_mini
            #region find <SoC, Volt> from low current data
            indexLow = 0;
            flstLoCurVoltPoints.Clear();
            //fSoCbk = lowcurSample.AdjustedExpData[indexLow].fAccMah;
            for (; indexLow < lowcurSample.AdjustedExpData.Count; indexLow++)
            {
                fTmpVolt1 = lowcurSample.AdjustedExpData[indexLow].fVoltage;
                fTmpSoC1 = lowcurSample.AdjustedExpData[indexLow].fAccMah;
                if (flstLoCurVoltPoints.Count < fFullCapacity)
                {
                    if (flstLoCurVoltPoints.Count == 0)
                    {
                        flstLoCurVoltPoints.Add(fTmpVolt1);
                        continue;
                    }
                    else
                    {
                        //fTmpSoC1 = fSoCbk - fTmpSoC1;
                        //fTmpSoC1 /= iNumOfMiniPoints;
                        if (fTmpSoC1 < flstLoCurVoltPoints.Count)
                        {
                        }
                        else
                        {
                            flstLoCurVoltPoints.Add(fTmpVolt1);
                        }
                    }
                }
            }
            for (int ilo = flstLoCurVoltPoints.Count; ilo <= fFullCapacity; ilo++)
            {
                flstLoCurVoltPoints.Add(iMinVoltage);
            }
            #endregion
            #region find <SoC, Volt> from high current data
            indexHigh = 0;
            flstHiCurVoltPoints.Clear();
            //fSoCbk = higcurSample.AdjustedExpData[indexHigh].fAccMah;
            for (; indexHigh < higcurSample.AdjustedExpData.Count; indexHigh++)
            {
                fTmpVolt1 = higcurSample.AdjustedExpData[indexHigh].fVoltage;
                fTmpSoC2 = higcurSample.AdjustedExpData[indexHigh].fAccMah;
                if (flstHiCurVoltPoints.Count < fFullCapacity)
                {
                    if (flstHiCurVoltPoints.Count == 0)
                    {
                        flstHiCurVoltPoints.Add(fTmpVolt1);
                        continue;
                    }
                    else
                    {
                        //fTmpSoC2 = fSoCbk - fTmpSoC1;
                        //fTmpSoC2 /= iNumOfMiniPoints;
                        if (fTmpSoC2 < flstHiCurVoltPoints.Count)
                        {
                        }
                        else
                        {
                            flstHiCurVoltPoints.Add(fTmpVolt1);
                        }
                    }
                }
            }
            for (int ihi = flstHiCurVoltPoints.Count; ihi <= fFullCapacity; ihi++)
            {
                flstHiCurVoltPoints.Add(iMinVoltage);
            }
            #endregion

            jj = 0;
            indexLow = 0; indexHigh = 0;
            fTmpSoC1 = flstLoCurVoltPoints[indexLow];
            fTmpSoC2 = flstHiCurVoltPoints[indexHigh];
            for (jj = 0; jj < (iNumOfMiniPoints + 1); jj++)
            {
                for (; indexLow < flstLoCurVoltPoints.Count; indexLow++)
                {
                    if (indexLow < (jj * fACCMiniStep))
                    {
                        fTmpSoC1 = flstLoCurVoltPoints[indexLow];
                    }
                    else
                    {
                        break;
                    }
                }
                for (; indexHigh < flstHiCurVoltPoints.Count; indexHigh++)
                {
                    if (indexHigh < (jj * fACCMiniStep))
                    {
                        fTmpSoC2 = flstHiCurVoltPoints[indexHigh];
                    }
                    else
                    {
                        break;
                    }
                }
                if (jj == 0)
                {
                    fTmpVolt1 = (fTmpSoC2 + fTmpSoC1) / 2;
                }
                else
                {
                    //fCurrent is saving in mA format
                    fTmpVolt1 = (fTmpSoC2 - fTmpSoC1) / ((Math.Abs(lowcurSample.fCurrent) - Math.Abs(higcurSample.fCurrent)) / 1000);
                    fTmpVolt1 *= (Math.Abs(lowcurSample.fCurrent) / 1000);
                    fTmpVolt1 += fTmpSoC1;
                }
                fLstTblM_OCV.Add(fTmpVolt1);
            }   //for (jj=0; jj < (iNumOfMiniPoints+1);jj++)
            #endregion

            return bReturn;
        }
        private static void GetLstOCV(out List<float> flstOCV)
        {
            throw new NotImplementedException();
        }

        private static void GetLstRCM_Volt(List<SourceData> ocvSource, List<SourceData> rcSource, RCModel rcModel, out List<List<float>> fLstRCM_Volt)
        {
            fLstRCM_Volt = new List<List<float>>();
            Int32 iMinVoltage = rcModel.MinVoltage;
            //Int32 iMaxVoltage;
            //GetVoltageBondary(rcSource, out iMinVoltage, out iMaxVoltage);
            var listfTemp = rcModel.listfTemp;
            var fc = rcModel.listfCurr[0];

            float fFullCapacity = rcSource[0].fAbsMaxCap - rcSource[0].fCapacityDiff;
            float fACCMiniStep = fMiniTableSteps * 0.01F * (fFullCapacity);

            foreach (float ft in listfTemp)     //from low temperature to list
            {
                foreach (var sds in rcSource)
                {
                    if ((sds.fTemperature == ft) &&
                        (sds.fCurrent == fc))
                    {
                        int i = 0;
                        var inListRCData = sds.AdjustedExpData;
                        float fPreVol = inListRCData[0].fVoltage;
                        List<float> flstTmp = new List<float>();
                        for (int j = 0; j < inListRCData.Count; j++)
                        {
                            if (i < iNumOfMiniPoints)
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
                        for (int ij = flstTmp.Count; ij <= iNumOfMiniPoints; ij++)
                        {
                            flstTmp.Add(iMinVoltage);
                        }

                        fLstRCM_Volt.Add(flstTmp);
                    }
                }
            }
        }

        private static void GetLstRC_T(List<List<float>> fLstRCM_Volt, out List<float> flstRC_T)
        {
            flstRC_T = new List<float>();

            for (int j = 0; j < fLstRCM_Volt.Count; j++)
            {
                List<float> flstTmp = fLstRCM_Volt[j];
                for (int i = 0; i < iNumOfMiniPoints + 1; i++)
                {
                    flstRC_T.Add(flstTmp[i]);
                }
            }

        }

        private static void GetLstTotalAcc(out List<float> flstTotalAcc, Project project, List<SourceData> rcSource, RCModel rcModel)
        {
            flstTotalAcc = new List<float>();
            List<float> fYPointACCall = new List<float>();
            var listfTemp = rcModel.listfTemp;
            var listfCurr = rcModel.listfCurr;
            var TableVoltagePoints = project.VoltagePoints;

            foreach (float ft in listfTemp)     //from low temperature to list
            {
                foreach (float fc in listfCurr)     //from low current to list
                {
                    foreach (var sds in rcSource)
                    {
                        if ((sds.fTemperature == ft) &&
                            (sds.fCurrent == fc))
                        {
                            int i = 0, j = 0, iCountP = 0;
                            float fPreSoC = -99999, fPreVol = -99999, fAvgSoc = -99999, fPreTemp = -9999, fPreCCount = 0;//, fAccCount = 0; ;
                            List<int> ypoints = new List<int>();
                            float fDesignCapacity = 3080;
                            float fCapaciDiff = 0;
                            float fRefMaxDiff = 0;
                            float fRefCCount = 0;
                            //(A200902)Francis, use the low_cur header setting to calculate
                            List<float> flstTmp = new List<float>();
                            float fAccCap = 0.0f;

                            //(M140718)Francis,
                            i = 0; j = 0;
                            var inListRCData = sds.AdjustedExpData;
                            for (i = 0; i < TableVoltagePoints.Count; i++)
                            {
                                for (; j < inListRCData.Count; j++)
                                {
                                    //(A20201015), memorize maximum accumulated capacity
                                    if (Math.Abs(inListRCData[j].fAccMah) > fAccCap)
                                        fAccCap = Math.Abs(inListRCData[j].fAccMah);//(A20200813)
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
                                        fAvgSoc = (fDesignCapacity - fCapaciDiff - fAvgSoc);    //convert to remaining
                                        fAvgSoc *= (10000 / fDesignCapacity);       //convert to 10000C
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
                                            fAvgSoc = (fDesignCapacity - fCapaciDiff - inListRCData[inListRCData.Count - 1].fAccMah);   //convert to remaining
                                            fAvgSoc *= (10000 / fDesignCapacity);           //convert to 10000C
                                        }
                                        else
                                        {
                                        }
                                    }
                                    else
                                    {
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
                                fAvgSoc = (fDesignCapacity - fCapaciDiff - fAvgSoc);    //convert to remaining
                                fAvgSoc *= (10000 / fDesignCapacity);       //convert to 10000C
                                if (fAvgSoc > 10000) fAvgSoc = 10000.0F;
                                if (fAvgSoc < 0) fAvgSoc = -9999.0F;
                                ypoints.Add(Convert.ToInt32(Math.Round(fAvgSoc, 0)));
                                i += 1;
                            }

                            if (ypoints.Count == TableVoltagePoints.Count)
                            {
                                ypoints.Sort();
                            }
                            else
                            {
                                for (int ii = ypoints.Count; ii < TableVoltagePoints.Count; ii++)
                                {
                                    ypoints.Add(0);
                                }
                                ypoints.Sort();
                                //because leakage of information, CreateNewErrorLog() need to be done outside of this funciton
                            }
                            //(A200902)francis, table_mini
                            fYPointACCall.Add(fAccCap);
                        }
                    }
                }
            }

            for (int j = 0; j < listfTemp.Count; j++)
            {
                flstTotalAcc.Add(fYPointACCall[j * listfCurr.Count]);
            }
        }

        private static void GetLstOutIR(int num, int v, List<float> ilstCurr, List<float> flstOCV, List<float> flstRC_T, out List<short> ilstOutIR)
        {
            double dbTmp1 = 0, dbTmp2 = 0;
            dbTmp1 = (float)(num) / (float)(v);
            dbTmp2 = Math.Round(dbTmp1, 0);
            dbTmp1 -= dbTmp2;
            Int16 i16Tmp = 0, i16Curr = 1;
            if (ilstCurr.Count() >= 1)
            {
                i16Curr = (short)ilstCurr[0];
                if (i16Curr < 0)
                    i16Curr /= -1000;
                else
                    i16Curr /= 1000;
            }
            ilstOutIR = new List<short>();
            List<double> dblstDiff = new List<double>();
            if (Math.Abs(dbTmp1) < 0.00001f)
            {
                for (int i = 0; i < (int)dbTmp2; i++)
                {
                    dblstDiff.Clear();
                    for (int j = 0; j < v; j++)
                    {
                        dbTmp1 = flstOCV[j] - flstRC_T[i * v + j];
                        dblstDiff.Add(dbTmp1 / i16Curr);
                    }
                    dbTmp1 = 0;
                    for (int j = 1; j <= (int)(dblstDiff.Count() / 2); j++)
                    {   //calculate from soc_99 to soc_50
                        dbTmp1 += dblstDiff[j];
                    }
                    dbTmp1 /= ((int)(dblstDiff.Count() / 2));
                    //dbTmp1 = dblstDiff[50];     //calculate at soc_50
                    i16Tmp = (Int16)Math.Round(dbTmp1 * 10, 0);
                    ilstOutIR.Add(i16Tmp);
                }
            }
        }

        private static void GetPoly(List<float> listfTemp, List<short> ilstOutIR, List<float> flstTotalAcc, out List<double> poly2EstFACC, out List<double> poly2EstIR)
        {
            var ilstTemp = listfTemp.Select(o => (short)o * 10).ToList();
            int i = 0;
            //object objMini = wsfMini.LinEst(ilstOutIR.ToArray(), ilstTemp.ToArray(), true, true);
            double[] xdata = new double[ilstTemp.Count];
            double[] ydata = new double[ilstOutIR.Count];
            double[] zdata = new double[flstTotalAcc.Count];
            for (i = 0; i < ilstTemp.Count; i++)
                xdata[i] = Convert.ToDouble(ilstTemp[i]);
            for (i = 0; i < ilstOutIR.Count; i++)
                ydata[i] = Convert.ToDouble(ilstOutIR[i]);
            for (i = 0; i < flstTotalAcc.Count; i++)
                zdata[i] = Convert.ToDouble(flstTotalAcc[i]);
            //Tuple<double, double> lineEst = Fit.Line(xdata, ydata);
            poly2EstIR = Fit.Polynomial(xdata, ydata, 2).ToList();        //2-order polynomial
            poly2EstFACC = Fit.Polynomial(xdata, zdata, 2).ToList();        //2-order polynomial
        }

        public static void GenerateMiniDriver(MiniModel miniModel, Project project)
        {
            List<string> strFilePaths;
            TableMakerService.GetDriverFilePaths(project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), "mini", out strFilePaths);
            List<string> strHHeaderComments;
            UInt32 uErr = 0;
            TableMakerService.InitializeHeaderInfor(ref uErr, project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), project.LimitedChargeVoltage.ToString(), project.CutoffDischargeVoltage.ToString(), out strHHeaderComments);
            var OutFolder = $@"{GlobalSettings.RootPath}{project.BatteryType.Name}\{project.Name}\{GlobalSettings.ProductFolderName}";
            GenerateMiniCHFiles(OutFolder, strFilePaths[0], strFilePaths[1], strHHeaderComments, miniModel.iOCVVolt, miniModel.fCTABase, miniModel.fCTASlope, miniModel.poly2EstFACC, miniModel.poly2EstIR);

        }

        private static void GenerateMiniCHFiles(string OutFolder, string strCFileMiniName, string strHFileMiniName, List<string> strHHeaderComments, List<int> ilstOCVVolt, double fCTABase, double fCTASlope, List<double> poly2EstFACC, List<double> poly2EstIR)
        {
            short iEnlargeIR = 10000;
            FileStream fswrite = null;
            StreamWriter FileContent = null;
            #region create Driver H file
            var strMiniC = System.IO.Path.Combine(OutFolder, strCFileMiniName);
            var strMiniH = System.IO.Path.Combine(OutFolder, strHFileMiniName);
            try
            {
                fswrite = File.Open(strMiniH, FileMode.Create, FileAccess.Write, FileShare.None);
                FileContent = new StreamWriter(fswrite, new UTF8Encoding(false));// Encoding.Default);
            }
            catch { }

            int i = 0;
            foreach (string shc in strHHeaderComments)
            {
                FileContent.WriteLine(shc);
            }

            var ilstCellTempData = TableMakerService.GenerateSampleCellTempData();
            #region write H file content to
            FileContent.WriteLine(string.Format("#ifndef _TABLE_MINI_H_"));
            FileContent.WriteLine(string.Format("#define _TABLE_MINI_H_"));
            FileContent.WriteLine(string.Format("\n"));
            FileContent.WriteLine(string.Format("#define FGM_OCV_NUM\t\t{0}", ilstOCVVolt.Count));
            FileContent.WriteLine(string.Format("#define FGM_TEMPERATURE_NUM\t\t{0}", ilstCellTempData.Count / 2));
            FileContent.WriteLine(string.Format("#define FGM_TR_NUM\t\t2"));//{0}", (int)(dbTmp2)));        //currently we direct code 2
            FileContent.WriteLine(string.Format(""));
            FileContent.WriteLine(string.Format("#define CTABASE\t\t{0}", fCTABase));
            FileContent.WriteLine(string.Format("#define CTASLOPE\t\t{0}", fCTASlope));
            FileContent.WriteLine(string.Format(""));
            FileContent.WriteLine(string.Format("/****************************************************************************"));
            FileContent.WriteLine(string.Format("* Struct section"));
            FileContent.WriteLine(string.Format("*  add struct #define here if any"));
            FileContent.WriteLine(string.Format("***************************************************************************/"));
            FileContent.WriteLine(string.Format("typedef struct tag_one_latitude_data {{"));
            FileContent.WriteLine(string.Format("\tshort\t\t\t\t\tx;//"));
            FileContent.WriteLine(string.Format("\tshort\t\t\t\t\ty;//"));
            FileContent.WriteLine(string.Format("}} one_latitude_data_t;"));
            FileContent.WriteLine(string.Format(""));
            FileContent.WriteLine(string.Format("typedef struct tag_cof_func_data {{"));
            FileContent.WriteLine(string.Format("\tint\t\t\t\t\ta;//"));
            FileContent.WriteLine(string.Format("\tint\t\t\t\t\tb;//"));
            FileContent.WriteLine(string.Format("\tint\t\t\t\t\tc;//"));
            FileContent.WriteLine(string.Format("}} cof_func_data_t;"));
            FileContent.WriteLine(string.Format(""));
            FileContent.WriteLine(string.Format("/****************************************************************************"));
            FileContent.WriteLine(string.Format("* extern variable declaration section"));
            FileContent.WriteLine(string.Format("***************************************************************************/"));
            FileContent.WriteLine(string.Format("extern one_latitude_data_t fgm_ocv_data[];"));
            FileContent.WriteLine(string.Format("extern one_latitude_data_t fgm_cell_temp_data[];"));
            FileContent.WriteLine(string.Format("extern cof_func_data_t fgm_tr_data[];"));
            FileContent.WriteLine(string.Format("\n"));
            FileContent.WriteLine(string.Format("#endif	//_TABLE_MINI_H_"));
            #endregion
            FileContent.Close();
            fswrite.Close();
            #endregion

            #region create Driver C file
            try
            {
                fswrite = File.Open(strMiniC, FileMode.Create, FileAccess.Write, FileShare.None);
                FileContent = new StreamWriter(fswrite, new UTF8Encoding(false));// Encoding.Default);
            }
            catch (Exception ec)
            {
            }

            i = 0;
            foreach (string scc in strHHeaderComments)
            {

                FileContent.WriteLine(scc);
            }
            #region write C file content to
            FileContent.WriteLine(string.Format("#include \"{0}\"", strHFileMiniName));
            FileContent.WriteLine(string.Format("\n"));
            FileContent.WriteLine(string.Format("/*****************************************************************************"));
            FileContent.WriteLine(string.Format("* OCV table"));
            FileContent.WriteLine(string.Format("* x_dat:\tcell mini-voltage"));
            FileContent.WriteLine(string.Format("* y_dat:\tresidual capacity in percentage format"));
            FileContent.WriteLine(string.Format("****************************************************************************/"));
            FileContent.WriteLine(string.Format("one_latitude_data_t fgm_ocv_data[FGM_OCV_NUM] = "));
            FileContent.WriteLine(string.Format("{{"));
            string strCTmp = "";
            List<float> ilstOCVSoC = TableMakerService.GetOCVSocPoints();
            for (i = 0; i < ilstOCVVolt.Count; i++)
            {
                strCTmp = string.Format("\t{{{0}, \t{1}", ilstOCVVolt[i], Math.Round(ilstOCVSoC[i], 0)) + "},";
                if ((i > 2) && (i % 10 == 9))
                {
                    FileContent.WriteLine(strCTmp);
                }
                else
                {
                    FileContent.Write(strCTmp);
                }
            }
            //strCTmp = strCFileContents[iLineOCVCont];
            //strCTmp = strCTmp.Substring(strCTmp.IndexOf('{')+1);
            //strCTmp = strCTmp.Substring(0, strCTmp.IndexOf(';')-1);
            //string[] strOCVCont = strCTmp.Split(chSeperate, StringSplitOptions.None);
            //i = 1;
            //foreach(string strPo in strOCVCont)
            //{
            //    if ((i > 2) && (i % 20 == 0))
            //    {
            //        FileContent.WriteLine(string.Format("{0}, ", strPo));
            //    }
            //    else
            //    {
            //        FileContent.Write(string.Format("{0}, ", strPo));
            //    }
            //    i += 1;
            //}
            FileContent.Write(string.Format("\n"));
            FileContent.WriteLine(string.Format("}};"));
            //write sample cell_temp_data
            FileContent.WriteLine(string.Format("/*****************************************************************************"));
            FileContent.WriteLine(string.Format("* Cell temperature table"));
            FileContent.WriteLine(string.Format("* x_dat:	cell mini-voltage"));
            FileContent.WriteLine(string.Format("* y_dat:	temperature in 0.1degC format"));
            FileContent.WriteLine(string.Format("****************************************************************************/"));
            FileContent.WriteLine(string.Format("one_latitude_data_t fgm_cell_temp_data[FGM_TEMPERATURE_NUM] = "));
            FileContent.WriteLine(string.Format("{{"));
            for (i = 0; i < ilstCellTempData.Count; i++)
            {
                strCTmp = string.Format("\t{{{0}, \t{1}", ilstCellTempData[i], ilstCellTempData[++i]) + "},";
                if ((i > 2) && (i % 10 == 9))
                {
                    FileContent.WriteLine(strCTmp);
                }
                else
                {
                    FileContent.Write(strCTmp);
                }
            }
            FileContent.Write(string.Format("\n"));
            FileContent.WriteLine(string.Format("}};"));
            FileContent.WriteLine(string.Format("\n"));
            FileContent.WriteLine(string.Format("/*****************************************************************************"));
            FileContent.WriteLine(string.Format("* TR table"));
            FileContent.WriteLine(string.Format("* tr data: tr function coefficient table"));
            FileContent.WriteLine(string.Format("*\t\t\tfunc_1: facc estimation"));
            FileContent.WriteLine(string.Format("*\t\t\tfunc_2: mid-ir estimation"));
            FileContent.WriteLine(string.Format("****************************************************************************/"));
            FileContent.WriteLine(string.Format("cof_func_data_t fgm_tr_data[FGM_TR_NUM]={{"));
            //for (int ic = 0; ic < dbTmp2; ic++)
            //{
            //    FileContent.WriteLine(string.Format("\t{{{0},\t{1}", ilstTemp[ic], ilstOutIR[ic]) + "},");
            //}
            FileContent.Write(string.Format("\t{{"));
            for (int ic = poly2EstFACC.Count - 1; ic >= 0; ic--)
            {
                if (ic == 0)
                {
                    FileContent.WriteLine(string.Format("{0}}},", Convert.ToInt32(poly2EstFACC[ic])));
                }
                else
                {
                    FileContent.Write(string.Format("{0},\t\t", Convert.ToInt32(poly2EstFACC[ic] * iEnlargeIR)));
                }
            }
            FileContent.Write(string.Format("\t{{"));
            for (int ic = poly2EstIR.Count - 1; ic >= 0; ic--)
            {
                if (ic == 0)
                {
                    FileContent.WriteLine(string.Format("{0}}},", Convert.ToInt32(poly2EstIR[ic])));
                }
                else
                {
                    FileContent.Write(string.Format("{0},\t\t", Convert.ToInt32(poly2EstIR[ic] * iEnlargeIR)));
                }
            }
            FileContent.WriteLine(string.Format("}};"));
            #endregion

            FileContent.Close();
            fswrite.Close();
            #endregion

            #endregion
        }
    }
}