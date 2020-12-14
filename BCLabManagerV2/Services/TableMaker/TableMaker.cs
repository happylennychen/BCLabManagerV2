using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using BCLabManager.Model;
using MathNet.Numerics;

namespace BCLabManager
{
    static public class TableMaker
    {
        static public int iNumOfPoints = 65;
        static public float fPerSteps = 1.5625F;
        static public int iMinPercent = 0;
        static public int iMaxPercent = 10000;
        static public int iSOCStepmV = 16;
        const string version = "V020";
        static public int iNumOfMiniPoints = 100;           //(A200831)francis, for table_mini
        static public float fMiniTableSteps = 1;        //(A200831)francis, for table_mini
        public static List<Int32> GenerateSampleCellTempData()
        {
            List<Int32> ilstCellTempData = new List<Int32>();
            ilstCellTempData.Clear();
            ilstCellTempData.Add(74);
            ilstCellTempData.Add(1200);
            ilstCellTempData.Add(84);
            ilstCellTempData.Add(1150);
            ilstCellTempData.Add(96);
            ilstCellTempData.Add(1100);
            ilstCellTempData.Add(110);
            ilstCellTempData.Add(1050);
            ilstCellTempData.Add(126);
            ilstCellTempData.Add(1000);
            ilstCellTempData.Add(145);
            ilstCellTempData.Add(950);
            ilstCellTempData.Add(167);
            ilstCellTempData.Add(900);
            ilstCellTempData.Add(192);
            ilstCellTempData.Add(850);
            ilstCellTempData.Add(222);
            ilstCellTempData.Add(800);
            ilstCellTempData.Add(257);
            ilstCellTempData.Add(750);
            ilstCellTempData.Add(297);
            ilstCellTempData.Add(700);
            ilstCellTempData.Add(343);
            ilstCellTempData.Add(650);
            ilstCellTempData.Add(397);
            ilstCellTempData.Add(600);
            ilstCellTempData.Add(458);
            ilstCellTempData.Add(550);
            ilstCellTempData.Add(528);
            ilstCellTempData.Add(500);
            ilstCellTempData.Add(607);
            ilstCellTempData.Add(450);
            ilstCellTempData.Add(694);
            ilstCellTempData.Add(400);
            ilstCellTempData.Add(789);
            ilstCellTempData.Add(350);
            ilstCellTempData.Add(892);
            ilstCellTempData.Add(300);
            ilstCellTempData.Add(1000);
            ilstCellTempData.Add(250);
            ilstCellTempData.Add(1111);
            ilstCellTempData.Add(200);
            ilstCellTempData.Add(1223);
            ilstCellTempData.Add(150);
            ilstCellTempData.Add(1333);
            ilstCellTempData.Add(100);
            ilstCellTempData.Add(1437);
            ilstCellTempData.Add(50);
            ilstCellTempData.Add(1534);
            ilstCellTempData.Add(0);
            ilstCellTempData.Add(1621);
            ilstCellTempData.Add(-50);
            ilstCellTempData.Add(1697);
            ilstCellTempData.Add(-100);
            ilstCellTempData.Add(1762);
            ilstCellTempData.Add(-150);
            ilstCellTempData.Add(1817);
            ilstCellTempData.Add(-200);
            ilstCellTempData.Add(1861);
            ilstCellTempData.Add(-250);
            ilstCellTempData.Add(1896);
            ilstCellTempData.Add(-300);
            ilstCellTempData.Add(1924);
            ilstCellTempData.Add(-350);
            ilstCellTempData.Add(1945);
            ilstCellTempData.Add(-400);
            return ilstCellTempData;
        }
        public static List<float> GetOCVSocPoints()
        {
            var lstfPoints = new List<float>();
            float fSoCStep = fPerSteps * 100;		//=156.25
            int iSoCCount = iNumOfPoints;		//=65
            for (int i = 0; i < iSoCCount; i++)
            {
                lstfPoints.Add(fSoCStep * i);
            }

            lstfPoints.Sort();
            return lstfPoints;
        }
        public static bool CreateFile(string strStandardH, List<string> fileContent)
        {
            FileStream fs;
            StreamWriter sw;

            try
            {
                fs = File.Open(strStandardH, FileMode.Create, FileAccess.Write, FileShare.None);
                sw = new StreamWriter(fs, new UTF8Encoding(false));// Encoding.Default);
            }
            catch (Exception eh)
            {

                return false;
            }

            foreach (var line in fileContent)
            {
                sw.WriteLine(line);
            }
            sw.Close();
            fs.Close();
            return true;
        }
        public static void Make( Project project, IEnumerable<Program> programs, List<Tester> testers, bool v1, bool v2, bool v3, bool v4, bool v5)
        {
            OCVModel ocvModel = null;
            RCModel rcModel = null;
            MiniModel miniModel = null;
            List<SourceData> ocvSource = null;
            List<SourceData> rcSource = null;
            List<TableMakerProduct> tableMakerProducts = new List<TableMakerProduct>();
            if (v1)
            {
                ocvSource = GetOCVSource(project, programs.SingleOrDefault(o => o.Type.Name == "OCV"), testers);
                ocvModel = GetOCVModel(ocvSource);
                /*TableMakerProduct ocvTable =*/
                GenerateOCVTable(project, ocvModel);
                //tableMakerProducts.Add(ocvTable);
            }

            if (v2)
            {
                rcSource = GetRCSource(project, programs.Select(o => o).Where(o => o.Type.Name == "RC").ToList(), testers);
                rcModel = GetRCModel(rcSource, project);
                /*TableMakerProduct rcTable =*/
                GenerateRCTable(project, rcModel);
                //tableMakerProducts.Add(rcTable);
            }
            if (v3)
            {
                GenerateDriver(new StandardContentConverter(), ocvModel, rcModel, project);
            }
            if (v4)
            {
                GenerateDriver(new AndroidContentConverter(), ocvModel, rcModel, project);
            }
            if (v5)
            {
                miniModel = GetMiniModel(ocvSource, rcSource, ocvModel, rcModel, project);
                GenerateMini(miniModel, project);
            }
            //foreach (var tmp in tableMakerProducts) 
            //    tableMakerProductService.SuperAdd(tmp);
        }
        #region OCV
        private static List<SourceData> GetOCVSource(Project project, Program program, List<Tester> testers)
        {
            var trs = program.Recipes.Select(o => o.TestRecords.ToList()).ToList();
            List<TestRecord> testRecords = new List<TestRecord>();
            foreach (var tr in trs)
            {
                testRecords = testRecords.Concat(tr).ToList();
            }
            List<SourceData> MaxSDList = new List<SourceData>();   //找出最大的sd
            foreach (var rtName in program.RecipeTemplates)
            {
                var trGroup = testRecords.Select(o => o).Where(o => o.RecipeStr.Contains($"Deg-{rtName}")).ToList();
                List<SourceData> SDList = new List<SourceData>();   //找出最大的sd
                foreach (var tr in trGroup)
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
                }
                SourceData maxSD = SDList.OrderByDescending(o => o.fAccmAhrCap).First();
                MaxSDList.Add(maxSD);
            }
            return MaxSDList;
        }
        private static OCVModel GetOCVModel(List<SourceData> MaxSDList)
        {
            OCVModel output = new OCVModel();



            Int32 iMinVoltage;
            Int32 iMaxVoltage;
            GetVoltageBondary(MaxSDList, out iMinVoltage, out iMaxVoltage);
            List<Int32> iOCVVolt;
            if (CreateNewOCVPoints(MaxSDList, iMinVoltage, iMaxVoltage, out iOCVVolt))
            {
                ;
            }
            output.iOCVVolt = iOCVVolt;
            return output;
        }
        private static void GetVoltageBondary(List<SourceData> list, out Int32 iMinVoltage, out Int32 iMaxVoltage)
        {
            iMinVoltage = 99999;
            iMaxVoltage = -99999;
            foreach (var sd in list)
            {
                if (iMinVoltage > sd.fMinExpVolt)
                {
                    iMinVoltage = (int)(sd.fMinExpVolt + 0.5F);
                }
                if (iMaxVoltage < sd.fMaxExpVolt)
                {
                    iMaxVoltage = (int)sd.fMaxExpVolt;
                }
            }
        }

