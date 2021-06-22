using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public static class Module
    {
        public static string DataPreprocessor { get; } = "Data Preprocessor";
        public static string Database { get; } = "Database";
        public static string NAS { get; } = "NAS";
    }
}
