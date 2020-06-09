using System;
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
        //List<RecipeTemplate> _RecipeTemplates;
        //List<ChargeTemperatureClass> _chargeTemperatures;
        //List<ChargeCurrentClass> _chargeCurrents;
        //List<DischargeTemperatureClass> _dischargeTemperatures;
        //List<DischargeCurrentClass> _dischargeCurrents;
        private RecipeTemplateServiceClass _recipeTemplateServcie;
        private StepTemplateServiceClass _stepTemplateService;
        RecipeTemplateViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllRecipeTemplatesViewModel(
            RecipeTemplateServiceClass recipeTemplateServcie, StepTemplateServiceClass stepTemplateService
            )
        {
            _recipeTemplateServcie = recipeTemplateServcie;
            _stepTemplateService = stepTemplateService;
            this.CreateAllRecipeTemplates(_recipeTemplateServcie.Items);
            _recipeTemplateServcie.Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var recipeTemplate = item as RecipeTemplate;
                        this.AllRecipeTemplates.Add(new RecipeTemplateViewModel(recipeTemplate));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var recipeTemplate = item as StepTemplate;
                        var deletetarget = this.AllRecipeTemplates.SingleOrDefault(o => o.Id == recipeTemplate.Id);
                        this.AllRecipeTemplates.Remove(deletetarget);
                    }
                    break;
            }
        }

        void CreateAllRecipeTemplates(ObservableCollection<RecipeTemplate> recipeTemplates)
        {
            List<RecipeTemplateViewModel> all =
                (from subt in recipeTemplates
                 select new RecipeTemplateViewModel(subt)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllRecipeTemplates = new ObservableCollection<RecipeTemplateViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the RecipeModelViewModel objects.
        /// </summary>
        public ObservableCollection<RecipeTemplateViewModel> AllRecipeTemplates { get; private set; }

        public ObservableCollection<StepViewModel> Steps
        {
            get 
            {
                if (SelectedItem == null)
                    return null;
                //return SelectedItem._recipeTemplate.Steps;
                var output = new ObservableCollection<StepViewModel>();
                foreach(var step in SelectedItem._recipeTemplate.Steps.OrderBy(o=>o.Order))
                {
                    output.Add(new StepViewModel(step));
                }
                return output;
            }
        }

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
                    RaisePropertyChanged("Steps");
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
                    model,
                    _stepTemplateService.Items
                    );      //实例化一个新的view model
            //viewmodel.DisplayName = "RecipeTemplate-Create";
            viewmodel.commandType = CommandType.Create;
            var RecipeViewInstance = new RecipeTemplateView();      //实例化一个新的view
            RecipeViewInstance.DataContext = viewmodel;
            RecipeViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                foreach (var step in model.Steps)
                {
                    int order = 1;
                    step.Order = order++;
                }

                _recipeTemplateServcie.SuperAdd(model);
            }
        }
        private void Edit()
        {
            //RecipeTemplate model = new RecipeTemplate();      //实例化一个新的model
            //RecipeTemplateEditViewModel viewmodel =
            //    new RecipeTemplateEditViewModel(
            //        model//,
            //        //_chargeTemperatures,
            //        //_chargeCurrents,
            //        //_dischargeTemperatures,
            //        //_dischargeCurrents
            //        );      //实例化一个新的view model
            ////viewmodel.Name = _selectedItem.Name;
            //viewmodel.Id = _selectedItem.Id;
            ////viewmodel.ChargeTemperature = viewmodel.AllChargeTemperatures.SingleOrDefault(o=>o.Id == _selectedItem.ChargeTemperature.Id);
            ////viewmodel.ChargeCurrent = viewmodel.AllChargeCurrents.SingleOrDefault(o => o.Id == _selectedItem.ChargeCurrent.Id);
            ////viewmodel.DischargeTemperature = viewmodel.AllDischargeTemperatures.SingleOrDefault(o=>o.Id == _selectedItem.DischargeTemperature.Id);
            ////viewmodel.DischargeCurrent = viewmodel.AllDischargeCurrents.SingleOrDefault(o=>o.Id == _selectedItem.DischargeCurrent.Id);
            ////viewmodel.DisplayName = "Recipe-Edit";
            //viewmodel.commandType = CommandType.Edit;
            //var RecipeViewInstance = new RecipeTemplateView();      //实例化一个新的view
            //RecipeViewInstance.DataContext = viewmodel;
            //RecipeViewInstance.ShowDialog();
            //if (viewmodel.IsOK == true)
            //{
            //    _recipeTemplateServcie.SuperUpdate(model);
            //}
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            RecipeTemplate model = new RecipeTemplate();      //实例化一个新的model
            model.Name = _selectedItem._recipeTemplate.Name;
            //model.Current = _selectedItem._recipeTemplate.Current;
            //model.Temperature = _selectedItem._recipeTemplate.Temperature;
            foreach (var step in _selectedItem._recipeTemplate.Steps)
            {
                var m = new Step(step.StepTemplate);
                model.Steps.Add(m);
            }
            RecipeTemplateEditViewModel viewmodel =
                new RecipeTemplateEditViewModel(
                    model,
                    _stepTemplateService.Items
                    //_chargeCurrents,
                    //_dischargeTemperatures,
                    //_dischargeCurrents
                    );      //实例化一个新的view model
            //viewmodel.Id = _selectedItem.Id;
            //viewmodel.DisplayName = "Recipe-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var RecipeViewInstance = new RecipeTemplateView();      //实例化一个新的view
            RecipeViewInstance.DataContext = viewmodel;
            RecipeViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _recipeTemplateServcie.SuperAdd(model);
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
