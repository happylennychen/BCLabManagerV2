using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class ProjectClass : BindableBase
    {

        public int Id { get; set; }
        //public String Name { get; set; }
        //public ProgramGroupClass Group { get; set; }
        //public BatteryTypeClass BatteryType { get; set; }
        //public String Requester { get; set; }
        //public DateTime RequestTime { get; set; }
        //public String Description { get; set; }
        private string _customer;
        public string Customer
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }
        private BatteryTypeClass _batteryType;
        public BatteryTypeClass BatteryType
        {
            get { return _batteryType; }
            set { SetProperty(ref _batteryType, value); }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        private Int32 _ratedCapacity;
        public Int32 RatedCapacity
        {
            get { return _ratedCapacity; }
            set { SetProperty(ref _ratedCapacity, value); }
        }
        private Int32 _limitedChargeVoltage;
        public Int32 LimitedChargeVoltage
        {
            get { return _limitedChargeVoltage; }
            set { SetProperty(ref _limitedChargeVoltage, value); }
        }
        private Int32 _cutoffDischargeVoltage;
        public Int32 CutoffDischargeVoltage
        {
            get { return _cutoffDischargeVoltage; }
            set { SetProperty(ref _cutoffDischargeVoltage, value); }
        }
        private string _voltagePoints;
        public string VoltagePoints
        {
            get { return _voltagePoints; }
            set { SetProperty(ref _voltagePoints, value); }
        }
        public ObservableCollection<ProjectProductClass> ProjectProducts { get; set; } = new ObservableCollection<ProjectProductClass>();

        public ObservableCollection<ProgramClass> Programs { get; set; } = new ObservableCollection<ProgramClass>();
        public ObservableCollection<EvSettingClass> EvSettings { get; set; } = new ObservableCollection<EvSettingClass>();

        public ProjectClass()           //Create用到
        {
        }

        //public ProjectClass(String Name, BatteryTypeClass BatteryType, String Requester, DateTime RequestTime, String Description, ObservableCollection<RecipeClass> Recipes) //Clone用到
        //{
        //    this.Name = Name;
        //    this.BatteryType = BatteryType;
        //    this.Requester = Requester;
        //    this.RequestTime = RequestTime;
        //    this.Description = Description;
        //    this.Recipes = Recipes;
        //}

        //public ProgramClass Clone() //Edit Save As用到
        //{
        //    List<RecipeClass> all =
        //        (from sub in Recipes
        //         select sub.Clone()).ToList();
        //    ObservableCollection<RecipeClass> clonelist = new ObservableCollection<RecipeClass>(all);
        //    return new ProgramClass(this.Name, this.BatteryType, this.Requester, this.RequestTime, this.Description, clonelist);
        //}
    }
}
