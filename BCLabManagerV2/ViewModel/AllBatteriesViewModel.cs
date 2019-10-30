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
    public class AllBatteriesViewModel : ViewModelBase
    {
        #region Fields

        //readonly BatteryTypeRepository _batterytypeRepository;
        readonly BatteryRepository _batteryRepository;
        readonly BatteryTypeRepository _batteryTypeRepository;
        BatteryViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllBatteriesViewModel()
        {

            _batteryRepository = Repositories._batteryRepository;
            _batteryTypeRepository = Repositories._batterytypeRepository;

            // Subscribe for notifications of when a new customer is saved.
            _batteryRepository.ItemAdded += this.OnBatteryAddedToRepository;

            // Populate the AllBatteryTypes collection with _batterytypeRepository.
            this.CreateAllBatteries();
        }

        void CreateAllBatteries()
        {
            List<BatteryViewModel> all =
                (from bat in _batteryRepository.GetItems()
                 select new BatteryViewModel(bat,_batteryRepository, _batteryTypeRepository)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //foreach (BatteryModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnBatteryModelViewModelPropertyChanged;

            this.AllBatteries = new ObservableCollection<BatteryViewModel>(all);     //再转换成Observable
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the BatteryModelViewModel objects.
        /// </summary>
        public ObservableCollection<BatteryViewModel> AllBatteries { get; private set; }

        public BatteryViewModel SelectedItem    //绑定选中项，从而改变batteries
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

        public List<AssetUsageRecordViewModel> Records //绑定选中battery的Records。只显示，所以只有get没有set。每次改选type，都要重新做一次查询    //不需要ObservableCollection，因为每次变化都已经被通知了
        //如果不是用查询，那么需要维护一个二维List。每一个Battery，对应一个List。用空间换时间。
        {
            get
            {
                if (SelectedItem == null)
                    return null;
                List<AssetUsageRecordViewModel> all =
                  (from bat in _batteryRepository.GetItems()
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
            BatteryClass bc = new BatteryClass();      //实例化一个新的model
            BatteryViewModel bvm = new BatteryViewModel(bc, _batteryRepository, _batteryTypeRepository);      //实例化一个新的view model
            bvm.DisplayName = "Battery-Create";
            bvm.commandType = CommandType.Create;
            var BatteryViewInstance = new BatteryView();      //实例化一个新的view
            BatteryViewInstance.DataContext = bvm;
            BatteryViewInstance.ShowDialog();                   //设置viewmodel属性
            if (bvm.IsOK == true)
            {
                _batteryRepository.AddItem(bc);
            }
        }
        private void Edit()
        {
            BatteryClass bc = new BatteryClass();      //实例化一个新的model
            BatteryViewModel bvm = new BatteryViewModel(bc, _batteryRepository, _batteryTypeRepository);      //实例化一个新的view model
            bvm.Name = _selectedItem.Name;
            bvm.BatteryType = _selectedItem.BatteryType;
            bvm.CycleCount = _selectedItem.CycleCount;
            bvm.Status = _selectedItem.Status;
            bvm.DisplayName = "Battery-Edit";
            bvm.commandType = CommandType.Edit;
            var BatteryViewInstance = new BatteryView();      //实例化一个新的view
            BatteryViewInstance.DataContext = bvm;
            BatteryViewInstance.ShowDialog();
            if (bvm.IsOK == true)
            {
                _selectedItem.Name = bvm.Name;
                _selectedItem.BatteryType = bvm.BatteryType;
                _selectedItem.CycleCount = bvm.CycleCount;
                _selectedItem.Status = bvm.Status;
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            BatteryClass bc = new BatteryClass();      //实例化一个新的model
            BatteryViewModel bvm = new BatteryViewModel(bc, _batteryRepository, _batteryTypeRepository);      //实例化一个新的view model
            bvm.Name = _selectedItem.Name;
            bvm.BatteryType = _selectedItem.BatteryType;
            bvm.CycleCount = _selectedItem.CycleCount;
            bvm.Status = _selectedItem.Status;
            bvm.DisplayName = "Battery-Edit";
            bvm.commandType = CommandType.SaveAs;
            var BatteryViewInstance = new BatteryView();      //实例化一个新的view
            BatteryViewInstance.DataContext = bvm;
            BatteryViewInstance.ShowDialog();
            if (bvm.IsOK == true)
            {
                _batteryRepository.AddItem(bc);
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
            foreach (BatteryViewModel custVM in this.AllBatteries)
                custVM.Dispose();

            this.AllBatteries.Clear();
            //this.AllBatteryModels.CollectionChanged -= this.OnCollectionChanged;

            _batteryRepository.ItemAdded -= this.OnBatteryAddedToRepository;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        void OnBatteryAddedToRepository(object sender, ItemAddedEventArgs<BatteryClass> e)
        {
            var viewModel = new BatteryViewModel(e.NewItem, _batteryRepository, _batteryTypeRepository);
            this.AllBatteries.Add(viewModel);
        }

        #endregion // Event Handling Methods
    }
}
