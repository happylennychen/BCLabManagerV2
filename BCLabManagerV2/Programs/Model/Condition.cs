using System;
using Prism.Mvvm;

namespace BCLabManager.Model
{
    public class Condition:BindableBase
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
        public Condition()
        {
            Parameter = Parameter.NA;
            Mark = CompareMarkEnum.NA;
            Value = 0;
        }

        internal Condition Clone()
        {
            Condition output = new Condition();
            output.Parameter = this.Parameter;
            output.Mark = this.Mark;
            output.Value = this.Value;
            return output;
        }
    }
}