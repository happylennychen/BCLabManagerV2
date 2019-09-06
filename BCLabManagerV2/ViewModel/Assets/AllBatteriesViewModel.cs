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
        ObservableCollection<BatteryClass> _batteries;
        List<BatteryTypeClass> _batteryTypes;
        BatteryViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllBatteriesViewModel(ObservableCollection<BatteryClass> batteries, List<BatteryTypeClass> batteryTypes)
        {
            _batteries = batteries;
            _batteryTypes = batteryTypes;
            this.CreateAllBatteries(batteries);
        }
        void CreateAllBatteries(ObservableCollection<BatteryClass> batteries)
        {
            List<BatteryClass> allbatteries =
                (from bat in batteries
                 select bat).ToList();

            var all = allbatteries.Select(i=>new BatteryViewModel(i)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

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
                    //OnPropertyChanged("Records"); //通知Records改变
                }
            }
        }

        //public List<AssetUsageRecordViewModel> Records //绑定选中battery的Records。只显示，所以只有get没有set。每次改选type，都要重新做一次查询    //不需要ObservableCollection，因为每次变化都已经被通知了
        ////如果不是用查询，那么需要维护一个二维List。每一个Battery，对应一个List。用空间换时间。
        //{
        //    get
        //    {
        //        var dbContext = new AppDbContext();
        //        if (SelectedItem == null)
        //            return null;
        //        List<AssetUsageRecordViewModel> all =
        //          (from bat in dbContext.Batteries
        //           where bat.Name == SelectedItem.Name
        //           from record in bat.Records
        //           select new AssetUsageRecordViewModel(record)).ToList();
        //        return all;
        //    }
        //}

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
            BatteryEditViewModel bevm = new BatteryEditViewModel(bc, _batteryTypes);      //实例化一个新的view model
            bevm.DisplayName = "Battery-Create";
            bevm.commandType = CommandType.Create;
            var BatteryViewInstance = new BatteryView();      //实例化一个新的view
            BatteryViewInstance.DataContext = bevm;
            BatteryViewInstance.ShowDialog();                   //设置viewmodel属性
            if (bevm.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    //dbContext.Batteries.Add(bc);    //不能直接这样写，不然会报错。这里不是添加一个全新的graph，而是添加一个新的bc，然后修改关系
                    var newb = new BatteryClass()
                    {
                        Name = bc.Name,
                        CycleCount = bc.CycleCount
                    };
                    newb.BatteryType = dbContext.BatteryTypes.SingleOrDefault(i => i.Id == bc.BatteryType.Id);
                    dbContext.Batteries.Add(newb);
                    dbContext.SaveChanges();    //side effect是会更新newb的id
                    bc = newb;                  //所以把newb存到using语句外面的bc里
                }
                _batteries.Add(bc);
                this.AllBatteries.Add(new BatteryViewModel(bc));    //然后用bc生成vm，这样ID就会更新
            }
        }
        private void Edit()
        {
            BatteryClass bc = new BatteryClass();      //实例化一个新的model
            BatteryEditViewModel bevm = new BatteryEditViewModel(bc, _batteryTypes);      //实例化一个新的view model
            bevm.Id = _selectedItem.Id;
            bevm.Name = _selectedItem.Name;
            //bevm.BatteryType = _selectedItem.BatteryType;     //不能用这种方式，猜是因为传来传去之后，_selectedItem.BatteryType已经不是bevm.AllBatteryTypes的一员了
            bevm.BatteryType = bevm.AllBatteryTypes.SingleOrDefault(i=>i.Id == _selectedItem.BatteryType.Id);   //所以改用Id来找到List里的item
            bevm.CycleCount = _selectedItem.CycleCount;
            bevm.Status = _selectedItem.Status;
            bevm.DisplayName = "Battery-Edit";
            bevm.commandType = CommandType.Edit;
            var BatteryViewInstance = new BatteryView();      //实例化一个新的view
            BatteryViewInstance.DataContext = bevm;
            BatteryViewInstance.ShowDialog();
            if (bevm.IsOK == true)
            {
                _selectedItem.Name = bevm.Name;
                _selectedItem.BatteryType = bevm.BatteryType;
                _selectedItem.CycleCount = bevm.CycleCount;
                using (var dbContext = new AppDbContext())
                {
                    var bat = dbContext.Batteries.SingleOrDefault(b => b.Id == _selectedItem.Id);
                    bat.Name = bc.Name;
                    bat.BatteryType = dbContext.BatteryTypes.SingleOrDefault(bt => bt.Id == bc.BatteryType.Id);
                    bat.CycleCount = bc.CycleCount;

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
            BatteryClass bc = new BatteryClass();      //实例化一个新的model
            BatteryEditViewModel bevm = new BatteryEditViewModel(bc, _batteryTypes);      //实例化一个新的view model
            bevm.Name = _selectedItem.Name;
            bevm.BatteryType = bevm.AllBatteryTypes.SingleOrDefault(i => i.Id == _selectedItem.BatteryType.Id);
            bevm.CycleCount = _selectedItem.CycleCount;
            bevm.Status = _selectedItem.Status;
            bevm.DisplayName = "Battery-Edit";
            bevm.commandType = CommandType.SaveAs;
            var BatteryViewInstance = new BatteryView();      //实例化一个新的view
            BatteryViewInstance.DataContext = bevm;
            BatteryViewInstance.ShowDialog();
            if (bevm.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    //dbContext.Batteries.Add(bc);    //不能直接这样写，不然会报错。这里不是添加一个全新的graph，而是添加一个新的bc，然后修改关系
                    var newb = new BatteryClass()
                    {
                        Name = bc.Name,
                        CycleCount = bc.CycleCount
                    };
                    newb.BatteryType = dbContext.BatteryTypes.SingleOrDefault(i => i.Id == bc.BatteryType.Id);
                    dbContext.Batteries.Add(newb);
                    dbContext.SaveChanges();    //side effect是会更新newb的id
                    bc = newb;                  //所以把newb存到using语句外面的bc里
                }
                _batteries.Add(bc);
                this.AllBatteries.Add(new BatteryViewModel(bc));    //然后用bc生成vm，这样ID就会更新
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
