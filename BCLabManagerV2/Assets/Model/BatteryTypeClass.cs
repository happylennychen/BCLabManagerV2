using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class BatteryTypeClass : BindableBase
    {
        public int Id { get; set; }
        private string _manufactor;
        public string Manufactor
        {
            get { return _manufactor; }
            set { SetProperty(ref _manufactor, value); }
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
        //private string _manufactor;
        //public String Manufactor { get { return _manufactor; } set { _manufactor = value;RaisePropertyChanged(); } }
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
        private Int32 _fullyChargedEndingTimeout;
        public Int32 FullyChargedEndingTimeout
        {
            get { return _fullyChargedEndingTimeout; }
            set { SetProperty(ref _fullyChargedEndingTimeout, value); }
        }
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
