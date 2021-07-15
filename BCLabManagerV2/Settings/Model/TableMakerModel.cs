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
        public List<Program> RCPrograms { get; set; }
        public List<Program> OCVPrograms { get; set; }
        public List<Program> Stage1RCPrograms { get; set; }
        public List<Program> Stage1OCVPrograms { get; set; }
        public List<Program> Stage2RCPrograms { get; set; }
        public List<Program> Stage2OCVPrograms { get; set; }
        public OCVModel OCVModel { get; set; }
        public RCModel RCModel { get; set; }
        public MiniModel MiniModel { get; set; }
        public StandardModel StandardModel { get; set; }
        public AndroidModel AndroidModel { get; set; }
        public OCVModel Stage1OCVModel { get; set; }
        public RCModel Stage1RCModel { get; set; }
        public MiniModel Stage1MiniModel { get; set; }
        public StandardModel Stage1StandardModel { get; set; }
        public AndroidModel Stage1AndroidModel { get; set; }
        public OCVModel Stage2OCVModel { get; set; }
        public RCModel Stage2RCModel { get; set; }
        public MiniModel Stage2MiniModel { get; set; }
        public StandardModel Stage2StandardModel { get; set; }
        public AndroidModel Stage2AndroidModel { get; set; }
    }
}
