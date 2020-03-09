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

        public static BatteryTypeClass LoadBatteryTypeByIndex(int index)
        {
            var row_number = index + 2;
            BatteryTypeClass output = new BatteryTypeClass();
            output.Manufactor = ExcelHelper.GetStringFromCell(BatteryTypeSheet, row_number, 2);
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

        public static BatteryClass LoadBatteryByIndex(int index, ref int btId)
        {
            var row_number = index + 2;
            BatteryClass output = new BatteryClass();
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

        public static TesterClass LoadTesterByIndex(int index)
        {
            int row_number = index + 2;
            TesterClass output = new TesterClass();
            output.Manufactor = ExcelHelper.GetStringFromCell(TesterSheet, row_number, 2);
            output.Name = ExcelHelper.GetStringFromCell(TesterSheet, row_number, 3);
            return output;
        }

        public static int GetTesterNumber()
        {
            return TesterSheet.UsedRange.Rows.Count - 1;
        }
        #endregion
        #region Channel

        public static ChannelClass CreateChanel(int id)
        {
            ChannelClass output = new ChannelClass();
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

        public static ChamberClass LoadChamberByIndex(int index)
        {
            int row_number = index + 2;
            ChamberClass output = new ChamberClass();
            output.Manufactor = ExcelHelper.GetStringFromCell(ChamberSheet, row_number, 2);
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
