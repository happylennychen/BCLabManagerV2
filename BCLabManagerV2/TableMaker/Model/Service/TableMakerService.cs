﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using MathNet.Numerics;
using Microsoft.EntityFrameworkCore;

namespace BCLabManager
{
    static public class TableMakerService
    {
        //(A20210610)Francis, as discussed with Kyle, TM should have a unique value for distinguishing by EmulatorII
        //value is synchronzed with EmulatorII and 10.22.4.249 DB
        public enum PRODUCT_TYPE_ID : ushort
        {
            PRODTYPE_OCVBYSOC = 1,
            PRODTYPE_SOCBYOCV,
            PRODTYPE_OCVTXT,
            PRODTYPE_RCTXT,
            PRODTYPE_RC_FALCONLY,
            PRODTYPE_HFILE_FWDRV,
            PRODTYPE_CFILE_FWDRV,
            PRODTYPE_HFILE_FALCONLY,
            PRODTYPE_CFILE_FALCONLY,
            PRODTYPE_HFILE_STANDARD,//=10
            PRODTYPE_CFILE_STANDARD,
            PRODTYPE_HFILE_MINI,    //=12
            PRODTYPE_CFILE_MINI,
            PRODTYPE_HFILE_LITE,    //=14
            PRODTYPE_CFILE_LITE,
            PRODTYPE_HFILE_RVTABLE, //=16, this is not synchronized to 10.22.4.249
            PRODTYPE_CFILE_RVTABLE,
            PRODTYPE_HFILE_KF,      //=18
            PRODTYPE_CFILE_KF,
            PRODTYPE_OCVTXT_STAGE0,         //=20, above, and equal to 20 are not synchronzed to 10.22.4.249 DB
            PRODTYPE_RCTXT_STAGE0,
            PRODTYPE_HFILE_STAGE0_STD,      //=22
            PRODTYPE_CFILE_STAGE0_STD,
            PRODTYPE_HFILE_STAGE0_MINI,     //=24
            PRODTYPE_CFILE_STAGE0_MINI,
            PRODTYPE_HFILE_STAGE0_LITE,     //=26
            PRODTYPE_CFILE_STAGE0_LITE,
            PRODTYPE_OCVTXT_STAGE1,         //=28
            PRODTYPE_RCTXT_STAGE1,
            PRODTYPE_HFILE_STAGE1_STD,      //=30
            PRODTYPE_CFILE_STAGE1_STD,
            PRODTYPE_HFILE_STAGE1_MINI,     //=32
            PRODTYPE_CFILE_STAGE1_MINI,
            PRODTYPE_HFILE_STAGE1_LITE,     //=34
            PRODTYPE_CFILE_STAGE1_LITE,
            PRODTYPE_MAX            //=36
        };
        static public int iNumOfPoints { get; set; } = 65;
        static public double fPerSteps { get; set; } = 1.5625F;
        static public int iSOCStepmV { get; set; } = 16;
        static public int iNumOfMiniPoints { get; set; } = 100;           //(A200831)francis, for table_mini
        static public double fMiniTableSteps { get; set; } = 1;        //(A200831)francis, for table_mini

