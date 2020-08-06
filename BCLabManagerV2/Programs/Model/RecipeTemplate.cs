using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class RecipeTemplate : BindableBase
    {
        public int Id { get; set; }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        //private double _current;
        //public double Current
        //{
        //    get { return _current; }
        //    set { SetProperty(ref _current, value); }
        //}
        //private double _temperature;
        //public double Temperature
        //{
        //    get { return _temperature; }
        //    set { SetProperty(ref _temperature, value); }
        //}

        public ObservableCollection<Step> Steps { get; set; } = new ObservableCollection<Step>();
        public ObservableCollection<StepV2> StepV2s { get; set; } = new ObservableCollection<StepV2>();
        public ObservableCollection<Protection> Protections { get; set; } = new ObservableCollection<Protection>();

        public RecipeTemplate()
        {
        }
    }
}
