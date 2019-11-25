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
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// Editable: True
    /// Updateable: 1. Chambers: True, 2. Records: True
    /// </summary>
    public class AllChambersViewModel : BindableBase
    {
        #region Fields

        //readonly ChamberRepository _chamberRepository;
        ChamberViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _deleteCommand;
        //ObservableCollection<ChamberClass> _chambers;
        private ChamberServieClass _chamberService;

        #endregion // Fields

        #region Constructor

        public AllChambersViewModel(ChamberServieClass chamberService)
        {
            _chamberService = chamberService;
            this.CreateAllChambers(_chamberService.Items);
            _chamberService.Items.CollectionChanged += Items_CollectionChanged;
            foreach (var chamber in _chamberService.Items)
                chamber.Records.CollectionChanged += Records_CollectionChanged;
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
                        var chamber = item as ChamberClass;
                        this.AllChambers.Add(new ChamberViewModel(chamber));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var chamber = item as ChamberClass;
                        var deletetarget = this.AllChambers.SingleOrDefault(o => o.Id == chamber.Id);
                        this.AllChambers.Remove(deletetarget);
                    }
                    break;
            }
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
                    //RaisePropertyChanged("SelectedType");
                    RaisePropertyChanged("Records"); //通知Records改变
                }
            }
        }

        public ObservableCollection<AssetUsageRecordViewModel> Records
        {
            get
            {
                if (SelectedItem == null)
                    return null;
                List<AssetUsageRecordViewModel> all =
                  (from cmb in _chamberService.Items
                   where cmb.Id == SelectedItem.Id
                   from record in cmb.Records
                   select new AssetUsageRecordViewModel(record)).ToList();
                return new ObservableCollection<AssetUsageRecordViewModel>(all);
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
            ChamberClass edititem = new ChamberClass();      //实例化一个新的model
            ChamberEditViewModel evm = new ChamberEditViewModel(edititem);      //实例化一个新的view model
            //evm.DisplayName = "Chamber-Create";
            evm.commandType = CommandType.Create;
            var ChamberViewInstance = new ChamberView();      //实例化一个新的view
            ChamberViewInstance.DataContext = evm;
            ChamberViewInstance.ShowDialog();                   //设置viewmodel属性
            if (evm.IsOK == true)
            {
                _chamberService.SuperAdd(edititem);
            }
        }
        private void Edit()
        {
            ChamberClass edititem = new ChamberClass();      //实例化一个新的model
            ChamberEditViewModel evm = new ChamberEditViewModel(edititem);      //实例化一个新的view model
            evm.Id = SelectedItem.Id;
            evm.Name = _selectedItem.Name;
            evm.Manufactor = _selectedItem.Manufactor;
            evm.LowTemp = _selectedItem.LowTemp;
            evm.HighTemp = _selectedItem.HighTemp;
            //evm.DisplayName = "Chamber-Edit";
            evm.commandType = CommandType.Edit;
            var ChamberViewInstance = new ChamberView();      //实例化一个新的view
            ChamberViewInstance.DataContext = evm;
            ChamberViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                _chamberService.SuperUpdate(edititem);
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
            //evm.DisplayName = "Chamber-Save As";
            evm.commandType = CommandType.SaveAs;
            var ChamberViewInstance = new ChamberView();      //实例化一个新的view
            ChamberViewInstance.DataContext = evm;
            ChamberViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                _chamberService.SuperAdd(m);
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedItem != null; }
        }
        private void Delete()
        {
            var model = _chamberService.Items.SingleOrDefault(o => o.Id == _selectedItem.Id);
            if (model.AssetUseCount > 0)
            {
                MessageBox.Show("Cannot delete using chamber.");
                return;
            }
            if (MessageBox.Show("Are you sure?", "Delete Chamber", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _chamberService.SuperRemove(_selectedItem.Id);
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
