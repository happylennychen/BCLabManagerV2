using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class TesterClass : BindableBase
    {
        public int Id { get; set; }
        //public String Manufactor { get; set; }
        //public String Name { get; set; }
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

        public TesterClass()
        { }

        //public TesterClass(String Manufactor, String Name)
        //{
        //    //this.TesterID = NextID;
        //    this.Manufactor = Manufactor;
        //    this.Name = Name;
        //}

        //public void Update(String Manufactor, String Name)
        //{
        //    this.Manufactor = Manufactor;
        //    this.Name = Name;
        //}

        public override string ToString()
        {
            return this.Name;
        }
    }
}
