using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class SubProgramTemplate : BindBase
    {
        public int Id { get; set; }
        //public String Name { get; set; }
        public ChargeTemperatureClass ChargeTemperature { get; set; }
        public ChargeCurrentClass ChargeCurrent { get; set; }
        public DischargeTemperatureClass DischargeTemperature { get; set; }
        public DischargeCurrentClass DischargeCurrent { get; set; }
        public TestCountEnum TestCount { get; set; }

        public SubProgramTemplate()
        {
        }
    }

    public class ChargeTemperatureClass : BindBase
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class ChargeCurrentClass : BindBase
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class DischargeTemperatureClass : BindBase
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class DischargeCurrentClass : BindBase
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
