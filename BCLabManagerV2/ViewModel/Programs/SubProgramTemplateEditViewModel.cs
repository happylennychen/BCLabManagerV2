using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;
using System.Windows;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class SubProgramTemplateEditViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields
        List<TemperatureClass> _temperatures;
        List<PercentageCurrentClass> _percentageCurrents;
        List<AbsoluteCurrentClass> _absoluteCurrents;
        List<DynamicCurrentClass> _dynamicCurrents;
        public readonly SubProgramTemplate _subProgramTemplate;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public SubProgramTemplateEditViewModel(
            SubProgramTemplate subProgramTemplateModel,
            List<TemperatureClass> temperatures,
            List<PercentageCurrentClass> percentageCurrents,
            List<AbsoluteCurrentClass> absoluteCurrents,
            List<DynamicCurrentClass> dynamicCurrents
            )
        {
            _temperatures = temperatures;
            _percentageCurrents = percentageCurrents;
            _absoluteCurrents = absoluteCurrents;
            _dynamicCurrents = dynamicCurrents;
            this.AllTemperatures = CreateAllTemperatures(temperatures);
            this.AllPercentageCurrents = CreateAllPercentageCurrents(percentageCurrents);
            this.AllAbsoluteCurrents = CreateAllAbsoluteCurrents(absoluteCurrents);
            this.AllDynamicCurrents = CreateAllDynamicCurrents(dynamicCurrents);
            _subProgramTemplate = subProgramTemplateModel;

            ChargeAbsoluteCurrentVisibility = Visibility.Hidden;
            ChargePercentageCurrentVisibility = Visibility.Hidden;
            ChargeDynamicCurrentVisibility = Visibility.Hidden;

            DischargeAbsoluteCurrentVisibility = Visibility.Hidden;
            DischargePercentageCurrentVisibility = Visibility.Hidden;
            DischargeDynamicCurrentVisibility = Visibility.Hidden;
        }

        private ObservableCollection<TemperatureClass> CreateAllTemperatures(List<TemperatureClass> temperatures)
        {
            return new ObservableCollection<TemperatureClass>(temperatures);
        }

        private ObservableCollection<PercentageCurrentClass> CreateAllPercentageCurrents(List<PercentageCurrentClass> percentageCurrents)
        {
            return new ObservableCollection<PercentageCurrentClass>(percentageCurrents);
        }

        private ObservableCollection<AbsoluteCurrentClass> CreateAllAbsoluteCurrents(List<AbsoluteCurrentClass> absoluteCurrents)
        {
            return new ObservableCollection<AbsoluteCurrentClass>(absoluteCurrents);
        }

        private ObservableCollection<DynamicCurrentClass> CreateAllDynamicCurrents(List<DynamicCurrentClass> dynamicCurrents)
        {
            return new ObservableCollection<DynamicCurrentClass>(dynamicCurrents);
        }
        #endregion // Constructor

        #region SubProgramTemplate Properties

        public int Id
        {
            get { return _subProgramTemplate.Id; }
            set
            {
                if (value == _subProgramTemplate.Id)
                    return;

                _subProgramTemplate.Id = value;

                base.OnPropertyChanged("Id");
            }
        }
        //public string Name
        //{
        //    get { return _subProgramTemplate.Name; }
        //    set
        //    {
        //        if (value == _subProgramTemplate.Name)
        //            return;

        //        _subProgramTemplate.Name = value;

        //        base.OnPropertyChanged("Name");
        //    }
        //}
        public TemperatureClass ChargeTemperature
        {
            set
            {
                if (value.Value == _subProgramTemplate.ChargeTemperature)
                    return;

                _subProgramTemplate.ChargeTemperature = value.Value;
            }
        }
        public CurrentTypeEnum ChargeCurrentType
        {
            get { return _subProgramTemplate.ChargeCurrentType; }
            set
            {
                if (value == CurrentTypeEnum.Percentage)
                {
                    ChargePercentageCurrentVisibility = Visibility.Visible;
                    ChargeAbsoluteCurrentVisibility = Visibility.Hidden;
                    ChargeDynamicCurrentVisibility = Visibility.Hidden;
                }
                else if (value == CurrentTypeEnum.Absolute)
                {
                    ChargePercentageCurrentVisibility = Visibility.Hidden;
                    ChargeAbsoluteCurrentVisibility = Visibility.Visible;
                    ChargeDynamicCurrentVisibility = Visibility.Hidden;
                }
                else if (value == CurrentTypeEnum.Dynamic)
                {
                    ChargePercentageCurrentVisibility = Visibility.Hidden;
                    ChargeAbsoluteCurrentVisibility = Visibility.Hidden;
                    ChargeDynamicCurrentVisibility = Visibility.Visible;
                }
                if (value == _subProgramTemplate.ChargeCurrentType)
                    return;

                _subProgramTemplate.ChargeCurrentType = value;

                base.OnPropertyChanged("ChargeCurrentType");
            }
        }
        //public double ChargeCurrent
        //{
        //    get { return _subProgramTemplate.ChargeCurrent; }
        //    set
        //    {
        //        if (value == _subProgramTemplate.ChargeCurrent)
        //            return;

        //        _subProgramTemplate.ChargeCurrent = value;

        //        base.OnPropertyChanged("ChargeCurrent");
        //    }
        //}
        public TemperatureClass DischargeTemperature
        {
            set
            {
                if (value.Value == _subProgramTemplate.DischargeTemperature)
                    return;

                _subProgramTemplate.DischargeTemperature = value.Value;
            }
        }
        public CurrentTypeEnum DischargeCurrentType
        {
            get { return _subProgramTemplate.DischargeCurrentType; }
            set
            {
                if (value == CurrentTypeEnum.Percentage)
                {
                    DischargePercentageCurrentVisibility = Visibility.Visible;
                    DischargeAbsoluteCurrentVisibility = Visibility.Hidden;
                    DischargeDynamicCurrentVisibility = Visibility.Hidden;
                }
                else if (value == CurrentTypeEnum.Absolute)
                {
                    DischargePercentageCurrentVisibility = Visibility.Hidden;
                    DischargeAbsoluteCurrentVisibility = Visibility.Visible;
                    DischargeDynamicCurrentVisibility = Visibility.Hidden;
                }
                else if (value == CurrentTypeEnum.Dynamic)
                {
                    DischargePercentageCurrentVisibility = Visibility.Hidden;
                    DischargeAbsoluteCurrentVisibility = Visibility.Hidden;
                    DischargeDynamicCurrentVisibility = Visibility.Visible;
                }
                if (value == _subProgramTemplate.DischargeCurrentType)
                    return;

                _subProgramTemplate.DischargeCurrentType = value;

                base.OnPropertyChanged("DischargeCurrentType");
            }
        }
        //public double DischargeCurrent
        //{
        //    get { return _subProgramTemplate.DischargeCurrent; }
        //    set
        //    {
        //        if (value == _subProgramTemplate.DischargeCurrent)
        //            return;

        //        _subProgramTemplate.DischargeCurrent = value;

        //        base.OnPropertyChanged("DischargeCurrent");
        //    }
        //}

        public TestCountEnum TestCount
        {
            get { return _subProgramTemplate.TestCount; }
            set
            {
                if (value == _subProgramTemplate.TestCount)
                    return;

                _subProgramTemplate.TestCount = value;

                base.OnPropertyChanged("TestCount");
            }
        }

        #endregion // Customer Properties

        #region Presentation Properties

        public List<TestCountEnum> TestCountOptions
        {
            get
            {
                List<TestCountEnum> all = new List<TestCountEnum>();
                all.Add(TestCountEnum.One);
                all.Add(TestCountEnum.Two);


                return all;
            }
        }

        public List<CurrentTypeEnum> CurrentTypeOptions
        {
            get
            {
                return new List<CurrentTypeEnum>()
                {
                    CurrentTypeEnum.Absolute,
                    CurrentTypeEnum.Dynamic,
                    CurrentTypeEnum.Percentage
                };
            }
        }

        public ObservableCollection<TemperatureClass> AllTemperatures //供选项
        {
            get;
            set;
        }
        public ObservableCollection<PercentageCurrentClass> AllPercentageCurrents //供选项
        {
            get;
            set;
        }
        public ObservableCollection<AbsoluteCurrentClass> AllAbsoluteCurrents //供选项
        {
            get;
            set;
        }

        public ObservableCollection<DynamicCurrentClass> AllDynamicCurrents //供选项
        {
            get;
            set;
        }

        public PercentageCurrentClass ChargePercentageCurrent { get; set; }
        public PercentageCurrentClass DischargePercentageCurrent { get; set; }

        public AbsoluteCurrentClass ChargeAbsoluteCurrent { get; set; }
        public AbsoluteCurrentClass DischargeAbsoluteCurrent { get; set; }
        public DynamicCurrentClass ChargeDynamicCurrent { get; set; }
        public DynamicCurrentClass DischargeDynamicCurrent { get; set; }

        private Visibility _chargePercentageCurrentVisibility;

        public Visibility ChargePercentageCurrentVisibility
        {
            get { return _chargePercentageCurrentVisibility; }
            set
            {
                _chargePercentageCurrentVisibility = value;
                base.OnPropertyChanged("ChargePercentageCurrentVisibility");
            }
        }

        private Visibility _dischargePercentageCurrentVisibility;

        public Visibility DischargePercentageCurrentVisibility
        {
            get { return _dischargePercentageCurrentVisibility; }
            set
            {
                _dischargePercentageCurrentVisibility = value;
                base.OnPropertyChanged("DischargePercentageCurrentVisibility");
            }
        }


        private Visibility _chargeAbsoluteCurrentVisibility;

        public Visibility ChargeAbsoluteCurrentVisibility
        {
            get { return _chargeAbsoluteCurrentVisibility; }
            set
            {
                _chargeAbsoluteCurrentVisibility = value;
                base.OnPropertyChanged("ChargeAbsoluteCurrentVisibility");
            }
        }

        private Visibility _dischargeAbsoluteCurrentVisibility;

        public Visibility DischargeAbsoluteCurrentVisibility
        {
            get { return _dischargeAbsoluteCurrentVisibility; }
            set
            {
                _dischargeAbsoluteCurrentVisibility = value;
                base.OnPropertyChanged("DischargeAbsoluteCurrentVisibility");
            }
        }

        private Visibility _chargeDynamicCurrentVisibility;

        public Visibility ChargeDynamicCurrentVisibility
        {
            get { return _chargeDynamicCurrentVisibility; }
            set
            {
                _chargeDynamicCurrentVisibility = value;
                base.OnPropertyChanged("ChargeDynamicCurrentVisibility");
            }
        }

        private Visibility _dischargeDynamicCurrentVisibility;

        public Visibility DischargeDynamicCurrentVisibility
        {
            get { return _dischargeDynamicCurrentVisibility; }
            set
            {
                _dischargeDynamicCurrentVisibility = value;
                base.OnPropertyChanged("DischargeDynamicCurrentVisibility");
            }
        }
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
            //if (!_subprogramtype.IsValid)
            //throw new InvalidOperationException(Resources.SubProgramTypeViewModel_Exception_CannotSave);

            //if (this.IsNewSubProgramType)
            //_subprogramtypeRepository.AddItem(_subprogramtype);

            //base.OnPropertyChanged("DisplayName");
            if(ChargeCurrentType == CurrentTypeEnum.Percentage)
                _subProgramTemplate.ChargeCurrent = ChargePercentageCurrent.Value;
            else if(ChargeCurrentType == CurrentTypeEnum.Absolute)
                _subProgramTemplate.ChargeCurrent = ChargeAbsoluteCurrent.Value;
            else if (ChargeCurrentType == CurrentTypeEnum.Dynamic)
                _subProgramTemplate.ChargeCurrent = ChargeDynamicCurrent.Value;

            if (DischargeCurrentType == CurrentTypeEnum.Percentage)
                _subProgramTemplate.DischargeCurrent = DischargePercentageCurrent.Value;
            else if (DischargeCurrentType == CurrentTypeEnum.Absolute)
                _subProgramTemplate.DischargeCurrent = DischargeAbsoluteCurrent.Value;
            else if (DischargeCurrentType == CurrentTypeEnum.Dynamic)
                _subProgramTemplate.DischargeCurrent = DischargeDynamicCurrent.Value;

            IsOK = true;
        }

        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewSubProgramTemplate
        {
            get
            {
                //int number = (
                //    from bat in _subprogramRepository.GetItems()
                //    where bat.Name == _subProgramTemplate.Name     //名字（某一个属性）一样就认为是一样的
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
            get { return IsNewSubProgramTemplate; }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSaveAs
        {
            get { return IsNewSubProgramTemplate; }
        }

        #endregion // Private Helpers
    }
}