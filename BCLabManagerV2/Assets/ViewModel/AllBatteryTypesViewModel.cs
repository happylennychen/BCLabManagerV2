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
    /// Updateable: 1. Battery Types: True, 2. Batteries: True
    /// </summary>
    public class AllBatteryTypesViewModel : BindableBase
    {
        #region Fields
        BatteryTypeViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _deleteCommand;
        private BatteryTypeServiceClass _batteryTypeService;
        private BatteryServiceClass _batteryService;

        #endregion // Fields

        #region Constructor

        public AllBatteryTypesViewModel(BatteryTypeServiceClass batteryTypeService, BatteryServiceClass batteryService)
        {
            _batteryTypeService = batteryTypeService;
            _batteryService = batteryService;
            this.CreateAllBatteryTypes(_batteryTypeService.Items);
            _batteryTypeService.Items.CollectionChanged += Items_CollectionChanged;
            _batteryService.Items.CollectionChanged += Batteries_CollectionChanged;
        }

        private void Batteries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("Batteries"); //通知Batteries改变
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var batteryType = item as BatteryTypeClass;
                        this.AllBatteryTypes.Add(new BatteryTypeViewModel(batteryType));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var batteryType = item as BatteryTypeClass;
                        var deletetarget = this.AllBatteryTypes.SingleOrDefault(o => o.Id == batteryType.Id);
                        this.AllBatteryTypes.Remove(deletetarget);
                    }
                    break;
            }
        }

        void CreateAllBatteryTypes(ObservableCollection<BatteryTypeClass> batteryTypes)
        {
            List<BatteryTypeViewModel> all =
                (from batT in batteryTypes
                 select new BatteryTypeViewModel(batT)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllBatteryTypes = new ObservableCollection<BatteryTypeViewModel>(all);     //再转换成Observable
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
                    RaisePropertyChanged("Batteries"); //通知Batteries改变
                }
            }
        }
        public ObservableCollection<BatteryViewModel> Batteries //从Domain取
        {
            get
            {
                if (SelectedItem == null)
                    return null;
                List<BatteryViewModel> all =
                  (from bat in _batteryService.Items
                   where bat.BatteryType.Id == SelectedItem.Id
                   select new BatteryViewModel(bat)).ToList();
                return new ObservableCollection<BatteryViewModel>(all);
            }
        }

        public ICommand CreateCommand
        {
            get
            {
                if (_createCommand == null)
                {
                    _createCommand = new RelayCommand(
                        param => { this.Create(); }
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
            BatteryTypeClass btc = new BatteryTypeClass();      //实例化一个新的model
            BatteryTypeEditViewModel btevm = new BatteryTypeEditViewModel(btc);      //实例化一个新的view model
            //btevm.DisplayName = "Battery Type-Create";
            var BatteryTypeViewInstance = new BatteryTypeView();      //实例化一个新的view
            BatteryTypeViewInstance.DataContext = btevm;
            BatteryTypeViewInstance.ShowDialog();                   //设置viewmodel属性
            if (btevm.IsOK == true)
            {
                _batteryTypeService.SuperAdd(btc);
            }
        }
        private void Edit()
        {
            BatteryTypeClass btc = new BatteryTypeClass();      //实例化一个新的model
            BatteryTypeEditViewModel btevm = new BatteryTypeEditViewModel(btc);      //实例化一个新的view model
            btevm.Id = _selectedItem.Id;
            btevm.Manufacturer = _selectedItem.Manufacturer;
            btevm.Material = _selectedItem.Material;
            btevm.Name = _selectedItem.Name;
            //btevm.DisplayName = "Battery Type-Edit";
            var BatteryTypeViewInstance = new BatteryTypeView();      //实例化一个新的view
            BatteryTypeViewInstance.DataContext = btevm;
            BatteryTypeViewInstance.ShowDialog();
            if (btevm.IsOK == true)
            {
                _batteryTypeService.SuperUpdate(btc);
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            BatteryTypeClass btc = new BatteryTypeClass();      //实例化一个新的model
            BatteryTypeEditViewModel btevm = new BatteryTypeEditViewModel(btc);      //实例化一个新的view model
            btevm.Manufacturer = _selectedItem.Manufacturer;
            btevm.Material = _selectedItem.Material;
            btevm.Name = _selectedItem.Name;
            btevm.CutoffDischargeVoltage = _selectedItem.CutoffDischargeVoltage;
            btevm.FullyChargedEndCurrent = _selectedItem.FullyChargedEndCurrent;
            btevm.FullyChargedEndingTimeout = _selectedItem.FullyChargedEndingTimeout;
            btevm.LimitedChargeVoltage = _selectedItem.LimitedChargeVoltage;
            btevm.NominalVoltage = _selectedItem.NominalVoltage;
            btevm.RatedCapacity = _selectedItem.RatedCapacity;
            btevm.TypicalCapacity = _selectedItem.TypicalCapacity;
            //btevm.DisplayName = "Battery Type-Save As";
            var BatteryTypeViewInstance = new BatteryTypeView();      //实例化一个新的view
            BatteryTypeViewInstance.DataContext = btevm;
            BatteryTypeViewInstance.ShowDialog();
            if (btevm.IsOK == true)
            {
                _batteryTypeService.SuperAdd(btc);
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedItem != null; }
        }
        private void Delete()
        {
            if (_batteryService.Items.Count(o => o.BatteryType.Id == _selectedItem.Id) != 0)
            {
                MessageBox.Show("Before deleting this battery type, please delete all batteries belong to it.");
                return;
            }
            if (MessageBox.Show("Are you sure?", "Delete Battery Type", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _batteryTypeService.SuperRemove(_selectedItem.Id);
            }
        }
        private bool CanDelete
        {
            get { return _selectedItem != null; }
        }
        #endregion //Private Helper

        #region Event Handling Methods

        #endregion // Event Handling Methods
    }
}
