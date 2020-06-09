using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class StepServiceClass
    {
        public ObservableCollection<Step> Items { get; set; }
        //public StepTemplateServiceClass StepTemplateService { get; set; } = new StepTemplateServiceClass();
    }
}
