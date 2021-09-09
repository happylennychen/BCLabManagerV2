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
using BCLabManager.View;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class RecipeTemplateEditViewModel : BindableBaseWithName, IDataErrorInfo
    {
        #region Fields
        public readonly RecipeTemplate _RecipeTemplate;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。
        StepV2 _selectedStep;
        CutOffBehaviorViewModel _selectedCOB;
        RelayCommand _okCommand;
        RelayCommand _pasteStepCommand;
        RelayCommand _copyStepCommand;
        bool _isOK;
        StepV2 stepBuffer;
        #endregion // Fields

        #region Constructor

        public RecipeTemplateEditViewModel(
            RecipeTemplate RecipeTemplateModel
            )
        {
            _RecipeTemplate = RecipeTemplateModel;
            _RecipeTemplate.StepV2s.CollectionChanged += StepV2s_CollectionChanged;
        }

        private void StepV2s_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in Steps)
                    {
                        var step = item as StepV2;
                        step.Index = Steps.IndexOf(step) + 1;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < Steps.Count; i++)
                    {
                        Steps[i].Index = i + 1;
                    }
                    break;
            }
        }


        //void CreateStepTemplates(ObservableCollection<StepTemplate> stepTemplates)
        //{
        //    List<StepTemplateViewModel> all =
        //        (from sub in stepTemplates
        //         select new StepTemplateViewModel(sub)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

        //    this.StepTemplates = new ObservableCollection<StepTemplateViewModel>(all);     //再转换成Observable
        //}
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

        public ObservableCollection<StepV2> Steps
        {
            get { return _RecipeTemplate.StepV2s; }
            set
            {
                if (value == _RecipeTemplate.StepV2s)
                    return;

                _RecipeTemplate.StepV2s = value;

                RaisePropertyChanged("Steps");
            }
        }

        public StepV2 SelectedStep
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

        //public CutOffBehaviorViewModel SelectedCOB
        //{
        //    get
        //    {
        //        return _selectedCOB;
        //    }
        //    set
        //    {
        //        if (_selectedCOB != value)
        //        {
        //            _selectedCOB = value;
        //        }
        //    }
        //}

        //public ObservableCollection<Protection> Protections
        //{
        //    get { return _RecipeTemplate.Protections; }
        //    set
        //    {
        //        if (value == _RecipeTemplate.Protections)
        //            return;

        //        _RecipeTemplate.Protections = value;

        //        RaisePropertyChanged("Protections");
        //    }
        //}
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

        public ICommand PasteStepCommand
        {
            get
            {
                if (_pasteStepCommand == null)
                {
                    _pasteStepCommand = new RelayCommand(
                        param => { this.PasteStep(); },
                        param => this.CanPasteStep
                            );
                }
                return _pasteStepCommand;
            }
        }

        public ICommand CopyStepCommand
        {
            get
            {
                if (_copyStepCommand == null)
                {
                    _copyStepCommand = new RelayCommand(
                        param => { this.CopyStep(); },
                        param => this.CanCopyStep
                            );
                }
                return _copyStepCommand;
            }
        }

        #endregion // Presentation Properties

        #region Public Methods

        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        public void OK()
        {
            IsOK = true;
        }

        public void CopyStep()       //对于model来说，需要将选中的sub 从_program.Recipes中移除。对于viewmodel来说，需要将这个viewmodel从this.Recipes中移除
        {
            stepBuffer = SelectedStep;
        }

        public void PasteStep()       //对于model来说，需要将选中的sub copy到_program.Recipes来。对于viewmodel来说，需要将这个copy出来的sub，包装成viewmodel并添加到this.Recipes里面去
        {
            var step = stepBuffer.Clone();
            //step.Index = Steps.Count + 1;
            //Steps.Add(step);
            Steps.Insert(Steps.IndexOf(SelectedStep), step);
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

        bool CanPasteStep
        {
            get { return stepBuffer != null; }
            //get { return true; }
        }

        bool CanCopyStep
        {
            get { return SelectedStep != null; }
            //get { return true; }
        }

        public string Error
        {
            get
            {
                return "!";
            }
        }

        public string this[string columnName]
        {
            get 
            { 
                if(columnName == "Name")
                {
                    if(Name!=null && Name.Contains('_'))
                    {
                        return "Do not use \"_\"";
                    }
                }
                return string.Empty;
            }
        }

        #endregion // Private Helpers
    }
}