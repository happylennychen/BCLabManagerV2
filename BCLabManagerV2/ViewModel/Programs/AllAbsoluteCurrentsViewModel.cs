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
    public class AllAbsoluteCurrentsViewModel : ViewModelBase
    {
        #region Fields
        List<AbsoluteCurrentClass> _dischargeTemperatures;
        AbsoluteCurrentViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllAbsoluteCurrentsViewModel(List<AbsoluteCurrentClass> dischargeTemperatures)
        {
            this.CreateAllDischargeTemperatures(dischargeTemperatures);
        }

        void CreateAllDischargeTemperatures(List<AbsoluteCurrentClass> dischargeTemperatures)
        {
            _dischargeTemperatures = dischargeTemperatures;
            List<AbsoluteCurrentViewModel> all =
                (from ct in dischargeTemperatures
                 select new AbsoluteCurrentViewModel(ct)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllDischargeTemperatures = new ObservableCollection<AbsoluteCurrentViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the SubProgramModelViewModel objects.
        /// </summary>
        public ObservableCollection<AbsoluteCurrentViewModel> AllDischargeTemperatures { get; private set; }

        public AbsoluteCurrentViewModel SelectedItem    //绑定选中项，从而改变subprograms
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
            AbsoluteCurrentClass model = new AbsoluteCurrentClass();      //实例化一个新的model
            AbsoluteCurrentEditViewModel viewmodel = new AbsoluteCurrentEditViewModel(model);      //实例化一个新的view model
            viewmodel.DisplayName = "DischargeTemperature-Create";
            viewmodel.commandType = CommandType.Create;
            var DischargeTemperatureEditViewInstance = new DischargeTemperatureEditView();      //实例化一个新的view
            DischargeTemperatureEditViewInstance.DataContext = viewmodel;
            DischargeTemperatureEditViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.AbsoluteCurrents.Add(model);
                    dbContext.SaveChanges();
                }
                _dischargeTemperatures.Add(model);
                this.AllDischargeTemperatures.Add(new AbsoluteCurrentViewModel(model));
            }
        }
        private void Edit()
        {
            AbsoluteCurrentClass model = new AbsoluteCurrentClass();      //实例化一个新的model
            AbsoluteCurrentEditViewModel viewmodel = new AbsoluteCurrentEditViewModel(model);      //实例化一个新的view model
            viewmodel.Value = _selectedItem.Value;
            viewmodel.DisplayName = "DischargeTemperature-Edit";
            viewmodel.commandType = CommandType.Edit;
            var DischargeTemperatureEditViewInstance = new DischargeTemperatureEditView();      //实例化一个新的view
            DischargeTemperatureEditViewInstance.DataContext = viewmodel;
            DischargeTemperatureEditViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _selectedItem.Value = viewmodel.Value;
                using (var dbContext = new AppDbContext())
                {
                    var ct = dbContext.AbsoluteCurrents.SingleOrDefault(i => i.Id == _selectedItem.Id);
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
            AbsoluteCurrentClass model = new AbsoluteCurrentClass();      //实例化一个新的model
            AbsoluteCurrentEditViewModel viewmodel = new AbsoluteCurrentEditViewModel(model);      //实例化一个新的view model
            viewmodel.Value = _selectedItem.Value;
            viewmodel.DisplayName = "DischargeTemperature-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var DischargeTemperatureEditViewInstance = new DischargeTemperatureEditView();      //实例化一个新的view
            DischargeTemperatureEditViewInstance.DataContext = viewmodel;
            DischargeTemperatureEditViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.AbsoluteCurrents.Add(model);
                    dbContext.SaveChanges();
                }
                _dischargeTemperatures.Add(model);
                this.AllDischargeTemperatures.Add(new AbsoluteCurrentViewModel(model));
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
            foreach (AbsoluteCurrentViewModel viewmodel in this.AllDischargeTemperatures)
                viewmodel.Dispose();

            this.AllDischargeTemperatures.Clear();
            //this.AllSubProgramModels.CollectionChanged -= this.OnCollectionChanged;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        #endregion // Event Handling Methods
    }
}
