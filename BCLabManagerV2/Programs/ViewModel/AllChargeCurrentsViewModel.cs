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
    public class AllChargeCurrentsViewModel : BindBase
    {
        #region Fields
        List<ChargeCurrentClass> _chargeCurrents;
        ChargeCurrentViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllChargeCurrentsViewModel(List<ChargeCurrentClass> chargeCurrents)
        {
            this.CreateAllChargeCurrents(chargeCurrents);
        }

        void CreateAllChargeCurrents(List<ChargeCurrentClass> chargeCurrents)
        {
            _chargeCurrents = chargeCurrents;
            List<ChargeCurrentViewModel> all =
                (from ct in chargeCurrents
                 select new ChargeCurrentViewModel(ct)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllChargeCurrents = new ObservableCollection<ChargeCurrentViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the SubProgramModelViewModel objects.
        /// </summary>
        public ObservableCollection<ChargeCurrentViewModel> AllChargeCurrents { get; private set; }

        public ChargeCurrentViewModel SelectedItem    //绑定选中项，从而改变subprograms
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
            ChargeCurrentClass model = new ChargeCurrentClass();      //实例化一个新的model
            ChargeCurrentEditViewModel viewmodel = new ChargeCurrentEditViewModel(model);      //实例化一个新的view model
            viewmodel.DisplayName = "ChargeCurrent-Create";
            viewmodel.commandType = CommandType.Create;
            var ChargeCurrentEditViewInstance = new ChargeCurrentEditView();      //实例化一个新的view
            ChargeCurrentEditViewInstance.DataContext = viewmodel;
            ChargeCurrentEditViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.ChargeCurrents.Add(model);
                    dbContext.SaveChanges();
                }
                _chargeCurrents.Add(model);
                this.AllChargeCurrents.Add(new ChargeCurrentViewModel(model));
            }
        }
        private void Edit()
        {
            ChargeCurrentClass model = new ChargeCurrentClass();      //实例化一个新的model
            ChargeCurrentEditViewModel viewmodel = new ChargeCurrentEditViewModel(model);      //实例化一个新的view model
            viewmodel.Name = _selectedItem.Name;
            viewmodel.DisplayName = "ChargeCurrent-Edit";
            viewmodel.commandType = CommandType.Edit;
            var ChargeCurrentEditViewInstance = new ChargeCurrentEditView();      //实例化一个新的view
            ChargeCurrentEditViewInstance.DataContext = viewmodel;
            ChargeCurrentEditViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _selectedItem.Name = viewmodel.Name;
                using (var dbContext = new AppDbContext())
                {
                    var ct = dbContext.ChargeCurrents.SingleOrDefault(i => i.Id == _selectedItem.Id);
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
            ChargeCurrentClass model = new ChargeCurrentClass();      //实例化一个新的model
            ChargeCurrentEditViewModel viewmodel = new ChargeCurrentEditViewModel(model);      //实例化一个新的view model
            viewmodel.Name = _selectedItem.Name;
            viewmodel.DisplayName = "ChargeCurrent-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var ChargeCurrentEditViewInstance = new ChargeCurrentEditView();      //实例化一个新的view
            ChargeCurrentEditViewInstance.DataContext = viewmodel;
            ChargeCurrentEditViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.ChargeCurrents.Add(model);
                    dbContext.SaveChanges();
                }
                _chargeCurrents.Add(model);
                this.AllChargeCurrents.Add(new ChargeCurrentViewModel(model));
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedItem != null; }
        }
        #endregion //Private Helper

        #region Event Handling Methods

        #endregion // Event Handling Methods
    }
}
