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
    public class RecipeTemplateChooseViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        public readonly RecipeTemplate _RecipeTemplate;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public RecipeTemplateChooseViewModel(
            RecipeTemplate RecipeTemplateModel
            )
        {
            _RecipeTemplate = RecipeTemplateModel;
        }

        #endregion // Constructor

        #region RecipeTemplateGroup Properties

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
        public RecipeTemplateGroup Group
        {
            get { return _RecipeTemplate.Group; }
            set
            {
                if (value == _RecipeTemplate.Group)
                    return;

                _RecipeTemplate.Group = value;

                RaisePropertyChanged("Group");
            }
        }
        #endregion // Customer Properties
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
        #region Presentation Properties

        #endregion // Presentation Properties

        #region Private Helpers

        #endregion // Private Helpers
    }
}