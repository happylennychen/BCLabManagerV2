using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class BatteryType : BindableBase
    {
        public int Id { get; set; }
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
        private string _material;
        public string Material
        {
            get { return _material; }
            set { SetProperty(ref _material, value); }
        }
        private Int32 _typicalCapacity;
        public Int32 TypicalCapacity
        {
            get { return _typicalCapacity; }
            set { SetProperty(ref _typicalCapacity, value); }
        }
        //private string _manufacturer;
        //public String Manufacturer { get { return _manufacturer; } set { _manufacturer = value;RaisePropertyChanged(); } }
        //public String Name { get; set; }
        //public String Material { get; set; }
        private Int32 _limitedChargeVoltage;
        public Int32 LimitedChargeVoltage
        {
            get { return _limitedChargeVoltage; }
            set { SetProperty(ref _limitedChargeVoltage, value); }
        }
        private Int32 _ratedCapacity;
        public Int32 RatedCapacity
        {
            get { return _ratedCapacity; }
            set { SetProperty(ref _ratedCapacity, value); }
        }
        private Int32 _nominalVoltage;
        public Int32 NominalVoltage
        {
            get { return _nominalVoltage; }
            set { SetProperty(ref _nominalVoltage, value); }
        }
        //public Int32 TypicalCapacity { get; set; }
        private Int32 _cutoffDischargeVoltage;
        public Int32 CutoffDischargeVoltage
        {
            get { return _cutoffDischargeVoltage; }
            set { SetProperty(ref _cutoffDischargeVoltage, value); }
        }
        private Int32 _fullyChargedEndCurrent;
        public Int32 FullyChargedEndCurrent
        {
            get { return _fullyChargedEndCurrent; }
            set { SetProperty(ref _fullyChargedEndCurrent, value); }
        }
        private Int32 _chargeCurrent;
        public Int32 ChargeCurrent
        {
            get { return _chargeCurrent; }
            set { SetProperty(ref _chargeCurrent, value); }
        }
        private Int32 _chargeLowTemp;
        public Int32 ChargeLowTemp
        {
            get { return _chargeLowTemp; }
            set { SetProperty(ref _chargeLowTemp, value); }
        }
        private Int32 _chargeHighTemp;
        public Int32 ChargeHighTemp
        {
            get { return _chargeHighTemp; }
            set { SetProperty(ref _chargeHighTemp, value); }
        }
        private Int32 _dischargeLowTemp;
        public Int32 DischargeLowTemp
        {
            get { return _dischargeLowTemp; }
            set { SetProperty(ref _dischargeLowTemp, value); }
        }
        private Int32 _dischargeHighTemp;
        public Int32 DischargeHighTemp
        {
            get { return _dischargeHighTemp; }
            set { SetProperty(ref _dischargeHighTemp, value); }
        }
        private Int32 _fullyChargedEndingTimeout;
        public Int32 FullyChargedEndingTimeout
        {
            get { return _fullyChargedEndingTimeout; }
            set { SetProperty(ref _fullyChargedEndingTimeout, value); }
        }
        public BatteryType()
        { }

        //public BatteryTypeClass(String Manufacturer, String Name, String Material, Int32 LimitedChargeVoltage, Int32 RatedCapacity, Int32 NominalVoltage, Int32 TypicalCapacity, Int32 CutoffDischargeVoltage)
        //{
        //    this.Manufacturer = Manufacturer;
        //    this.Name = Name;
        //    this.Material = Material;
        //    this.LimitedChargeVoltage = LimitedChargeVoltage;
        //    this.RatedCapacity = RatedCapacity;
        //    this.NominalVoltage = NominalVoltage;
        //    this.TypicalCapacity = TypicalCapacity;
        //    this.CutoffDischargeVoltage = CutoffDischargeVoltage;
        //}
        //public void Update(String Manufacturer, String Name, String Material, Int32 LimitedChargeVoltage, Int32 RatedCapacity, Int32 NominalVoltage, Int32 TypicalCapacity, Int32 CutoffDischargeVoltage)
        //{
        //    this.Manufacturer = Manufacturer;
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
