using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class RawDataClass:BindableBase
    {
        public int Id { get; set; }
        //public string FileName { get; set; }
        ////public byte[] BinaryData { get; set; }
        //public string MD5 { get; set; }
        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { SetProperty(ref _fileName, value); }
        }
        private string _md5;
        public string MD5
        {
            get { return _md5; }
            set { SetProperty(ref _md5, value); }
        }
    }
}
