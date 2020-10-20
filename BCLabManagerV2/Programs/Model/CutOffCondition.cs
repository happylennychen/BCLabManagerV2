using System;
using Prism.Mvvm;

namespace BCLabManager.Model
{
    public class CutOffCondition:BindableBase
    {
        public int Id { get; set; }
        private Parameter _parameter;
        public Parameter Parameter
        {
            get { return _parameter; }
            set { SetProperty(ref _parameter, value); }
        }
        private CompareMarkEnum _mark;
        public CompareMarkEnum Mark
        {
            get { return _mark; }
            set { SetProperty(ref _mark, value); }
        }
        private int _value;
        public int Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }
        private JumpType _jumpType;
        public JumpType JumpType
        {
            get { return _jumpType; }
            set { SetProperty(ref _jumpType, value); }
        }
        private int _index;
        public int Index
        {
            get { return _index; }
            set { SetProperty(ref _index, value); }
        }
        private string _loop1Target;
        public string Loop1Target
        {
            get { return _loop1Target; }
            set { SetProperty(ref _loop1Target, value); }
        }
        private ushort _loop1Count;
        public ushort Loop1Count
        {
            get { return _loop1Count; }
            set { SetProperty(ref _loop1Count, value); }
        }
        private string _loop2Target;
        public string Loop2Target
        {
            get { return _loop2Target; }
            set { SetProperty(ref _loop2Target, value); }
        }
        private ushort _loop2Count;
        public ushort Loop2Count
        {
            get { return _loop2Count; }
            set { SetProperty(ref _loop2Count, value); }
        }

        internal CutOffCondition Clone()
        {
            CutOffCondition output = new CutOffCondition();
            output.Index = this.Index;
            output.JumpType = this.JumpType;
            output.Loop1Count = this.Loop1Count;
            output.Loop1Target = this.Loop1Target;
            output.Loop2Count = this.Loop2Count;
            output.Loop2Target = this.Loop2Target;
            output.Mark = this.Mark;
            output.Parameter = this.Parameter;
            output.Value = this.Value;
            return output;
        }
    }
}