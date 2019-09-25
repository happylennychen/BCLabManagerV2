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
        public double LimitedChargeVoltage { get; set; }
        public double RatedCapacity { get; set; }
        public double NominalVoltage { get; set; }
        public double TypicalCapacity { get; set; }
        public double CutoffDischargeVoltage { get; set; }

        public BatteryTypeClass()
        { }

        public BatteryTypeClass(String Manufactor, String Name, String Material, Int32 LimitedChargeVoltage, Int32 RatedCapacity, Int32 NominalVoltage, Int32 TypicalCapacity, Int32 CutoffDischargeVoltage)
        {
            this.Manufactor = Manufactor;
            this.Name = Name;
            this.Material = Material;
            this.LimitedChargeVoltage = LimitedChargeVoltage;
            this.RatedCapacity = RatedCapacity;
            this.NominalVoltage = NominalVoltage;
            this.TypicalCapacity = TypicalCapacity;
            this.CutoffDischargeVoltage = CutoffDischargeVoltage;
        }
        public void Update(String Manufactor, String Name, String Material, Int32 LimitedChargeVoltage, Int32 RatedCapacity, Int32 NominalVoltage, Int32 TypicalCapacity, Int32 CutoffDischargeVoltage)
        {
            this.Manufactor = Manufactor;
            this.Name = Name;
            this.Material = Material;
            this.LimitedChargeVoltage = LimitedChargeVoltage;
            this.RatedCapacity = RatedCapacity;
            this.NominalVoltage = NominalVoltage;
            this.TypicalCapacity = TypicalCapacity;
            this.CutoffDischargeVoltage = CutoffDischargeVoltage;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
