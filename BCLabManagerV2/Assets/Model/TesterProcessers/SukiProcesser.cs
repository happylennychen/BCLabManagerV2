using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class SukiProcesser : ITesterProcesser
    {
        public DateTime[] GetTimeFromRawData(ObservableCollection<string> fileList)
        {
            DateTime[] output = new DateTime[2];
            output[0] = DateTime.Now;
            output[1] = DateTime.Now;
            return output;
        }
    }
}
