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
    /// <summary>
    /// Editable: True
    /// Updateable: 1. Batteries: True, 2. Records: True
    /// </summary>
    public class AllBatteriesViewModel : BindableBase
    {
        #region Fields
        private BatteryTypeServiceClass _batteryTypeServie;
        private BatteryServiceClass _batteryService;
        BatteryViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _deleteCommand;

        #endregion // Fields

        #region Constructor

        public AllBatteriesViewModel(BatteryServiceClass batteryService, BatteryTypeServiceClass batteryTypeServie)
        {
            _batteryTypeServie = batteryTypeServie;
            _batteryService = batteryService;
            this.CreateAllBatteries(_batteryService.Items);
            _batteryService.Items.CollectionChanged += Items_CollectionChanged;
            foreach(var battery in _batteryService.Items)
                battery.Records.CollectionChanged += Records_CollectionChanged;
        }

        private void Records_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("Records"); //通知Records改变
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var battery = item as Battery;
                        this.AllBatteries.Add(new BatteryViewModel(battery));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var battery = item as Battery;
                        var deletetarget = this.AllBatteries.SingleOrDefault(o => o.Id == battery.Id);
                        this.AllBatteries.Remove(deletetarget);
                    }
                    break;
            }
        }

        void CreateAllBatteries(ObservableCollection<Battery> batteries)
        {
            List<Battery> allbatteries =
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

        public ObservableCollection<AssetUsageRecordViewModel> Records //从Domain获取
        {
            get
            {
                if (SelectedItem == null)
                    return null;
                List<AssetUsageRecordViewModel> all =
                  (from bat in _batteryService.Items
                   where bat.Id == SelectedItem.Id
                   from record in bat.Records
                   select new AssetUsageRecordViewModel(record)).ToList();
                var output = new ObservableCollection<AssetUsageRecordViewModel>(all);
                //output.CollectionChanged += Output_CollectionChanged;
                return output;
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
            Battery editItem = new Battery();      //实例化一个新的model
            BatteryEditViewModel bevm = new BatteryEditViewModel(editItem, _batteryTypeServie.Items);      //实例化一个新的view model
            bevm.DisplayName = "Battery-Create";
            bevm.commandType = CommandType.Create;
            var BatteryViewInstance = new BatteryView();      //实例化一个新的view
            BatteryViewInstance.DataContext = bevm;
            BatteryViewInstance.ShowDialog();                   //设置viewmodel属性
            if (bevm.IsOK == true)
            {
                _batteryService.SuperAdd(editItem);

            }
        }
        private void Edit()
        {
            Battery editItem = new Battery();      //实例化一个新的model
            BatteryEditViewModel bevm = new BatteryEditViewModel(editItem, _batteryTypeServie.Items);      //实例化一个新的view model
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
                _batteryService.SuperUpdate(editItem);
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            Battery bc = new Battery();      //实例化一个新的model
            BatteryEditViewModel bevm = new BatteryEditViewModel(bc, _batteryTypeServie.Items);      //实例化一个新的view model
            bevm.Name = _selectedItem.Name;
            bevm.BatteryType = bevm.AllBatteryTypes.SingleOrDefault(i => i.Id == _selectedItem.BatteryType.Id);
            bevm.CycleCount = _selectedItem.CycleCount;
            bevm.AssetUseCount = _selectedItem.AssetUseCount;
            bevm.DisplayName = "Battery-Save As";
            bevm.commandType = CommandType.SaveAs;
            var BatteryViewInstance = new BatteryView();      //实例化一个新的view
            BatteryViewInstance.DataContext = bevm;
            BatteryViewInstance.ShowDialog();
            if (bevm.IsOK == true)
            {
                _batteryService.SuperAdd(bc);
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedItem != null; }
        }
        private void Delete()
        {
            var model = _batteryService.Items.SingleOrDefault(o => o.Id == _selectedItem.Id);
            if (model.AssetUseCount > 0)
            {
                MessageBox.Show("Cannot delete using battery.");
                return;
            }
            if (MessageBox.Show("Are you sure?", "Delete Battery", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _batteryService.SuperRemove(_selectedItem.Id);
            }
        }
        private bool CanDelete
        {
            get { return _selectedItem != null; }
        }
        #endregion //Private Helper
    }
}
