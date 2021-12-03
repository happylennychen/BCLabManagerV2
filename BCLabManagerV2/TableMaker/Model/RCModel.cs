using System.Collections.Generic;

namespace BCLabManager.Model
{
    public class RCModel
    {
        public List<double> listfCurr { get; set; } = new List<double>();
        public List<double> listfTemp { get; set; } = new List<double>();
        public List<List<int>> outYValue { get; set; } = new List<List<int>>();
        public double fCTABase { get; set; }
        public double fCTASlope { get; set; }
        public List<SourceData> SourceList { get; set; }
        public int MinVoltage { get; set; }
        public int MaxVoltage { get; set; }
        public string FileName { get; set; }
    }
}