using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    public class Stage1Source:BindableBase
    {
        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { SetProperty(ref _filePath, value); }
        }
        private bool _isCheck;

        public Stage1Source(string FilePath, bool IsCheck)
        {
            this.FilePath = FilePath;
            this.IsCheck = IsCheck;
        }

        public bool IsCheck
        {
            get { return _isCheck; }
            set { SetProperty(ref _isCheck, value); }
        }
    }
}