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
    public class AllBatteryTypesViewModel : ViewModelBase
    {
        #region Fields
        BatteryTypeViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        private List<BatteryTypeClass> _batteryTypes;

        #endregion // Fields

        #region Constructor

        public AllBatteryTypesViewModel(List<BatteryTypeClass> batteryTypes)
        {
            _batteryTypes = batteryTypes;
            this.CreateAllBatteryTypes(batteryTypes);
        }

        void CreateAllBatteryTypes(List<BatteryTypeClass> batteryTypes)
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
                    OnPropertyChanged("Batteries"); //通知Batteries改变
                }
            }
        }
        public List<BatteryViewModel> Batteries //绑定选中type的batteries。只显示，所以只有get没有set。每次改选type，都要重新做一次查询    //不需要ObservableCollection，因为每次变化都已经被通知了
        //如果不是用查询，那么需要维护一个二维List。每一个BatteryType，对应一个List。用空间换时间。
        {
            get
            {
                //var dbContext = new AppDbContext();
                //if (SelectedItem == null)
                //    return null;
                //List<BatteryViewModel> all =
                //  (from bat in dbContext.Batteries
                //   where bat.BatteryType.Name == SelectedItem.Name
                //   select new BatteryViewModel(bat)).ToList();
                //return all;
                return null;
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
            BatteryTypeEditViewModel btevm = new BatteryTypeEditViewModel(btc);      //实例化一个新的view model
            btevm.DisplayName = "Battery Type-Create";
            var BatteryTypeViewInstance = new BatteryTypeView();      //实例化一个新的view
            BatteryTypeViewInstance.DataContext = btevm;
            BatteryTypeViewInstance.ShowDialog();                   //设置viewmodel属性
            if (btevm.IsOK == true)
            {
                _batteryTypes.Add(btc);
                using (var dbContext = new AppDbContext())
                {
                    dbContext.BatteryTypes.Add(btc);
                    dbContext.SaveChanges();
                }
                this.AllBatteryTypes.Add(new BatteryTypeViewModel(btc));
            }
        }
        private void Edit()
        {
            BatteryTypeClass btc = new BatteryTypeClass();      //实例化一个新的model
            BatteryTypeEditViewModel btevm = new BatteryTypeEditViewModel(btc);      //实例化一个新的view model
            btevm.Manufactor = _selectedItem.Manufactor;
            btevm.Material = _selectedItem.Material;
            btevm.Name = _selectedItem.Name;
            btevm.DisplayName = "Battery Type-Edit";
            var BatteryTypeViewInstance = new BatteryTypeView();      //实例化一个新的view
            BatteryTypeViewInstance.DataContext = btevm;
            BatteryTypeViewInstance.ShowDialog();
            if (btevm.IsOK == true)
            {
                _selectedItem.Manufactor = btevm.Manufactor;
                _selectedItem.Material = btevm.Material;
                _selectedItem.Name = btevm.Name;
                using (var dbContext = new AppDbContext())
                {
                    var batT = dbContext.BatteryTypes.SingleOrDefault(b => b.Id == _selectedItem.Id);
                    batT.Manufactor = btc.Manufactor;
                    batT.Material = btc.Material;
                    batT.Name = btc.Name;
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
            BatteryTypeClass btc = new BatteryTypeClass();      //实例化一个新的model
            BatteryTypeEditViewModel btevm = new BatteryTypeEditViewModel(btc);      //实例化一个新的view model
            btevm.Manufactor = _selectedItem.Manufactor;
            btevm.Material = _selectedItem.Material;
            btevm.Name = _selectedItem.Name;
            btevm.DisplayName = "Battery Type-Save As";
            var BatteryTypeViewInstance = new BatteryTypeView();      //实例化一个新的view
            BatteryTypeViewInstance.DataContext = btevm;
            BatteryTypeViewInstance.ShowDialog();
            if (btevm.IsOK == true)
            {
                _batteryTypes.Add(btc);
                using (var dbContext = new AppDbContext())
                {
                    dbContext.BatteryTypes.Add(btc);
                    dbContext.SaveChanges();
                }
                this.AllBatteryTypes.Add(new BatteryTypeViewModel(btc));
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

            //_batterytypeRepository.ItemAdded -= this.OnBatteryModelAddedToRepository;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        //void OnBatteryModelAddedToRepository(object sender, ItemAddedEventArgs<BatteryTypeClass> e)
        //{
        //    var viewModel = new BatteryTypeViewModel(e.NewItem, _batterytypeRepository);
        //    this.AllBatteryTypes.Add(viewModel);
        //}

        #endregion // Event Handling Methods
    }
}