        public static string Version { get { return "V01"; } }
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
        public static bool GetSourceV2(Project project, List<TestRecord> testRecords, List<Tester> testers, out List<SourceData> SDList, out List<string> Sources)
        {
            SDList = new List<SourceData>();
            Sources = new List<string>();
            foreach (var tr in testRecords)
            {
                if (string.IsNullOrEmpty(tr.StdFilePath))
                    continue;
                SourceData sd = new SourceData();
                sd.fAbsMaxCap = project.AbsoluteMaxCapacity;
                sd.fCapacityDiff = tr.CapacityDifference;
                sd.fCurrent = tr.Current * (-1);
                sd.fCutoffDsgVolt = project.CutoffDischargeVoltage;
                sd.fLimitChgVolt = project.LimitedChargeVoltage;
                sd.fMeasureGain = tr.MeasurementGain;
                sd.fMeasureOffset = tr.MeasurementOffset;
                sd.fTemperature = tr.Temperature;
                sd.fTraceResis = tr.TraceResistance;
                if (SDList.Any(o => o.fCurrent == sd.fCurrent && o.fTemperature == sd.fTemperature))
                {
                    if (MessageBoxResult.Yes == MessageBox.Show($"Do you want to keep {tr.StdFilePath} instead of original file?", "Same Point Check", MessageBoxButton.YesNo))
                    {
                        var removeList = SDList.Select(o => o).Where(o => o.fCurrent == sd.fCurrent && o.fTemperature == sd.fTemperature).ToList();
                        foreach (var rmvsd in removeList)
                        {
                            SDList.Remove(rmvsd);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                var tester = testers.SingleOrDefault(o => o.Name == tr.TesterStr);
                var localPath = FileTransferHelper.Remote2Local(tr.StdFilePath);
                if (!File.Exists(localPath)) //本地不存在
                {
                    if (!FileTransferHelper.FileDownload(tr.StdFilePath, tr.StdMD5))  //下载不成功
                        return false;
                }
                UInt32 result = LoadStdToSource(localPath, ref sd);
                if (result == ErrorCode.NORMAL)
                {
                    SDList.Add(sd);
                    Sources.Add(tr.StdFilePath);
                }
            }
            return true;
        }

        private static uint LoadStdToSource(string filePath, ref SourceData output)
        {
            double fErrorRatio = 0.1F;       //90% * header.current
            double fErrorStep = 4.0F;
            char AcuTSeperater = ',';
            bool ret = false;
            UInt32 result = 0;
            Stream stmCSV = null;
            StreamReader stmContent = null;
            string strTemp;
            string[] strToken;
            char[] chSeperate = new char[] { AcuTSeperater };
            double fVoltn = -1F, fCurrn = -1F, fTempn = -1F, fAccmn = -1F, fVoltAdj;
            bool bReachHighVolt = false, bStartExpData = false, bReachLowVolt = false, bStopExpData = true;
            double ftmp;
            UInt16 iHighVoltDiff = 100;

            try
            {
                stmCSV = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                stmContent = new StreamReader(stmCSV);
            }
            catch (Exception e)
            {
                return ErrorCode.UNDEFINED;
            }

            //initialization
            result = 0;
            ret = true;

            while ((strTemp = stmContent.ReadLine()) != null)
            {
                #region skip other except log data line
                strToken = strTemp.Split(chSeperate, StringSplitOptions.None);  //split column by ',' character
                                                                                //if (strToken.Length < (int)O2TXTRecord.TxtAccMah)
                if (!double.TryParse(strToken[8], out ftmp)) //Leon:如果不是数据，代表不是数据行，继续读下一行，否则可执行后面的操作
                {
                    //raw data start for next line
                    continue;
                }
                #endregion

                #region call format parsing, to get correct value for log line

                //iNumSrlNow = 0;
                ftmp = 1000F;      //Chroma is in A/V/Ahr format
                                   //ParseChroFormat(strToken, out sVoltage, out sCurrent, out sTemp, out sAccM, out sDate, out iNumSrlNow);
                                   //if (!ParseStd(strToken, out sVoltage, out sCurrent, out sTemp, out sAccM, out sDate, out iNumSrlNow))
                                   //{
                                   //    return 1;
                                   //}
                StandardRow stdRow = new StandardRow(strTemp);
                if (stdRow.Index == 0)
                    continue;
                uint iNumSrlNow = stdRow.TimeInMS/1000;
                if (iNumSrlNow > (UInt32.MaxValue / 2 - 1))
                    iNumSrlNow = 1;
                if (iNumSrlNow == 0)
                    continue;
                fVoltn = stdRow.Voltage;
                fCurrn = stdRow.Current;
                fTempn = stdRow.Temperature;
                fAccmn = stdRow.Capacity;
                #endregion

                {

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
                        else if ((fCurrn < 0) && ((Math.Abs(fCurrn - output.fCurrent)) < (Math.Abs(output.fCurrent) * 0.1)))
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
                        if (result != 0)
                        {
                            //(M141117)Francis, if parsing error occurred, just let it keeps going parsing, no to break while loop
                            ret = false;
                        }
                        if (ret)
                        {
                            //RawDataNode nodeN = new RawDataNode(iNumSrlNow, sVoltage, sCurrent, sTemp, sAccM, sDate, ftmp);
                            TableMakerSourceDataRow nodeN = new TableMakerSourceDataRow(iNumSrlNow, fVoltn, fCurrn, fTempn, fAccmn);  //(M170628)Francis, did multiple before
                                                                                                                                      //TableRawData.Add(nodeN);
                            output.ReservedExpData.Add(nodeN);
                            fVoltAdj = (fVoltn - output.fMeasureOffset) / output.fMeasureGain
                                - (output.fCurrent * output.fTraceResis * 0.001F);
                            TableMakerSourceDataRow rdnAdjust = new TableMakerSourceDataRow(iNumSrlNow, fVoltAdj, fCurrn, fTempn, fAccmn);
                            output.AdjustedExpData.Add(rdnAdjust);
                        }
                    }   //if ((bStartExpData) && (!bStopExpData))
                    else if ((bStartExpData) && (bStopExpData))     //experiment data start and stop detect
                    {
                        if (ret)
                        {
                            //(A150806)Francis, if AccMah is jumping too much, skip last one record
                            if (Math.Abs(fAccmn - output.fAbsMaxCap) < (output.fAbsMaxCap * 0.05))
                            {
                                //RawDataNode nodeN = new RawDataNode(iNumSrlNow, sVoltage, sCurrent, sTemp, sAccM, sDate, ftmp);
                                TableMakerSourceDataRow nodeN = new TableMakerSourceDataRow(iNumSrlNow, fVoltn, fCurrn, fTempn, fAccmn);
                                //TableRawData.Add(nodeN);
                                output.ReservedExpData.Add(nodeN);
                                fVoltAdj = (fVoltn - output.fMeasureOffset) / output.fMeasureGain
                                    - (output.fCurrent * output.fTraceResis * 0.001F);
                                TableMakerSourceDataRow rdnAdjust = new TableMakerSourceDataRow(iNumSrlNow, fVoltAdj, fCurrn, fTempn, fAccmn);
                                output.AdjustedExpData.Add(rdnAdjust);
                            }
                        }
                        break;      //after last one ignore it
                    }
                }   //if ((sVoltage.Length != 0) && (sCurrent.Length != 0) &&
            }   //while ((strTemp = stmContent.ReadLine()) != null)
            stmContent.Close();

            double fSoCA;
            double fUsedMaxCap = output.fAccmAhrCap;



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

        public static List<double> GetOCVSocPoints()
        {
            var lstfPoints = new List<double>();
            double fSoCStep = fPerSteps * 100;		//=156.25
            int iSoCCount = iNumOfPoints;		//=65
            for (int i = 0; i < iSoCCount; i++)
            {
                lstfPoints.Add(fSoCStep * i);
            }

            lstfPoints.Sort();
            return lstfPoints;
        }
        public static void GetVoltageBondary(List<SourceData> list, out Int32 iMinVoltage, out Int32 iMaxVoltage)
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
        public static bool CreateFileFromLines(string filePath, List<string> fileContent)
        {
            FileStream fs;
            StreamWriter sw;

            try
            {
                fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
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
        public static bool GetDriverFileNames(string manufacturer, string betteryType, string absMaxCap, string type, Stage stage, out List<string> strFilePaths)
        {
            string description = (stage == Stage.N1) ? "stage1" : "stage2";
            bool bReturn = false;
            string strTmpFile = "";
            strFilePaths = new List<string>();

            if (true)
            {
                strTmpFile = manufacturer + "_" + betteryType + "_" + absMaxCap.ToString() + "mAhr";
                string strCFileStandardName = $"{strTmpFile}_{type}_{description}.c";
                string strHFileStandardName = $"{strTmpFile}_{type}_{description}.h";
                strFilePaths.Add(strCFileStandardName);
                strFilePaths.Add(strHFileStandardName);
            }

            return bReturn;
        }
        public static bool InitializeHeaderInfor(ref UInt32 uErr, string manufacturer, string betteryType, string absMaxCap, string limitChgVolt, string cutOffDsgVolt, string typeID, out List<string> strHHeaderComments)
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
                //string strAndroidEquip = "";
                //string strAndroidTester = "";
                //string strAndroidBatteryID = "";


                #region add comment header string content for C and H file
                strHHeaderComments.Add(string.Format("/*****************************************************************************"));
                strHHeaderComments.Add(string.Format("* Copyright(c) O2Micro, 2021. All rights reserved."));
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
                //strHHeaderComments.Add(string.Format("* Equipment: {0}", strAndroidEquip)); //26
                //strHHeaderComments.Add(string.Format("* Tester: {0}", strAndroidTester));   //27
                //strHHeaderComments.Add(string.Format("* Battery ID: {0}", strAndroidBatteryID));    //28
                //(A141024)Francis, add Version/Date/Comment 3 string into header comment of txt file
                //strHHeaderComments.Add(string.Format("* Version = "));
                //strHHeaderComments.Add(string.Format("* Date = "));
                //strHHeaderComments.Add(string.Format("* Comment = "));
                strHHeaderComments.Add(string.Format("* type_id = {0}", typeID));
                //(E141024)
                strHHeaderComments.Add(string.Format("*****************************************************************************/"));
                strHHeaderComments.Add(string.Format(""));
                #endregion
            }

            return bReturn;
        }

        internal static TableMakerProductType GetFileType(string v, Stage stage)
        {
            List<TableMakerProductType> types;
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                types = uow.TableMakerProductTypes.GetAll().ToList();
            }
            var id = GetFileTypeID(v, stage);
            return types.SingleOrDefault(o => o.Id == id);
        }

        internal static int GetFileTypeID(string v, Stage stage)
        {
            int id = -1;
            switch (stage)
            {
                case Stage.N0:
                    {
                        switch (v)
                        {
                            case "OCV": id = 20; break;
                            case "RC": id = 21; break;
                            case "StandardH": id = 22; break;
                            case "StandardC": id = 23; break;
                            case "MiniH": id = 24; break;
                            case "MiniC": id = 25; break;
                            case "LiteH": id = 26; break;
                            case "LiteC": id = 27; break;
                            case "AndroidH": id = 36; break;
                            case "AndroidC": id = 37; break;
                            default: id = -1; break;
                        }
                    }
                    break;
                case Stage.N1:
                    {
                        switch (v)
                        {
                            case "OCV": id = 28; break;
                            case "RC": id = 29; break;
                            case "StandardH": id = 30; break;
                            case "StandardC": id = 31; break;
                            case "MiniH": id = 32; break;
                            case "MiniC": id = 33; break;
                            case "LiteH": id = 34; break;
                            case "LiteC": id = 35; break;
                            case "AndroidH": id = 38; break;
                            case "AndroidC": id = 39; break;
                            default: id = -1; break;
                        }
                    }
                    break;
                case Stage.N2:
                    {
                        switch (v)
                        {
                            case "OCV": id = 3; break;
                            case "RC": id = 4; break;
                            case "StandardH": id = 10; break;
                            case "StandardC": id = 11; break;
                            case "MiniH": id = 12; break;
                            case "MiniC": id = 13; break;
                            case "LiteH": id = 14; break;
                            case "LiteC": id = 15; break;
                            case "AndroidH": id = 6; break;
                            case "AndroidC": id = 7; break;
                            default: id = -1; break;
                        }
                    }
                    break;
            }
            return id;
        }
    }
}