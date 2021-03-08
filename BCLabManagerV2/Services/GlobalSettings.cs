using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager
{
    public static class GlobalSettings
    {
        public static string DbPath { get; set; }
        public static string ConfigurationFilePath { get; } = "BCLabConfiguration.cfg";

        //public static string RootPath { get; set; } = @"D:\Issues\Open\BC_Lab\Data V3\";
        //public static string TempraryFolder { get; } = @"D:\Issues\Open\BC_Lab\Data test\";//AppDomain.CurrentDomain.BaseDirectory
        public static string LocalFolder { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Local Data\\");// No need for the sub folder
        public static string RootPath { get; } = @"\\10.3.4.16\bclab\data\";
        public static string TestDataFolderName { get; } = "Test Data";
        public static string TempDataFolderName { get; } = "Temp Data";
        public static string HeaderFolderName { get; } = @"Meta Data\Header";
        public static string SourceDataFolderName { get; } = @"Meta Data\Source";
        public static string ProductFolderName { get; } = "Table Maker Product";
        public static string EvResultFolderName { get; } = "Emulator Product";
    }
}
