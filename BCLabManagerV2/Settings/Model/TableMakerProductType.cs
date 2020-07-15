using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class TableMakerProductType : BindableBase
    {

        public int Id { get; set; }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        public override string ToString()
        {
            return this.Description;
        }

        public TableMakerProductType()           //Create用到
        {
        }
    }
}
