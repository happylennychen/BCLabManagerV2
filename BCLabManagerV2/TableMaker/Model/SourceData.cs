using System;
using System.Collections.Generic;
using System.IO;

namespace BCLabManager.Model
{
    public class SourceData
    {
        public float fTemperature { get; set; }
        public float fCurrent { get; set; }
        public float fMeasureGain { get; set; }
        public float fMeasureOffset { get; set; }
        public float fTraceResis { get; set; }
        public float fCapacityDiff { get; set; }
        public float fAbsMaxCap { get; set; }
        public float fLimitChgVolt { get; set; }
        public float fCutoffDsgVolt { get; set; }
        public float fAccmAhrCap { get; set; } = -99999;
        public float fMinExpVolt { get; set; } = 99999;
        public float fMaxExpVolt { get; set; } = -99999;

        public List<DataRow> ReservedExpData { get; set; } = new List<DataRow>();
        public List<DataRow> AdjustedExpData { get; set; } = new List<DataRow>();

        public SourceData(float current, float temperature)
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