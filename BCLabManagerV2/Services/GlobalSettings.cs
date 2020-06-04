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

        //public static string RootPath { get; set; } = @"Q:\807\Software\WH BC Lab\Data\";
        public static string RootPath { get; set; } = @"\\10.3.4.16\bclab\data\";
        public static string TestDataFolderName { get; set; } = "Test Data";
        public static string HeaderFolderName { get; set; } = @"Meta Data\Header";
        public static string SourceDataFolderName { get; set; } = @"Meta Data\Source";
        public static string ProductFolderName { get; set; } = "Table Maker Product";
        public static string EvResultFolderName { get; set; } = "Emulator Product";
    }
}
