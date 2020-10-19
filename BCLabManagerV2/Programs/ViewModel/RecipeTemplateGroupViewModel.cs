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
    public class RecipeTemplateGroupViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        public readonly RecipeTemplateGroup _RecipeTemplateGroup;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public RecipeTemplateGroupViewModel(RecipeTemplateGroup RecipeTemplateGroup)
        {
            _RecipeTemplateGroup = RecipeTemplateGroup;
            _RecipeTemplateGroup.PropertyChanged += _RecipeTemplateGroup_PropertyChanged;
        }

        private void _RecipeTemplateGroup_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region RecipeTemplateGroup Properties

        public int Id
        {
            get { return _RecipeTemplateGroup.Id; }
            set
            {
                if (value == _RecipeTemplateGroup.Id)
                    return;

                _RecipeTemplateGroup.Id = value;

                RaisePropertyChanged("Id");
            }
        }
        public string Name
        {
            get
            {
                return _RecipeTemplateGroup.Name;
            }
        }

        #endregion // Customer Properties

        #region Presentation Properties

        #endregion // Presentation Properties

    }
}