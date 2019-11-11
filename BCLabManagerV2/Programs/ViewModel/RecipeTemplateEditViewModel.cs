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
        //List<ChargeTemperatureClass> _chargeTemperatures;
        //List<ChargeCurrentClass> _chargeCurrents;
        //List<DischargeTemperatureClass> _dischargeTemperatures;
        //List<DischargeCurrentClass> _dischargeCurrents;
        public readonly RecipeTemplate _RecipeTemplate;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public RecipeTemplateEditViewModel(
            RecipeTemplate RecipeTemplateModel//,
            //List<ChargeTemperatureClass> chargeTemperatures,
            //List<ChargeCurrentClass> chargeCurrents,
            //List<DischargeTemperatureClass> dischargeTemperatures,
            //List<DischargeCurrentClass> dischargeCurrents
            )
        {
            //_chargeTemperatures = chargeTemperatures;
            //_chargeCurrents = chargeCurrents;
            //_dischargeTemperatures = dischargeTemperatures;
            //_dischargeCurrents = dischargeCurrents;
            //this.AllChargeTemperatures = CreateAllChargeTemperatures(chargeTemperatures);
            //this.AllChargeCurrents = CreateAllChargeCurrents(chargeCurrents);
            //this.AllDischargeTemperatures = CreateAllDischargeTemperatures(dischargeTemperatures);
            //this.AllDischargeCurrents = CreateAllDischargeCurrents(dischargeCurrents);
            _RecipeTemplate = RecipeTemplateModel;
        }

        //private ObservableCollection<ChargeTemperatureClass> CreateAllChargeTemperatures(List<ChargeTemperatureClass> chargeTemperatures)
        //{
        //    return new ObservableCollection<ChargeTemperatureClass>(chargeTemperatures);
        //}

        //private ObservableCollection<ChargeCurrentClass> CreateAllChargeCurrents(List<ChargeCurrentClass> chargeCurrents)
        //{
        //    return new ObservableCollection<ChargeCurrentClass>(chargeCurrents);
        //}

        //private ObservableCollection<DischargeTemperatureClass> CreateAllDischargeTemperatures(List<DischargeTemperatureClass> dischargeTemperatures)
        //{
        //    return new ObservableCollection<DischargeTemperatureClass>(dischargeTemperatures);
        //}

        //private ObservableCollection<DischargeCurrentClass> CreateAllDischargeCurrents(List<DischargeCurrentClass> dischargeCurrents)
        //{
        //    return new ObservableCollection<DischargeCurrentClass>(dischargeCurrents);
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
        //public string Name
        //{
        //    get { return _RecipeTemplate.Name; }
        //    set
        //    {
        //        if (value == _RecipeTemplate.Name)
        //            return;

        //        _RecipeTemplate.Name = value;

        //        RaisePropertyChanged("Name");
        //    }
        //}
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

        //public ObservableCollection<ChargeTemperatureClass> AllChargeTemperatures //供选项
        //{
        //    get;
        //    set;
        //}

        //public ObservableCollection<ChargeCurrentClass> AllChargeCurrents //供选项
        //{
        //    get;
        //    set;
        //}
        //public ObservableCollection<DischargeTemperatureClass> AllDischargeTemperatures //供选项
        //{
        //    get;
        //    set;
        //}

        //public ObservableCollection<DischargeCurrentClass> AllDischargeCurrents //供选项
        //{
        //    get;
        //    set;
        //}
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

        #endregion // Presentation Properties

        #region Public Methods

        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        public void OK()
        {
            //if (!_Recipetype.IsValid)
            //throw new InvalidOperationException(Resources.RecipeTypeViewModel_Exception_CannotSave);

            //if (this.IsNewRecipeType)
            //_RecipetypeRepository.AddItem(_Recipetype);

            //RaisePropertyChanged("DisplayName");
            IsOK = true;
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
                //int number = (
                //    from bat in _RecipeRepository.GetItems()
                //    where bat.Name == _RecipeTemplate.Name     //名字（某一个属性）一样就认为是一样的
                //    select bat).Count();
                //if (number != 0)
                //    return false;
                //else
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

        #endregion // Private Helpers
    }
}