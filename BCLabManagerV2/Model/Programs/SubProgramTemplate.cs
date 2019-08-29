using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class SubProgramTemplate : ModelBase
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public TestCountEnum TestCount { get; set; }

        public SubProgramTemplate()
        {
        }

        public SubProgramTemplate(String Name, TestCountEnum TestCount) : this()
        {
            this.Name = Name;
            this.TestCount = TestCount;
        }
    }
}
