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
    /// Updateable: 1. TableMakerProducts: True, 2. Records: True
    /// </summary>
    public class AllTableMakerProductsViewModel : BindableBase
    {
        #region Fields
        private TableMakerProductTypeServiceClass _tableMakerProductTypeServie;
        private TableMakerProductServiceClass _tableMakerProductService;
        private ProjectServiceClass _projectService;
        TableMakerProductViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _deleteCommand;

        #endregion // Fields

        #region Constructor

        public AllTableMakerProductsViewModel(TableMakerProductServiceClass tableMakerProductService, TableMakerProductTypeServiceClass tableMakerProductTypeServie, ProjectServiceClass projectService)
        {
            _tableMakerProductTypeServie = tableMakerProductTypeServie;
            _tableMakerProductService = tableMakerProductService;
            _projectService = projectService;
            this.CreateAllTableMakerProducts(_tableMakerProductService.Items);
            _tableMakerProductService.Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var tableMakerProduct = item as TableMakerProduct;
                        this.AllTableMakerProducts.Add(new TableMakerProductViewModel(tableMakerProduct));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var tableMakerProduct = item as TableMakerProduct;
                        var deletetarget = this.AllTableMakerProducts.SingleOrDefault(o => o.Id == tableMakerProduct.Id);
                        this.AllTableMakerProducts.Remove(deletetarget);
                    }
                    break;
            }
        }

        void CreateAllTableMakerProducts(ObservableCollection<TableMakerProduct> batteries)
        {
            List<TableMakerProduct> allbatteries =
                (from bat in batteries
                 select bat).ToList();

            var all = allbatteries.Select(i=>new TableMakerProductViewModel(i)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllTableMakerProducts = new ObservableCollection<TableMakerProductViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the TableMakerProductModelViewModel objects.
        /// </summary>
        public ObservableCollection<TableMakerProductViewModel> AllTableMakerProducts { get; private set; }

        public TableMakerProductViewModel SelectedItem    //绑定选中项，从而改变batteries
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
            TableMakerProduct editItem = new TableMakerProduct();      //实例化一个新的model
            TableMakerProductEditViewModel bevm = new TableMakerProductEditViewModel(editItem, _projectService.Items, _tableMakerProductTypeServie.Items);      //实例化一个新的view model
            bevm.DisplayName = "Table Maker Product-Create";
            bevm.commandType = CommandType.Create;
            var TableMakerProductViewInstance = new TableMakerProductView();      //实例化一个新的view
            TableMakerProductViewInstance.DataContext = bevm;
            TableMakerProductViewInstance.ShowDialog();                   //设置viewmodel属性
            if (bevm.IsOK == true)
            {
                _tableMakerProductService.FileOperation(editItem);
                _tableMakerProductService.SuperAdd(editItem);

            }
        }
        private void Edit()
        {
            TableMakerProduct editItem = new TableMakerProduct();      //实例化一个新的model
            TableMakerProductEditViewModel bevm = new TableMakerProductEditViewModel(editItem, _projectService.Items, _tableMakerProductTypeServie.Items);      //实例化一个新的view model
            bevm.Id = _selectedItem.Id;
            bevm.FilePath = _selectedItem.FilePath;
            bevm.Project = bevm.AllProjects.SingleOrDefault(i => i.Id == _selectedItem.Project.Id);
            bevm.IsValid = _selectedItem.IsValid;
            bevm.TableMakerProductType = bevm.AllTableMakerProductTypes.SingleOrDefault(i=>i.Id == _selectedItem.TableMakerProductType.Id);   //所以改用Id来找到List里的item
            bevm.DisplayName = "Table Maker Product-Edit";
            bevm.commandType = CommandType.Edit;
            var TableMakerProductViewInstance = new TableMakerProductView();      //实例化一个新的view
            TableMakerProductViewInstance.DataContext = bevm;
            TableMakerProductViewInstance.ShowDialog();
            if (bevm.IsOK == true)
            {
                _tableMakerProductService.SuperUpdate(editItem);
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            TableMakerProduct bc = new TableMakerProduct();      //实例化一个新的model
            TableMakerProductEditViewModel bevm = new TableMakerProductEditViewModel(bc, _projectService.Items, _tableMakerProductTypeServie.Items);      //实例化一个新的view model
            bevm.FilePath = _selectedItem.FilePath;
            bevm.Project = bevm.AllProjects.SingleOrDefault(i => i.Id == _selectedItem.Project.Id);
            bevm.TableMakerProductType = bevm.AllTableMakerProductTypes.SingleOrDefault(i => i.Id == _selectedItem.TableMakerProductType.Id);
            bevm.IsValid = _selectedItem.IsValid;
            bevm.DisplayName = "Table Maker Product-Save As";
            bevm.commandType = CommandType.SaveAs;
            var TableMakerProductViewInstance = new TableMakerProductView();      //实例化一个新的view
            TableMakerProductViewInstance.DataContext = bevm;
            TableMakerProductViewInstance.ShowDialog();
            if (bevm.IsOK == true)
            {
                _tableMakerProductService.SuperAdd(bc);
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedItem != null; }
        }
        private void Delete()
        {
            var model = _tableMakerProductService.Items.SingleOrDefault(o => o.Id == _selectedItem.Id);
            if (MessageBox.Show("Are you sure?", "Delete TableMakerProduct", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _tableMakerProductService.SuperRemove(_selectedItem.Id);
            }
        }
        private bool CanDelete
        {
            get { return _selectedItem != null; }
        }
        #endregion //Private Helper
    }
}
