using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager
{
    public static class GlobalSettings
    {
        public static string DbPath { get; set; }
        public static string ConfigurationFilePath { get; set; } = "BCLabConfiguration.cfg";

        public const int RoomTemperatureConstant = -9999;
        public const double ChargeTimeCoff = 1.2;
        public const double DischargeTimeCoff = 1.2;
    }
}
