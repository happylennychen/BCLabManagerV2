using BCLabManager.Model;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BCLabManager
{
    public static class LiteDriverMaker
    {
        public static void GetLiteModel(List<SourceData> ocvSource, List<SourceData> rcSource, OCVModel ocvModel, RCModel rcModel, Project project, ref LiteModel liteModel)
        {
            //standardModel.iOCVVolt = ocvModel.iOCVVolt;

            //standardModel.fCTABase = rcModel.fCTABase;
            //standardModel.fCTASlope = rcModel.fCTASlope;
            //standardModel.listfCurr = rcModel.listfCurr;
            //standardModel.listfTemp = rcModel.listfTemp;
            //standardModel.outYValue = rcModel.outYValue;
            liteModel.ilistCurr = rcModel.listfCurr.Select(o => (short)o).ToList();
            List<float> flstKeodCont = new List<float>();
            List<float> flstAccAtEoD = new List<float>();
            List<double[]> flstdbDCapCof;
            List<double[]> flstdbKeodCof;
            var ilstTemp = rcModel.listfTemp.Select(o => (short)o).ToList();
            GetDCapCof(ilstTemp, liteModel.ilistCurr, flstKeodCont, flstAccAtEoD, project.AbsoluteMaxCapacity, out flstdbDCapCof, out flstdbKeodCof);
            liteModel.flstdbDCapCof = flstdbDCapCof;
            liteModel.flstdbKeodCof = flstdbKeodCof;
        }

        private static void GetDCapCof(List<short> ilstTemp, List<short> ilstCurr, List<float> flstKeodCont, List<float> flstAccAtEoD, float fullCapacity,  out List<double[]> flstdbDCapCof, out List<double[]> flstdbKeodCof)
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

        public static void GenerateLiteDriver(LiteModel liteModel, Project project, bool isRemoteOutput)
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
            List<string> strFilePaths = liteModel.FilePaths;
            List<string> strHHeaderComments;
            UInt32 uErr = 0;
            TableMakerService.InitializeHeaderInfor(ref uErr, project.BatteryType.Manufacturer, project.BatteryType.Name, project.AbsoluteMaxCapacity.ToString(), project.LimitedChargeVoltage.ToString(), project.CutoffDischargeVoltage.ToString(), out strHHeaderComments);
            GenerateCHFiles(OutFolder, strFilePaths[0], strFilePaths[1], strHHeaderComments, liteModel);
        }
        private static bool GenerateCHFiles(string OutFolder, string strCFileStandardName, string strHFileStandardName, List<string> strHHeaderComments, LiteModel liteModel)
        {
            string standardCFilePath = System.IO.Path.Combine(OutFolder, strCFileStandardName);
            string standardHFilePath = System.IO.Path.Combine(OutFolder, strHFileStandardName);

            List<string> hFileContent = GetHFileContent(strHFileStandardName, strHHeaderComments, liteModel.ilistCurr.Count);
            TableMakerService.CreateFile(standardHFilePath, hFileContent);
            List<string> cFileContent = GetCFileContent(strCFileStandardName, strHFileStandardName, strHHeaderComments, liteModel.ilistCurr, liteModel.flstdbDCapCof, liteModel.flstdbKeodCof, liteModel.flstTblOCVCof);
            TableMakerService.CreateFile(standardCFilePath, cFileContent);
            return true;
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
                else if (i == strHHeaderComments.Count - 2)
                {
                    //(A20210610)Francis, add a unique value
                    string strCTmp;
                    //if (tblSample.u16StageX == 2)
                    strCTmp = "* " + "type_id = " + string.Format("{0:D}", TableMakerService.PRODUCT_TYPE_ID.PRODTYPE_HFILE_LITE);
                    output.Add(strCTmp);
                    //(E20210610)

                    output.Add(shc);
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

        private static List<string> GetCFileContent(string strCFileName, string strStandardH, List<string> strHHeaderComments, List<short> ilstCurr, List<double[]> flstdbDCapCof, List<double[]> flstdbKeodCof, List<float> flstTblOCVCof)
        {
            int iLineCmtHCFile = 4;
            string strCTmp = string.Empty;
            int i = 0;
            List<string> output = new List<string>();
            var ilstCellTempData = TableMakerService.GenerateSampleCellTempData();
            foreach (string shc in strHHeaderComments)
            {
                if (i == iLineCmtHCFile)
                {
                    output.Add(shc + strCFileName);
                }
                else if (i == strHHeaderComments.Count - 2)
                {
                    //(A20210610)Francis, add a unique value
                    //if (tblSample.u16StageX == 2)
                    strCTmp = "* " + "type_id = " + string.Format("{0:D}", TableMakerService.PRODUCT_TYPE_ID.PRODTYPE_HFILE_LITE);
                    output.Add(strCTmp);
                    //(E20210610)

                    output.Add(shc);
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
                    strCTmp += string.Format("\t{0},", flstTblOCVCof[i]);
                else
                    strCTmp += string.Format("\t{0}", flstTblOCVCof[i]);
            }
            output.Add(strCTmp);
            output.Add(string.Format("}};"));
            output.Add(string.Format("/*****************************************************************************"));
            output.Add(string.Format("* Cell temperature table"));
            output.Add(string.Format("* x_dat:	cell mini-voltage"));
            output.Add(string.Format("* y_dat:	temperature in 0.1degC format"));
            output.Add(string.Format("****************************************************************************/"));
            output.Add(string.Format("one_latitude_data_t fgl_cell_temp_data[FGL_TEMPERATURE_NUM] = "));
            output.Add(string.Format("{{"));
            for (i = 0; i < ilstCellTempData.Count; i++)
            {
                var tmp = string.Format("\t{{{0}, \t{1}", ilstCellTempData[i], ilstCellTempData[++i]) + "},";
                if ((i > 2) && (i % 10 == 9))
                {
                    output.Add(strCTmp);
                    strCTmp = string.Empty;
                }
                else
                {
                    strCTmp += tmp;
                }
            }
            output.Add(string.Empty);
            output.Add(string.Format("}};"));
            output.Add(string.Format("\n"));
            output.Add(string.Format("/*****************************************************************************"));
            output.Add(string.Format("* RCAP and KEOD Function Table Y-Axis Data"));
            output.Add(string.Format("****************************************************************************/"));
            strCTmp = string.Format("int\ty_data[YNUM] = {{");
            for (int ic = 0; ic < ilstCurr.Count; ic++)
            {
                short shtmp = ilstCurr[ic];
                if (shtmp < 0) shtmp *= -1;
                if (ic != ilstCurr.Count - 1)
                    strCTmp += (string.Format("{0}, ", shtmp));
                else
                    strCTmp += (string.Format("{0}", shtmp));
            }
            output.Add(string.Format(strCTmp + "}};\n"));
            output.Add(string.Format("/*****************************************************************************"));
            output.Add(string.Format("* RCAP Function Table Coefficient Data"));
            output.Add(string.Format("****************************************************************************/"));
            output.Add(string.Format("float dcap_cof[YNUM][FNUM3D] = {{"));
            for (int id = 0; id < flstdbDCapCof.Count; id++)
            {
                strCTmp = (string.Format("\t{{"));
                double[] dbtmp = flstdbDCapCof[id];
                for (int idd = 0; idd < dbtmp.Length; idd++)
                {
                    if (idd < dbtmp.Length - 1)
                        strCTmp += (string.Format("{0:F10}, ", dbtmp[idd]));
                    else
                        strCTmp += (string.Format("{0:F10}", dbtmp[idd]));
                }
                output.Add(string.Format(strCTmp += "}},"));
            }
            output.Add(string.Format("}};\n"));
            output.Add(string.Format("/*****************************************************************************"));
            output.Add(string.Format("* KEOD Function Table Coefficient Data"));
            output.Add(string.Format("****************************************************************************/"));
            output.Add(string.Format("float keod_cof[YNUM][FNUM3D] = {{"));
            for (int id = 0; id < flstdbKeodCof.Count; id++)
            {
                strCTmp = (string.Format("\t{{"));
                double[] dbtmp = flstdbKeodCof[id];
                for (int idd = 0; idd < dbtmp.Length; idd++)
                {
                    if (idd < dbtmp.Length - 1)
                        strCTmp += (string.Format("{0:F10}, ", dbtmp[idd]));
                    else
                        strCTmp += (string.Format("{0:F10}", dbtmp[idd]));
                }
                output.Add(string.Format("}},"));
            }
            output.Add(string.Format("}};\n"));
            return output;
        }
    }
}