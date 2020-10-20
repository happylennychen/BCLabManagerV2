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
using System.Windows;

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
        private RecipeTemplateGroupServiceClass _recipeTemplateGroupServcie;
        private StepTemplateServiceClass _stepTemplateService;
        RecipeTemplateViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _abandonCommand;
        RelayCommand _createGroupCommand;
        RelayCommand _manageGroupCommand;

        #endregion // Fields

        #region Constructor

        public AllRecipeTemplatesViewModel(
            RecipeTemplateServiceClass recipeTemplateService, StepTemplateServiceClass stepTemplateService, RecipeTemplateGroupServiceClass recipeTemplateGroupService
            )
        {
            _recipeTemplateServcie = recipeTemplateService;
            _stepTemplateService = stepTemplateService;
            _recipeTemplateGroupServcie = recipeTemplateGroupService;
            this.CreateAllRecipeTemplates(_recipeTemplateServcie.Items);
            this.CreateAllRecipeTemplateGroups(_recipeTemplateGroupServcie.Items);
            _recipeTemplateServcie.Items.CollectionChanged += Items_CollectionChanged;
            _recipeTemplateGroupServcie.Items.CollectionChanged += Groups_CollectionChanged;
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

        private void Groups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var recipeTemplateGroup = item as RecipeTemplateGroup;
                        this.AllGroups.Add(new RecipeTemplateGroupViewModel(recipeTemplateGroup));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var recipeTemplateGroup = item as RecipeTemplateGroup;
                        var deletetarget = this.AllGroups.SingleOrDefault(o => o.Id == recipeTemplateGroup.Id);
                        this.AllGroups.Remove(deletetarget);
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
        void CreateAllRecipeTemplateGroups(ObservableCollection<RecipeTemplateGroup> recipeTemplateGroups)
        {
            List<RecipeTemplateGroupViewModel> all =
                (from subt in recipeTemplateGroups
                 select new RecipeTemplateGroupViewModel(subt)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllGroups = new ObservableCollection<RecipeTemplateGroupViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the RecipeModelViewModel objects.
        /// </summary>
        public ObservableCollection<RecipeTemplateViewModel> AllRecipeTemplates { get; private set; }

        public ObservableCollection<StepV2ViewModel> Steps
        {
            get
            {
                if (_selectedItem != null)
                    return new ObservableCollection<StepV2ViewModel>(_selectedItem.Steps.OrderBy(o=>o.Index));
                else
                    return null;
            }
        }
        public ObservableCollection<ProtectionViewModel> Protections
        {
            get
            {
                if (_selectedItem != null)
                    return _selectedItem.Protections;
                else
                    return null;
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
                    RaisePropertyChanged("Protections");
                }
            }
        }

        public ObservableCollection<RecipeTemplateGroupViewModel> AllGroups { get; private set; }
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
        public ICommand AbandonCommand
        {
            get
            {
                if (_abandonCommand == null)
                {
                    _abandonCommand = new RelayCommand(
                        param => { this.Abandon(); }
                        );
                }
                return _abandonCommand;
            }
        }

        public ICommand CreateGroupCommand
        {
            get
            {
                if (_createGroupCommand == null)
                {
                    _createGroupCommand = new RelayCommand(
                        param => { this.CreateGroup(); }
                        );
                }
                return _createGroupCommand;
            }
        }

        public ICommand ManageGroupCommand
        {
            get
            {
                if (_manageGroupCommand == null)
                {
                    _manageGroupCommand = new RelayCommand(
                        param => { this.ManageGroup(); }
                        );
                }
                return _manageGroupCommand;
            }
        }
        #endregion // Public Interface

        #region Private Helper
        private void Create()
        {
            RecipeTemplate model = new RecipeTemplate();      //实例化一个新的model
            RecipeTemplateEditViewModel viewmodel = 
                new RecipeTemplateEditViewModel(
                    model
                    );      //实例化一个新的view model
            //viewmodel.DisplayName = "RecipeTemplate-Create";
            viewmodel.commandType = CommandType.Create;
            var RecipeViewInstance = new RecipeTemplateView();      //实例化一个新的view
            RecipeViewInstance.DataContext = viewmodel;
            RecipeViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                //foreach (var step in model.Steps)
                //{
                //    int order = 1;
                //    step.Order = order++;
                //}

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
            foreach (var step in _selectedItem._recipeTemplate.StepV2s)
            {
                //var m = new StepV2(step.StepTemplate);
                var newstep = step.Clone();
                model.StepV2s.Add(newstep);
            }
            foreach (var protection in _selectedItem._recipeTemplate.Protections)
            {
                var newprotection = protection.Clone();
                model.Protections.Add(newprotection);
            }
            RecipeTemplateEditViewModel viewmodel =
                new RecipeTemplateEditViewModel(
                    model
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
        private void Abandon()
        {
            
            if (MessageBox.Show("Are you sure?", "Abandon Selected Recipe", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _recipeTemplateServcie.Abandon(SelectedItem._recipeTemplate);
            }
        }
        private void CreateGroup()
        {
            RecipeTemplateGroup model = new RecipeTemplateGroup();      //实例化一个新的model
            RecipeTemplateGroupEditViewModel viewmodel =
                new RecipeTemplateGroupEditViewModel(
                    model
                    );      //实例化一个新的view model
            //viewmodel.DisplayName = "RecipeTemplate-Create";
            viewmodel.commandType = CommandType.Create;
            var RecipeGroupViewInstance = new RecipeTemplateGroupView();      //实例化一个新的view
            RecipeGroupViewInstance.DataContext = viewmodel;
            RecipeGroupViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                //foreach (var step in model.Steps)
                //{
                //    int order = 1;
                //    step.Order = order++;
                //}

                _recipeTemplateGroupServcie.SuperAdd(model);
            }
        }
        private void ManageGroup()
        {

        }
        #endregion //Private Helper

        #region Event Handling Methods

        #endregion // Event Handling Methods
    }
}
