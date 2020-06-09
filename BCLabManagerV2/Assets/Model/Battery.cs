using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class Battery : Asset
    {
        public int Id { get; set; }
        //public String Name { get; set; }
        //public BatteryTypeClass BatteryType { get; set; }
        //public Double CycleCount { get; set; }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private BatteryType _batteryType;
        public BatteryType BatteryType
        {
            get { return _batteryType; }
            set { SetProperty(ref _batteryType, value); }
        }
        private double _cycleCount;
        public double CycleCount
        {
            get { return _cycleCount; }
            set { SetProperty(ref _cycleCount, value); }
        }

        public Battery()
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
