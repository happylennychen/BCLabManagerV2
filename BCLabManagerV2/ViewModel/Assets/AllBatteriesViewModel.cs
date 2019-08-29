using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace BCLabManager.ViewModel
{
    public class AllBatteriesViewModel : ViewModelBase
    {
        #region Fields

        //readonly BatteryTypeRepository _batterytypeRepository;
        BatteryViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllBatteriesViewModel()
        {

            // Subscribe for notifications of when a new customer is saved.
            //_batteryRepository.ItemAdded += this.OnBatteryAddedToRepository;

            // Populate the AllBatteryTypes collection with _batterytypeRepository.
            this.CreateAllBatteries();
        }

        /*void CreateAllBatteries()
        {
            List<BatteryViewModel> all =
                (from bat in _batteryRepository.GetItems()
                 select new BatteryViewModel(bat,_batteryRepository, _batteryTypeRepository)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //foreach (BatteryModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnBatteryModelViewModelPropertyChanged;

            this.AllBatteries = new ObservableCollection<BatteryViewModel>(all);     //再转换成Observable
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }*/
        void CreateAllBatteries()
        {
            var dbContext = new AppDbContext();

            List<BatteryViewModel> all =
                (from bat in dbContext.Batteries.Include(i=>i.BatteryType)
                 select new BatteryViewModel(bat)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)
            //var all = dbContext.Batteries
            //    .Include(i=>i.BatteryType)

            this.AllBatteries = new ObservableCollection<BatteryViewModel>(all);     //再转换成Observable
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
                var dbContext = new AppDbContext();
                if (SelectedItem == null)
                    return null;
                List<AssetUsageRecordViewModel> all =
                  (from bat in dbContext.Batteries
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
            BatteryViewModel bvm = new BatteryViewModel(bc);      //实例化一个新的view model
            bvm.DisplayName = "Battery-Create";
            bvm.commandType = CommandType.Create;
            var BatteryViewInstance = new BatteryView();      //实例化一个新的view
            BatteryViewInstance.DataContext = bvm;
            BatteryViewInstance.ShowDialog();                   //设置viewmodel属性
            if (bvm.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    //dbContext.Batteries.Add(bc);
                    BatteryClass newBc = new BatteryClass();
                    newBc.Name = bvm.Name;
                    newBc.CycleCount = bvm.CycleCount;
                    newBc.Status = AssetStatusEnum.IDLE;
                    newBc.BatteryType = dbContext.BatteryTypes.SingleOrDefault(bT=>bT.Name == bvm.BatteryType);
                    dbContext.Batteries.Add(newBc);
                    dbContext.SaveChanges();
                    bvm = new BatteryViewModel(newBc);
                }
                this.AllBatteries.Add(bvm);
                //_batteryRepository.AddItem(bc);
            }
        }
        private void Edit()
        {
            BatteryClass bc = new BatteryClass();      //实例化一个新的model
            BatteryViewModel bvm = new BatteryViewModel(bc);      //实例化一个新的view model
            bvm.Id = _selectedItem.Id;
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
                var dbContext = new AppDbContext();
                var bat = dbContext.Batteries.SingleOrDefault(b => b.Id == _selectedItem.Id);
                _selectedItem.Name = bvm.Name;
                _selectedItem.BatteryType = bvm.BatteryType;
                _selectedItem.CycleCount = bvm.CycleCount;
                _selectedItem.Status = bvm.Status;

                bat.Name = bc.Name;
                bat.BatteryType = dbContext.BatteryTypes.SingleOrDefault(bt=>bt.Id == bc.BatteryType.Id);
                bat.CycleCount = bc.CycleCount;
                bat.Status = bc.Status;

                dbContext.SaveChanges();
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            BatteryClass bc = new BatteryClass();      //实例化一个新的model
            BatteryViewModel bvm = new BatteryViewModel(bc);      //实例化一个新的view model
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
                using (var dbContext = new AppDbContext())
                {
                    //dbContext.Batteries.Add(bc);
                    BatteryClass newBc = new BatteryClass();
                    newBc.Name = bvm.Name;
                    newBc.CycleCount = bvm.CycleCount;
                    newBc.Status = AssetStatusEnum.IDLE;
                    newBc.BatteryType = dbContext.BatteryTypes.SingleOrDefault(bT => bT.Name == bvm.BatteryType);
                    dbContext.Batteries.Add(newBc);
                    dbContext.SaveChanges();
                }
                this.AllBatteries.Add(bvm);
                //_batteryRepository.AddItem(bc);
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

            //_batteryRepository.ItemAdded -= this.OnBatteryAddedToRepository;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        //void OnBatteryAddedToRepository(object sender, ItemAddedEventArgs<BatteryClass> e)
        //{
        //    var viewModel = new BatteryViewModel(e.NewItem, _batteryRepository, _batteryTypeRepository);
        //    this.AllBatteries.Add(viewModel);
        //}

        #endregion // Event Handling Methods
    }
}
