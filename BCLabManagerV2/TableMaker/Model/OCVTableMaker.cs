using BCLabManager.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace BCLabManager
{
    public static class OCVTableMaker
    {
        static public int iMinPercent = 0;
        static public int iMaxPercent = 10000;
        #region OCV
        public static void GetOCVModel(List<SourceData> MaxSDList, ref OCVModel ocvModel)
        {
            OCVModel output = ocvModel;

            output.SourceList = MaxSDList;

            Int32 iMinVoltage;
            Int32 iMaxVoltage;
            TableMakerService.GetVoltageBondary(MaxSDList, out iMinVoltage, out iMaxVoltage);
            output.MinVoltage = iMinVoltage;
            output.MaxVoltage = iMaxVoltage;
            List<Int32> iOCVVolt;
            if (CreateNewOCVPoints(MaxSDList, iMinVoltage, iMaxVoltage, out iOCVVolt))
            {
                ;
            }
            output.iOCVVolt = iOCVVolt;
        }
        private static bool CreateNewOCVPoints(List<SourceData> lstSample2, int iMinVoltage, int iMaxVoltage, out List<int> iOCVVolt)
        {
            iOCVVolt = new List<int>();
            bool bReturn = false;
            UInt32 result = 0;
            SourceData lowcurSample, higcurSample;
            double fSoCVoltLow, fSoCVoltHigh, fTmpVolt1;
            double fRsocn = 1.0F;
            int fmulti = (int)(((double)(iMaxVoltage - iMinVoltage)) * 10F / TableMakerService.iSOCStepmV);
            int ileft = (int)fmulti % 10;

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
            iOCVVolt.Clear();
            //iSOCVolt.Clear();
            //iSOCBit.Clear();

            var lstfPoints = TableMakerService.GetOCVSocPoints();
            lstfPoints.Reverse();		//from high to low SoC
            foreach (double fSoC1Point in lstfPoints)
            {
                fSoCVoltLow = 0; fSoCVoltHigh = 0;	//default value
                result = GetSoCVolt(lowcurSample, fSoC1Point, out fSoCVoltLow);
                result = GetSoCVolt(higcurSample, fSoC1Point, out fSoCVoltHigh);
                //found 2 sets <SoC, Volt> from low/high current experiment data, add inot OCV list according Guoyan calculation
                fRsocn = (fSoCVoltLow - fSoCVoltHigh) / (Math.Abs(higcurSample.fCurrent) - Math.Abs(lowcurSample.fCurrent));
                fTmpVolt1 = fSoCVoltLow + Math.Abs(lowcurSample.fCurrent) * fRsocn;
                if (fTmpVolt1 > lowcurSample.fLimitChgVolt) fTmpVolt1 = lowcurSample.fLimitChgVolt;
                if (fTmpVolt1 < lowcurSample.fCutoffDsgVolt) fTmpVolt1 = lowcurSample.fCutoffDsgVolt;
                if (fTmpVolt1 < 0) MessageBox.Show("Minus Voltage Got");	//Fran debug
                iOCVVolt.Add((int)(fTmpVolt1 + 0.5F));
            }

            //check iOCVVolt points number
            if (iOCVVolt.Count != TableMakerService.iNumOfPoints)
            {
                result = 1;
                return bReturn;
            }
            else
            {
                iOCVVolt.Sort();
                bReturn = true;
            }

            return bReturn;
        }
        private static UInt32 GetSoCVolt(SourceData sourceData, double fSoC1Point, out double output)
        {
            UInt32 result = 0;
            #region find <SoC, Volt> from low current data
            output = 0;
            double fTmpSoC1 = 0; //fTmpSoC2 = 0; //default value
            double fSoCbk = 0; double fVoltbk = 0;    //default value
            double fTmpVolt1;
            int index = 0;
            for (; index < sourceData.AdjustedExpData.Count; index++)
            {
                fTmpVolt1 = sourceData.AdjustedExpData[index].fVoltage;
                fTmpSoC1 = sourceData.AdjustedExpData[index].fSoCAdj;
                if (fSoC1Point >= fTmpSoC1)
                {
                    if (index == 0)  //first record
                    {
                        output = fTmpVolt1;
                        fSoCbk = fTmpSoC1;
                        fVoltbk = fTmpVolt1;
                    }
                    else
                    {
                        if (fSoC1Point == fTmpSoC1)
                        {
                            output = fTmpVolt1;
                            fSoCbk = fTmpSoC1;
                            fVoltbk = fTmpVolt1;
                        }
                        else
                        {
                            output = fTmpVolt1;    //no modify fSoCbk, fVoltbk
                        }
                    }
                    break;
                }
                else
                {
                    fSoCbk = fTmpSoC1;
                    fVoltbk = fTmpVolt1;
                }
            }
            if (index < sourceData.AdjustedExpData.Count)      //in experiment range
            {
                if ((Math.Abs(fSoCbk - fSoC1Point) > 5) ||
                    (Math.Abs(fSoC1Point - fTmpSoC1) > 5))//SoC difference is bigger than 5, error
                {
                    result = 1;
                }
                else
                {
                    //if in 5 (10000C unit) range, use linear interpolation
                    if (fSoCbk != fTmpSoC1)     //must use this!!
                    {
                        output += (fVoltbk - output) * (fSoC1Point - fTmpSoC1) / (fSoCbk - fTmpSoC1);
                    }
                }
            }
            else        //out of experiment range
            {
                //use linear extrapolation, use last one point copy currently and temporarily 
                if (fVoltbk != 0)
                {
                    output = fVoltbk;
                }
                else
                {
                    if (sourceData.AdjustedExpData.Count > 1)
                        output = sourceData.AdjustedExpData[sourceData.AdjustedExpData.Count - 1].fVoltage;
                }
            }
            return result;
            #endregion
        }
        public static TableMakerProduct GenerateOCVTable(Stage stage, Project project, string time, OCVModel ocvModel)
        {
            var OutFolder = $@"{GlobalSettings.LocalFolder}{project.BatteryType.Name}\{project.Name}\{GlobalSettings.ProductFolderName}\{time}";
            if (!Directory.Exists(OutFolder))
            {
                Directory.CreateDirectory(OutFolder);
            }
            string localPath = Path.Combine(OutFolder, ocvModel.FileName);
            List<string> OCVHeader = GetOCVFileHeader(stage, project);
            List<string> OCVContent = GetOCVFileContent(ocvModel.iOCVVolt);
            TableMakerService.CreateFileFromLines(localPath, OCVHeader.Concat(OCVContent).ToList());
            string remotePath, MD5;
            FileTransferHelper.FileUpload(localPath, out remotePath, out MD5);
            TableMakerProduct tmp = new TableMakerProduct();
            tmp.FilePath = remotePath;
            tmp.MD5 = MD5;
            tmp.IsValid = true;
            tmp.Type = TableMakerService.GetFileType("OCV", stage);
            return tmp;
        }
        public static string GetOCVTableFileName(Project project, Stage stage)
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
            string outputFilePath = project.BatteryType.Manufacturer + sFileSeperator + project.BatteryType.Name + sFileSeperator + project.AbsoluteMaxCapacity.ToString() + "mAhr" + sFileSeperator + "OCV" + sFileSeperator + description + ".txt";
            return outputFilePath;
        }
        private static List<string> GetOCVFileHeader(Stage stage, Project project)
        {
            //(E170317)

            List<string> strOCVHeader = new List<string>();
            strOCVHeader.Clear();
            strOCVHeader.Add(string.Format("//[Description]"));
            strOCVHeader.Add(string.Format("// Open Circuit Voltage as a function of cell capacity"));
            strOCVHeader.Add(string.Format("// This table is used at initial startup only to determine remaining cell capacity as "));
            strOCVHeader.Add(string.Format("// a fraction of full capacity, based on the open curcuit (no load, rested) cell voltage. "));
            strOCVHeader.Add(string.Format(""));
            strOCVHeader.Add(string.Format("// Please note that the cell must not have been charged or discharged for several "));
            strOCVHeader.Add(string.Format("// hours prior to this remaining capacity determination, or remaining capacity may "));
            strOCVHeader.Add(string.Format("// be considerable in error"));
            strOCVHeader.Add(string.Format(""));
            strOCVHeader.Add(string.Format("//Table Header Information:"));
            strOCVHeader.Add(string.Format(""));
            strOCVHeader.Add(string.Format("//Manufacturer = {0}", project.BatteryType.Manufacturer));
            strOCVHeader.Add(string.Format("//Battery Type = {0}", project.BatteryType.Name));
            strOCVHeader.Add(string.Format("//Equipment = "));
            strOCVHeader.Add(string.Format("//Built Date = {0} {1} {2}", DateTime.Now.Year.ToString("D4"), DateTime.Now.Month.ToString("D2"), DateTime.Now.Day.ToString("D2")));
            strOCVHeader.Add(string.Format("//MinimalVoltage = {0}", project.CutoffDischargeVoltage));
            strOCVHeader.Add(string.Format("//MaximalVoltage = {0}", project.LimitedChargeVoltage));
            strOCVHeader.Add(string.Format("//FullAbsoluteCapacity = {0}", project.AbsoluteMaxCapacity));
            strOCVHeader.Add(string.Format("//Age = {0}", "1"));
            strOCVHeader.Add(string.Format("//Tester = "));
            strOCVHeader.Add(string.Format("//Battery ID = "));
            strOCVHeader.Add(string.Format("//Version = "));
            strOCVHeader.Add(string.Format("//Date = "));
            strOCVHeader.Add(string.Format("//Comment = "));
            int type_id = TableMakerService.GetFileTypeID("OCV", stage);
            strOCVHeader.Add(string.Format("//type_id = {0}", type_id.ToString()));
            //(E141024)
            strOCVHeader.Add(string.Format(""));
            return strOCVHeader;
        }

        public static List<string> GetOCVFileContent(List<Int32> inputData)
        {
            List<string> OCVContent = new List<string>();
            double fStep = 0;
            double fTemp = 0;
            Int32 iTemp = 0;
            string strXt;
            string OCVXValuesTmp;

            //(A170308)Francis,
            OCVContent.Clear();
            OCVContent.Add(string.Format(""));
            OCVContent.Add(string.Format("//table header"));
            OCVContent.Add(string.Format(""));
            OCVContent.Add(string.Format("6 \t\t //DO NOT CHANGE: word length of header(including this length)"));
            OCVContent.Add(string.Format("1 \t\t //DO NOT CHANGE: control, use as scale control "));
            OCVContent.Add(string.Format("1 \t\t //DO NOT CHANGE: number of axis"));
            //strFalconLYOCVContent.Add(string.Format("{0} \t\t //x axis points: maximum 65 points", TableVoltagePoints.Count));
            OCVContent.Add(string.Format("{0} \t\t //x axis points: maximum 65 points", inputData.Count));
            OCVContent.Add(string.Format("1 \t\t //DO NOT CHANGE: y axis entries per x axis"));
            OCVContent.Add(string.Format("{0} \t\t //DO NOT CHANGE: total length in points", inputData.Count * 2 + 6));
            OCVContent.Add(string.Format(""));
            OCVContent.Add(string.Format("//x (independent) axis: low cell open circuit millivolts:"));
            OCVContent.Add(string.Format("// (this is the cell voltage read after 24 hours \"rest\": no charge or discharge)"));
            OCVContent.Add(string.Format("//must be in increasing order: need not be evenly spaced"));
            OCVContent.Add(string.Format(""));
            strXt = "";
            foreach (Int32 idata in inputData)
            {
                strXt += string.Format("{0}, ", idata);
            }
            //(A140917)Francis, bugid=15206, delete last comma ','
            strXt = strXt.Substring(0, strXt.Length - 2);
            //(E140917)
            OCVContent.Add(strXt);
            OCVContent.Add(string.Format(""));
            OCVContent.Add(string.Format(""));
            OCVContent.Add(string.Format(""));
            OCVContent.Add(string.Format("//y (dependent) axis: 10000 * full capacity (at .02C or less) for above voltages "));
            OCVContent.Add(string.Format(""));
            fStep = (double)(iMaxPercent - iMinPercent) /
                    (double)(TableMakerService.iNumOfPoints - 1);
            //strXt = "";
            OCVXValuesTmp = "";

            iTemp = 0;
            fTemp = 0F;
            for (int i = 0; i < TableMakerService.iNumOfPoints; i++)
            {
                //iTemp = Int32.Parse((fStep * i).ToString());
                OCVXValuesTmp += string.Format("{0:D5}, ", iTemp);
                fTemp += (double)(fStep);
                iTemp = Convert.ToInt32(Math.Round(fTemp, 0));
            }
            //(A140917)Francis, bugid=15206, delete last comma ','
            OCVXValuesTmp = OCVXValuesTmp.Substring(0, OCVXValuesTmp.Length - 2);
            //(E140917)
            OCVContent.Add(OCVXValuesTmp);
            //(E170308)
            return OCVContent;
        }
        public static bool GenerateOCVTableFile(ref UInt32 uErr, string filePath, List<string> oCVHeader, List<string> oCVContent)
        {
            //int iline = 0;
            bool bReturn = false;
            FileStream fsOCV = null;
            StreamWriter stmOCV = null;

            try
            {
                fsOCV = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                stmOCV = new StreamWriter(fsOCV, new UTF8Encoding(false));
            }
            catch (Exception ec)
            {
                uErr = 1;
                return bReturn;
            }

            foreach (string stocvh in oCVHeader)
            {
                stmOCV.WriteLine(stocvh);
            }
            foreach (string stocvc in oCVContent)
            {
                stmOCV.WriteLine(stocvc);
            }

            stmOCV.Close();
            fsOCV.Close();

            bReturn = true;

            return bReturn;
        }
        #endregion
    }
}