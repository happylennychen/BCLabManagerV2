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
    public class PercentageCurrentEditViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields
        public readonly PercentageCurrentClass _model;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public PercentageCurrentEditViewModel(PercentageCurrentClass model)
        {
            _model = model;
        }

        #endregion // Constructor

        #region ChargeCurrent Properties

        public int Id
        {
            get { return _model.Id; }
            set
            {
                if (value == _model.Id)
                    return;

                _model.Id = value;

                base.OnPropertyChanged("Id");
            }
        }
        public double Value
        {
            get { return _model.Value; }
            set
            {
                if (value == _model.Value)
                    return;

                _model.Value = value;

                base.OnPropertyChanged("Value");
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
            //if (!_subprogramtype.IsValid)
            //throw new InvalidOperationException(Resources.SubProgramTypeViewModel_Exception_CannotSave);

            //if (this.IsNewSubProgramType)
            //_subprogramtypeRepository.AddItem(_subprogramtype);

            //base.OnPropertyChanged("DisplayName");
            IsOK = true;
        }

        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewChargeCurrent
        {
            get
            {
                //int number = (
                //    from bat in _subprogramRepository.GetItems()
                //    where bat.Name == _ChargeCurrent.Name     //名字（某一个属性）一样就认为是一样的
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
            get { return IsNewChargeCurrent; }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSaveAs
        {
            get { return IsNewChargeCurrent; }
        }

        #endregion // Private Helpers
    }
}