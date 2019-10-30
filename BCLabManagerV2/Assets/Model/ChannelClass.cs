using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class ChannelClass : AssetClass
    {
        public int Id { get; set; }
        public TesterClass Tester { get; set; }
        public String Name { get; set; }

        public ChannelClass()
        { }

        //public ChannelClass(String Name, TesterClass Tester)
        //{
        //    this.Tester = Tester;
        //    this.Name = Name;
        //}

        //public void Update(String Name, TesterClass Tester)
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
