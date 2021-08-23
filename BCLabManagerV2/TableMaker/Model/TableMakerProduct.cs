using Prism.Mvvm;
namespace BCLabManager.Model
{
    public class TableMakerProduct : BindableBase
    {
        public int Id { get; set; }
        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { SetProperty(ref _filePath, value); }
        }
        private string _MD5;
        public string MD5
        {
            get { return _MD5; }
            set { SetProperty(ref _MD5, value); }
        }
        private TableMakerProductType _type;
        public TableMakerProductType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }
        private Project _project;
        public Project Project
        {
            get { return _project; }
            set { SetProperty(ref _project, value); }
        }
        private bool _isvalid;
        public bool IsValid
        {
            get { return _isvalid; }
            set { SetProperty(ref _isvalid, value); }
        }
    }
}