using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class StepV2ServiceClass
    {
        public ObservableCollection<StepV2> Items { get; set; }
        //public StepTemplateServiceClass StepTemplateService { get; set; } = new StepTemplateServiceClass();
    }
}
