using System;
using System.Collections.Generic;

namespace BCLabManager.Model
{
    public class MiniModel
    {
        public List<Int32> iOCVVolt { get; set; } = new List<int>();
        public double fCTABase { get; set; }
        public double fCTASlope { get; set; }
        public List<double> poly2EstFACC { get; set; } = new List<double>();
        public List<double> poly2EstIR { get; set; } = new List<double>();
        public List<string> FilePaths { get; set; }
    }
}