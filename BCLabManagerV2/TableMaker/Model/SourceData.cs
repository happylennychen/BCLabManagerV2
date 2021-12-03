using System;
using System.Collections.Generic;
using System.IO;

namespace BCLabManager.Model
{
    public class SourceData
    {
        public double fTemperature { get; set; }
        public double fCurrent { get; set; }
        public double fMeasureGain { get; set; }
        public double fMeasureOffset { get; set; }
        public double fTraceResis { get; set; }
        public double fCapacityDiff { get; set; }
        public double fAbsMaxCap { get; set; }
        public double fLimitChgVolt { get; set; }
        public double fCutoffDsgVolt { get; set; }
        public double fAccmAhrCap { get; set; } = -99999;
        public double fMinExpVolt { get; set; } = 99999;
        public double fMaxExpVolt { get; set; } = -99999;

        public List<TableMakerSourceDataRow> ReservedExpData { get; set; } = new List<TableMakerSourceDataRow>();
        public List<TableMakerSourceDataRow> AdjustedExpData { get; set; } = new List<TableMakerSourceDataRow>();

        public SourceData(double current, double temperature)
        {
            this.fAbsMaxCap = 3080;
            this.fAccmAhrCap = -9999;  //被更新
            this.fCapacityDiff = 0;
            this.fCurrent = current;
            this.fCutoffDsgVolt = 2500;
            this.fLimitChgVolt = 4200;
            this.fMeasureGain = 1;
            this.fMeasureOffset = 0;
            this.fTemperature = temperature;
            this.fTraceResis = 0;
        }

        public SourceData()
        {
        }

        public SourceData ShallowCopy()
        {
            return (SourceData)this.MemberwiseClone();
        }
    }
}