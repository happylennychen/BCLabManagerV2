using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class ProgramClass : ModelBase
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Requester { get; set; }
        public DateTime RequestDate { get; set; }
        public String Description { get; set; }
        public DateTime CompleteDate { get; set; }
        public ObservableCollection<SubProgramClass> SubPrograms { get; set; }

        public ProgramClass()           //Create用到
        {
            SubPrograms = new ObservableCollection<SubProgramClass>();
        }

        public ProgramClass(String Name, String Requester, DateTime RequestDate, String Description, ObservableCollection<SubProgramClass> SubPrograms) //Clone用到
        {
            this.Name = Name;
            this.Requester = Requester;
            this.RequestDate = RequestDate;
            this.Description = Description;
            this.SubPrograms = SubPrograms;
        }

        /*public void Update(String Name, String Requester, DateTime RequestDate, String Description, List<SubProgramClass> SubPrograms)  //没用？
        {
            this.Name = Name;
            this.Requester = Requester;
            this.RequestDate = RequestDate;
            this.Description = Description;
            this.SubPrograms = SubPrograms;
        }*/

        public void Update(ProgramClass model)  //Edit用到
        {
            this.Name = model.Name;
            this.Requester = model.Requester;
            this.RequestDate = model.RequestDate;
            this.Description = model.Description;
            this.SubPrograms = model.SubPrograms;
        }

        public ProgramClass Clone() //Edit Save As用到
        {
            List<SubProgramClass> all =
                (from sub in SubPrograms
                 select sub.Clone()).ToList();
            ObservableCollection<SubProgramClass> clonelist = new ObservableCollection<SubProgramClass>(all);
            return new ProgramClass(this.Name, this.Requester, this.RequestDate, this.Description, clonelist);
        }
    }
}
