using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BCLabManager.ViewModel
{
    public class AllChambersViewModel : ViewModelBase
    {
        #region Fields

        readonly ChamberRepository _chamberRepository;
        ChamberViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllChambersViewModel(ChamberRepository chamberRepository)
        {
            if (chamberRepository == null)
                throw new ArgumentNullException("chamberRepository");

            _chamberRepository = chamberRepository;

            // Subscribe for notifications of when a new customer is saved.
            _chamberRepository.ItemAdded += this.OnChamberAddedToRepository;

            // Populate the AllChamberTypes collection with _chambertypeRepository.
            this.CreateAllChambers();
        }

        void CreateAllChambers()
        {
            List<ChamberViewModel> all =
                (from bat in _chamberRepository.GetItems()
                 select new ChamberViewModel(bat,_chamberRepository)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //foreach (ChamberModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnChamberModelViewModelPropertyChanged;

            this.AllChambers = new ObservableCollection<ChamberViewModel>(all);     //再转换成Observable
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the ChamberModelViewModel objects.
        /// </summary>
        public ObservableCollection<ChamberViewModel> AllChambers { get; private set; }

        public ChamberViewModel SelectedItem    //绑定选中项，从而改变chambers
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    //OnPropertyChanged("SelectedType");
                    OnPropertyChanged("Records"); //通知Records改变
                }
            }
        }

        public List<AssetUsageRecordViewModel> Records //绑定选中chamber的Records。只显示，所以只有get没有set。每次改选type，都要重新做一次查询    //不需要ObservableCollection，因为每次变化都已经被通知了
        //如果不是用查询，那么需要维护一个二维List。每一个Chamber，对应一个List。用空间换时间。
        {
            get
            {
                if (SelectedItem == null)
                    return null;
                List<AssetUsageRecordViewModel> all =
                  (from bat in _chamberRepository.GetItems()
                   where bat.Name == SelectedItem.Name
                   from record in bat.Records
                   select new AssetUsageRecordViewModel(record)).ToList();
                return all;
            }
        }

        public ICommand CreateCommand
        {
            get
            {
                if (_createCommand == null)
                {
                    _createCommand = new RelayCommand(
                        param => { this.Create();}
                        );
                }
                return _createCommand;
            }
        }
        public ICommand EditCommand
        {
            get
            {
                if (_editCommand == null)
                {
                    _editCommand = new RelayCommand(
                        param => { this.Edit(); },
                        param => this.CanEdit
                        );
                }
                return _editCommand;
            }
        }
        public ICommand SaveAsCommand
        {
            get
            {
                if (_saveAsCommand == null)
                {
                    _saveAsCommand = new RelayCommand(
                        param => { this.SaveAs(); },
                        param => this.CanSaveAs
                        );
                }
                return _saveAsCommand;
            }
        }

        #endregion // Public Interface

        #region Private Helper
        private void Create()
        {
            ChamberClass bc = new ChamberClass();      //实例化一个新的model
            ChamberViewModel bvm = new ChamberViewModel(bc, _chamberRepository);      //实例化一个新的view model
            bvm.DisplayName = "Chamber-Create";
            bvm.commandType = CommandType.Create;
            var ChamberViewInstance = new ChamberView();      //实例化一个新的view
            ChamberViewInstance.DataContext = bvm;
            ChamberViewInstance.ShowDialog();                   //设置viewmodel属性
            if (bvm.IsOK == true)
            {
                _chamberRepository.AddItem(bc);
            }
        }
        private void Edit()
        {
            ChamberClass bc = new ChamberClass();      //实例化一个新的model
            ChamberViewModel bvm = new ChamberViewModel(bc, _chamberRepository);      //实例化一个新的view model
            bvm.Name = _selectedItem.Name;
            bvm.Manufactor = _selectedItem.Manufactor;
            bvm.LowTemp = _selectedItem.LowTemp;
            bvm.HighTemp = _selectedItem.HighTemp;
            bvm.DisplayName = "Chamber-Edit";
            bvm.commandType = CommandType.Edit;
            var ChamberViewInstance = new ChamberView();      //实例化一个新的view
            ChamberViewInstance.DataContext = bvm;
            ChamberViewInstance.ShowDialog();
            if (bvm.IsOK == true)
            {
                _selectedItem.Name = bvm.Name;
                _selectedItem.Manufactor = bvm.Manufactor;
                _selectedItem.LowTemp = bvm.LowTemp;
                _selectedItem.HighTemp = bvm.HighTemp;
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            ChamberClass bc = new ChamberClass();      //实例化一个新的model
            ChamberViewModel bvm = new ChamberViewModel(bc, _chamberRepository);      //实例化一个新的view model
            bvm.Name = _selectedItem.Name;
            bvm.Manufactor = _selectedItem.Manufactor;
            bvm.LowTemp = _selectedItem.LowTemp;
            bvm.HighTemp = _selectedItem.HighTemp;
            bvm.DisplayName = "Chamber-Save As";
            bvm.commandType = CommandType.SaveAs;
            var ChamberViewInstance = new ChamberView();      //实例化一个新的view
            ChamberViewInstance.DataContext = bvm;
            ChamberViewInstance.ShowDialog();
            if (bvm.IsOK == true)
            {
                _chamberRepository.AddItem(bc);
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedItem != null; }
        }
        #endregion //Private Helper
        #region  Base Class Overrides

        protected override void OnDispose()
        {
            foreach (ChamberViewModel custVM in this.AllChambers)
                custVM.Dispose();

            this.AllChambers.Clear();
            //this.AllChamberModels.CollectionChanged -= this.OnCollectionChanged;

            _chamberRepository.ItemAdded -= this.OnChamberAddedToRepository;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        void OnChamberAddedToRepository(object sender, ItemAddedEventArgs<ChamberClass> e)
        {
            var viewModel = new ChamberViewModel(e.NewItem, _chamberRepository);
            this.AllChambers.Add(viewModel);
        }

        #endregion // Event Handling Methods
    }
}
