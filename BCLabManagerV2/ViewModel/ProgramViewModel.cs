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
    public class ProgramViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        readonly ProgramClass _program;
        readonly ProgramRepository _programRepository;
        readonly SubProgramRepository _subprogramRepository;
        SubProgramViewModel _leftselectedSubProgram;
        SubProgramViewModel _rightselectedSubProgram;
        RelayCommand _okCommand;
        RelayCommand _addCommand;
        RelayCommand _removeCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public ProgramViewModel(ProgramClass programmodel, ProgramRepository programRepository, SubProgramRepository subprogramRepository)
        {
            if (programmodel == null)
                throw new ArgumentNullException("programmodel");

            if (programRepository == null)
                throw new ArgumentNullException("programRepository");

            if (subprogramRepository == null)
                throw new ArgumentNullException("subprogramRepository");

            _program = programmodel;
            _programRepository = programRepository;
            _subprogramRepository = subprogramRepository;
            this.CreateAllSubPrograms();
        }


        void CreateAllSubPrograms()
        {
            List<SubProgramViewModel> all =
                (from bat in _subprogramRepository.GetItems()
                 select new SubProgramViewModel(bat, _subprogramRepository)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //foreach (SubProgramModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnSubProgramModelViewModelPropertyChanged;

            this.AllSubPrograms = new ObservableCollection<SubProgramViewModel>(all);     //再转换成Observable
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
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

                base.OnPropertyChanged("Name");
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

                base.OnPropertyChanged("Requester");
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

                base.OnPropertyChanged("Description");
            }
        }

        public DateTime RequestDate
        {
            get { return _program.RequestDate; }
            set
            {
                if (value == _program.RequestDate)
                    return;

                _program.RequestDate = value;

                base.OnPropertyChanged("RequestDate");
            }
        }

        public ObservableCollection<SubProgramViewModel> SubPrograms        //这个是当前program所拥有的subprograms
        {
            get
            {
                if (_program.SubPrograms == null)
                    return new ObservableCollection<SubProgramViewModel>();
                List<SubProgramViewModel> all =
                    (from bat in _program.SubPrograms
                     select new SubProgramViewModel(bat, _subprogramRepository)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

                //foreach (SubProgramModelViewModel batmod in all)
                //batmod.PropertyChanged += this.OnSubProgramModelViewModelPropertyChanged;

                return new ObservableCollection<SubProgramViewModel>(all);     //再转换成Observable
            }
            set
            {
                _program.SubPrograms =
                    (from subvm in value
                     select subvm._subprogram).ToList();
            }
        }

        #endregion // Customer Properties

        #region Presentation Properties

        public ObservableCollection<SubProgramViewModel> AllSubPrograms { get; private set; }   //展示所有SubProgram以便选用,跟SubPrograms是不一样的

        public SubProgramViewModel LeftSelectedSubProgram    
        {
            get
            {
                return _leftselectedSubProgram;
            }
            set
            {
                if (_leftselectedSubProgram != value)
                {
                    _leftselectedSubProgram = value;
                }
            }
        }

        public SubProgramViewModel RightSelectedSubProgram
        {
            get
            {
                return _rightselectedSubProgram;
            }
            set
            {
                if (_rightselectedSubProgram != value)
                {
                    _rightselectedSubProgram = value;
                }
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

        public ICommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                {
                    _addCommand = new RelayCommand(
                        param => { this.Add(); },
                        param => this.CanAdd
                            );
                }
                return _addCommand;
            }
        }

        #endregion // Presentation Properties

        #region Public Methods

        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        public void OK()
        {
            //if (!_programtype.IsValid)
            //throw new InvalidOperationException(Resources.ProgramTypeViewModel_Exception_CannotSave);

            //if (this.IsNewProgramType)
            //_programtypeRepository.AddItem(_programtype);

            //base.OnPropertyChanged("DisplayName");
            IsOK = true;
        }

        public void Add()
        {
            SubPrograms.Add(LeftSelectedSubProgram);
            _program.SubPrograms.Add(LeftSelectedSubProgram._subprogram);
        }

        public ProgramViewModel Clone()
        {
            return new ProgramViewModel(_program.Clone(), _programRepository, _subprogramRepository);
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
                int number = (
                    from bat in _programRepository.GetItems()
                    where bat.Name == _program.Name     //名字（某一个属性）一样就认为是一样的
                    select bat).Count();
                if (number != 0)
                    return false; 
                return !_programRepository.ContainsItem(_program);
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanCreate
        {
            get { return IsNewProgram; }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSaveAs
        {
            get { return IsNewProgram; }
        }

        bool CanAdd
        {
            get { return LeftSelectedSubProgram!=null; }
        }

        #endregion // Private Helpers
    }
}