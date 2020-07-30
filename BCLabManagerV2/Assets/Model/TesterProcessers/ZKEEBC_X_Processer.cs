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
    public class ZKEEBC_X_Processer : ITesterProcesser
    {
        public bool CheckChannelNumber(string filepath, string channelnumber)
        {
            throw new NotImplementedException();
        }

        public DateTime[] GetTimeFromRawData(ObservableCollection<string> fileList)
        {
            return null;
        }
    }
}
