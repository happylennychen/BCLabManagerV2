using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class ChamberClass : AssetClass
    {
        public int Id { get; set; }
        public String Manufactor { get; set; }
        public String Name { get; set; }
        public Double LowestTemperature { get; set; }
        public Double HighestTemperature { get; set; }

        public ChamberClass()
        { }

        public ChamberClass(String Manufactor, String Name, String TemperatureRange)
        {
            this.Manufactor = Manufactor;
            this.Name = Name;
            this.LowestTemperature = LowestTemperature;
            this.HighestTemperature = HighestTemperature;
        }

        public void Update(String Manufactor, String Name, String TemperatureRange)
        {
            this.Manufactor = Manufactor;
            this.Name = Name;
            this.LowestTemperature = LowestTemperature;
            this.HighestTemperature = HighestTemperature;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
