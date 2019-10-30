using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class BatteryTypeClass : ModelBase
    {
        public int Id { get; set; }
        public String Manufactor { get; set; }
        public String Name { get; set; }
        public String Material { get; set; }
        public Int32 LimitedChargeVoltage { get; set; }
        public Int32 RatedCapacity { get; set; }
        public Int32 NominalVoltage { get; set; }
        public Int32 TypicalCapacity { get; set; }
        public Int32 CutoffDischargeVoltage { get; set; }

        public BatteryTypeClass()
        { }

        //public BatteryTypeClass(String Manufactor, String Name, String Material, Int32 LimitedChargeVoltage, Int32 RatedCapacity, Int32 NominalVoltage, Int32 TypicalCapacity, Int32 CutoffDischargeVoltage)
        //{
        //    this.Manufactor = Manufactor;
        //    this.Name = Name;
        //    this.Material = Material;
        //    this.LimitedChargeVoltage = LimitedChargeVoltage;
        //    this.RatedCapacity = RatedCapacity;
        //    this.NominalVoltage = NominalVoltage;
        //    this.TypicalCapacity = TypicalCapacity;
        //    this.CutoffDischargeVoltage = CutoffDischargeVoltage;
        //}
        //public void Update(String Manufactor, String Name, String Material, Int32 LimitedChargeVoltage, Int32 RatedCapacity, Int32 NominalVoltage, Int32 TypicalCapacity, Int32 CutoffDischargeVoltage)
        //{
        //    this.Manufactor = Manufactor;
        //    this.Name = Name;
        //    this.Material = Material;
        //    this.LimitedChargeVoltage = LimitedChargeVoltage;
        //    this.RatedCapacity = RatedCapacity;
        //    this.NominalVoltage = NominalVoltage;
        //    this.TypicalCapacity = TypicalCapacity;
        //    this.CutoffDischargeVoltage = CutoffDischargeVoltage;
        //}

        public override string ToString()
        {
            return this.Name;
        }
    }
}
