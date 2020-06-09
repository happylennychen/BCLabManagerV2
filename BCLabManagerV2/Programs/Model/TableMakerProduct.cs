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
        private TableMakerProductType _type;
        public TableMakerProductType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }
        private bool _isvalid;
        public bool is_valid
        {
            get { return _isvalid; }
            set { SetProperty(ref _isvalid, value); }
        }
    }
}