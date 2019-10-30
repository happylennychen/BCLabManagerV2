using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class ChamberViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        readonly ChamberClass _chamber;
        readonly ChamberRepository _chamberRepository;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public ChamberViewModel(ChamberClass chambermodel, ChamberRepository chamberRepository)
        {
            if (chambermodel == null)
                throw new ArgumentNullException("chambermodel");

            if (chamberRepository == null)
                throw new ArgumentNullException("chamberRepository");

            _chamber = chambermodel;
            _chamberRepository = chamberRepository;
            _chamber.PropertyChanged += _chamber_PropertyChanged;
        }

        private void _chamber_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region ChamberClass Properties

        public string Name
        {
            get { return _chamber.Name; }
            set
            {
                if (value == _chamber.Name)
                    return;

                _chamber.Name = value;

                base.OnPropertyChanged("Name");
            }
        }

        public string Manufactor
        {
            get { return _chamber.Manufactor; }
            set
            {
                if (value == _chamber.Manufactor)
                    return;

                _chamber.Manufactor = value;

                base.OnPropertyChanged("Manufactor");
            }
        }

        public Double LowTemp
        {
            get { return _chamber.LowestTemperature; }
            set
            {
                if (value == _chamber.LowestTemperature)
                    return;

                _chamber.LowestTemperature = value;

                base.OnPropertyChanged("LowTemp");
            }
        }

        public Double HighTemp
        {
            get { return _chamber.HighestTemperature; }
            set
            {
                if (value == _chamber.HighestTemperature)
                    return;

                _chamber.HighestTemperature = value;

                base.OnPropertyChanged("HighTemp");
            }
        }

        public AssetStatusEnum Status
        {
            get { return _chamber.Status; }
            set
            {
                if (value == _chamber.Status)
                    return;

                _chamber.Status = value;

                base.OnPropertyChanged("Status");
            }
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
            //if (!_chambertype.IsValid)
            //throw new InvalidOperationException(Resources.ChamberTypeViewModel_Exception_CannotSave);

            //if (this.IsNewChamberType)
            //_chambertypeRepository.AddItem(_chambertype);

            //base.OnPropertyChanged("DisplayName");
            IsOK = true;
        }

        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewChamber
        {
            get
            {
                int number = (
                    from bat in _chamberRepository.GetItems()
                    where bat.Name == _chamber.Name     //名字（某一个属性）一样就认为是一样的
                    select bat).Count();
                if (number != 0)
                    return false; 
                return !_chamberRepository.ContainsItem(_chamber);
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanCreate
        {
            get { return IsNewChamber; }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSaveAs
        {
            get { return IsNewChamber; }
        }

        #endregion // Private Helpers
    }
}