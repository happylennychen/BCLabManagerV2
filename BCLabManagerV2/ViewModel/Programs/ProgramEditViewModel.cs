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
    /// A UI-friendly wrapper for a Program object.
    /// </summary>
    public class ProgramEditViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        public ProgramClass _program;            //为了AllProgramsViewModel中的Edit，不得不开放给viewmodel。以后再想想有没有别的办法。
        SubProgramTemplateViewModel _selectedSubProgramTemplate;
        SubProgramViewModel _selectedSubProgram;
        RelayCommand _okCommand;
        RelayCommand _addCommand;
        RelayCommand _removeCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public ProgramEditViewModel(ProgramClass programmodel, List<SubProgramTemplate> subProgramTemplates)
        {
            _program = programmodel;
            this.CreateAllSubProgramTemplates(subProgramTemplates);
            this.CreateSubPrograms();
        }


        void CreateAllSubProgramTemplates(List<SubProgramTemplate> subProgramTemplates)
        {
            List<SubProgramTemplateViewModel> all =
                (from sub in subProgramTemplates
                 select new SubProgramTemplateViewModel(sub)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllSubProgramTemplates = new ObservableCollection<SubProgramTemplateViewModel>(all);     //再转换成Observable
        }
        void CreateSubPrograms()
        {
            List<SubProgramViewModel> all =
                (from sub in _program.SubPrograms
                 select new SubProgramViewModel(sub)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //foreach (SubProgramModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnSubProgramModelViewModelPropertyChanged;

            this.SubPrograms = new ObservableCollection<SubProgramViewModel>(all);     //再转换成Observable
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }
        #endregion // Constructor

        #region ProgramClass Properties

        public int Id
        {
            get { return _program.Id; }
            set
            {
                if (value == _program.Id)
                    return;

                _program.Id = value;

                base.OnPropertyChanged("Id");
            }
        }

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

        public ObservableCollection<SubProgramViewModel> SubPrograms { get; set; }        //这个是当前program所拥有的subprograms
        /*{
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
        }*/

        #endregion // Customer Properties

        #region Presentation Properties

        public ObservableCollection<SubProgramTemplateViewModel> AllSubProgramTemplates { get; private set; }   //展示所有SubProgramTemplate以便选用,跟SubPrograms是不一样的

        public SubProgramTemplateViewModel SelectedSubProgramTemplate
        {
            get
            {
                return _selectedSubProgramTemplate;
            }
            set
            {
                if (_selectedSubProgramTemplate != value)
                {
                    _selectedSubProgramTemplate = value;
                }
            }
        }

        public SubProgramViewModel SelectedSubProgram
        {
            get
            {
                return _selectedSubProgram;
            }
            set
            {
                if (_selectedSubProgram != value)
                {
                    _selectedSubProgram = value;
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

        public ICommand RemoveCommand
        {
            get
            {
                if (_removeCommand == null)
                {
                    _removeCommand = new RelayCommand(
                        param => { this.Remove(); },
                        param => this.CanRemove
                            );
                }
                return _removeCommand;
            }
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

        public void Add()       //对于model来说，需要将选中的sub copy到_program.SubPrograms来。对于viewmodel来说，需要将这个copy出来的sub，包装成viewmodel并添加到this.SubPrograms里面去
        {
            var newsubmodel = new SubProgramClass(SelectedSubProgramTemplate._subProgramTemplate);
            var newsubviewmodel = new SubProgramViewModel(newsubmodel);
            _program.SubPrograms.Add(newsubmodel);
            this.SubPrograms.Add(newsubviewmodel);
        }

        public void Remove()       //对于model来说，需要将选中的sub 从_program.SubPrograms中移除。对于viewmodel来说，需要将这个viewmodel从this.SubPrograms中移除
        {
            _program.SubPrograms.Remove(SelectedSubProgram._subprogram);
            this.SubPrograms.Remove(SelectedSubProgram);
        }

        //public ProgramViewModel Clone()
        //{
        //    return new ProgramViewModel(_program.Clone(), _programRepository, _subprogramRepository);
        //}

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
                var dbContext = new AppDbContext();
                int number = (
                    from bat in dbContext.Programs
                    where bat.Name == _program.Name     //名字（某一个属性）一样就认为是一样的
                    select bat).Count();
                if (number != 0)
                    return false;
                else
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

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSaveAs
        {
            get { return IsNewProgram; }
        }

        bool CanAdd
        {
            get { return SelectedSubProgramTemplate!=null; }
        }

        bool CanRemove
        {
            get { return SelectedSubProgram != null; }     //如果已经有数据，可否删除？
        }

        #endregion // Private Helpers
    }
}