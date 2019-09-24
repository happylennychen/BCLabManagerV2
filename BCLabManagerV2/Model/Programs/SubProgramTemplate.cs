using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class SubProgramTemplate : ModelBase
    {
        public int Id { get; set; }
        public String Name
        {
            get
            {
                string ctstr = "";
                string ccstr = "";
                string dtstr = "";
                string dcstr = "";
                if (this.ChargeTemperature == -9999)
                    ctstr = "Room";
                else
                    ctstr = this.ChargeTemperature.ToString() + " deg";

                if (ChargeCurrentType == CurrentType.Absolute)
                    ccstr = this.ChargeCurrent.ToString() + "mA";
                else if (ChargeCurrentType == CurrentType.Percentage)
                    ccstr = this.ChargeCurrent.ToString() + "C";
                else if (ChargeCurrentType == CurrentType.Dynamic)
                    ccstr = "D" + this.ChargeCurrent.ToString("D4");

                if (this.DischargeTemperature == -9999)
                    dtstr = "Room";
                else
                    dtstr = this.DischargeTemperature.ToString() + " deg";

                if (DischargeCurrentType == CurrentType.Absolute)
                    dcstr = this.DischargeCurrent.ToString() + "mA";
                else if (DischargeCurrentType == CurrentType.Percentage)
                    dcstr = this.DischargeCurrent.ToString() + "C";
                else if (DischargeCurrentType == CurrentType.Dynamic)
                    dcstr = "D" + this.DischargeCurrent.ToString("D4");

                return $"{ctstr} {ccstr} charge, {dtstr} {dcstr} discharge";
            }
        }//需判断type
        public double ChargeTemperature { get; set; }
        public double ChargeCurrent { get; set; }
        public CurrentType ChargeCurrentType { get; set; }
        public double DischargeTemperature { get; set; }
        public double DischargeCurrent { get; set; }
        public CurrentType DischargeCurrentType { get; set; }
        public TestCountEnum TestCount { get; set; }
        public SubProgramTemplate()
        {
        }
    }

    public class TemperatureClass : ModelBase
    {
        public int Id { get; set; }
        public double Value { get; set; }

        public override string ToString()
        {
            return this.Value.ToString() + " deg";
        }
    }

    public class PercentageCurrentClass : ModelBase
    {
        public int Id { get; set; }
        public double Value { get; set; }

        public override string ToString()
        {
            return this.Value.ToString() + "C";
        }
    }

    public class AbsoluteCurrentClass : ModelBase
    {
        public int Id { get; set; }
        public double Value { get; set; }

        public override string ToString()
        {
            return this.Value.ToString() + "mA";
        }
    }

    public class DynamicCurrentClass : ModelBase
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public override string ToString()
        {
            return "D"+ Value.ToString("D4");
        }
    }

    public enum CurrentType
    {
        Percentage,
        Absolute,
        Dynamic
    }
}
