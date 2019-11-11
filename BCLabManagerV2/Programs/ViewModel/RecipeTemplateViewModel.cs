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
        public readonly RecipeTemplate _RecipeTemplate;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public RecipeTemplateViewModel(RecipeTemplate RecipeTemplate)
        {
            _RecipeTemplate = RecipeTemplate;
            _RecipeTemplate.PropertyChanged += _RecipeTemplate_PropertyChanged;
        }

        private void _RecipeTemplate_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
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
            get
            {
                //return $"{_RecipeTemplate.ChargeTemperature.Name} {_RecipeTemplate.ChargeCurrent} charge, {_RecipeTemplate.DischargeTemperature} {_RecipeTemplate.DischargeCurrent} discharge";
                return "";
            }
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