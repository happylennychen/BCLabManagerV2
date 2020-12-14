using System.Collections.Generic;

namespace BCLabManager.Model
{
    public class RCModel
    {
        public List<float> listfCurr { get; set; } = new List<float>();
        public List<float> listfTemp { get; set; } = new List<float>();
        public List<List<int>> outYValue { get; set; } = new List<List<int>>();
        public double fCTABase { get; set; }
        public double fCTASlope { get; set; }
    }
}