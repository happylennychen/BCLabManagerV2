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
    public enum CompareMarkEnum
    {
        NA,
        LargerThan,
        SmallThan
    }
    public class Step : BindableBase
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
        private CompareMarkEnum _compareMark;
        public CompareMarkEnum CompareMark
        {
            get { return _compareMark; }
            set { SetProperty(ref _compareMark, value); }
        }
        private double _capacity;
        public double CRate
        {
            get { return _capacity; }
            set { SetProperty(ref _capacity, value); }
        }
        public int Order { get; set; }
        public Step()
        {
        }

        public Step(StepTemplate stepTemplate)
        {
            StepTemplate = stepTemplate;
        }
    }
}
