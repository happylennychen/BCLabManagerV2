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
    public class AllChargeTemperaturesViewModel : ViewModelBase
    {
        #region Fields
        List<ChargeTemperatureClass> _chargeTemperatures;
        ChargeTemperatureViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllChargeTemperaturesViewModel(List<ChargeTemperatureClass> chargeTemperatures)
        {
            this.CreateAllChargeTemperatures(chargeTemperatures);
        }

        void CreateAllChargeTemperatures(List<ChargeTemperatureClass> chargeTemperatures)
        {
            _chargeTemperatures = chargeTemperatures;
            List<ChargeTemperatureViewModel> all =
                (from ct in chargeTemperatures
                 select new ChargeTemperatureViewModel(ct)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllChargeTemperatures = new ObservableCollection<ChargeTemperatureViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the SubProgramModelViewModel objects.
        /// </summary>
        public ObservableCollection<ChargeTemperatureViewModel> AllChargeTemperatures { get; private set; }

        public ChargeTemperatureViewModel SelectedItem    //绑定选中项，从而改变subprograms
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
            ChargeTemperatureClass model = new ChargeTemperatureClass();      //实例化一个新的model
            ChargeTemperatureEditViewModel viewmodel = new ChargeTemperatureEditViewModel(model);      //实例化一个新的view model
            viewmodel.DisplayName = "ChargeTemperature-Create";
            viewmodel.commandType = CommandType.Create;
            var ChargeTemperatureEditViewInstance = new ChargeTemperatureEditView();      //实例化一个新的view
            ChargeTemperatureEditViewInstance.DataContext = viewmodel;
            ChargeTemperatureEditViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.ChargeTemperatures.Add(model);
                    dbContext.SaveChanges();
                }
                _chargeTemperatures.Add(model);
                this.AllChargeTemperatures.Add(new ChargeTemperatureViewModel(model));
            }
        }
        private void Edit()
        {
            ChargeTemperatureClass model = new ChargeTemperatureClass();      //实例化一个新的model
            ChargeTemperatureEditViewModel viewmodel = new ChargeTemperatureEditViewModel(model);      //实例化一个新的view model
            viewmodel.Name = _selectedItem.Name;
            viewmodel.DisplayName = "ChargeTemperature-Edit";
            viewmodel.commandType = CommandType.Edit;
            var ChargeTemperatureEditViewInstance = new ChargeTemperatureEditView();      //实例化一个新的view
            ChargeTemperatureEditViewInstance.DataContext = viewmodel;
            ChargeTemperatureEditViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _selectedItem.Name = viewmodel.Name;
                using (var dbContext = new AppDbContext())
                {
                    var ct = dbContext.ChargeTemperatures.SingleOrDefault(i => i.Id == _selectedItem.Id);
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
            ChargeTemperatureClass model = new ChargeTemperatureClass();      //实例化一个新的model
            ChargeTemperatureEditViewModel viewmodel = new ChargeTemperatureEditViewModel(model);      //实例化一个新的view model
            viewmodel.Name = _selectedItem.Name;
            viewmodel.DisplayName = "ChargeTemperature-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var ChargeTemperatureEditViewInstance = new ChargeTemperatureEditView();      //实例化一个新的view
            ChargeTemperatureEditViewInstance.DataContext = viewmodel;
            ChargeTemperatureEditViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.ChargeTemperatures.Add(model);
                    dbContext.SaveChanges();
                }
                _chargeTemperatures.Add(model);
                this.AllChargeTemperatures.Add(new ChargeTemperatureViewModel(model));
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
            foreach (ChargeTemperatureViewModel viewmodel in this.AllChargeTemperatures)
                viewmodel.Dispose();

            this.AllChargeTemperatures.Clear();
            //this.AllSubProgramModels.CollectionChanged -= this.OnCollectionChanged;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        #endregion // Event Handling Methods
    }
}
