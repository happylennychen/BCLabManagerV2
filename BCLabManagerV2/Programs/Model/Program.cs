using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class Program : BindableBase
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
        //private BatteryTypeClass _batteryType;
        //public BatteryTypeClass BatteryType
        //{
        //    get { return _batteryType; }
        //    set { SetProperty(ref _batteryType, value); }
        //}
        private ulong _order;
        public ulong Order
        {
            get { return _order; }
            set { SetProperty(ref _order, value); }
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
        private DateTime _endTime;
        public DateTime EndTime
        {
            get => _endTime;
            set { SetProperty(ref _endTime, value); }
        }
        private DateTime _est;
        public DateTime EST
        {
            get { return _est; }
            set { SetProperty(ref _est, value); }
        }
        private DateTime _eet;
        public DateTime EET
        {
            get { return _eet; }
            set { SetProperty(ref _eet, value); }
        }
        private TimeSpan _ed;
        public TimeSpan ED
        {
            get { return _ed; }
            set { SetProperty(ref _ed, value); }
        }
        private ProgramType _type;
        public ProgramType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        private bool _isInvalid = false;
        public bool IsInvalid
        {
            get { return _isInvalid; }
            set { SetProperty(ref _isInvalid, value); }
        }
        private string _tableFilePath;
        public string TableFilePath
        {
            get { return _tableFilePath; }
            set { SetProperty(ref _tableFilePath, value); }
        }
        //private string _type;
        //public string Type
        //{
        //    get { return _type; }
        //    set { SetProperty(ref _type, value); }
        //}
        private Project _project;
        public Project Project
        {
            get { return _project; }
            set { SetProperty(ref _project, value); }
        }
        private List<int> _temperatures = new List<int>();
        public List<int> Temperatures
        {
            get { return _temperatures; }
            set { SetProperty(ref _temperatures, value); }
        }
        //private List<RecipeTemplate> _recipeTemplates;
        //public List<RecipeTemplate> RecipeTemplates
        //{
        //    get { return _recipeTemplates; }
        //    set { SetProperty(ref _recipeTemplates, value); }
        //}
        private List<string> _recipeTemplates = new List<string>();
        public List<string> RecipeTemplates
        {
            get { return _recipeTemplates; }
            set { SetProperty(ref _recipeTemplates, value); }
        }
        public ObservableCollection<Recipe> Recipes { get; set; } = new ObservableCollection<Recipe>();

        public Program()           //Create用到
        {
        }

        public Program(String Name, Project Project, ProgramType Type, String Requester, DateTime RequestTime, String Description, ObservableCollection<Recipe> Recipes) //Clone用到
        {
            this.Name = Name;
            this.Project = Project;
            this.Type = Type;
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

        public Program Clone() //Edit Save As用到
        {
            List<Recipe> all =
                (from sub in Recipes
                 select sub.Clone()).ToList();
            ObservableCollection<Recipe> clonelist = new ObservableCollection<Recipe>(all);
            return new Program(this.Name, this.Project, this.Type, this.Requester, this.RequestTime, this.Description, clonelist);
        }
        public override string ToString()
        {
            return this.Name;
        }

        internal void BuildRecipes(List<RecipeTemplate> _recipeTemplates, Dictionary<string, int> dic)
        {
            foreach (var temperature in this.Temperatures)
            {
                foreach (var rectempStr in this.RecipeTemplates)
                {
                    var rectemp = _recipeTemplates.SingleOrDefault(o => o.Name == rectempStr);
                    var count = dic[rectemp.Name];
                    for (int i = 0; i < count; i++)
                    {
                        var model = new Recipe(rectemp, this.Project.BatteryType);
                        model.Temperature = temperature;
                        this.Recipes.Add(model);
                    }
                }
            }
            foreach (var sub in this.Recipes)
            {
                foreach (var tr in sub.TestRecords)
                    tr.ProgramStr = this.Name;
            }
        }
    }
}
