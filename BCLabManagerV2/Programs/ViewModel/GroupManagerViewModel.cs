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
using System.Windows;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class GroupManagerViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields
        RecipeTemplateGroup _group;
        RecipeTemplateGroupServiceClass _recipeTemplateGroupServcie;
        RecipeTemplateServiceClass _recipeTemplateServcie;
        RelayCommand _applyCommand;
        #endregion // Fields

        #region Constructor

        public GroupManagerViewModel(RecipeTemplateGroupServiceClass recipeTemplateGroupServcie, RecipeTemplateServiceClass recipeTemplateServcie)
        {
            _recipeTemplateGroupServcie = recipeTemplateGroupServcie;
            _recipeTemplateServcie = recipeTemplateServcie;
            CreateRecipeTemplates();
        }

        private void CreateRecipeTemplates()
        {
            List<RecipeTemplateChooseViewModel> all = _recipeTemplateServcie.Items.Select(o => new RecipeTemplateChooseViewModel(o)).ToList();
            RecipeTemplates = new ObservableCollection<RecipeTemplateChooseViewModel>(all);
        }

        #endregion // Constructor

        #region Presentation Properties


        public RecipeTemplateGroup Group   //选中项
        {
            get
            {
                //if (_record.BatteryStr == null)
                //return "/";
                return _group;
            }
            set
            {
                if (value == _group)
                    return;

                _group = value;

                RaisePropertyChanged("Group");
            }
        }

        public ObservableCollection<RecipeTemplateGroup> AllGroups    //供选项
        {
            get
            {
                return _recipeTemplateGroupServcie.Items;
            }
        }

        public ObservableCollection<RecipeTemplateChooseViewModel> RecipeTemplates { get; set; }
        #endregion // Presentation Properties

        #region Public Methods

        public ICommand ApplyCommand
        {
            get
            {
                if (_applyCommand == null)
                {
                    _applyCommand = new RelayCommand(
                        param => { this.Apply(); }
                        );
                }
                return _applyCommand;
            }
        }
        #endregion // Public Methods

        #region Private Helpers


        private void Apply()
        {
            var ret = MessageBox.Show("Are you sure?", "Warning", MessageBoxButton.YesNo);
            if (ret == MessageBoxResult.Yes)
            {
                foreach (var rec in RecipeTemplates.Where(o => o.IsSelected == true))
                {
                    rec.Group = Group;
                    _recipeTemplateServcie.SuperUpdate(rec._RecipeTemplate);
                }
            }
        }
        #endregion // Private Helpers
    }
}