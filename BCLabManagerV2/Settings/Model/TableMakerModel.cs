using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class TableMakerModel : BindableBase
    {
        public Project Project { get; set; }
        public List<Tester> Testers { get; set; }
        public List<Program> Programs { get; set; }
        public OCVModel OCVModel { get; set; }
        public RCModel RCModel { get; set; }
        public MiniModel MiniModel { get; set; }
        public StandardModel StandardModel { get; set; }
        public AndroidModel AndroidModel { get; set; }
        public LiteModel LiteModel { get; set; }
    }
}
