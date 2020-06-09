using BCLabManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using Microsoft.EntityFrameworkCore;

namespace BCLabManager.Services
{
    public static class ReportLoader   //这个类用来将Excel Report中的数据读出来
    {
        //private static string ExcelFilePath = "";
        //private static Workbook ExcelWKB = null;
        private static _Worksheet BatteryTypeSheet = null;
        private static _Worksheet BatterySheet = null;
        private static _Worksheet TesterSheet = null;
        public static void Init()
        {
            ExcelHelper.Init();
            BatteryTypeSheet = ExcelHelper.GetWorkSheetByName("Battery Models");
            BatterySheet = ExcelHelper.GetWorkSheetByName("Batteries");
            TesterSheet = ExcelHelper.GetWorkSheetByName("Tester");
            ChamberSheet = ExcelHelper.GetWorkSheetByName("Chamber");
        }
        #region battery type

        public static BatteryType LoadBatteryTypeByIndex(int index)
        {
            var row_number = index + 2;
            BatteryType output = new BatteryType();
            output.Manufacturer = ExcelHelper.GetStringFromCell(BatteryTypeSheet, row_number, 2);
            output.Name = ExcelHelper.GetStringFromCell(BatteryTypeSheet, row_number, 3);
            output.Material = ExcelHelper.GetStringFromCell(BatteryTypeSheet, row_number, 4);
            output.LimitedChargeVoltage = Convert.ToInt32(ExcelHelper.GetStringFromCell(BatteryTypeSheet, row_number, 5));
            output.RatedCapacity = Convert.ToInt32(ExcelHelper.GetStringFromCell(BatteryTypeSheet, row_number, 6));
            output.NominalVoltage = Convert.ToInt32(ExcelHelper.GetStringFromCell(BatteryTypeSheet, row_number, 7));
            output.TypicalCapacity = Convert.ToInt32(ExcelHelper.GetStringFromCell(BatteryTypeSheet, row_number, 8));
            output.CutoffDischargeVoltage = Convert.ToInt32(ExcelHelper.GetStringFromCell(BatteryTypeSheet, row_number, 9));
            return output;
        }

        public static int GetBatteryTypeNumber()
        {
            return BatteryTypeSheet.UsedRange.Rows.Count - 1;
        }
        #endregion
        #region Battery

        public static Battery LoadBatteryByIndex(int index, ref int btId)
        {
            var row_number = index + 2;
            Battery output = new Battery();
            output.Name = ExcelHelper.GetStringFromCell(BatterySheet, row_number, 3);
            btId = Convert.ToInt32(ExcelHelper.GetStringFromCell(BatterySheet, row_number, 2));
            return output;
        }

        public static int GetBatteryNumber()
        {
            return BatterySheet.UsedRange.Rows.Count - 1;
        }
        #endregion
        #region Tester

        public static Tester LoadTesterByIndex(int index)
        {
            int row_number = index + 2;
            Tester output = new Tester();
            output.Manufacturer = ExcelHelper.GetStringFromCell(TesterSheet, row_number, 2);
            output.Name = ExcelHelper.GetStringFromCell(TesterSheet, row_number, 3);
            return output;
        }

        public static int GetTesterNumber()
        {
            return TesterSheet.UsedRange.Rows.Count - 1;
        }
        #endregion
        #region Channel

        public static Channel CreateChanel(int id)
        {
            Channel output = new Channel();
            output.Name = "Channel " + id.ToString();
            return output;
        }

        public static int GetChannelNumber()
        {
            return 4;
        }
        #endregion
        #region Chamber
        private static _Worksheet ChamberSheet = null;

        public static Chamber LoadChamberByIndex(int index)
        {
            int row_number = index + 2;
            Chamber output = new Chamber();
            output.Manufacturer = ExcelHelper.GetStringFromCell(ChamberSheet, row_number, 2);
            output.Name = ExcelHelper.GetStringFromCell(ChamberSheet, row_number, 3);
            output.LowestTemperature = Convert.ToInt32(ExcelHelper.GetStringFromCell(ChamberSheet, row_number, 4));
            output.HighestTemperature = Convert.ToInt32(ExcelHelper.GetStringFromCell(ChamberSheet, row_number, 5));
            return output;
        }

        public static int GetChamberNumber()
        {
            return ChamberSheet.UsedRange.Rows.Count - 1;
        }
        #endregion
    }
}
