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
        public static string RootPath { get; set; } = @"D:\Issues\Open\BC_Lab\Data\";
        public static string RawDataFolderName { get; set; } = "Raw Data";
        public static string TestDataFolderName { get; set; } = "Test Data";
        public static string HeaderFolderName { get; set; } = "Header";
        public static string SourceDataFolderName { get; set; } = "Source Data";
        public static string ProductFolderName { get; set; } = "Product";
        public static string EvResultFolderName { get; set; } = "Ev Result";
    }
}
