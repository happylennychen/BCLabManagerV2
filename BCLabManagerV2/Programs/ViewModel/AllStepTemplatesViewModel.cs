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
    public class AllStepTemplatesViewModel : BindableBase
    {
        #region Fields
        private StepTemplateServiceClass _stepTemplateServcie;
        StepTemplateViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllStepTemplatesViewModel(
            StepTemplateServiceClass stepTemplateServcie
            )
        {
            _stepTemplateServcie = stepTemplateServcie;
            this.CreateAllStepTemplates(_stepTemplateServcie.Items);
            _stepTemplateServcie.Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var stepTemplate = item as StepTemplate;
                        this.AllStepTemplates.Add(new StepTemplateViewModel(stepTemplate));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var stepTemplate = item as StepTemplate;
                        var deletetarget = this.AllStepTemplates.SingleOrDefault(o => o.Id == stepTemplate.Id);
                        this.AllStepTemplates.Remove(deletetarget);
                    }
                    break;
            }
        }

        void CreateAllStepTemplates(ObservableCollection<StepTemplate> stepTemplates)
        {
            List<StepTemplateViewModel> all =
                (from subt in stepTemplates
                 select new StepTemplateViewModel(subt)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllStepTemplates = new ObservableCollection<StepTemplateViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the StepModelViewModel objects.
        /// </summary>
        public ObservableCollection<StepTemplateViewModel> AllStepTemplates { get; private set; }

        public StepTemplateViewModel SelectedItem    //绑定选中项，从而改变Steps
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
            StepTemplate model = new StepTemplate();      //实例化一个新的model
            StepTemplateEditViewModel viewmodel = 
                new StepTemplateEditViewModel(
                    model//, 
                    //_chargeTemperatures,
                    //_chargeCurrents,
                    //_dischargeTemperatures,
                    //_dischargeCurrents
                    );      //实例化一个新的view model
            //viewmodel.DisplayName = "StepTemplate-Create";
            viewmodel.commandType = CommandType.Create;
            var StepViewInstance = new StepTemplateView();      //实例化一个新的view
            StepViewInstance.DataContext = viewmodel;
            StepViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                _stepTemplateServcie.SuperAdd(model);
            }
        }
        private void Edit()
        {
            StepTemplate model = new StepTemplate();      //实例化一个新的model
            StepTemplateEditViewModel viewmodel =
                new StepTemplateEditViewModel(
                    model//,
                    );      //实例化一个新的view model
            //viewmodel.Name = _selectedItem.Name;
            viewmodel.Id = _selectedItem.Id;
            viewmodel.CurrentInput = _selectedItem.CurrentInput;
            viewmodel.CurrentUnit = _selectedItem.CurrentUnit;
            viewmodel.CutOffConditionValue = _selectedItem.CutOffConditionValue;
            viewmodel.CutOffConditionType = _selectedItem.CutOffConditionType;
            viewmodel.Temperature = _selectedItem.Temperature;
            //viewmodel.DisplayName = "Step-Edit";
            viewmodel.commandType = CommandType.Edit;
            var StepViewInstance = new StepTemplateView();      //实例化一个新的view
            StepViewInstance.DataContext = viewmodel;
            StepViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _stepTemplateServcie.SuperUpdate(model);
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            StepTemplate model = new StepTemplate();      //实例化一个新的model
            StepTemplateEditViewModel viewmodel =
                new StepTemplateEditViewModel(
                    model//,
                    );      //实例化一个新的view model
            //viewmodel.Name = _selectedItem.Name;
            //viewmodel.Id = _selectedItem.Id;
            viewmodel.CurrentInput = _selectedItem.CurrentInput;
            viewmodel.CurrentUnit = _selectedItem.CurrentUnit;
            viewmodel.CutOffConditionValue = _selectedItem.CutOffConditionValue;
            viewmodel.CutOffConditionType = _selectedItem.CutOffConditionType;
            viewmodel.Temperature = _selectedItem.Temperature;
            //viewmodel.DisplayName = "Step-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var StepViewInstance = new StepTemplateView();      //实例化一个新的view
            StepViewInstance.DataContext = viewmodel;
            StepViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _stepTemplateServcie.SuperAdd(model);
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
