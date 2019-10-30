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
    public class AllDischargeTemperaturesViewModel : BindBase
    {
        #region Fields
        List<DischargeTemperatureClass> _dischargeTemperatures;
        DischargeTemperatureViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllDischargeTemperaturesViewModel(List<DischargeTemperatureClass> dischargeTemperatures)
        {
            this.CreateAllDischargeTemperatures(dischargeTemperatures);
        }

        void CreateAllDischargeTemperatures(List<DischargeTemperatureClass> dischargeTemperatures)
        {
            _dischargeTemperatures = dischargeTemperatures;
            List<DischargeTemperatureViewModel> all =
                (from ct in dischargeTemperatures
                 select new DischargeTemperatureViewModel(ct)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllDischargeTemperatures = new ObservableCollection<DischargeTemperatureViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the SubProgramModelViewModel objects.
        /// </summary>
        public ObservableCollection<DischargeTemperatureViewModel> AllDischargeTemperatures { get; private set; }

        public DischargeTemperatureViewModel SelectedItem    //绑定选中项，从而改变subprograms
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
            DischargeTemperatureClass model = new DischargeTemperatureClass();      //实例化一个新的model
            DischargeTemperatureEditViewModel viewmodel = new DischargeTemperatureEditViewModel(model);      //实例化一个新的view model
            viewmodel.DisplayName = "DischargeTemperature-Create";
            viewmodel.commandType = CommandType.Create;
            var DischargeTemperatureEditViewInstance = new DischargeTemperatureEditView();      //实例化一个新的view
            DischargeTemperatureEditViewInstance.DataContext = viewmodel;
            DischargeTemperatureEditViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.DischargeTemperatures.Add(model);
                    dbContext.SaveChanges();
                }
                _dischargeTemperatures.Add(model);
                this.AllDischargeTemperatures.Add(new DischargeTemperatureViewModel(model));
            }
        }
        private void Edit()
        {
            DischargeTemperatureClass model = new DischargeTemperatureClass();      //实例化一个新的model
            DischargeTemperatureEditViewModel viewmodel = new DischargeTemperatureEditViewModel(model);      //实例化一个新的view model
            viewmodel.Name = _selectedItem.Name;
            viewmodel.DisplayName = "DischargeTemperature-Edit";
            viewmodel.commandType = CommandType.Edit;
            var DischargeTemperatureEditViewInstance = new DischargeTemperatureEditView();      //实例化一个新的view
            DischargeTemperatureEditViewInstance.DataContext = viewmodel;
            DischargeTemperatureEditViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _selectedItem.Name = viewmodel.Name;
                using (var dbContext = new AppDbContext())
                {
                    var ct = dbContext.DischargeTemperatures.SingleOrDefault(i => i.Id == _selectedItem.Id);
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
            DischargeTemperatureClass model = new DischargeTemperatureClass();      //实例化一个新的model
            DischargeTemperatureEditViewModel viewmodel = new DischargeTemperatureEditViewModel(model);      //实例化一个新的view model
            viewmodel.Name = _selectedItem.Name;
            viewmodel.DisplayName = "DischargeTemperature-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var DischargeTemperatureEditViewInstance = new DischargeTemperatureEditView();      //实例化一个新的view
            DischargeTemperatureEditViewInstance.DataContext = viewmodel;
            DischargeTemperatureEditViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.DischargeTemperatures.Add(model);
                    dbContext.SaveChanges();
                }
                _dischargeTemperatures.Add(model);
                this.AllDischargeTemperatures.Add(new DischargeTemperatureViewModel(model));
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
