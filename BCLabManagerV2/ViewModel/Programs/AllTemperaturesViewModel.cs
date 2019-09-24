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
    public class AllTemperaturesViewModel : ViewModelBase
    {
        #region Fields
        List<TemperatureClass> _chargeTemperatures;
        TemperatureViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllTemperaturesViewModel(List<TemperatureClass> chargeTemperatures)
        {
            this.CreateAllChargeTemperatures(chargeTemperatures);
        }

        void CreateAllChargeTemperatures(List<TemperatureClass> chargeTemperatures)
        {
            _chargeTemperatures = chargeTemperatures;
            List<TemperatureViewModel> all =
                (from ct in chargeTemperatures
                 select new TemperatureViewModel(ct)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllChargeTemperatures = new ObservableCollection<TemperatureViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the SubProgramModelViewModel objects.
        /// </summary>
        public ObservableCollection<TemperatureViewModel> AllChargeTemperatures { get; private set; }

        public TemperatureViewModel SelectedItem    //绑定选中项，从而改变subprograms
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
            TemperatureClass model = new TemperatureClass();      //实例化一个新的model
            TemperatureEditViewModel viewmodel = new TemperatureEditViewModel(model);      //实例化一个新的view model
            viewmodel.DisplayName = "ChargeTemperature-Create";
            viewmodel.commandType = CommandType.Create;
            var TemperatureEditViewInstance = new TemperatureEditView();      //实例化一个新的view
            TemperatureEditViewInstance.DataContext = viewmodel;
            TemperatureEditViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Temperatures.Add(model);
                    dbContext.SaveChanges();
                }
                _chargeTemperatures.Add(model);
                this.AllChargeTemperatures.Add(new TemperatureViewModel(model));
            }
        }
        private void Edit()
        {
            TemperatureClass model = new TemperatureClass();      //实例化一个新的model
            TemperatureEditViewModel viewmodel = new TemperatureEditViewModel(model);      //实例化一个新的view model
            viewmodel.Value = _selectedItem.Value;
            viewmodel.DisplayName = "ChargeTemperature-Edit";
            viewmodel.commandType = CommandType.Edit;
            var ChargeTemperatureEditViewInstance = new TemperatureEditView();      //实例化一个新的view
            ChargeTemperatureEditViewInstance.DataContext = viewmodel;
            ChargeTemperatureEditViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _selectedItem.Value = viewmodel.Value;
                using (var dbContext = new AppDbContext())
                {
                    var ct = dbContext.Temperatures.SingleOrDefault(i => i.Id == _selectedItem.Id);
                    ct.Value = _selectedItem.Value;
                    dbContext.SaveChanges();
                }
            }
        }
        private bool CanEdit
        {
            get { return (_selectedItem != null && _selectedItem.Value!=-9999); }
        }
        private void SaveAs()
        {
            TemperatureClass model = new TemperatureClass();      //实例化一个新的model
            TemperatureEditViewModel viewmodel = new TemperatureEditViewModel(model);      //实例化一个新的view model
            viewmodel.Value = _selectedItem.Value;
            viewmodel.DisplayName = "ChargeTemperature-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var ChargeTemperatureEditViewInstance = new TemperatureEditView();      //实例化一个新的view
            ChargeTemperatureEditViewInstance.DataContext = viewmodel;
            ChargeTemperatureEditViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Temperatures.Add(model);
                    dbContext.SaveChanges();
                }
                _chargeTemperatures.Add(model);
                this.AllChargeTemperatures.Add(new TemperatureViewModel(model));
            }
        }
        private bool CanSaveAs
        {
            get { return (_selectedItem != null && _selectedItem.Value != -9999); }
        }
        #endregion //Private Helper
        #region  Base Class Overrides

        protected override void OnDispose()
        {
            foreach (TemperatureViewModel viewmodel in this.AllChargeTemperatures)
                viewmodel.Dispose();

            this.AllChargeTemperatures.Clear();
            //this.AllSubProgramModels.CollectionChanged -= this.OnCollectionChanged;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        #endregion // Event Handling Methods
    }
}
