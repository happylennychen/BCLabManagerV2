using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class Project : BindableBase
    {

        public int Id { get; set; }
        //public String Name { get; set; }
        //public ProgramGroupClass Group { get; set; }
        //public BatteryTypeClass BatteryType { get; set; }
        //public String Requester { get; set; }
        //public DateTime RequestTime { get; set; }
        //public String Description { get; set; }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private string _customer;
        public string Customer
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }
        private BatteryType _batteryType;
        public BatteryType BatteryType
        {
            get { return _batteryType; }
            set { SetProperty(ref _batteryType, value); }
        }
        public ObservableCollection<Program> Programs { get; set; } = new ObservableCollection<Program>();
        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        private Int32 _absoluteMaxCapacity;
        public Int32 AbsoluteMaxCapacity
        {
            get { return _absoluteMaxCapacity; }
            set { SetProperty(ref _absoluteMaxCapacity, value); }
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
        private List<int> _voltagePoints=new List<int>();
        //[NotMapped]
        public List<int> VoltagePoints
        {
            get { return _voltagePoints; }
            set { SetProperty(ref _voltagePoints, value); }
        }
        //public ObservableCollection<TableMakerProduct> TableMakerProducts { get; set; } = new ObservableCollection<TableMakerProduct>();
        public ObservableCollection<ProjectSetting> ProjectSettings { get; set; } = new ObservableCollection<ProjectSetting>();
        public ObservableCollection<Program> Programs { get; set; } = new ObservableCollection<Program>();
        public uint DefaultEOD
        {
            get 
            {
                if (ProjectSettings.Count > 0)
                {
                    var ps = ProjectSettings.SingleOrDefault(o => o.is_valid == true);
                    return (uint)ps.discharge_end_voltage_mv;
                }
                else
                    return 0;
            }
        }
        public ObservableCollection<EmulatorResult> EmulatorResults { get; set; } = new ObservableCollection<EmulatorResult>();
        public ObservableCollection<ReleasePackage> ReleasePackages { get; set; } = new ObservableCollection<ReleasePackage>();

        public Project()           //Create用到
        {
        }
        public override string ToString()
        {
            return this.Name;
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
