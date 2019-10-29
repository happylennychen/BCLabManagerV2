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
    public class AllDischargeCurrentsViewModel : ViewModelBase
    {
        #region Fields
        List<DischargeCurrentClass> _dischargeCurrents;
        DischargeCurrentViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllDischargeCurrentsViewModel(List<DischargeCurrentClass> dischargeCurrents)
        {
            this.CreateAllDischargeCurrents(dischargeCurrents);
        }

        void CreateAllDischargeCurrents(List<DischargeCurrentClass> dischargeCurrents)
        {
            _dischargeCurrents = dischargeCurrents;
            List<DischargeCurrentViewModel> all =
                (from ct in dischargeCurrents
                 select new DischargeCurrentViewModel(ct)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllDischargeCurrents = new ObservableCollection<DischargeCurrentViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the SubProgramModelViewModel objects.
        /// </summary>
        public ObservableCollection<DischargeCurrentViewModel> AllDischargeCurrents { get; private set; }

        public DischargeCurrentViewModel SelectedItem    //绑定选中项，从而改变subprograms
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
            DischargeCurrentClass model = new DischargeCurrentClass();      //实例化一个新的model
            DischargeCurrentEditViewModel viewmodel = new DischargeCurrentEditViewModel(model);      //实例化一个新的view model
            viewmodel.DisplayName = "DischargeCurrent-Create";
            viewmodel.commandType = CommandType.Create;
            var DischargeCurrentEditViewInstance = new DischargeCurrentEditView();      //实例化一个新的view
            DischargeCurrentEditViewInstance.DataContext = viewmodel;
            DischargeCurrentEditViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.DischargeCurrents.Add(model);
                    dbContext.SaveChanges();
                }
                _dischargeCurrents.Add(model);
                this.AllDischargeCurrents.Add(new DischargeCurrentViewModel(model));
            }
        }
        private void Edit()
        {
            DischargeCurrentClass model = new DischargeCurrentClass();      //实例化一个新的model
            DischargeCurrentEditViewModel viewmodel = new DischargeCurrentEditViewModel(model);      //实例化一个新的view model
            viewmodel.Name = _selectedItem.Name;
            viewmodel.DisplayName = "DischargeCurrent-Edit";
            viewmodel.commandType = CommandType.Edit;
            var DischargeCurrentEditViewInstance = new DischargeCurrentEditView();      //实例化一个新的view
            DischargeCurrentEditViewInstance.DataContext = viewmodel;
            DischargeCurrentEditViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _selectedItem.Name = viewmodel.Name;
                using (var dbContext = new AppDbContext())
                {
                    var ct = dbContext.DischargeCurrents.SingleOrDefault(i => i.Id == _selectedItem.Id);
                    ct.Name = _selectedItem.Name;
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
            DischargeCurrentClass model = new DischargeCurrentClass();      //实例化一个新的model
            DischargeCurrentEditViewModel viewmodel = new DischargeCurrentEditViewModel(model);      //实例化一个新的view model
            viewmodel.Name = _selectedItem.Name;
            viewmodel.DisplayName = "DischargeCurrent-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var DischargeCurrentEditViewInstance = new DischargeCurrentEditView();      //实例化一个新的view
            DischargeCurrentEditViewInstance.DataContext = viewmodel;
            DischargeCurrentEditViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.DischargeCurrents.Add(model);
                    dbContext.SaveChanges();
                }
                _dischargeCurrents.Add(model);
                this.AllDischargeCurrents.Add(new DischargeCurrentViewModel(model));
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
            foreach (DischargeCurrentViewModel viewmodel in this.AllDischargeCurrents)
                viewmodel.Dispose();

            this.AllDischargeCurrents.Clear();
            //this.AllSubProgramModels.CollectionChanged -= this.OnCollectionChanged;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        #endregion // Event Handling Methods
    }
}
