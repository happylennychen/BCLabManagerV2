using System;
using System.Collections.Generic;
using BCLabManager.Model;

namespace BCLabManager
{
    public static class StandardDriverMaker
    {
        public static void GetStandardModel(OCVModel ocvModel, RCModel rcModel, ref StandardModel standardModel)
        {
            standardModel.iOCVVolt = ocvModel.iOCVVolt;

            standardModel.fCTABase = rcModel.fCTABase;
            standardModel.fCTASlope = rcModel.fCTASlope;
            standardModel.listfCurr = rcModel.listfCurr;
            standardModel.listfTemp = rcModel.listfTemp;
            standardModel.outYValue = rcModel.outYValue;
        }

        internal static void GenerateStandardDriver(StandardModel standardModel, Project project, bool isRemoteOutput)
        {
            var rootPath = string.Empty;
            if (isRemoteOutput)
            {
                rootPath = GlobalSettings.RemotePath;
            }
            else
            {
                rootPath = GlobalSettings.LocalFolder;
            }
            var OutFolder = $@"{rootPath}{project.BatteryType.Name}\{project.Name}\{GlobalSettings.ProductFolderName}";
            List<string> strFilePaths = standardModel.FilePaths;
            List<string> strHHeaderComments;
            UInt32 uErr = 0;
            TableMakerService.InitializeHeaderInfor(ref uErr, project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), project.LimitedChargeVoltage.ToString(), project.CutoffDischargeVoltage.ToString(), out strHHeaderComments);
            GenerateCHFiles(ref uErr, OutFolder, strFilePaths[0], strFilePaths[1], strHHeaderComments, standardModel.iOCVVolt, project.VoltagePoints, standardModel.listfTemp, standardModel.listfCurr, standardModel.outYValue, standardModel.fCTABase, standardModel.fCTASlope);
        }
        private static bool GenerateCHFiles(ref UInt32 uErr, string OutFolder, string strCFileStandardName, string strHFileStandardName, List<string> strHHeaderComments, List<int> ilstOCVVolt, List<int> voltList, List<float> listfTemp, List<float> listfCurr, List<List<int>> outYValue, double fCTABase, double fCTASlope)
        {
            bool bReturn;
            string standardCFilePath = System.IO.Path.Combine(OutFolder, strCFileStandardName);
            string standardHFilePath = System.IO.Path.Combine(OutFolder, strHFileStandardName);

            List<string> hFileContent = GetHFileContent(strHFileStandardName, strHHeaderComments, ilstOCVVolt, voltList, listfCurr, listfTemp, fCTABase, fCTASlope);
            TableMakerService.CreateFile(standardHFilePath, hFileContent);

            List<string> cFileContent = GetCFileContent(strCFileStandardName, strHFileStandardName, strHHeaderComments, ilstOCVVolt, voltList, listfTemp, listfCurr, outYValue, fCTABase, fCTASlope);
            TableMakerService.CreateFile(standardCFilePath, cFileContent);

            bReturn = true;
            uErr = 1;

            return bReturn;
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
            output.Add(string.Format("#include \"{0}\"", strHFileStandardName));
            output.Add(string.Format("\n"));
            //write ocv_data
            output.Add(string.Format("/*****************************************************************************"));
            output.Add(string.Format("* OCV table"));
            output.Add(string.Format("* x_dat:\tcell mini-voltage"));
            output.Add(string.Format("* y_dat:\tresidual capacity in percentage format"));
            output.Add(string.Format("****************************************************************************/"));
            output.Add(string.Format("one_latitude_data_t ocv_data[OCV_DATA_NUM] = "));
            output.Add(string.Format("{{"));


            var ilstOCVSoC = TableMakerService.GetOCVSocPoints();
            for (i = 0; i < ilstOCVVolt.Count; i++)
            {
                strCTmp = string.Format("\t{{{0}, \t{1}", ilstOCVVolt[i], Math.Round(ilstOCVSoC[i], 0)) + "},";
                if ((i > 2) && (i % 10 == 9))
                {
                    line += (strCTmp);
                    output.Add(line);
                    line = "";
                }
                else
                {
                    line += (strCTmp);
                }
            }
            output.Add(line);
            line = "";
            output.Add(string.Format("}};"));
            //write sample cell_temp_data
            output.Add(string.Format("/*****************************************************************************"));
            output.Add(string.Format("* Cell temperature table"));
            output.Add(string.Format("* x_dat:	cell mini-voltage"));
            output.Add(string.Format("* y_dat:	temperature in 0.1degC format"));
            output.Add(string.Format("****************************************************************************/"));
            output.Add(string.Format("one_latitude_data_t cell_temp_data[TEMPERATURE_DATA_NUM] = "));
            output.Add(string.Format("{{"));

            var ilstCellTempData = TableMakerService.GenerateSampleCellTempData();
            for (i = 0; i < ilstCellTempData.Count; i++)
            {
                strCTmp = string.Format("\t{{{0}, \t{1}", ilstCellTempData[i], ilstCellTempData[++i]) + "},";
                if ((i > 2) && (i % 10 == 9))
                {
                    line += (strCTmp);
                    output.Add(line);
                    line = "";
                }
                else
                {
                    line += (strCTmp);
                }
            }
            output.Add(line);
            output.Add(string.Format("}};\n"));

            //RC table X Axis value, in mV format
            //2550, 2595, 2632, 2680, 2711, 2771, 2816, 2859, 2916, 2941, 2967, 2990, 3005, 3029, 3052, 3063, 3084, 3105, 3130, 3158, 3193, 3228, 3256, 3285, 3308, 3340, 3360, 3392, 3413, 3446, 3474, 3502, 3525, 3559, 3581, 3613, 3633, 3658, 3675, 3705, 3730, 3758, 3793, 3828, 3856, 3885, 3908, 3940, 3960, 3992, 4013, 4046, 4074, 4102, 4125, 4159, 4181, 4194, 4200};
            output.Add(string.Format("// RC table X Axis value, in mV format"));
            line = (string.Format("int\tXAxisElement[XAxis] = {{"));
            for (i = 0; i < voltList.Count; i++)
            {
                if ((i == voltList.Count - 1))
                {
                    strCTmp = string.Format("{0}", voltList[i]);
                }
                else
                {
                    strCTmp = string.Format("{0},", voltList[i]);
                }
                line += (strCTmp);
            }
            line += "};";
            output.Add(line);
            output.Add(string.Empty);
            //RC table Y Axis value, in mA format
            //1000, 3000, 6000, 10000, 15000, 20000, 25000};
            output.Add(string.Format("// RC table Y Axis value, in mA format"));
            line = (string.Format("int\tYAxisElement[YAxis] = {{"));
            for (i = 0; i < listfCurr.Count; i++)
            {
                if ((i == listfCurr.Count - 1))
                {
                    strCTmp = string.Format("{0}", listfCurr[i] * -1);
                }
                else
                {
                    strCTmp = string.Format("{0},", listfCurr[i] * -1);
                }
                line += (strCTmp);
            }
            line += "};";
            output.Add(line);
            output.Add(string.Empty);
            // RC table Z Axis value, in 10*'C format
            //-49, 100, 200, 300, 400, 500};
            output.Add(string.Format("// RC table Z Axis value, in 10*'C format"));
            line = (string.Format("int\tZAxisElement[ZAxis] = {{"));
            for (i = 0; i < listfTemp.Count; i++)
            {
                if ((i == listfTemp.Count - 1))
                {
                    strCTmp = string.Format("{0}", (Convert.ToInt16(listfTemp[i])) * 10);
                }
                else
                {
                    strCTmp = string.Format("{0},", (Convert.ToInt16(listfTemp[i])) * 10);
                }
                line += (strCTmp);
            }
            line += "};";
            output.Add(line);
            output.Add(string.Empty);
            // contents of RC table, its unit is 10000C, 1C = DesignCapacity
            //const int	RCtable[YAxis*ZAxis][XAxis]={
            output.Add(string.Format("// contents of RC table, its unit is 10000C, 1C = DesignCapacity"));
            output.Add(string.Format("int\tRCtable[YAxis*ZAxis][XAxis]={{"));
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
                            strCTmp = string.Format("{0},", outYValue[i * listfCurr.Count + ic][iv]);
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

            #region write H file content to

            var ilstCellTempData = TableMakerService.GenerateSampleCellTempData();
            output.Add(string.Format("#ifndef _TABLE_STANDARD_H_"));
            output.Add(string.Format("#define _TABLE_STANDARD_H_"));
            output.Add(string.Format("\n"));
            output.Add(string.Format("#define OCV_DATA_NUM\t\t{0}", ilstOCVVolt.Count));
            output.Add(string.Format("#define TEMPERATURE_DATA_NUM\t\t{0}", ilstCellTempData.Count / 2));
            output.Add(string.Format(""));
            output.Add($"#define XAxis \t\t {voltList.Count}");
            output.Add($"#define YAxis \t\t {listfCurr.Count}");
            output.Add($"#define ZAxis \t\t {listfTemp.Count}");
            output.Add(string.Format(""));
            output.Add(string.Format("#define CTABASE\t\t{0}", fCTABase));
            output.Add(string.Format("#define CTASLOPE\t\t{0}", fCTASlope));
            output.Add(string.Format(""));
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
            output.Add(string.Format("extern one_latitude_data_t ocv_data[];"));
            output.Add(string.Format("extern one_latitude_data_t cell_temp_data[];"));
            output.Add(string.Format("extern int XAxisElement[];"));
            output.Add(string.Format("extern int YAxisElement[];"));
            output.Add(string.Format("extern int ZAxisElement[];"));
            output.Add(string.Format("extern int RCtable[YAxis*ZAxis][XAxis];"));
            output.Add(string.Format("\n"));
            output.Add(string.Format("#endif	//_TABLE_STANDARD_H_"));
            #endregion
            return output;
        }
    }
}