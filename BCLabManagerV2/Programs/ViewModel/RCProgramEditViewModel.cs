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
    /// A UI-friendly wrapper for a Program object.
    /// </summary>
    public class RCProgramEditViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields

        public Program _program;            //为了AllProgramsViewModel中的Edit，不得不开放给viewmodel。以后再想想有没有别的办法。
        ObservableCollection<Project> _projects;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public RCProgramEditViewModel(
            Program programmodel,
            ObservableCollection<Project> projects)
        {
            _program = programmodel;
            _projects = projects;
        }
        #endregion // Constructor

        #region ProgramClass Properties

        public string Name
        {
            get { return _program.Name; }
            set
            {
                if (value == _program.Name)
                    return;

                _program.Name = value;

                RaisePropertyChanged("Name");
            }
        }

        public Project Project       //选中项
        {
            get
            {
                //if (_batteryType == null)
                //return null;
                return _program.Project;
            }
            set
            {
                if (value == _program.Project)
                    return;

                _program.Project = value;

                RaisePropertyChanged("Project");
            }
        }

        public ObservableCollection<Project> AllProjects //供选项
        {
            get
            {
                ObservableCollection<Project> all = _projects;

                return new ObservableCollection<Project>(all);
            }
        }
        public string Requester
        {
            get { return _program.Requester; }
            set
            {
                if (value == _program.Requester)
                    return;

                _program.Requester = value;

                RaisePropertyChanged("Requester");
            }
        }

        public string Description
        {
            get { return _program.Description; }
            set
            {
                if (value == _program.Description)
                    return;

                _program.Description = value;

                RaisePropertyChanged("Description");
            }
        }

        public DateTime RequestDate
        {
            get { return _program.RequestTime; }
            set
            {
                if (value == _program.RequestTime)
                    return;

                _program.RequestTime = value;

                RaisePropertyChanged("RequestDate");
            }
        }
        private double _chargeRate;
        public double ChargeRate
        {
            get { return _chargeRate; }
            set { SetProperty(ref _chargeRate, value); }
        }
        private double _idleTime;
        public double IdleTime
        {
            get { return _idleTime; }
            set { SetProperty(ref _idleTime, value); }
        }
        private ObservableCollection<CurrentPoint> _currents = new ObservableCollection<CurrentPoint>();
        public ObservableCollection<CurrentPoint> Currents
        {
            get { return _currents; }
            set { SetProperty(ref _currents, value); }
        }
        private ObservableCollection<TemperaturePoint> _temperatures = new ObservableCollection<TemperaturePoint>();
        public ObservableCollection<TemperaturePoint> Temperatures
        {
            get { return _temperatures; }
            set { SetProperty(ref _temperatures, value); }
        }


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
                    _okCommand = new RelayCommand(
                        param => { this.OK(); },
                        param => this.CanCreate
                        );
                }
                return _okCommand;
            }
        }

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
            IsOK = true;
        }

        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewProgram
        {
            get
            {
                //var dbContext = new AppDbContext();
                //int number = (
                //    from bat in dbContext.Programs
                //    where bat.Name == _program.Name     //名字（某一个属性）一样就认为是一样的
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
            get { return IsNewProgram; }
        }

        #endregion // Private Helpers
    }

    public class CurrentPoint : BindableBase
    {
        private double _current;
        public double Current
        {
            get { return _current; }
            set { SetProperty(ref _current, value); }
        }
    }

    public class TemperaturePoint : BindableBase
    {
        private double _temperature;
        public double Temperature
        {
            get { return _temperature; }
            set { SetProperty(ref _temperature, value); }
        }
    }
}