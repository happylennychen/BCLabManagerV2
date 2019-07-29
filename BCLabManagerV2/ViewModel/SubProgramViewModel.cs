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
    public class SubProgramViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        public readonly SubProgramClass _subprogram;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。
        readonly SubProgramRepository _subprogramRepository;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public SubProgramViewModel(SubProgramClass subprogrammodel, SubProgramRepository subprogramRepository)
        {
            if (subprogrammodel == null)
                throw new ArgumentNullException("subprogrammodel");

            if (subprogramRepository == null)
                throw new ArgumentNullException("subprogramRepository");

            _subprogram = subprogrammodel;
            _subprogramRepository = subprogramRepository;
            this.CreateTestRecords();
        }


        void CreateTestRecords()
        {
            List<TestRecordViewModel> all1 =
                (from ft in _subprogram.FirstTestRecords
                 select new TestRecordViewModel(ft)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.Test1Records = new ObservableCollection<TestRecordViewModel>(all1);     //再转换成Observable


            List<TestRecordViewModel> all2 =
                (from ft in _subprogram.SecondTestRecords
                 select new TestRecordViewModel(ft)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.Test2Records = new ObservableCollection<TestRecordViewModel>(all2);     //再转换成Observable
        }

        #endregion // Constructor

        #region SubProgramClass Properties

        public string Name
        {
            get { return _subprogram.Name; }
            set
            {
                if (value == _subprogram.Name)
                    return;

                _subprogram.Name = value;

                base.OnPropertyChanged("Name");
            }
        }

        public TestCountEnum TestCount
        {
            get { return _subprogram.TestCount; }
            set
            {
                if (value == _subprogram.TestCount)
                    return;

                _subprogram.TestCount = value;

                base.OnPropertyChanged("TestCount");
            }
        }

        public ObservableCollection<TestRecordViewModel> Test1Records { get; private set; }        //这个是当前sub program所拥有的test1

        public ObservableCollection<TestRecordViewModel> Test2Records { get; private set; }        //这个是当前sub program所拥有的test2

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

        public SubProgramViewModel Clone()
        {
            return new SubProgramViewModel(_subprogram.Clone(), _subprogramRepository);
        }

        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewSubProgram
        {
            get
            {
                int number = (
                    from bat in _subprogramRepository.GetItems()
                    where bat.Name == _subprogram.Name     //名字（某一个属性）一样就认为是一样的
                    select bat).Count();
                if (number != 0)
                    return false; 
                return !_subprogramRepository.ContainsItem(_subprogram);
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanCreate
        {
            get { return IsNewSubProgram; }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSaveAs
        {
            get { return IsNewSubProgram; }
        }

        #endregion // Private Helpers
    }
}