﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager
{
    public static class GlobalSettings
    {
        public static string ConfigurationFilePath { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration.json");
        public static string RunningLogFilePath { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Running Log\\", $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.log");

        //public static string RootPath { get; set; } = @"D:\Issues\Open\BC_Lab\Data V3\";
        //public static string TempraryFolder { get; } = @"D:\Issues\Open\BC_Lab\Data test\";//AppDomain.CurrentDomain.BaseDirectory
        //public static string LocalFolder { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Local Data\\");// No need for the sub folder
        private static string _localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Local Data\\");
        public static string LocalPath
        {
            get { return _localPath; }
            set
            {
                if (value.EndsWith(@"\"))
                    _localPath = value;
                else
                    _localPath = $@"{value}\";
            }
        }
        private static string _remotePath = @"\\10.3.4.16\bclab\data\";
        public static string RemotePath 
        { 
            get { return _remotePath; } 
            set 
            {
                if (value.EndsWith(@"\"))
                    _remotePath = value;
                else
                    _remotePath = $@"{value}\";
            }
        }
        public static bool EnableTest { get; set; } = false;
        private static string _mappingPath = @"D:\BCLM Mapping Folder\";
        public static string MappingPath
        {
            get { return _mappingPath; }
            set
            {
                if (value.EndsWith(@"\"))
                    _mappingPath = value;
                else
                    _mappingPath = $@"{value}\";
            }
        }
        public static string UniversalPath 
        { 
            get 
            {
                if (!EnableTest)
                    return RemotePath;
                else
                    return MappingPath;
            } 
        }
        public static string TestDataFolderName { get; } = "Test Data";
        public static string TempDataFolderName { get; } = "Temp Data";
        public static string HeaderFolderName { get; } = @"Meta Data\Header";
        public static string SourceDataFolderName { get; } = @"Meta Data\Source";
        public static string ProductFolderName { get; } = "Table Maker Product";
        public static string EvResultFolderName { get; } = "Emulator Product";
        public static string RunningLogFolder { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Running Log\\");
        public static string TestPlanFolder { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test Plan\\");
        public static string DatabaseHost { get; set; } = "10.22.4.38";
        //public static string DatabaseHost { get; set; } = "localhost";
        public static string DatabaseName { get; set; } = "BCLM";
        //public static string DatabaseName { get; set; } = "TMRecord";
        public static string DatabaseUser { get; set; } = "postgres";
        public static string DatabasePassword { get; set; } = "123456";
    }
}
