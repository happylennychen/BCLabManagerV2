using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class Channel : Asset
    {
        public int Id { get; set; }
        //public Tester Tester { get; set; }
        //public String Name { get; set; }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private Tester _tester;
        public Tester Tester
        {
            get { return _tester; }
            set { SetProperty(ref _tester, value); }
        }

        public Channel()
        { }

        //public Channel(String Name, Tester Tester)
        //{
        //    this.Tester = Tester;
        //    this.Name = Name;
        //}

        //public void Update(String Name, Tester Tester)
        //{
        //    this.Tester = Tester;
        //    this.Name = Name;
        //}

        public override string ToString()
        {
            return this.Name;
        }
    }
}
