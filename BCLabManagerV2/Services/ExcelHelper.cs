using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Services
{
    public static class ExcelHelper
    {
        private static string ExcelFilePath = @"Q:\807\Software\WH BC Lab\Report\BC Lab Report V4 20200226.xlsm";
        private static Workbook ExcelWKB = null;
        public static void Init()
        {
            var excelApp = new Application();
            //_Worksheet excelSHEET = null;
            try
            {
                ExcelWKB = excelApp.Workbooks.Open(ExcelFilePath);
            }
            catch (Exception c)
            {
                System.Windows.MessageBox.Show(c.ToString());
            }
        }
        public static _Worksheet GetWorkSheetByName(string sheetName)
        {
            foreach (_Worksheet st in ExcelWKB.Sheets)
            {
                if (st.Name == sheetName)
                {
                    return st;
                }
            }
            return null;
        }
        public static string GetStringFromCell(_Worksheet sheet, int row_number, int column_number)
        {
            return ((Range)sheet.Cells[row_number, column_number]).Text.ToString();
        }
    }
}
