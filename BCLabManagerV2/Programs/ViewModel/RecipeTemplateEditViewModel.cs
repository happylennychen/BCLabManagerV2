using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class RecipeTemplateEditViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        public readonly RecipeTemplate _RecipeTemplate;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。
        StepTemplateViewModel _selectedStepTemplate;
        StepViewModel _selectedStep;
        RelayCommand _okCommand;
        RelayCommand _addCommand;
        RelayCommand _removeCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public RecipeTemplateEditViewModel(
            RecipeTemplate RecipeTemplateModel,
            ObservableCollection<StepTemplate> stepTemplates
            )
        {
            _RecipeTemplate = RecipeTemplateModel;
            this.CreateStepTemplates(stepTemplates);
            this.CreateSteps();
        }


        void CreateStepTemplates(ObservableCollection<StepTemplate> stepTemplates)
        {
            List<StepTemplateViewModel> all =
                (from sub in stepTemplates
                 select new StepTemplateViewModel(sub)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.StepTemplates = new ObservableCollection<StepTemplateViewModel>(all);     //再转换成Observable
        }
        void CreateSteps()
        {
            List<StepViewModel> all =
                (from step in _RecipeTemplate.Steps
                 select new StepViewModel(step)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //foreach (RecipeModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnRecipeModelViewModelPropertyChanged;

            this.Steps = new ObservableCollection<StepViewModel>(all);     //再转换成Observable
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }
        #endregion // Constructor

        #region RecipeTemplate Properties

        public int Id
        {
            get { return _RecipeTemplate.Id; }
            set
            {
                if (value == _RecipeTemplate.Id)
                    return;

                _RecipeTemplate.Id = value;

                RaisePropertyChanged("Id");
            }
        }
        public string Name
        {
            get { return _RecipeTemplate.Name; }
            set
            {
                if (value == _RecipeTemplate.Name)
                    return;

                _RecipeTemplate.Name = value;

                RaisePropertyChanged("Name");
            }
        }
        public double Current
        {
            get { return _RecipeTemplate.Current; }
            set
            {
                if (value == _RecipeTemplate.Current)
                    return;

                _RecipeTemplate.Current = value;

                RaisePropertyChanged("Current");
            }
        }
        public double Temperature
        {
            get { return _RecipeTemplate.Temperature; }
            set
            {
                if (value == _RecipeTemplate.Temperature)
                    return;

                _RecipeTemplate.Temperature = value;

                RaisePropertyChanged("Temperature");
            }
        }
        public ObservableCollection<StepTemplateViewModel> StepTemplates { get; set; }

        public ObservableCollection<StepViewModel> Steps { get; set; }


        public StepTemplateViewModel SelectedStepTemplate
        {
            get
            {
                return _selectedStepTemplate;
            }
            set
            {
                if (_selectedStepTemplate != value)
                {
                    _selectedStepTemplate = value;
                }
            }
        }

        public StepViewModel SelectedStep
        {
            get
            {
                return _selectedStep;
            }
            set
            {
                if (_selectedStep != value)
                {
                    _selectedStep = value;
                }
            }
        }
        #endregion // Customer Properties

        #region Presentation Properties

        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    switch (commandType)
                    {
                        case CommandType.Create:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); },
                                param => this.CanCreate
                                );
                            break;
                        case CommandType.Edit:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); }
                                );
                            break;
                        case CommandType.SaveAs:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); },
                                param => this.CanSaveAs
                                );
                            break;
                    }
                }
                return _okCommand;
            }
        }

        public CommandType commandType
        { get; set; }

        public bool IsOK
        {
            get { return _isOK; }
            set { _isOK = value; }
        }

        public ICommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                {
                    _addCommand = new RelayCommand(
                        param => { this.Add(); },
                        param => this.CanAdd
                            );
                }
                return _addCommand;
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new RelayCommand(
                        param => { this.Remove(); },
                        param => this.CanRemove
                            );
                }
                return _removeCommand;
            }
        }

        #endregion // Presentation Properties

        #region Public Methods

        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        public void OK()
        {
            int i = 0;
            foreach (var step in _RecipeTemplate.Steps) //创建时设定Order
            {
                step.Order = i;
                i++;
            }
            IsOK = true;
        }

        public void Add()       //对于model来说，需要将选中的sub copy到_program.Recipes来。对于viewmodel来说，需要将这个copy出来的sub，包装成viewmodel并添加到this.Recipes里面去
        {
            var m = new StepClass(SelectedStepTemplate._stepTemplate);
            var vm = new StepViewModel(m);
            _RecipeTemplate.Steps.Add(m);
            this.Steps.Add(vm);
        }

        public void Remove()       //对于model来说，需要将选中的sub 从_program.Recipes中移除。对于viewmodel来说，需要将这个viewmodel从this.Recipes中移除
        {
            _RecipeTemplate.Steps.Remove(SelectedStep._step);
            this.Steps.Remove(SelectedStep);
        }

        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewRecipeTemplate
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanCreate
        {
            get { return IsNewRecipeTemplate; }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSaveAs
        {
            get { return IsNewRecipeTemplate; }
        }

        bool CanAdd
        {
            get { return SelectedStepTemplate != null; }
        }

        bool CanRemove
        {
            get { return SelectedStep != null; }     //如果已经有数据，可否删除？
        }

        #endregion // Private Helpers
    }
}