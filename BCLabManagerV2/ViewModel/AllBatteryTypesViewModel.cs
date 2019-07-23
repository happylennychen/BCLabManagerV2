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
    public class AllBatteryTypesViewModel : ViewModelBase
    {
        #region Fields

        readonly BatteryTypeRepository _batterytypeRepository;
        readonly BatteryRepository _batteryRepository;
        BatteryTypeViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllBatteryTypesViewModel(BatteryTypeRepository batterytypeRepository, BatteryRepository batteryRepository)
        {
            if (batterytypeRepository == null)
                throw new ArgumentNullException("batterytypeRepository");

            if (batteryRepository == null)
                throw new ArgumentNullException("batteryRepository");

            _batterytypeRepository = batterytypeRepository;

            _batteryRepository = batteryRepository;

            // Subscribe for notifications of when a new customer is saved.
            _batterytypeRepository.ItemAdded += this.OnBatteryModelAddedToRepository;

            // Populate the AllBatteryTypes collection with _batterytypeRepository.
            this.CreateAllBatteryTypes();
        }

        void CreateAllBatteryTypes()
        {
            List<BatteryTypeViewModel> all =
                (from batT in _batterytypeRepository.GetItems()
                 select new BatteryTypeViewModel(batT, _batterytypeRepository)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //foreach (BatteryModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnBatteryModelViewModelPropertyChanged;

            this.AllBatteryTypes = new ObservableCollection<BatteryTypeViewModel>(all);     //再转换成Observable
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the BatteryModelViewModel objects.
        /// </summary>
        public ObservableCollection<BatteryTypeViewModel> AllBatteryTypes { get; private set; }

        public BatteryTypeViewModel SelectedItem    //绑定选中项，从而改变batteries
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
                    OnPropertyChanged("Batteries"); //通知Batteries改变
                }
            }
        }
        public List<BatteryViewModel> Batteries //绑定选中type的batteries。只显示，所以只有get没有set。每次改选type，都要重新做一次查询    //不需要ObservableCollection，因为每次变化都已经被通知了
        //如果不是用查询，那么需要维护一个二维List。每一个BatteryType，对应一个List。用空间换时间。
        {
            get
            {
                if (SelectedItem == null)
                    return null;
                List<BatteryViewModel> all =
                  (from bat in _batteryRepository.GetItems()
                   where bat.BatteryType.Name == SelectedItem.Name
                   select new BatteryViewModel(bat, _batteryRepository, _batterytypeRepository)).ToList();
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
            BatteryTypeClass btc = new BatteryTypeClass();      //实例化一个新的model
            BatteryTypeViewModel btvm = new BatteryTypeViewModel(btc, _batterytypeRepository);      //实例化一个新的view model
            btvm.DisplayName = "Battery Type-Create";
            var BatteryTypeViewInstance = new BatteryTypeView();      //实例化一个新的view
            BatteryTypeViewInstance.DataContext = btvm;
            BatteryTypeViewInstance.ShowDialog();                   //设置viewmodel属性
            if (btvm.IsOK == true)
            {
                _batterytypeRepository.AddItem(btc);
            }
        }
        private void Edit()
        {
            BatteryTypeClass btc = new BatteryTypeClass();      //实例化一个新的model
            BatteryTypeViewModel btvm = new BatteryTypeViewModel(btc, _batterytypeRepository);      //实例化一个新的view model
            btvm.Manufactor = _selectedItem.Manufactor;
            btvm.Material = _selectedItem.Material;
            btvm.Name = _selectedItem.Name;
            btvm.DisplayName = "Battery Type-Edit";
            var BatteryTypeViewInstance = new BatteryTypeView();      //实例化一个新的view
            BatteryTypeViewInstance.DataContext = btvm;
            BatteryTypeViewInstance.ShowDialog();
            if (btvm.IsOK == true)
            {
                _selectedItem.Manufactor = btvm.Manufactor;
                _selectedItem.Material = btvm.Material;
                _selectedItem.Name = btvm.Name;
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            BatteryTypeClass btc = new BatteryTypeClass();      //实例化一个新的model
            BatteryTypeViewModel btvm = new BatteryTypeViewModel(btc, _batterytypeRepository);      //实例化一个新的view model
            btvm.Manufactor = _selectedItem.Manufactor;
            btvm.Material = _selectedItem.Material;
            btvm.Name = _selectedItem.Name;
            btvm.DisplayName = "Battery Type-Save As";
            var BatteryTypeViewInstance = new BatteryTypeView();      //实例化一个新的view
            BatteryTypeViewInstance.DataContext = btvm;
            BatteryTypeViewInstance.ShowDialog();
            if (btvm.IsOK == true)
            {
                _batterytypeRepository.AddItem(btc);
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
            foreach (BatteryTypeViewModel custVM in this.AllBatteryTypes)
                custVM.Dispose();

            this.AllBatteryTypes.Clear();
            //this.AllBatteryModels.CollectionChanged -= this.OnCollectionChanged;

            _batterytypeRepository.ItemAdded -= this.OnBatteryModelAddedToRepository;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        void OnBatteryModelAddedToRepository(object sender, ItemAddedEventArgs<BatteryTypeClass> e)
        {
            var viewModel = new BatteryTypeViewModel(e.NewItem, _batterytypeRepository);
            this.AllBatteryTypes.Add(viewModel);
        }

        #endregion // Event Handling Methods
    }
}
