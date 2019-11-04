using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class ModelService<T> : IModelService<T> where T:class
    {
        public ObservableCollection<T> Items { get; set; }
        public void Add(T item)
        { 
        }
    }
}
