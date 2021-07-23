using System;
using System.Collections.Generic;

namespace BCLabManager.Model
{
    public class LiteModel
    {
        public List<short> ilistCurr { get; set; }
        public List<double[]> flstdbDCapCof { get; set; }
        public List<double[]> flstdbKeodCof { get; set; }
        public List<float> flstTblOCVCof { get; set; }
        public List<string> FilePaths { get; set; }
    }
}