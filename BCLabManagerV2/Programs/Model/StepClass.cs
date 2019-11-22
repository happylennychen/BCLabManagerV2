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
        public StepTemplate StepTemplate { get; set; }

        private string _loopLabel;
        public string LoopLabel
        {
            get { return _loopLabel; }
            set { SetProperty(ref _loopLabel, value); }
        }
        private string _loopTarget;
        public string LoopTarget
        {
            get { return _loopTarget; }
            set { SetProperty(ref _loopTarget, value); }
        }
        private ushort _loopCount;
        public ushort LoopCount
        {
            get { return _loopCount; }
            set { SetProperty(ref _loopCount, value); }
        }
        public StepClass()
        {
        }

        public StepClass(StepTemplate stepTemplate)
        {
            StepTemplate = stepTemplate;
        }
    }
}
