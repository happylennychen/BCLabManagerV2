using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BCLabManager.ViewModel
{
    public class AllDynamicCurrentsViewModel : ViewModelBase
    {
        #region Fields
        List<DynamicCurrentClass> _dynamicCurrents;
        DynamicCurrentViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllDynamicCurrentsViewModel(List<DynamicCurrentClass> dynamicCurrents)
        {
            this.CreateAlldynamicCurrents(dynamicCurrents);
        }

        void CreateAlldynamicCurrents(List<DynamicCurrentClass> dynamicCurrents)
        {
            _dynamicCurrents = dynamicCurrents;
            List<DynamicCurrentViewModel> all =
                (from ct in dynamicCurrents
                 select new DynamicCurrentViewModel(ct)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AlldynamicCurrents = new ObservableCollection<DynamicCurrentViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the SubProgramModelViewModel objects.
        /// </summary>
        public ObservableCollection<DynamicCurrentViewModel> AlldynamicCurrents { get; private set; }

        public DynamicCurrentViewModel SelectedItem    //绑定选中项，从而改变subprograms
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

        #endregion // Public Interface

        #region Private Helper
        private void Create()
        {
            DynamicCurrentClass model = new DynamicCurrentClass();      //实例化一个新的model
            DynamicCurrentEditViewModel viewmodel = new DynamicCurrentEditViewModel(model);      //实例化一个新的view model
            viewmodel.DisplayName = "DischargeCurrent-Create";
            viewmodel.commandType = CommandType.Create;
            var DischargeCurrentEditViewInstance = new DischargeCurrentEditView();      //实例化一个新的view
            DischargeCurrentEditViewInstance.DataContext = viewmodel;
            DischargeCurrentEditViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.DynamicCurrents.Add(model);
                    dbContext.SaveChanges();
                }
                _dynamicCurrents.Add(model);
                this.AlldynamicCurrents.Add(new DynamicCurrentViewModel(model));
            }
        }
        private void Edit()
        {
            DynamicCurrentClass model = new DynamicCurrentClass();      //实例化一个新的model
            DynamicCurrentEditViewModel viewmodel = new DynamicCurrentEditViewModel(model);      //实例化一个新的view model
            viewmodel.Value = _selectedItem.Value;
            viewmodel.DisplayName = "DischargeCurrent-Edit";
            viewmodel.commandType = CommandType.Edit;
            var DischargeCurrentEditViewInstance = new DischargeCurrentEditView();      //实例化一个新的view
            DischargeCurrentEditViewInstance.DataContext = viewmodel;
            DischargeCurrentEditViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _selectedItem.Value = viewmodel.Value;
                using (var dbContext = new AppDbContext())
                {
                    var ct = dbContext.DynamicCurrents.SingleOrDefault(i => i.Id == _selectedItem.Id);
                    ct.Value = _selectedItem.Value;
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
            DynamicCurrentClass model = new DynamicCurrentClass();      //实例化一个新的model
            DynamicCurrentEditViewModel viewmodel = new DynamicCurrentEditViewModel(model);      //实例化一个新的view model
            viewmodel.Value = _selectedItem.Value;
            viewmodel.DisplayName = "DischargeCurrent-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var DischargeCurrentEditViewInstance = new DischargeCurrentEditView();      //实例化一个新的view
            DischargeCurrentEditViewInstance.DataContext = viewmodel;
            DischargeCurrentEditViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.DynamicCurrents.Add(model);
                    dbContext.SaveChanges();
                }
                _dynamicCurrents.Add(model);
                this.AlldynamicCurrents.Add(new DynamicCurrentViewModel(model));
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
            foreach (DynamicCurrentViewModel viewmodel in this.AlldynamicCurrents)
                viewmodel.Dispose();

            this.AlldynamicCurrents.Clear();
            //this.AllSubProgramModels.CollectionChanged -= this.OnCollectionChanged;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        #endregion // Event Handling Methods
    }
}
