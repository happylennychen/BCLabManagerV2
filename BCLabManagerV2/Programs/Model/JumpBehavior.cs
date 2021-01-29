using System;
using Prism.Mvvm;

namespace BCLabManager.Model
{
    public class JumpBehavior:BindableBase
    {
        public int Id { get; set; }

        private Condition _condition = new Condition();
        public Condition Condition
        {
            get { return _condition; }
            set { SetProperty(ref _condition, value); }
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

        internal JumpBehavior Clone()
        {
            JumpBehavior output = new JumpBehavior();
            output.Condition = this.Condition.Clone();
            output.JumpType = this.JumpType;
            output.Index = this.Index;
            return output;
        }
    }
}