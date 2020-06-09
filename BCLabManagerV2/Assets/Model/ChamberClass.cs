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
        //public String Manufacturer { get; set; }
        //public String Name { get; set; }
        //public Double LowestTemperature { get; set; }
        //public Double HighestTemperature { get; set; }
        private string _manufacturer;
        public string Manufacturer
        {
            get { return _manufacturer; }
            set { SetProperty(ref _manufacturer, value); }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private double _lowestTemperature;
        public double LowestTemperature
        {
            get { return _lowestTemperature; }
            set { SetProperty(ref _lowestTemperature, value); }
        }
        private double _highestTemperature;
        public double HighestTemperature
        {
            get { return _highestTemperature; }
            set { SetProperty(ref _highestTemperature, value); }
        }

        public ChamberClass()
        { }

        //public ChamberClass(String Manufacturer, String Name, String TemperatureRange)
        //{
        //    this.Manufacturer = Manufacturer;
        //    this.Name = Name;
        //    this.LowestTemperature = LowestTemperature;
        //    this.HighestTemperature = HighestTemperature;
        //}

        //public void Update(String Manufacturer, String Name, String TemperatureRange)
        //{
        //    this.Manufacturer = Manufacturer;
        //    this.Name = Name;
        //    this.LowestTemperature = LowestTemperature;
        //    this.HighestTemperature = HighestTemperature;
        //}

        public override string ToString()
        {
            return this.Name;
        }
    }
}
