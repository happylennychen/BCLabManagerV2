using Prism.Mvvm;

namespace BCLabManager.Model
{
    public class Protection: BindableBase
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
    }
}