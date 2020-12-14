using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;
using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class ProjectEditViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields

        readonly Project _project;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public ProjectEditViewModel(Project project, ObservableCollection<BatteryType> batteryTypes)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            _project = project;

            CreateAllBatteryTypes(batteryTypes);
        }

        void CreateAllBatteryTypes(ObservableCollection<BatteryType> batteryTypes)
        {

            this.AllBatteryTypes = batteryTypes;
        }

        #endregion // Constructor

        #region ProjectClass Properties

        public int Id
        {
            get { return _project.Id; }
            set
            {
                if (value == _project.Id)
                    return;

                _project.Id = value;

                RaisePropertyChanged("Id");
            }
        }
        public string Name
        {
            get { return _project.Name; }
            set
            {
                if (value == _project.Name)
                    return;

                _project.Name = value.Trim();

                RaisePropertyChanged("Name");
            }
        }
        public string Customer
        {
            get { return _project.Customer; }
            set
            {
                if (value == _project.Customer)
                    return;

                _project.Customer = value;

                RaisePropertyChanged("Customer");
            }
        }
        public int CutoffDischargeVoltage
        {
            get { return _project.CutoffDischargeVoltage; }
            set
            {
                if (value == _project.CutoffDischargeVoltage)
                    return;

                _project.CutoffDischargeVoltage = value;

                RaisePropertyChanged("CutoffDischargeVoltage");
            }
        }
        public int LimitedChargeVoltage
        {
            get { return _project.LimitedChargeVoltage; }
            set
            {
                if (value == _project.LimitedChargeVoltage)
                    return;

                _project.LimitedChargeVoltage = value;

                RaisePropertyChanged("LimitedChargeVoltage");
            }
        }
        public int AbsoluteMaxCapacity
        {
            get { return _project.AbsoluteMaxCapacity; }
            set
            {
                if (value == _project.AbsoluteMaxCapacity)
                    return;

                _project.AbsoluteMaxCapacity = value;

                RaisePropertyChanged("AbsoluteMaxCapacity");
            }
        }
        private string _voltagePoints;
        public string VoltagePoints
        {
            get { return _voltagePoints; }
            set
            {
                if (value == _voltagePoints)
                    return;

                _voltagePoints = value;

                RaisePropertyChanged("VoltagePoints");
            }
        }
        public string Description
        {
            get { return _project.Description; }
            set
            {
                if (value == _project.Description)
                    return;

                _project.Description = value;

                RaisePropertyChanged("Description");
            }
        }

        #endregion // Customer Properties

        #region Presentation Properties

        public BatteryType BatteryType
        {
            get { return _project.BatteryType; }
            set
            {
                if (value == _project.BatteryType)
                    return;

                _project.BatteryType = value;

                RaisePropertyChanged("BatteryType");
                AbsoluteMaxCapacity = _project.BatteryType.RatedCapacity;
                LimitedChargeVoltage = _project.BatteryType.LimitedChargeVoltage;
                CutoffDischargeVoltage = _project.BatteryType.CutoffDischargeVoltage;
            }
        }

        public ObservableCollection<BatteryType> AllBatteryTypes
        {
            get; set;
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
            //if (!_batterytype.IsValid)
            //throw new InvalidOperationException(Resources.BatteryTypeViewModel_Exception_CannotSave);

            //if (this.IsNewBatteryType)
            //_batterytypeRepository.AddItem(_batterytype);

            //RaisePropertyChanged("DisplayName");
            if (VoltagePoints!= null && VoltagePoints != string.Empty)
                _project.VoltagePoints = VoltagePoints.Split(',').Select(o => Convert.ToUInt32(o)).ToList();
            IsOK = true;
        }

        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewProject
        {
            get
            {
                //var dbContext = new AppDbContext();
                //int number = (
                //    from bat in dbContext.Batteries
                //    where bat.Name == _battery.Name     //名字（某一个属性）一样就认为是一样的
                //    select bat).Count();
                //if (number != 0)
                //    return false;
                //else
                //    return true;
                return true;
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanCreate
        {
            get { return IsNewProject; }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSaveAs
        {
            get { return IsNewProject; }
        }

        #endregion // Private Helpers
    }
}