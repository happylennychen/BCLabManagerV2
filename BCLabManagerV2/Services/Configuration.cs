using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager
{
    public class Configuration
    {
        public string RemotePath { get; set; }
        public bool EnableTest { get; set; }
        public string MappingPath { get; set; }
        public string LocalPath { get; set; }
        public string DatabaseHost { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }
    }
}
