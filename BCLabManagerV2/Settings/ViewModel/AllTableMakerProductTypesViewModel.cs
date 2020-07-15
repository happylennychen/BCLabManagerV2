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
    public class AllTableMakerProductTypesViewModel : BindableBase
    {
        #region Fields
        TableMakerProductTypeViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _deleteCommand;
        private TableMakerProductTypeServiceClass _programTypeService;

        #endregion // Fields

        #region Constructor

        public AllTableMakerProductTypesViewModel(TableMakerProductTypeServiceClass programTypeService)
        {
            _programTypeService = programTypeService;
            this.CreateAllTableMakerProductTypes(_programTypeService.Items);
            _programTypeService.Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var programType = item as TableMakerProductType;
                        this.AllTableMakerProductTypes.Add(new TableMakerProductTypeViewModel(programType));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var programType = item as TableMakerProductType;
                        var deletetarget = this.AllTableMakerProductTypes.SingleOrDefault(o => o.Id == programType.Id);
                        this.AllTableMakerProductTypes.Remove(deletetarget);
                    }
                    break;
            }
        }

        void CreateAllTableMakerProductTypes(ObservableCollection<TableMakerProductType> programTypes)
        {
            List<TableMakerProductTypeViewModel> all =
                (from proT in programTypes
                 select new TableMakerProductTypeViewModel(proT)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllTableMakerProductTypes = new ObservableCollection<TableMakerProductTypeViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the BatteryModelViewModel objects.
        /// </summary>
        public ObservableCollection<TableMakerProductTypeViewModel> AllTableMakerProductTypes { get; private set; }

        public TableMakerProductTypeViewModel SelectedItem    //绑定选中项
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
                }
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
            TableMakerProductType proT = new TableMakerProductType();      //实例化一个新的model
            TableMakerProductTypeEditViewModel proTevm = new TableMakerProductTypeEditViewModel(proT);      //实例化一个新的view model
            //btevm.DisplayName = "Battery Type-Create";
            var TableMakerProductTypeEditViewInstance = new TableMakerProductTypeView();      //实例化一个新的view
            TableMakerProductTypeEditViewInstance.DataContext = proTevm;
            TableMakerProductTypeEditViewInstance.ShowDialog();                   //设置viewmodel属性
            if (proTevm.IsOK == true)
            {
                _programTypeService.SuperAdd(proT);
            }
        }
        private void Edit()
        {
            TableMakerProductType proT = new TableMakerProductType();      //实例化一个新的model
            TableMakerProductTypeEditViewModel proTevm = new TableMakerProductTypeEditViewModel(proT);      //实例化一个新的view model
            proTevm.Id = _selectedItem.Id;
            proTevm.Description = _selectedItem.Description;

            var TableMakerProductTypeEditViewInstance = new TableMakerProductTypeView();      //实例化一个新的view
            TableMakerProductTypeEditViewInstance.DataContext = proTevm;
            TableMakerProductTypeEditViewInstance.ShowDialog();
            if (proTevm.IsOK == true)
            {
                _programTypeService.SuperUpdate(proT);
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            TableMakerProductType proT = new TableMakerProductType();      //实例化一个新的model
            TableMakerProductTypeEditViewModel proTevm = new TableMakerProductTypeEditViewModel(proT);      //实例化一个新的view model
            proTevm.Description = _selectedItem.Description;

            var TableMakerProductTypeEditViewInstance = new TableMakerProductTypeView();      //实例化一个新的view
            TableMakerProductTypeEditViewInstance.DataContext = proTevm;
            TableMakerProductTypeEditViewInstance.ShowDialog();
            if (proTevm.IsOK == true)
            {
                _programTypeService.SuperAdd(proT);
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedItem != null; }
        }
        private void Delete()
        {
            //if (_batteryService.Items.Count(o => o.BatteryType.Id == _selectedItem.Id) != 0)
            //{
            //    MessageBox.Show("Before deleting this battery type, please delete all batteries belong to it.");
            //    return;
            //}
            //if (MessageBox.Show("Are you sure?", "Delete Battery Type", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //{
            //    _batteryTypeService.SuperRemove(_selectedItem.Id);
            //}
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
