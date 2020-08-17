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
using System.Windows.Media;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class StepV2ViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        public readonly StepV2 _step;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public StepV2ViewModel(StepV2 step)
        {
            _step = step;
            _step.PropertyChanged += _Step_PropertyChanged;
            CreateCOCs();
        }

        private void CreateCOCs()
        {
            List<CutOffConditionViewModel> all =
                (from sub in _step.CutOffConditions
                 select new CutOffConditionViewModel(sub)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //foreach (RecipeModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnRecipeModelViewModelPropertyChanged;

            this.CutOffConditions = new ObservableCollection<CutOffConditionViewModel>(all);     //再转换成Observable
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }

        private void _Step_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region StepV2 Properties

        public int Id
        {
            get { return _step.Id; }
            set
            {
                if (value == _step.Id)
                    return;

                _step.Id = value;

                RaisePropertyChanged("Id");
            }
        }
        public int Index
        {
            get
            {
                return _step.Index;
            }
        }
        public int Prerest
        {
            get
            {
                return _step.Prerest;
            }
        }
        public int Rest
        {
            get
            {
                return _step.Rest;
            }
        }
        public TesterAction TesterAction
        {
            get
            {
                return _step.Action;
            }
        }
        public string Loop1Label
        {
            get
            {
                return _step.Loop1Label;
            }
        }
        public string Loop2Label
        {
            get
            {
                return _step.Loop2Label;
            }
        }
        public ObservableCollection<CutOffConditionViewModel> CutOffConditions
        { get; set; }
        #endregion // Customer Properties
    }
}