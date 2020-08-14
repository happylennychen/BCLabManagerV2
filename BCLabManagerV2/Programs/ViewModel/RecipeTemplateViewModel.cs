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
    public class RecipeTemplateViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        public readonly RecipeTemplate _recipeTemplate;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public RecipeTemplateViewModel(RecipeTemplate RecipeTemplate)
        {
            _recipeTemplate = RecipeTemplate;
            _recipeTemplate.PropertyChanged += _RecipeTemplate_PropertyChanged;
        }

        private void _RecipeTemplate_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region RecipeTemplate Properties

        public int Id
        {
            get { return _recipeTemplate.Id; }
            set
            {
                if (value == _recipeTemplate.Id)
                    return;

                _recipeTemplate.Id = value;

                RaisePropertyChanged("Id");
            }
        }
        public string Name
        {
            get
            {
                return _recipeTemplate.Name;
            }
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
        private int _count;
        public int Count
        {
            get { return _count; }
            set { SetProperty(ref _count, value); }
        }
        //public ChargeTemperatureClass ChargeTemperature
        //{
        //    get { return _RecipeTemplate.ChargeTemperature; }
        //    set
        //    {
        //        if (value == _RecipeTemplate.ChargeTemperature)
        //            return;

        //        _RecipeTemplate.ChargeTemperature = value;

        //        RaisePropertyChanged("ChargeTemperature");
        //    }
        //}
        //public ChargeCurrentClass ChargeCurrent
        //{
        //    get { return _RecipeTemplate.ChargeCurrent; }
        //    set
        //    {
        //        if (value == _RecipeTemplate.ChargeCurrent)
        //            return;

        //        _RecipeTemplate.ChargeCurrent = value;

        //        RaisePropertyChanged("ChargeCurrent");
        //    }
        //}
        //public DischargeTemperatureClass DischargeTemperature
        //{
        //    get { return _RecipeTemplate.DischargeTemperature; }
        //    set
        //    {
        //        if (value == _RecipeTemplate.DischargeTemperature)
        //            return;

        //        _RecipeTemplate.DischargeTemperature = value;

        //        RaisePropertyChanged("DischargeTemperature");
        //    }
        //}
        //public DischargeCurrentClass DischargeCurrent
        //{
        //    get { return _RecipeTemplate.DischargeCurrent; }
        //    set
        //    {
        //        if (value == _RecipeTemplate.DischargeCurrent)
        //            return;

        //        _RecipeTemplate.DischargeCurrent = value;

        //        RaisePropertyChanged("DischargeCurrent");
        //    }
        //}

        #endregion // Customer Properties

        #region Presentation Properties

        #endregion // Presentation Properties

    }
}