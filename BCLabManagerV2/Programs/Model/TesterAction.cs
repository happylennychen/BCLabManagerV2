using Prism.Mvvm;

namespace BCLabManager.Model
{
    public class TesterAction: BindableBase
    {
        public int Id { get; set; }
        private ActionMode _mode;
        public ActionMode Mode
        {
            get { return _mode; }
            set { SetProperty(ref _mode, value); }
        }
        private int _voltage;
        public int Voltage
        {
            get { return _voltage; }
            set { SetProperty(ref _voltage, value); }
        }
        private int _current;
        public int Current
        {
            get { return _current; }
            set { SetProperty(ref _current, value); }
        }
        private int _power;
        public int Power
        {
            get { return _power; }
            set { SetProperty(ref _power, value); }
        }
    }
}