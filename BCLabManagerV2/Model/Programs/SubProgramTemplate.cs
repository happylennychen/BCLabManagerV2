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
                if (this.ChargeTemperature == GlobalSettings.RoomTemperatureConstant)
                    ctstr = "Room";
                else
                    ctstr = this.ChargeTemperature.ToString() + " deg";

                if (ChargeCurrentType == CurrentTypeEnum.Absolute)
                    ccstr = this.ChargeCurrent.ToString() + "mA";
                else if (ChargeCurrentType == CurrentTypeEnum.Percentage)
                    ccstr = this.ChargeCurrent.ToString() + "C";
                else if (ChargeCurrentType == CurrentTypeEnum.Dynamic)
                    ccstr = "D" + this.ChargeCurrent.ToString();

                if (this.DischargeTemperature == GlobalSettings.RoomTemperatureConstant)
                    dtstr = "Room";
                else
                    dtstr = this.DischargeTemperature.ToString() + " deg";

                if (DischargeCurrentType == CurrentTypeEnum.Absolute)
                    dcstr = this.DischargeCurrent.ToString() + "mA";
                else if (DischargeCurrentType == CurrentTypeEnum.Percentage)
                    dcstr = this.DischargeCurrent.ToString() + "C";
                else if (DischargeCurrentType == CurrentTypeEnum.Dynamic)
                    dcstr = "D" + this.DischargeCurrent.ToString();

                return $"{ctstr} {ccstr} charge, {dtstr} {dcstr} discharge";
            }
        }//需判断type
        public double ChargeTemperature { get; set; }
        public double ChargeCurrent { get; set; }
        public CurrentTypeEnum ChargeCurrentType { get; set; }
        public double DischargeTemperature { get; set; }
        public double DischargeCurrent { get; set; }
        public CurrentTypeEnum DischargeCurrentType { get; set; }
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
            if (Value == GlobalSettings.RoomTemperatureConstant)
                return "Room";
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
            return "D"+ Value.ToString();
        }
    }

    public enum CurrentTypeEnum
    {
        Percentage,
        Absolute,
        Dynamic
    }
}
