using System;
using System.Collections.Generic;

namespace BCLabManager.Model
{
    public class OCVModel
    {
        public List<SourceData> SourceList { get; set; }
        public List<Int32> iOCVVolt { get; set; }
        public int MinVoltage { get; set; }
        public int MaxVoltage { get; set; }
        public string FileName { get; set; }
    }
}