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
    public class SubProgramTemplateEditViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields
        public readonly SubProgramTemplate _subProgramTemplate;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public SubProgramTemplateEditViewModel(SubProgramTemplate subProgramTemplateModel)
        {
            if (subProgramTemplateModel == null)
                throw new ArgumentNullException("subProgramTemplateModel");

            _subProgramTemplate = subProgramTemplateModel;
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
        public string Name
        {
            get { return _subProgramTemplate.Name; }
            set
            {
                if (value == _subProgramTemplate.Name)
                    return;

                _subProgramTemplate.Name = value;

                base.OnPropertyChanged("Name");
            }
        }

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