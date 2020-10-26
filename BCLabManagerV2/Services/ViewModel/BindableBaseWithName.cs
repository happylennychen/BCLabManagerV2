using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    public class BindableBaseWithName:BindableBase
    {
        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set { SetProperty(ref _displayName, value); }
        }
    }
}