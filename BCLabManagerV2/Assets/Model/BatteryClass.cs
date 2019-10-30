using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class BatteryClass : AssetClass
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public BatteryTypeClass BatteryType { get; set; }
        public Double CycleCount { get; set; }

        public BatteryClass()
        { }

        //public BatteryClass(String Name, BatteryTypeClass BatteryType, Double CycleCount = 0)
        //{
        //    //this.BatteryID = NextID;
        //    this.Name = Name;
        //    this.BatteryType = BatteryType;
        //    this.CycleCount = CycleCount;
        //}

        //public void Update(String Name, BatteryTypeClass BatteryType, Double CycleCount = 0)
        //{
        //    this.Name = Name;
        //    this.BatteryType = BatteryType;
        //    this.CycleCount = CycleCount;
        //}

        public override string ToString()
        {
            return this.Name;
        }
    }
}
