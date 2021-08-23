using System;
using System.Collections.Generic;
using System.IO;
using BCLabManager.Model;

namespace BCLabManager
{
    public static class AndroidDriverMaker
    {
        public static void GetAndroidModel(OCVModel ocvModel, RCModel rcModel, ref AndroidModel androidModel)
        {
            androidModel.iOCVVolt = ocvModel.iOCVVolt;

            androidModel.fCTABase = rcModel.fCTABase;
            androidModel.fCTASlope = rcModel.fCTASlope;
            androidModel.listfCurr = rcModel.listfCurr;
            androidModel.listfTemp = rcModel.listfTemp;
            androidModel.outYValue = rcModel.outYValue;
        }

        internal static List<TableMakerProduct> GenerateAndroidDriver(Stage stage, AndroidModel androidModel, string time, Project project, List<int> VoltagePoints)
        {
            var rootPath = string.Empty;
            //if (isRemoteOutput)
            //{
            //    rootPath = GlobalSettings.RemotePath;
            //}
            //else
            //{
                rootPath = GlobalSettings.LocalFolder;
            //}
            var OutFolder = $@"{rootPath}{project.BatteryType.Name}\{project.Name}\{GlobalSettings.ProductFolderName}\{time}";
            if (!Directory.Exists(OutFolder))
            {
                Directory.CreateDirectory(OutFolder);
            }
            List<string> strFilePaths = androidModel.FileNames;
            List<string> strHHeaderComments, strCHeaderComments;
            UInt32 uErr = 0;
            int type_id = TableMakerService.GetFileTypeID("AndroidH", stage);
            TableMakerService.InitializeHeaderInfor(ref uErr, project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), project.LimitedChargeVoltage.ToString(), project.CutoffDischargeVoltage.ToString(), type_id.ToString(), out strHHeaderComments);
            type_id = TableMakerService.GetFileTypeID("AndroidC", stage);
            TableMakerService.InitializeHeaderInfor(ref uErr, project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), project.LimitedChargeVoltage.ToString(), project.CutoffDischargeVoltage.ToString(), type_id.ToString(), out strCHeaderComments);
            return GenerateCHFiles(stage, ref uErr, OutFolder, strFilePaths[0], strFilePaths[1], strHHeaderComments, strCHeaderComments, androidModel.iOCVVolt, VoltagePoints, androidModel.listfTemp, androidModel.listfCurr, androidModel.outYValue, androidModel.fCTABase, androidModel.fCTASlope);
        }
        #region Driver

        //Initialize content of H file, currently using hard coding in code, but hopely we can read it from file, a sample file in particular folder

        private static List<TableMakerProduct> GenerateCHFiles(Stage stage, ref UInt32 uErr, string OutFolder, string strCFileStandardName, string strHFileStandardName, List<string> strHHeaderComments, List<string> strCHeaderComments, List<int> ilstOCVVolt, List<int> voltList, List<float> listfTemp, List<float> listfCurr, List<List<int>> outYValue, double fCTABase, double fCTASlope)
        {
            string localcFilePath = System.IO.Path.Combine(OutFolder, strCFileStandardName);
            string localhFilePath = System.IO.Path.Combine(OutFolder, strHFileStandardName);

            List<string> hFileContent = GetHFileContent(strHFileStandardName, strHHeaderComments, ilstOCVVolt, voltList, listfCurr, listfTemp, fCTABase, fCTASlope);
            TableMakerService.CreateFileFromLines(localhFilePath, hFileContent);

            List<string> cFileContent = GetCFileContent(strCFileStandardName, strHFileStandardName, strCHeaderComments, ilstOCVVolt, voltList, listfTemp, listfCurr, outYValue, fCTABase, fCTASlope);
            TableMakerService.CreateFileFromLines(localcFilePath, cFileContent);


            string targetPath = FileTransferHelper.GetRemotePath(localcFilePath, 5);
            FileTransferHelper.FileCopyWithMD5Check(localcFilePath, targetPath);
            List<TableMakerProduct> output = new List<TableMakerProduct>();
            TableMakerProduct ctmp = new TableMakerProduct();
            ctmp.FilePath = targetPath;
            ctmp.IsValid = true;
            ctmp.Type = TableMakerService.GetFileType("AndroidC", stage);
            output.Add(ctmp);

            targetPath = FileTransferHelper.GetRemotePath(localhFilePath, 5);
            FileTransferHelper.FileCopyWithMD5Check(localhFilePath, targetPath);
            TableMakerProduct htmp = new TableMakerProduct();
            htmp.FilePath = targetPath;
            htmp.IsValid = true;
            htmp.Type = TableMakerService.GetFileType("AndroidH", stage);
            output.Add(htmp);

            return output;
        }
        private static List<string> GetCFileContent(string strCFileStandardName, string strHFileStandardName, List<string> strHHeaderComments, List<int> ilstOCVVolt, List<int> voltList, List<float> listfTemp, List<float> listfCurr, List<List<int>> outYValue, double fCTABase, double fCTASlope)
        {
            string line = "";
            List<string> output = new List<string>();
            int i = 0;
            int iLineCmtHCFile = 4;
            string strCTmp = "";
            foreach (string scc in strHHeaderComments)
            {
                if (i == iLineCmtHCFile)
                {
                    output.Add(scc + strCFileStandardName);
                }
                else
                {
                    output.Add(scc);
                }
                i++;
            }
            output.Add("#include <linux/kernel.h>");
            output.Add("#include \"table.h\" ");
            output.Add("");
            output.Add(string.Format("/*****************************************************************************"));
            output.Add(string.Format("* Global variables section - Exported"));
            output.Add(string.Format("* add declaration of global variables that will be exported here"));
            output.Add(string.Format("* e.g."));
            output.Add(string.Format("*	int8_t foo;"));
            output.Add(string.Format("****************************************************************************/"));

            output.Add(string.Format("const char* table_version = \"xxxx\";"));
            output.Add("");
            output.Add(string.Format("const char * battery_id[BATTERY_ID_NUM] = {{ \"XXXX\", \"YYYY\" }};"));     //line 9
            output.Add($"//const int32_t fCTABase = {fCTABase};");                     //line 10
            output.Add($"//const int32_t fCTASlope = {fCTASlope};");                     //line 11
            //write ocv_data
            output.Add("");
            line = (string.Format("one_latitude_data_t ocv_data[OCV_DATA_NUM] = "));
            line += (string.Format("{{"));


            var ilstOCVSoC = TableMakerService.GetOCVSocPoints();
            for (i = 0; i < ilstOCVVolt.Count; i++)
            {
                if (i == ilstOCVVolt.Count - 1)
                    strCTmp = string.Format("{{{0}, {1}", ilstOCVVolt[i], Math.Round(ilstOCVSoC[i] / 100, 0)) + "}";
                else
                    strCTmp = string.Format("{{{0}, {1}", ilstOCVVolt[i], Math.Round(ilstOCVSoC[i] / 100, 0)) + "},";
                line += (strCTmp);
            }
            line += "};";
            output.Add(line);
            line = "";
            output.Add("");
            //write sample cell_temp_data
            output.Add("//real current to soc ");
            output.Add("//one_latitude_data_t	charge_data[CHARGE_DATA_NUM] = { };");
            output.Add("");

            //RC table X Axis value, in mV format
            //2550, 2595, 2632, 2680, 2711, 2771, 2816, 2859, 2916, 2941, 2967, 2990, 3005, 3029, 3052, 3063, 3084, 3105, 3130, 3158, 3193, 3228, 3256, 3285, 3308, 3340, 3360, 3392, 3413, 3446, 3474, 3502, 3525, 3559, 3581, 3613, 3633, 3658, 3675, 3705, 3730, 3758, 3793, 3828, 3856, 3885, 3908, 3940, 3960, 3992, 4013, 4046, 4074, 4102, 4125, 4159, 4181, 4194, 4200};
            output.Add(string.Format("//RC table X Axis value, in mV format"));
            line = (string.Format("int32_t\tXAxisElement[XAxis] = {{"));
            for (i = 0; i < voltList.Count; i++)
            {
                if ((i == voltList.Count - 1))
                {
                    strCTmp = string.Format("{0}", voltList[i]);
                }
                else
                {
                    strCTmp = string.Format("{0}, ", voltList[i]);
                }
                line += (strCTmp);
            }
            line += "};";
            output.Add(line);
            output.Add(string.Empty);
            //RC table Y Axis value, in mA format
            //1000, 3000, 6000, 10000, 15000, 20000, 25000};
            output.Add(string.Format("//RC table Y Axis value, in mA format"));
            line = (string.Format("int32_t\tYAxisElement[YAxis] = {{"));
            for (i = 0; i < listfCurr.Count; i++)
            {
                if ((i == listfCurr.Count - 1))
                {
                    strCTmp = string.Format("{0}", listfCurr[i] * -1);
                }
                else
                {
                    strCTmp = string.Format("{0}, ", listfCurr[i] * -1);
                }
                line += (strCTmp);
            }
            line += "};";
            output.Add(line);
            output.Add(string.Empty);
            // RC table Z Axis value, in 10*'C format
            //-49, 100, 200, 300, 400, 500};
            output.Add(string.Format("//RC table Z Axis value, in 10*'C format"));
            line = (string.Format("int32_t\tZAxisElement[ZAxis] = {{"));
            for (i = 0; i < listfTemp.Count; i++)
            {
                if ((i == listfTemp.Count - 1))
                {
                    strCTmp = string.Format("{0}", (Convert.ToInt16(listfTemp[i] * 10)));
                }
                else
                {
                    strCTmp = string.Format("{0},", (Convert.ToInt16(listfTemp[i] * 10)));
                }
                line += (strCTmp);
            }
            line += "};";
            output.Add(line);
            output.Add(string.Empty);
            // contents of RC table, its unit is 10000C, 1C = DesignCapacity
            //const int	RCtable[YAxis*ZAxis][XAxis]={
            output.Add(string.Format("// contents of RC table, its unit is 10000C, 1C = DesignCapacity"));
            output.Add(string.Format("int32_t\tRCtable[YAxis*ZAxis][XAxis]={{"));
            for (i = 0; i < listfTemp.Count; i++)
            {
                output.Add(string.Format("\n//temp = {0} ^C", listfTemp[i]));
                for (int ic = 0; ic < listfCurr.Count; ic++)
                {
                    line = (string.Format("{{"));
                    for (int iv = 0; iv < voltList.Count; iv++)
                    {
                        if (iv == voltList.Count - 1)
                        {
                            strCTmp = string.Format("{0}", outYValue[i * listfCurr.Count + ic][iv]);
                        }
                        else
                        {
                            strCTmp = string.Format("{0}, ", outYValue[i * listfCurr.Count + ic][iv]);
                        }
                        line += (strCTmp);
                    }
                    line += "},";
                    output.Add(line);
                }
            }
            output.Add(string.Format("}};"));
            return output;
        }

        //Initialize content of H file, currently using hard coding in code, but hopely we can read it from file, a sample file in particular folder
        private static List<string> GetHFileContent(string strStandardH, List<string> strHHeaderComments, List<int> ilstOCVVolt, List<int> voltList, List<float> listfCurr, List<float> listfTemp, double fCTABase, double fCTASlope)
        {
            int iLineCmtHCFile = 4;
            int i = 0;
            List<string> output = new List<string>();
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
            #region add content string for H file
            output.Add(string.Format("#ifndef _TABLE_H_"));
            output.Add(string.Format("#define _TABLE_H_"));
            output.Add(string.Format(""));
            output.Add(string.Format(""));
            output.Add(string.Format("#define OCV_DATA_NUM\t\t{0}", ilstOCVVolt.Count));
            output.Add(string.Format("//#define CHARGE_DATA_NUM \t 51"));   //line 5
            output.Add(string.Format(""));
            output.Add(string.Format(""));
            output.Add($"#define XAxis \t\t {voltList.Count}");
            output.Add($"#define YAxis \t\t {listfCurr.Count}");
            output.Add($"#define ZAxis \t\t {listfTemp.Count}");
            output.Add(string.Format(""));                                                            //(A140805)Francis, for battery id string array
            output.Add(string.Format("#define BATTERY_ID_NUM \t 2")); //(A140805)Francis, for battery id string array //line 13
            output.Add(string.Format(""));
            output.Add(string.Format(""));
            output.Add(string.Format("/****************************************************************************"));
            output.Add(string.Format("* Struct section"));
            output.Add(string.Format("*  add struct #define here if any"));
            output.Add(string.Format("***************************************************************************/"));
            output.Add(string.Format("typedef struct tag_one_latitude_data {{"));
            output.Add(string.Format(" \t int32_t \t\t\t x;//"));         //line 20
            output.Add(string.Format(" \t int32_t \t\t\t y;//"));         //line 21
            output.Add(string.Format("}} one_latitude_data_t;"));
            output.Add(string.Format(""));
            output.Add(string.Format(""));
            output.Add(string.Format("/****************************************************************************"));
            output.Add(string.Format("* extern variable declaration section"));
            output.Add(string.Format("***************************************************************************/"));
            output.Add(string.Format("extern const char *battery_id[] ;"));
            output.Add(string.Format(""));
            output.Add(string.Format("extern const char *table_version;"));
            output.Add("");
            output.Add(string.Format("#endif"));
            output.Add(string.Format(""));
            #endregion
            return output;

        }
    }
    #endregion
}