        private static bool CreateNewOCVPoints(List<SourceData> lstSample2, int iMinVoltage, int iMaxVoltage, out List<int> iOCVVolt)
        {
            iOCVVolt = new List<int>();
            bool bReturn = false;
            UInt32 result = 0;
            SourceData lowcurSample, higcurSample;
            float fSoCVoltLow, fSoCVoltHigh, fTmpVolt1;
            float fRsocn = 1.0F;
            int fmulti = (int)(((float)(iMaxVoltage - iMinVoltage)) * 10F / iSOCStepmV);
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
            fTmpVolt1 = fmulti * iSOCStepmV + iMinVoltage;

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

            var lstfPoints = GetOCVSocPoints();
            lstfPoints.Reverse();		//from high to low SoC
            foreach (float fSoC1Point in lstfPoints)
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
            if (iOCVVolt.Count != iNumOfPoints)
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
        private static UInt32 GetSoCVolt(SourceData sourceData, float fSoC1Point, out float output)
        {
            UInt32 result = 0;
            #region find <SoC, Volt> from low current data
            output = 0;
            float fTmpSoC1 = 0; //fTmpSoC2 = 0; //default value
            float fSoCbk = 0; float fVoltbk = 0;    //default value
            float fTmpVolt1;
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
        private static TableMakerProduct GenerateOCVTable(Project project, OCVModel ocvModel)
        {
            string filePath = GetOCVTableFilePath(project);
            List<string> OCVHeader = GetOCVFileHeader(project);
            List<string> OCVContent = GetOCVFileContent(ocvModel.iOCVVolt);
            UInt32 result = 0;
            //GenerateOCVTableFile(ref result, filePath, OCVHeader, OCVContent);
            TableMaker.CreateFile(filePath, OCVHeader.Concat(OCVContent).ToList());
            TableMakerProduct tmp = new TableMakerProduct();
            tmp.FilePath = filePath;
            tmp.IsValid = true;
            tmp.Project = project;
            return tmp;
        }
        private static string GetOCVTableFilePath(Project project)
        {
            string folder = $@"{GlobalSettings.RootPath}{project.BatteryType.Name}\{project.Name}\{GlobalSettings.ProductFolderName}";
            string sFileSeperator = "_";
            //(A170308)Francis, falconly use file output folder
            string outputFilePath = "OCV" + sFileSeperator + project.BatteryType.Manufacturer +
                                sFileSeperator + project.BatteryType.Name +
                                sFileSeperator + project.AbsoluteMaxCapacity.ToString() + "mAhr" +
                                sFileSeperator + project.LimitedChargeVoltage + "mV" +
                                sFileSeperator + project.CutoffDischargeVoltage + "mV" +
                                sFileSeperator + version +
                                sFileSeperator + DateTime.Now.Year.ToString("D4") +
                                DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2") +
                                "_Arm.txt";
            outputFilePath = System.IO.Path.Combine(folder, outputFilePath);
            return outputFilePath;
        }
        private static List<string> GetOCVFileHeader(Project project)
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
            //(E141024)
            strOCVHeader.Add(string.Format(""));
            return strOCVHeader;
        }

        public static List<string> GetOCVFileContent(List<Int32> inputData)
        {
            List<string> OCVContent = new List<string>();
            float fStep = 0;
            float fTemp = 0;
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
            fStep = (float)(iMaxPercent - iMinPercent) /
                    (float)(iNumOfPoints - 1);
            //strXt = "";
            OCVXValuesTmp = "";

            iTemp = 0;
            fTemp = 0F;
            for (int i = 0; i < iNumOfPoints; i++)
            {
                //iTemp = Int32.Parse((fStep * i).ToString());
                OCVXValuesTmp += string.Format("{0:D5}, ", iTemp);
                fTemp += (float)(fStep);
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
        #region RC
        private static List<SourceData> GetRCSource(Project project, List<Program> programs, List<Tester> testers)
        {
            var trs = programs.Select(o => o.Recipes.Select(i => i.TestRecords.Where(j => j.Status == TestStatus.Completed).ToList()).ToList()).ToList();
            List<TestRecord> testRecords = new List<TestRecord>();
            foreach (var tr in trs)
            {
                foreach (var t in tr)
                    testRecords = testRecords.Concat(t).ToList();
            }
            List<SourceData> SDList = new List<SourceData>();
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
            }
            return SDList;
        }
        private static RCModel GetRCModel(List<SourceData> SDList, Project project)
        {
            RCModel output = new RCModel();
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
            return output;
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

        private static bool CreateYPoints_CTAV0026(List<uint> TableVoltagePoints, ref List<Int32> ypoints, ref float fRefMaxDiff, ref float fRefCCount, List<DataRow> inListRCData, ref UInt32 uErr, float fDesignCapacity, float fCapaciDiff)
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

        private static TableMakerProduct GenerateRCTable(Project project, RCModel rcModel)
        {
            string filePath = GetRCTableFilePath(project);
            var strRCHeader = GetRCFileHeader(project, rcModel.fCTABase, rcModel.fCTASlope);
            var strRCContent = GetRCFileContent(rcModel.outYValue, project.VoltagePoints, rcModel.listfTemp, rcModel.listfCurr);
            UInt32 uErr = 0;
            TableMaker.CreateFile(filePath, strRCHeader.Concat(strRCContent).ToList());
            TableMakerProduct tmp = new TableMakerProduct();
            tmp.FilePath = filePath;
            tmp.IsValid = true;
            tmp.Project = project;
            return tmp;
        }
        private static string GetRCTableFilePath(Project project)
        {
            string folder = $@"{GlobalSettings.RootPath}{project.BatteryType.Name}\{project.Name}\{GlobalSettings.ProductFolderName}";
            string sFileSeperator = "_";
            //(A170308)Francis, falconly use file output folder
            string outputFilePath = "RC" + sFileSeperator + project.BatteryType.Manufacturer +
                                sFileSeperator + project.BatteryType.Name +
                                sFileSeperator + project.AbsoluteMaxCapacity + "mAhr" +
                                sFileSeperator + project.LimitedChargeVoltage + "mV" +
                                sFileSeperator + project.CutoffDischargeVoltage + "mV" +
                                sFileSeperator + version +
                                sFileSeperator + DateTime.Now.Year.ToString("D4") +
                                DateTime.Now.Month.ToString("D2") + DateTime.Now.Day.ToString("D2") +
                                ".txt";
            outputFilePath = System.IO.Path.Combine(folder, outputFilePath);
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

        private static List<string> GetRCFileContent(List<List<Int32>> rcYval, List<uint> TableVoltagePoints, List<float> listfTemp, List<float> listfCurr)
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
        #region Driver
        private static void GenerateDriver(IContentConverter iContentConverter, OCVModel ocvModel, RCModel rcModel, Project project)
        {
            List<string> strFilePaths;
            GetFilePaths(project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), iContentConverter.Type, out strFilePaths);
            List<string> strHHeaderComments;
            UInt32 uErr = 0;
            InitializeHeaderInfor(ref uErr, project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), project.LimitedChargeVoltage.ToString(), project.CutoffDischargeVoltage.ToString(), out strHHeaderComments);
            var OutFolder = $@"{GlobalSettings.RootPath}{project.BatteryType.Name}\{project.Name}\{GlobalSettings.ProductFolderName}";
            GenerateCHFiles(iContentConverter, ref uErr, OutFolder, strFilePaths[0], strFilePaths[1], strHHeaderComments, ocvModel.iOCVVolt, project.VoltagePoints, rcModel.listfTemp, rcModel.listfCurr, rcModel.outYValue, rcModel.fCTABase, rcModel.fCTASlope);
        }
        public static bool GetFilePaths(string manufacturer, string betteryType, string absMaxCap, string type, out List<string> strFilePaths)
        {
            bool bReturn = false;
            string strTmpFile = "";
            strFilePaths = new List<string>();

            if (true)
            {
                strTmpFile = manufacturer + "_" + betteryType + "_" + absMaxCap.ToString() + "mAhr";
                string strCFileStandardName = strTmpFile + "_" + type + ".c";
                string strHFileStandardName = strTmpFile + "_" + type + ".h";
                strFilePaths.Add(strCFileStandardName);
                strFilePaths.Add(strHFileStandardName);
            }

            return bReturn;
        }
        public static bool InitializeHeaderInfor(ref UInt32 uErr, string manufacturer, string betteryType, string absMaxCap, string limitChgVolt, string cutOffDsgVolt, out List<string> strHHeaderComments)
        {
            bool bReturn = false;
            strHHeaderComments = new List<string>();

            if (true)
            {
                string strAndroidManufacture = manufacturer;
                string strAndroidBatteryModel = betteryType;
                string fAndroidFullCap = absMaxCap;
                string fAndroidLimitChgVoltage = limitChgVolt;
                string fAndroidCutoffDsgVoltage = cutOffDsgVolt;
                string strAndroidEquip = "";
                string strAndroidTester = "";
                string strAndroidBatteryID = "";


                #region add comment header string content for C and H file
                strHHeaderComments.Add(string.Format("/*****************************************************************************"));
                strHHeaderComments.Add(string.Format("* Copyright(c) O2Micro, 2019. All rights reserved."));
                strHHeaderComments.Add(string.Format("*"));
                strHHeaderComments.Add(string.Format("* O2Micro battery gauge driver"));
                strHHeaderComments.Add(string.Format("* File: "));  //4
                strHHeaderComments.Add(string.Format("*"));
                strHHeaderComments.Add(string.Format("* $Source: /data/code/CVS"));
                strHHeaderComments.Add(string.Format("* $Revision: 4.00.01 $"));
                strHHeaderComments.Add(string.Format("*"));
                strHHeaderComments.Add(string.Format("* This program is free software and can be redistributed with or without modification"));
                strHHeaderComments.Add(string.Format("* it under the terms of the GNU General Public License version 2 as"));
                strHHeaderComments.Add(string.Format("* published by the Free Software Foundation."));
                strHHeaderComments.Add(string.Format("*"));
                strHHeaderComments.Add(string.Format("* This Source Code Reference Design for O2MICRO Battery Gauge access (\\u201cReference Design\\u201d) "));
                strHHeaderComments.Add(string.Format("* is sole for the use of PRODUCT INTEGRATION REFERENCE ONLY, and contains confidential "));
                strHHeaderComments.Add(string.Format("* and privileged information of O2Micro International Limited. O2Micro shall have no "));
                strHHeaderComments.Add(string.Format("* liability to any PARTY FOR THE RELIABILITY, SERVICEABILITY FOR THE RESULT OF PRODUCT "));
                strHHeaderComments.Add(string.Format("* INTEGRATION, or results from: (i) any modification or attempted modification of the "));
                strHHeaderComments.Add(string.Format("* Reference Design by any party, or (ii) the combination, operation or use of the "));
                strHHeaderComments.Add(string.Format("* Reference Design with non-O2Micro Reference Design."));
                strHHeaderComments.Add(string.Format("*"));
                strHHeaderComments.Add(string.Format("* Battery Manufacture: {0}", strAndroidManufacture));
                strHHeaderComments.Add(string.Format("* Battery Model: {0}", strAndroidBatteryModel));
                strHHeaderComments.Add(string.Format("* Absolute Max Capacity(mAhr): {0}", fAndroidFullCap));
                strHHeaderComments.Add(string.Format("* Limited Charge Voltage(mV): {0}", fAndroidLimitChgVoltage));
                strHHeaderComments.Add(string.Format("* Cutoff Discharge Voltage(mV): {0}", fAndroidCutoffDsgVoltage));
                strHHeaderComments.Add(string.Format("* Equipment: {0}", strAndroidEquip)); //26
                strHHeaderComments.Add(string.Format("* Tester: {0}", strAndroidTester));   //27
                strHHeaderComments.Add(string.Format("* Battery ID: {0}", strAndroidBatteryID));    //28
                                                                                                    //(A141024)Francis, add Version/Date/Comment 3 string into header comment of txt file
                strHHeaderComments.Add(string.Format("* Version = "));
                strHHeaderComments.Add(string.Format("* Date = "));
                strHHeaderComments.Add(string.Format("* Comment = "));
                //(E141024)
                strHHeaderComments.Add(string.Format("*****************************************************************************/"));
                strHHeaderComments.Add(string.Format(""));
                #endregion
            }

            return bReturn;
        }

        //Initialize content of H file, currently using hard coding in code, but hopely we can read it from file, a sample file in particular folder

        private static bool GenerateCHFiles(IContentConverter icc, ref UInt32 uErr, string OutFolder, string strCFileStandardName, string strHFileStandardName, List<string> strHHeaderComments, List<int> ilstOCVVolt, List<uint> voltList, List<float> listfTemp, List<float> listfCurr, List<List<int>> outYValue, double fCTABase, double fCTASlope)
        {
            bool bReturn;
            string standardCFilePath = System.IO.Path.Combine(OutFolder, strCFileStandardName);
            string standardHFilePath = System.IO.Path.Combine(OutFolder, strHFileStandardName);

            List<string> hFileContent = icc.GetHFileContent(strHFileStandardName, strHHeaderComments, ilstOCVVolt, voltList, listfCurr, listfTemp, fCTABase, fCTASlope);
            TableMaker.CreateFile(standardHFilePath, hFileContent);

            List<string> cFileContent = icc.GetCFileContent(strCFileStandardName, strHFileStandardName, strHHeaderComments, ilstOCVVolt, voltList, listfTemp, listfCurr, outYValue, fCTABase, fCTASlope);
            TableMaker.CreateFile(standardCFilePath, cFileContent);

            bReturn = true;
            uErr = 1;

            return bReturn;
        }

        #region Mini
        private static MiniModel GetMiniModel(List<SourceData> ocvSource, List<SourceData> rcSource, OCVModel ocvModel, RCModel rcModel, Project project)
        {
            MiniModel output = new MiniModel();
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
            return output;
        }
        private static bool GetLstTblM_OCV(List<SourceData> lstSample2, out List<float> fLstTblM_OCV)
        {
            Int32 iMinVoltage;
            Int32 iMaxVoltage;
            GetVoltageBondary(lstSample2, out iMinVoltage, out iMaxVoltage);
            fLstTblM_OCV = new List<float>();
            bool bReturn = false;
            int indexLow, indexHigh;
            SourceData lowcurSample, higcurSample;
            float fTmpSoC1, fTmpSoC2, fTmpVolt1;
            int fmulti = (int)(((float)(iMaxVoltage - iMinVoltage)) * 10F / iSOCStepmV);
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
            fTmpVolt1 = fmulti * iSOCStepmV + iMinVoltage;

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
            Int32 iMinVoltage;
            Int32 iMaxVoltage;
            GetVoltageBondary(rcSource, out iMinVoltage, out iMaxVoltage);
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

        private static void GenerateMini(MiniModel miniModel, Project project)
        {
            List<string> strFilePaths;
            GetFilePaths(project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), "mini", out strFilePaths);
            List<string> strHHeaderComments;
            UInt32 uErr = 0;
            InitializeHeaderInfor(ref uErr, project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), project.LimitedChargeVoltage.ToString(), project.CutoffDischargeVoltage.ToString(), out strHHeaderComments);
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

            var ilstCellTempData = TableMaker.GenerateSampleCellTempData();
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
            List<float> ilstOCVSoC = GetOCVSocPoints();
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
        #endregion
    }
}