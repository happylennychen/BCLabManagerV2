using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class ProgramClass : BindableBase
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
        private BatteryTypeClass _batteryType;
        public BatteryTypeClass BatteryType
        {
            get { return _batteryType; }
            set { SetProperty(ref _batteryType, value); }
        }
        private string _requester;
        public string Requester
        {
            get { return _requester; }
            set { SetProperty(ref _requester, value); }
        }
        private DateTime _requestTime;
        public DateTime RequestTime
        {
            get => _requestTime;
            set { SetProperty(ref _requestTime, value); }
        }
        private DateTime _startTime;
        public DateTime StartTime
        {
            get => _startTime;
            set { SetProperty(ref _startTime, value); }
        }
        private DateTime _completeTime;
        public DateTime CompleteTime
        {
            get => _completeTime;
            set { SetProperty(ref _completeTime, value); }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        public ObservableCollection<RecipeClass> Recipes { get; set; } = new ObservableCollection<RecipeClass>();

        public ProgramClass()           //Create用到
        {
        }

        public ProgramClass(String Name, BatteryTypeClass BatteryType, String Requester, DateTime RequestTime, String Description, ObservableCollection<RecipeClass> Recipes) //Clone用到
        {
            this.Name = Name;
            this.BatteryType = BatteryType;
            this.Requester = Requester;
            this.RequestTime = RequestTime;
            this.Description = Description;
            this.Recipes = Recipes;
        }

        /*public void Update(String Name, String Requester, DateTime RequestDate, String Description, List<RecipeClass> Recipes)  //没用？
        {
            this.Name = Name;
            this.Requester = Requester;
            this.RequestDate = RequestDate;
            this.Description = Description;
            this.Recipes = Recipes;
        }*/

        //public void Update(ProgramClass model)  //Edit用到
        //{
        //    this.Name = model.Name;
        //    this.Requester = model.Requester;
        //    this.RequestDate = model.RequestDate;
        //    this.Description = model.Description;
        //    this.Recipes = model.Recipes;
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
