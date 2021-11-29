using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class StandardRow
    {
        public uint Index { get; set; }
        public uint TimeInMS { get; set; }
        public ActionMode Mode { get; set; }
        public double Current { get; set; } //mA，充电为正，放电为负
        public double Voltage { get; set; } //mV
        public double Temperature { get; set; } //celcius
        public double Capacity { get; set; }    //mAh
        public double TotalCapacity { get; set; }   //mAh
        public RowStatus Status { get; set; }
        public override string ToString()
        {
            return $@"{Index},{TimeInMS},{(byte)Mode},{Current},{Voltage},{Temperature},{Capacity},{TotalCapacity},{(byte)Status}";
        }
    }
}
