using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class RawDataClass
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        //public byte[] BinaryData { get; set; }
        public string MD5 { get; set; }
    }
}
