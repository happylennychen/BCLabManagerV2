using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.DataAccess;
using Prism.Mvvm;

namespace BCLabManager.Model
{
    public class StepClass : BindableBase
    {
        public int Id { get; set; }

        public StepClass()
        {
        }
    }
}
