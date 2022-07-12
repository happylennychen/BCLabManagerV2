using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Prism.Mvvm;

namespace BCLabManager.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CutOffBehavior:BindableBase
    {
        [JsonProperty]
        public int Id { get; set; }
        private Condition _condition = new Condition();
        [JsonProperty]
        public Condition Condition
        {
            get { return _condition; }
            set { SetProperty(ref _condition, value); }
        }
        #region Loop Behavior
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
        #endregion
        private ObservableCollection<JumpBehavior> _jumpBehaviors = new ObservableCollection<JumpBehavior>();
        [JsonProperty]
        public ObservableCollection<JumpBehavior> JumpBehaviors
        {
            get { return _jumpBehaviors; }
            set { SetProperty(ref _jumpBehaviors, value); }
        }
        internal CutOffBehavior Clone()
        {
            CutOffBehavior output = new CutOffBehavior();
            output.Condition = this.Condition.Clone();
            output.Loop1Count = this.Loop1Count;
            output.Loop1Target = this.Loop1Target;
            output.Loop2Count = this.Loop2Count;
            output.Loop2Target = this.Loop2Target;
            foreach (var jpb in JumpBehaviors)
            {
                JumpBehavior newjpb = jpb.Clone();
                output.JumpBehaviors.Add(newjpb);
            }
            return output;
        }
    }
}