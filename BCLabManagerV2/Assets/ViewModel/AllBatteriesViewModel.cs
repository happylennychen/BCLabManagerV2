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
using System.Windows;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    public class AllBatteriesViewModel : BindableBase
    {
        #region Fields
        ObservableCollection<BatteryClass> _batteries;
        ObservableCollection<BatteryTypeClass> _batteryTypes;
        BatteryViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _deleteCommand;

        #endregion // Fields

        #region Constructor

        public AllBatteriesViewModel(ObservableCollection<BatteryClass> batteries, ObservableCollection<BatteryTypeClass> batteryTypes)
        {
            _batteries = batteries;
            _batteryTypes = batteryTypes;
            this.CreateAllBatteries(batteries);
            EventBooking();
        }
        private void EventBooking()
        {
            foreach (var battery in _batteries)
            {
                battery.PropertyChanged += Battery_PropertyChanged; ;
            }
        }

        private void Battery_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
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
                    RaisePropertyChanged("Records"); //通知Records改变
                }
            }
        }

        //public List<AssetUsageRecordViewModel> Records //绑定选中battery的Records。只显示，所以只有get没有set。每次改选type，都要重新做一次查询    //不需要ObservableCollection，因为每次变化都已经被通知了
        ////如果不是用查询，那么需要维护一个二维List。每一个Battery，对应一个List。用空间换时间。
        //{
        //    get
        //    {
        //        if (SelectedItem == null)
        //            return null;
        //        using (var dbContext = new AppDbContext())
        //        {
        //            List<AssetUsageRecordViewModel> all =
        //              (from bat in dbContext.Batteries
        //               where bat.Id == SelectedItem.Id
        //               from record in bat.Records
        //               select new AssetUsageRecordViewModel(record)).ToList();
        //            return all;
        //        }
        //    }
        //}

        public List<AssetUsageRecordViewModel> Records //从Domain获取
        {
            get
            {
                if (SelectedItem == null)
                    return null;
                List<AssetUsageRecordViewModel> all =
                  (from bat in _batteries
                   where bat.Id == SelectedItem.Id
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
            bevm.AssetUseCount = _selectedItem.AssetUseCount;
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
            bevm.AssetUseCount = _selectedItem.AssetUseCount;
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
        private void Delete()
        {
            using (var dbContext = new AppDbContext())
            {
                var model = dbContext.Batteries.SingleOrDefault(o => o.Id == _selectedItem.Id);
                if (model.AssetUseCount > 0)
                {
                    MessageBox.Show("Cannot delete using battery.");
                    return;
                }
                if (MessageBox.Show("Are you sure?", "Delete Battery", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    dbContext.Batteries.Remove(model);
                    dbContext.SaveChanges();

                    model = _batteries.SingleOrDefault(o => o.Id == _selectedItem.Id);
                    _batteries.Remove(model);
                    this.AllBatteries.Remove(_selectedItem);
                }
            }
        }
        private bool CanDelete
        {
            get { return _selectedItem != null; }
        }
        #endregion //Private Helper

        #region Event Handling Methods

        //void OnBatteryAddedToRepository(object sender, ItemAddedEventArgs<BatteryClass> e)
        //{
        //    var viewModel = new BatteryViewModel(e.NewItem, _batteryRepository, _batteryTypeRepository);
        //    this.AllBatteries.Add(viewModel);
        //}

        #endregion // Event Handling Methods
    }
}
