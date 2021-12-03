using System.Collections.Generic;

namespace BCLabManager.Model
{
    public class StandardModel
    {
        public List<int> iOCVVolt { get; set; }

        public List<string> FileNames { get; internal set; }
        public double fCTABase { get; internal set; }
        public double fCTASlope { get; internal set; }
        public List<double> listfCurr { get; internal set; }
        public List<double> listfTemp { get; internal set; }
        public List<List<int>> outYValue { get; internal set; }
    }
}