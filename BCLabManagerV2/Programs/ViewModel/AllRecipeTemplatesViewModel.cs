﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    public class AllRecipeTemplatesViewModel : BindableBase
    {
        #region Fields
        List<RecipeTemplate> _RecipeTemplates;
        //List<ChargeTemperatureClass> _chargeTemperatures;
        //List<ChargeCurrentClass> _chargeCurrents;
        //List<DischargeTemperatureClass> _dischargeTemperatures;
        //List<DischargeCurrentClass> _dischargeCurrents;
        RecipeTemplateViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllRecipeTemplatesViewModel(
            List<RecipeTemplate> RecipeTemplates//,
            //List<ChargeTemperatureClass> chargeTemperatures,
            //List<ChargeCurrentClass> chargeCurrents,
            //List<DischargeTemperatureClass> dischargeTemperatures,
            //List<DischargeCurrentClass> dischargeCurrents
            )
        {
            //_chargeTemperatures = chargeTemperatures;
            //_chargeCurrents = chargeCurrents;
            //_dischargeTemperatures = dischargeTemperatures;
            //_dischargeCurrents = dischargeCurrents;
            this.CreateAllRecipeTemplates(RecipeTemplates);
        }

        void CreateAllRecipeTemplates(List<RecipeTemplate> RecipeTemplates)
        {
            _RecipeTemplates = RecipeTemplates;
            List<RecipeTemplateViewModel> all =
                (from subt in RecipeTemplates
                 select new RecipeTemplateViewModel(subt)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllRecipeTemplates = new ObservableCollection<RecipeTemplateViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the RecipeModelViewModel objects.
        /// </summary>
        public ObservableCollection<RecipeTemplateViewModel> AllRecipeTemplates { get; private set; }

        public RecipeTemplateViewModel SelectedItem    //绑定选中项，从而改变Recipes
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
                    //RaisePropertyChanged("SelectedType");
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
            RecipeTemplate model = new RecipeTemplate();      //实例化一个新的model
            RecipeTemplateEditViewModel viewmodel = 
                new RecipeTemplateEditViewModel(
                    model//, 
                    //_chargeTemperatures,
                    //_chargeCurrents,
                    //_dischargeTemperatures,
                    //_dischargeCurrents
                    );      //实例化一个新的view model
            //viewmodel.DisplayName = "RecipeTemplate-Create";
            viewmodel.commandType = CommandType.Create;
            var RecipeViewInstance = new RecipeTemplateView();      //实例化一个新的view
            RecipeViewInstance.DataContext = viewmodel;
            RecipeViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                //RecipeTemplate newsub;
                using (var dbContext = new AppDbContext())
                {
                    //dbContext.RecipeTemplates.Add(model);
                    var newsub = new RecipeTemplate()
                    {
                        //ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Id == model.ChargeTemperature.Id),
                        //ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Id == model.ChargeCurrent.Id),
                        //DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Id == model.DischargeTemperature.Id),
                        //DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Id == model.DischargeCurrent.Id),
                    };
                    dbContext.RecipeTemplates.Add(newsub);
                    dbContext.SaveChanges();
                    model = newsub;
                }
                _RecipeTemplates.Add(model);
                this.AllRecipeTemplates.Add(new RecipeTemplateViewModel(model));
            }
        }
        private void Edit()
        {
            RecipeTemplate model = new RecipeTemplate();      //实例化一个新的model
            RecipeTemplateEditViewModel viewmodel =
                new RecipeTemplateEditViewModel(
                    model//,
                    //_chargeTemperatures,
                    //_chargeCurrents,
                    //_dischargeTemperatures,
                    //_dischargeCurrents
                    );      //实例化一个新的view model
            //viewmodel.Name = _selectedItem.Name;
            viewmodel.Id = _selectedItem.Id;
            //viewmodel.ChargeTemperature = viewmodel.AllChargeTemperatures.SingleOrDefault(o=>o.Id == _selectedItem.ChargeTemperature.Id);
            //viewmodel.ChargeCurrent = viewmodel.AllChargeCurrents.SingleOrDefault(o => o.Id == _selectedItem.ChargeCurrent.Id);
            //viewmodel.DischargeTemperature = viewmodel.AllDischargeTemperatures.SingleOrDefault(o=>o.Id == _selectedItem.DischargeTemperature.Id);
            //viewmodel.DischargeCurrent = viewmodel.AllDischargeCurrents.SingleOrDefault(o=>o.Id == _selectedItem.DischargeCurrent.Id);
            //viewmodel.DisplayName = "Recipe-Edit";
            viewmodel.commandType = CommandType.Edit;
            var RecipeViewInstance = new RecipeTemplateView();      //实例化一个新的view
            RecipeViewInstance.DataContext = viewmodel;
            RecipeViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                //_selectedItem.Name = viewmodel.Name;
                //_selectedItem.ChargeTemperature = viewmodel.ChargeTemperature;
                //_selectedItem.ChargeCurrent = viewmodel.ChargeCurrent;
                //_selectedItem.DischargeTemperature = viewmodel.DischargeTemperature;
                //_selectedItem.DischargeCurrent = viewmodel.DischargeCurrent;
                using (var dbContext = new AppDbContext())
                {
                    var sub = dbContext.RecipeTemplates.SingleOrDefault(i => i.Id == _selectedItem.Id);
                    //sub.Name = _selectedItem.Name;
                    //sub.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Id == model.ChargeTemperature.Id);
                    //sub.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Id == model.ChargeCurrent.Id);
                    //sub.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Id == model.DischargeTemperature.Id);
                    //sub.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Id == model.DischargeCurrent.Id);
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
            RecipeTemplate model = new RecipeTemplate();      //实例化一个新的model
            RecipeTemplateEditViewModel viewmodel =
                new RecipeTemplateEditViewModel(
                    model//,
                    //_chargeTemperatures,
                    //_chargeCurrents,
                    //_dischargeTemperatures,
                    //_dischargeCurrents
                    );      //实例化一个新的view model
            //viewmodel.Name = _selectedItem.Name;
            viewmodel.Id = _selectedItem.Id;
            //viewmodel.ChargeTemperature = viewmodel.AllChargeTemperatures.SingleOrDefault(o => o.Id == _selectedItem.ChargeTemperature.Id);
            //viewmodel.ChargeCurrent = viewmodel.AllChargeCurrents.SingleOrDefault(o => o.Id == _selectedItem.ChargeCurrent.Id);
            //viewmodel.DischargeTemperature = viewmodel.AllDischargeTemperatures.SingleOrDefault(o => o.Id == _selectedItem.DischargeTemperature.Id);
            //viewmodel.DischargeCurrent = viewmodel.AllDischargeCurrents.SingleOrDefault(o => o.Id == _selectedItem.DischargeCurrent.Id);
            //viewmodel.DisplayName = "Recipe-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var RecipeViewInstance = new RecipeTemplateView();      //实例化一个新的view
            RecipeViewInstance.DataContext = viewmodel;
            RecipeViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                //RecipeTemplate newsub;
                using (var dbContext = new AppDbContext())
                {
                    //dbContext.RecipeTemplates.Add(model);
                    var newsub = new RecipeTemplate()
                    {
                        //ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Id == model.ChargeTemperature.Id),
                        //ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Id == model.ChargeCurrent.Id),
                        //DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Id == model.DischargeTemperature.Id),
                        //DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Id == model.DischargeCurrent.Id)
                    };
                    dbContext.RecipeTemplates.Add(newsub);
                    dbContext.SaveChanges();
                    model = newsub;
                }
                _RecipeTemplates.Add(model);
                this.AllRecipeTemplates.Add(new RecipeTemplateViewModel(model));
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
