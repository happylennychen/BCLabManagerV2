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
using System.IO;

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
        RelayCommand _renameCommand;
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
                        var batteryType = item as BatteryType;
                        this.AllBatteryTypes.Add(new BatteryTypeViewModel(batteryType));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var batteryType = item as BatteryType;
                        var deletetarget = this.AllBatteryTypes.SingleOrDefault(o => o.Id == batteryType.Id);
                        this.AllBatteryTypes.Remove(deletetarget);
                    }
                    break;
            }
        }

        void CreateAllBatteryTypes(ObservableCollection<BatteryType> batteryTypes)
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
        public ICommand RenameCommand
        {
            get
            {
                if (_renameCommand == null)
                {
                    _renameCommand = new RelayCommand(
                        param => { this.Rename(); },
                        param => this.CanRename
                        );
                }
                return _renameCommand;
            }
        }
        public void Update(string sources, string oldBatteryTypeName, string newBatteryTypeName, out string source)
        {
            source = null;
            if (sources.Contains($@"\{oldBatteryTypeName}\"))
            {
                source = sources.Replace($@"\{oldBatteryTypeName}\", $@"\{newBatteryTypeName}\");
            }


        }

        #endregion // Public Interface

        #region Private Helper
        private void Create()
        {
            BatteryType btc = new BatteryType();      //实例化一个新的model
            BatteryTypeEditViewModel btevm = new BatteryTypeEditViewModel(btc);      //实例化一个新的view model
            btevm.DisplayName = "Battery Type-Create";
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
            BatteryType btc = new BatteryType();      //实例化一个新的model
            BatteryTypeEditViewModel btevm = new BatteryTypeEditViewModel(btc);      //实例化一个新的view model
            btevm.Id = _selectedItem.Id;
            btevm.Manufacturer = _selectedItem.Manufacturer;
            btevm.Material = _selectedItem.Material;
            btevm.Name = _selectedItem.Name;
            btevm.DisplayName = "Battery Type-Edit";
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
            BatteryType btc = new BatteryType();      //实例化一个新的model
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
            btevm.DisplayName = "Battery Type-Save As";
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
        private void Rename()
        {
            BatteryType btc = new BatteryType();
            BatteryTypeEditViewModel btevm = new BatteryTypeEditViewModel(btc);
            btevm.Id = _selectedItem.Id;
            btevm.Name = _selectedItem.Name;
            btevm.DisplayName = "Battery Type-Rename";
            var BatteryTypeViewInstance = new BatteryTypeView();
            BatteryTypeViewInstance.DataContext = btevm;
            BatteryTypeViewInstance.ShowDialog();
            if (!btevm.IsOK)
            {
                return;
            }
            using (var context = new AppDbContext())
            {
                var tmrs = context.TableMakerRecords.ToList();
                var tmps = context.TableMakerProducts.ToList();
                var bt = context.BatteryTypes.Include(tr => tr.Projects).Where(tr => tr.Id == btevm.Id).ToList();
                var trs = context.TestRecords.Include(tr => tr.Recipe.Program.Project).Where(tr => tr.BatteryTypeStr == bt[0].Name && tr.ProgramStr != null && tr.ProjectStr != null).ToList();
                var oldName = bt[0].Name;
                bt[0].Name = btevm.Name;
                var newName = bt[0].Name;
                var newPath = trs[0].TestFilePath.Replace($@"\{oldName}\", $@"\{newName}\");
                Directory.Move(trs[0].TestFilePath.Remove(trs[0].TestFilePath.LastIndexOf(trs[0].ProjectStr)), newPath.Remove(newPath.LastIndexOf(trs[0].ProjectStr)));
                foreach (var tr in trs)
                {
                    if (tr.TestFilePath.Contains($@"\{oldName}\"))
                    {
                        tr.TestFilePath = tr.TestFilePath.Replace($@"\{oldName}\", $@"\{newName}\");
                        tr.BatteryTypeStr = tr.BatteryTypeStr.Replace($@"{oldName}", $@"{newName}");
                    }
                    if (tr.StdFilePath != null)
                    {
                        tr.StdFilePath = tr.StdFilePath.Replace($@"\{oldName}\", $@"\{newName}\");
                    }
                }
                foreach (var tmr in tmrs)
                {
                    for (int n = 0; n < tmr.OCVSources.Count; n++)
                    {
                        Update(tmr.OCVSources[n], oldName, newName, out string source);
                        if (source != null)
                        {
                            tmr.OCVSources[n] = source;
                        }
                    }
                    for (int n = 0; n < tmr.RCSources.Count; n++)
                    {
                        Update(tmr.RCSources[n], oldName, newName, out string source);
                        if (source != null)
                        {
                            tmr.RCSources[n] = source;
                        }
                    }
                }
                foreach (var tmp in tmps)
                {
                    if (tmp.FilePath.Contains($@"\{oldName}\"))
                    {
                        tmp.FilePath = tmp.FilePath.Replace($@"\{oldName}\", $@"\{newName}\");
                        var remotepath = tmp.FilePath;
                        tmp.FilePath = remotepath.Replace($@"_{oldName}_", $@"_{newName}_");
                        FileTransferHelper.FileRename(remotepath, Path.GetFileName(tmp.FilePath));
                    }
                }
                context.SaveChanges();
                MessageBox.Show("complete");
            }
        }
        private bool CanRename
        {
            get { return _selectedItem != null; }
        }
        #endregion //Private Helper

        #region Event Handling Methods

        #endregion // Event Handling Methods
    }
}
