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
    public class AllSubProgramTemplatesViewModel : BindBase
    {
        #region Fields
        List<SubProgramTemplate> _subProgramTemplates;
        List<ChargeTemperatureClass> _chargeTemperatures;
        List<ChargeCurrentClass> _chargeCurrents;
        List<DischargeTemperatureClass> _dischargeTemperatures;
        List<DischargeCurrentClass> _dischargeCurrents;
        SubProgramTemplateViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllSubProgramTemplatesViewModel(
            List<SubProgramTemplate> subProgramTemplates,
            List<ChargeTemperatureClass> chargeTemperatures,
            List<ChargeCurrentClass> chargeCurrents,
            List<DischargeTemperatureClass> dischargeTemperatures,
            List<DischargeCurrentClass> dischargeCurrents
            )
        {
            _chargeTemperatures = chargeTemperatures;
            _chargeCurrents = chargeCurrents;
            _dischargeTemperatures = dischargeTemperatures;
            _dischargeCurrents = dischargeCurrents;
            this.CreateAllSubProgramTemplates(subProgramTemplates);
        }

        void CreateAllSubProgramTemplates(List<SubProgramTemplate> subProgramTemplates)
        {
            _subProgramTemplates = subProgramTemplates;
            List<SubProgramTemplateViewModel> all =
                (from subt in subProgramTemplates
                 select new SubProgramTemplateViewModel(subt)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllSubProgramTemplates = new ObservableCollection<SubProgramTemplateViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the SubProgramModelViewModel objects.
        /// </summary>
        public ObservableCollection<SubProgramTemplateViewModel> AllSubProgramTemplates { get; private set; }

        public SubProgramTemplateViewModel SelectedItem    //绑定选中项，从而改变subprograms
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
            SubProgramTemplate model = new SubProgramTemplate();      //实例化一个新的model
            SubProgramTemplateEditViewModel viewmodel = 
                new SubProgramTemplateEditViewModel(
                    model, 
                    _chargeTemperatures,
                    _chargeCurrents,
                    _dischargeTemperatures,
                    _dischargeCurrents
                    );      //实例化一个新的view model
            viewmodel.DisplayName = "SubProgramTemplate-Create";
            viewmodel.commandType = CommandType.Create;
            var SubProgramViewInstance = new SubProgramTemplateView();      //实例化一个新的view
            SubProgramViewInstance.DataContext = viewmodel;
            SubProgramViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                //SubProgramTemplate newsub;
                using (var dbContext = new AppDbContext())
                {
                    //dbContext.SubProgramTemplates.Add(model);
                    var newsub = new SubProgramTemplate()
                    {
                        ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Id == model.ChargeTemperature.Id),
                        ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Id == model.ChargeCurrent.Id),
                        DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Id == model.DischargeTemperature.Id),
                        DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Id == model.DischargeCurrent.Id),
                        TestCount = model.TestCount
                    };
                    dbContext.SubProgramTemplates.Add(newsub);
                    dbContext.SaveChanges();
                    model = newsub;
                }
                _subProgramTemplates.Add(model);
                this.AllSubProgramTemplates.Add(new SubProgramTemplateViewModel(model));
            }
        }
        private void Edit()
        {
            SubProgramTemplate model = new SubProgramTemplate();      //实例化一个新的model
            SubProgramTemplateEditViewModel viewmodel =
                new SubProgramTemplateEditViewModel(
                    model,
                    _chargeTemperatures,
                    _chargeCurrents,
                    _dischargeTemperatures,
                    _dischargeCurrents
                    );      //实例化一个新的view model
            //viewmodel.Name = _selectedItem.Name;
            viewmodel.Id = _selectedItem.Id;
            viewmodel.ChargeTemperature = viewmodel.AllChargeTemperatures.SingleOrDefault(o=>o.Id == _selectedItem.ChargeTemperature.Id);
            viewmodel.ChargeCurrent = viewmodel.AllChargeCurrents.SingleOrDefault(o => o.Id == _selectedItem.ChargeCurrent.Id);
            viewmodel.DischargeTemperature = viewmodel.AllDischargeTemperatures.SingleOrDefault(o=>o.Id == _selectedItem.DischargeTemperature.Id);
            viewmodel.DischargeCurrent = viewmodel.AllDischargeCurrents.SingleOrDefault(o=>o.Id == _selectedItem.DischargeCurrent.Id);
            viewmodel.TestCount = _selectedItem.TestCount;
            viewmodel.DisplayName = "SubProgram-Edit";
            viewmodel.commandType = CommandType.Edit;
            var SubProgramViewInstance = new SubProgramTemplateView();      //实例化一个新的view
            SubProgramViewInstance.DataContext = viewmodel;
            SubProgramViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                //_selectedItem.Name = viewmodel.Name;
                _selectedItem.ChargeTemperature = viewmodel.ChargeTemperature;
                _selectedItem.ChargeCurrent = viewmodel.ChargeCurrent;
                _selectedItem.DischargeTemperature = viewmodel.DischargeTemperature;
                _selectedItem.DischargeCurrent = viewmodel.DischargeCurrent;
                _selectedItem.TestCount = viewmodel.TestCount;
                using (var dbContext = new AppDbContext())
                {
                    var sub = dbContext.SubProgramTemplates.SingleOrDefault(i => i.Id == _selectedItem.Id);
                    //sub.Name = _selectedItem.Name;
                    sub.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Id == model.ChargeTemperature.Id);
                    sub.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Id == model.ChargeCurrent.Id);
                    sub.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Id == model.DischargeTemperature.Id);
                    sub.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Id == model.DischargeCurrent.Id);
                    sub.TestCount = _selectedItem.TestCount;
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
            SubProgramTemplate model = new SubProgramTemplate();      //实例化一个新的model
            SubProgramTemplateEditViewModel viewmodel =
                new SubProgramTemplateEditViewModel(
                    model,
                    _chargeTemperatures,
                    _chargeCurrents,
                    _dischargeTemperatures,
                    _dischargeCurrents
                    );      //实例化一个新的view model
            //viewmodel.Name = _selectedItem.Name;
            viewmodel.Id = _selectedItem.Id;
            viewmodel.ChargeTemperature = viewmodel.AllChargeTemperatures.SingleOrDefault(o => o.Id == _selectedItem.ChargeTemperature.Id);
            viewmodel.ChargeCurrent = viewmodel.AllChargeCurrents.SingleOrDefault(o => o.Id == _selectedItem.ChargeCurrent.Id);
            viewmodel.DischargeTemperature = viewmodel.AllDischargeTemperatures.SingleOrDefault(o => o.Id == _selectedItem.DischargeTemperature.Id);
            viewmodel.DischargeCurrent = viewmodel.AllDischargeCurrents.SingleOrDefault(o => o.Id == _selectedItem.DischargeCurrent.Id);
            viewmodel.TestCount = _selectedItem.TestCount;
            viewmodel.DisplayName = "SubProgram-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var SubProgramViewInstance = new SubProgramTemplateView();      //实例化一个新的view
            SubProgramViewInstance.DataContext = viewmodel;
            SubProgramViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                //SubProgramTemplate newsub;
                using (var dbContext = new AppDbContext())
                {
                    //dbContext.SubProgramTemplates.Add(model);
                    var newsub = new SubProgramTemplate()
                    {
                        ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Id == model.ChargeTemperature.Id),
                        ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Id == model.ChargeCurrent.Id),
                        DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Id == model.DischargeTemperature.Id),
                        DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Id == model.DischargeCurrent.Id),
                        TestCount = model.TestCount
                    };
                    dbContext.SubProgramTemplates.Add(newsub);
                    dbContext.SaveChanges();
                    model = newsub;
                }
                _subProgramTemplates.Add(model);
                this.AllSubProgramTemplates.Add(new SubProgramTemplateViewModel(model));
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
