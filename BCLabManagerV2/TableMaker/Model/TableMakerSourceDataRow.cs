using System;

namespace BCLabManager.Model
{
    public class TableMakerSourceDataRow
    {
        public UInt32 uSerailNum { get; set; }
        public double fVoltage { get; set; }
        public double fCurrent { get; set; }
        public double fTemperature { get; set; }
        public double fAccMah { get; set; }
        //public DateTime dtRecord { get; set; }
        public double fSoCAdj { get; set; }

        public TableMakerSourceDataRow(UInt32 uN, double fV, double fC, double fT, double fAcc)
        {

            uSerailNum = uN;
            fVoltage = fV;
            fCurrent = fC;
            fTemperature = fT;
            fAccMah = fAcc;
            if (fAccMah < -0.0001)
                fAccMah *= -1.0F;
        }

        public TableMakerSourceDataRow(UInt32 uN, double fV, double fC, double fT, double fAcc, double fUnit = 1)
        {

            uSerailNum = uN;
            fVoltage = fV * fUnit;
            fCurrent = fC * fUnit;
            fTemperature = fT;
            fAccMah = fAcc * fUnit;
            if (fAccMah < -0.0001)
                fAccMah *= -1.0F;
        }
    }
}