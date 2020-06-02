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
    public class AllProgramTypesViewModel : BindableBase
    {
        #region Fields
        ProgramTypeViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _deleteCommand;
        private ProgramTypeServiceClass _programTypeService;

        #endregion // Fields

        #region Constructor

        public AllProgramTypesViewModel(ProgramTypeServiceClass programTypeService)
        {
            _programTypeService = programTypeService;
            this.CreateAllProgramTypes(_programTypeService.Items);
            _programTypeService.Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var programType = item as ProgramTypeClass;
                        this.AllProgramTypes.Add(new ProgramTypeViewModel(programType));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var programType = item as ProgramTypeClass;
                        var deletetarget = this.AllProgramTypes.SingleOrDefault(o => o.Id == programType.Id);
                        this.AllProgramTypes.Remove(deletetarget);
                    }
                    break;
            }
        }

        void CreateAllProgramTypes(ObservableCollection<ProgramTypeClass> programTypes)
        {
            List<ProgramTypeViewModel> all =
                (from proT in programTypes
                 select new ProgramTypeViewModel(proT)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllProgramTypes = new ObservableCollection<ProgramTypeViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the BatteryModelViewModel objects.
        /// </summary>
        public ObservableCollection<ProgramTypeViewModel> AllProgramTypes { get; private set; }

        public ProgramTypeViewModel SelectedItem    //绑定选中项
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
            ProgramTypeClass proT = new ProgramTypeClass();      //实例化一个新的model
            ProgramTypeEditViewModel proTevm = new ProgramTypeEditViewModel(proT);      //实例化一个新的view model
            //btevm.DisplayName = "Battery Type-Create";
            var ProgramTypeEditViewInstance = new ProgramTypeView();      //实例化一个新的view
            ProgramTypeEditViewInstance.DataContext = proTevm;
            ProgramTypeEditViewInstance.ShowDialog();                   //设置viewmodel属性
            if (proTevm.IsOK == true)
            {
                _programTypeService.SuperAdd(proT);
            }
        }
        private void Edit()
        {
            ProgramTypeClass proT = new ProgramTypeClass();      //实例化一个新的model
            ProgramTypeEditViewModel proTevm = new ProgramTypeEditViewModel(proT);      //实例化一个新的view model
            proTevm.Id = _selectedItem.Id;
            proTevm.Name = _selectedItem.Name;
            proTevm.Description = _selectedItem.Description;

            var ProgramTypeEditViewInstance = new ProgramTypeView();      //实例化一个新的view
            ProgramTypeEditViewInstance.DataContext = proTevm;
            ProgramTypeEditViewInstance.ShowDialog();
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
            ProgramTypeClass proT = new ProgramTypeClass();      //实例化一个新的model
            ProgramTypeEditViewModel proTevm = new ProgramTypeEditViewModel(proT);      //实例化一个新的view model
            proTevm.Name = _selectedItem.Name;
            proTevm.Description = _selectedItem.Description;

            var ProgramTypeEditViewInstance = new ProgramTypeView();      //实例化一个新的view
            ProgramTypeEditViewInstance.DataContext = proTevm;
            ProgramTypeEditViewInstance.ShowDialog();
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
