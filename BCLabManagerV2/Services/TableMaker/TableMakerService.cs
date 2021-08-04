using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using BCLabManager.Model;
using MathNet.Numerics;

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
        static public float fPerSteps { get; set; } = 1.5625F;
        static public int iSOCStepmV { get; set; } = 16;
        static public int iNumOfMiniPoints { get; set; } = 100;           //(A200831)francis, for table_mini
        static public float fMiniTableSteps { get; set; } = 1;        //(A200831)francis, for table_mini

        public static string GetLocalPath(string testFilePath)
        {
            return testFilePath.Replace(GlobalSettings.RemotePath, GlobalSettings.LocalFolder);
        }

        public static string Version { get { return "V020"; } }
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
        public static bool CreateFile(string filePath, List<string> fileContent)
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
        public static bool GetDriverFileNames(string manufacturer, string betteryType, string absMaxCap, string type, out List<string> strFilePaths)
        {
            bool bReturn = false;
            string strTmpFile = "";
            strFilePaths = new List<string>();

            if (true)
            {
                strTmpFile = manufacturer + "_" + betteryType + "_" + absMaxCap.ToString() + "mAhr";
                string strCFileStandardName = $"{strTmpFile}_{type}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.c";
                string strHFileStandardName = $"{strTmpFile}_{type}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.h";
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

        public static void GetFileNames(ref TableMakerModel tableMakerModel)
        {
            var project = tableMakerModel.Project;
            //var programs = tableMakerModel.Programs;
            var testers = tableMakerModel.Testers;

            OCVModel ocvModel = new OCVModel();
            RCModel rcModel = new RCModel();
            MiniModel miniModel = new MiniModel();
            StandardModel standardModel = new StandardModel();
            AndroidModel androidModel = new AndroidModel();
            LiteModel liteModel = new LiteModel();

            tableMakerModel.OCVModel = ocvModel;
            tableMakerModel.RCModel = rcModel;
            tableMakerModel.MiniModel = miniModel;
            tableMakerModel.StandardModel = standardModel;
            tableMakerModel.AndroidModel = androidModel;
            tableMakerModel.LiteModel = liteModel;

            ocvModel.FileName = OCVTableMaker.GetOCVTableFileName(project);
            rcModel.FileName = RCTableMaker.GetRCTableFileName(project);
            List<string> strFileNames;
            GetDriverFileNames(project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), "mini", out strFileNames);
            miniModel.FileNames = strFileNames;
            GetDriverFileNames(project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), "standard", out strFileNames);
            standardModel.FileNames = strFileNames;
            GetDriverFileNames(project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), "android", out strFileNames);
            androidModel.FileNames = strFileNames;
            GetDriverFileNames(project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), "lite", out strFileNames);
            liteModel.FileNames = strFileNames;
        }

        public static void Build(ref TableMakerModel tableMakerModel, uint uEodVoltage)
        {
            try
            {
                var project = tableMakerModel.Project;
                var ocvprograms = tableMakerModel.OCVPrograms;
                var rcprograms = tableMakerModel.RCPrograms;
                var stage1ocvprograms = tableMakerModel.Stage1OCVPrograms;
                var stage1rcprograms = tableMakerModel.Stage1RCPrograms;
                var stage2ocvprograms = tableMakerModel.Stage2OCVPrograms;
                var stage2rcprograms = tableMakerModel.Stage2RCPrograms;
                var testers = tableMakerModel.Testers;
                var ocvModel = tableMakerModel.OCVModel;
                var rcModel = tableMakerModel.RCModel;
                var miniModel = tableMakerModel.MiniModel;
                var standardModel = tableMakerModel.StandardModel;
                var androidModel = tableMakerModel.AndroidModel;
                var liteModel = tableMakerModel.LiteModel;

                //List<SourceData> ocvSource;
                //OCVTableMaker.GetOCVSource(project, programs.Where(o => o.Type.Name == "OCV").ToList(), testers, out ocvSource, isRemoteData);
//=======
                var stage1ocvModel = tableMakerModel.Stage1OCVModel;
                var stage1rcModel = tableMakerModel.Stage1RCModel;
                var stage1miniModel = tableMakerModel.Stage1MiniModel;
                var stage1standardModel = tableMakerModel.Stage1StandardModel;
                var stage1androidModel = tableMakerModel.Stage1AndroidModel;
                var stage1liteModel = tableMakerModel.Stage1LiteModel;
                var stage2ocvModel = tableMakerModel.Stage2OCVModel;
                var stage2rcModel = tableMakerModel.Stage2RCModel;
                var stage2miniModel = tableMakerModel.Stage2MiniModel;
                var stage2standardModel = tableMakerModel.Stage2StandardModel;
                var stage2androidModel = tableMakerModel.Stage2AndroidModel;
                var stage2liteModel = tableMakerModel.Stage2LiteModel;


                GenerateFilePackage(ocvprograms, rcprograms, project, testers, uEodVoltage, ref ocvModel, ref rcModel, ref miniModel, ref standardModel, ref androidModel, ref liteModel);
                //GenerateFilePackage(stage1ocvprograms, stage1rcprograms, project, testers, ref stage1ocvModel, ref stage1rcModel, ref stage1miniModel, ref stage1standardModel, ref stage1androidModel);
                //GenerateFilePackage(stage2ocvprograms, stage2rcprograms, project, testers, ref stage2ocvModel, ref stage2rcModel, ref stage2miniModel, ref stage2standardModel, ref stage2androidModel);


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private static void GenerateFilePackage(List<Program> ocvprograms, List<Program> rcprograms, Project project, List<Tester> testers, uint uEodVoltage, ref OCVModel ocvModel, ref RCModel rcModel, ref MiniModel miniModel, ref StandardModel standardModel, ref AndroidModel androidModel, ref LiteModel liteModel)
        {
            List<SourceData> ocvSource = null;
            if (ocvprograms != null && ocvprograms.Count != 0)
            {
                OCVTableMaker.GetOCVSource(project, ocvprograms, testers, out ocvSource);
//>>>>>>> DataGroupingForTableMaker
                OCVTableMaker.GetOCVModel(ocvSource, ref ocvModel);
                OCVTableMaker.GenerateOCVTable(project, ocvModel);
            }

            if (rcprograms != null && rcprograms.Count != 0 && ocvSource != null)
            {
                List<SourceData> rcSource;
//<<<<<<< HEAD
                //RCTableMaker.GetRCSource(project, programs.Select(o => o).Where(o => o.Type.Name == "RC").ToList(), testers, out rcSource, isRemoteData);
                //=======
                RCTableMaker.GetRCSource(project, rcprograms, testers, out rcSource);
                //>>>>>>> DataGroupingForTableMaker
                RCTableMaker.GetRCModel(rcSource, project, ref rcModel);
                MiniDriverMaker.GetMiniModel(ocvSource, rcSource, ocvModel, rcModel, project, ref miniModel);

                StandardDriverMaker.GetStandardModel(ocvModel, rcModel, ref standardModel);

                AndroidDriverMaker.GetAndroidModel(ocvModel, rcModel, ref androidModel);
//<<<<<<< HEAD


                //OCVTableMaker.GenerateOCVTable(project, ocvModel, isRemoteOutput);
                //RCTableMaker.GenerateRCTable(project, rcModel, isRemoteOutput);
                //MiniDriverMaker.GenerateMiniDriver(miniModel, project, isRemoteOutput);
                //StandardDriverMaker.GenerateStandardDriver(standardModel, project, isRemoteOutput);
                //AndroidDriverMaker.GenerateAndroidDriver(androidModel, project, isRemoteOutput);

                //LiteDriverMaker.GetLiteModel(uEodVoltage, ocvSource, rcSource, ocvModel, rcModel, project, ref liteModel);
                //LiteDriverMaker.GenerateLiteDriver(liteModel, project, isRemoteOutput);
                //=======
                RCTableMaker.GenerateRCTable(project, rcModel);
                MiniDriverMaker.GenerateMiniDriver(miniModel, project);
                StandardDriverMaker.GenerateStandardDriver(standardModel, project);
                AndroidDriverMaker.GenerateAndroidDriver(androidModel, project);

                LiteDriverMaker.GetLiteModel(uEodVoltage, ocvSource, rcSource, ocvModel, rcModel, project, ref liteModel);
                LiteDriverMaker.GenerateLiteDriver(liteModel, project);
                //>>>>>>> DataGroupingForTableMaker
            }
        }
    }
}