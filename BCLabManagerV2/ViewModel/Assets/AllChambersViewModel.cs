using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

namespace BCLabManager.ViewModel
{
    public class AllChambersViewModel : ViewModelBase
    {
        #region Fields

        //readonly ChamberRepository _chamberRepository;
        ChamberViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _deleteCommand;
        ObservableCollection<ChamberClass> _chambers;

        #endregion // Fields

        #region Constructor

        public AllChambersViewModel(ObservableCollection<ChamberClass> chambers)
        {
            _chambers = chambers;
            this.CreateAllChambers(chambers);
        }

        void CreateAllChambers(ObservableCollection<ChamberClass> chambers)
        {
            List<ChamberViewModel> all =
                (from cmb in chambers
                 select new ChamberViewModel(cmb)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllChambers = new ObservableCollection<ChamberViewModel>(all);     //再转换成Observable
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
                using (var dbContext = new AppDbContext())
                {
                    List<AssetUsageRecordViewModel> all =
                      (from cmb in dbContext.Chambers
                       where cmb.Id == SelectedItem.Id
                       from record in cmb.Records
                       select new AssetUsageRecordViewModel(record)).ToList();
                    return all;
                }
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
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(
                        param => { this.Delete(); },
                        param => this.CanDelete
                        );
                }
                return _deleteCommand;
            }
        }

        #endregion // Public Interface

        #region Private Helper
        private void Create()
        {
            ChamberClass m = new ChamberClass();      //实例化一个新的model
            ChamberEditViewModel evm = new ChamberEditViewModel(m);      //实例化一个新的view model
            evm.DisplayName = "Chamber-Create";
            evm.commandType = CommandType.Create;
            var ChamberViewInstance = new ChamberView();      //实例化一个新的view
            ChamberViewInstance.DataContext = evm;
            ChamberViewInstance.ShowDialog();                   //设置viewmodel属性
            if (evm.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Chambers.Add(m);
                    dbContext.SaveChanges();
                }
                _chambers.Add(m);
                this.AllChambers.Add(new ChamberViewModel(m));
            }
        }
        private void Edit()
        {
            ChamberClass m = new ChamberClass();      //实例化一个新的model
            ChamberEditViewModel evm = new ChamberEditViewModel(m);      //实例化一个新的view model
            evm.Name = _selectedItem.Name;
            evm.Manufactor = _selectedItem.Manufactor;
            evm.LowTemp = _selectedItem.LowTemp;
            evm.HighTemp = _selectedItem.HighTemp;
            evm.DisplayName = "Chamber-Edit";
            evm.commandType = CommandType.Edit;
            var ChamberViewInstance = new ChamberView();      //实例化一个新的view
            ChamberViewInstance.DataContext = evm;
            ChamberViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                _selectedItem.Name = evm.Name;
                _selectedItem.Manufactor = evm.Manufactor;
                _selectedItem.LowTemp = evm.LowTemp;
                _selectedItem.HighTemp = evm.HighTemp;

                using (var dbContext = new AppDbContext())
                {
                    var cmb = dbContext.Chambers.SingleOrDefault(b => b.Id == _selectedItem.Id);
                    cmb.Name = evm.Name;
                    cmb.Manufactor = evm.Manufactor;
                    cmb.LowestTemperature = evm.LowTemp;
                    cmb.HighestTemperature = evm.HighTemp;
                    dbContext.SaveChanges(); 
                }
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            ChamberClass m = new ChamberClass();      //实例化一个新的model
            ChamberEditViewModel evm = new ChamberEditViewModel(m);      //实例化一个新的view model
            evm.Name = _selectedItem.Name;
            evm.Manufactor = _selectedItem.Manufactor;
            evm.LowTemp = _selectedItem.LowTemp;
            evm.HighTemp = _selectedItem.HighTemp;
            evm.DisplayName = "Chamber-Save As";
            evm.commandType = CommandType.SaveAs;
            var ChamberViewInstance = new ChamberView();      //实例化一个新的view
            ChamberViewInstance.DataContext = evm;
            ChamberViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Chambers.Add(m);
                    dbContext.SaveChanges();
                }
                _chambers.Add(m);
                this.AllChambers.Add(new ChamberViewModel(m));
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedItem != null; }
        }
        private void Delete()
        {
            using (var dbContext = new AppDbContext())
            {
                var model = dbContext.Chambers.SingleOrDefault(o => o.Id == _selectedItem.Id);
                if (model.AssetUseCount > 0)
                {
                    MessageBox.Show("Cannot delete using chamber.");
                    return;
                }
                if (MessageBox.Show("Are you sure?", "Delete Chamber", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    dbContext.Chambers.Remove(model);
                    dbContext.SaveChanges();

                    model = _chambers.SingleOrDefault(o => o.Id == _selectedItem.Id);
                    _chambers.Remove(model);
                    this.AllChambers.Remove(_selectedItem);
                }
            }
        }
        private bool CanDelete
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

            //_chamberRepository.ItemAdded -= this.OnChamberAddedToRepository;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        //void OnChamberAddedToRepository(object sender, ItemAddedEventArgs<ChamberClass> e)
        //{
        //    var viewModel = new ChamberViewModel(e.NewItem, _chamberRepository);
        //    this.AllChambers.Add(viewModel);
        //}

        #endregion // Event Handling Methods
    }
}
