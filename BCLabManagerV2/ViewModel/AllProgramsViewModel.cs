using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BCLabManager.ViewModel
{
    public class AllProgramsViewModel : ViewModelBase
    {
        #region Fields

        readonly ProgramRepository _programRepository;
        readonly SubProgramRepository _subprogramRepository;
        ProgramViewModel _selectedProgram;
        SubProgramViewModel _selectedSubProgram;
        //TestViewModel _selectedTest1;     //Test1区域选中项
        //TestViewModel _selectedTest2;     //Test2区域选中项
        //TestViewModel _selectedTest;      //Test1,Test2选中项中，拥有焦点的那一个
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllProgramsViewModel(ProgramRepository programRepository, SubProgramRepository subprogramRepository)
        {
            if (programRepository == null)
                throw new ArgumentNullException("programRepository");

            _programRepository = programRepository;

            if (subprogramRepository == null)
                throw new ArgumentNullException("subprogramRepository");

            _subprogramRepository = subprogramRepository;

            // Subscribe for notifications of when a new customer is saved.
            _programRepository.ItemAdded += this.OnProgramAddedToRepository;

            // Populate the AllProgramTypes collection with _programtypeRepository.
            this.CreateAllPrograms();
        }

        void CreateAllPrograms()
        {
            List<ProgramViewModel> all =
                (from bat in _programRepository.GetItems()
                 select new ProgramViewModel(bat,_programRepository, _subprogramRepository)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //foreach (ProgramModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnProgramModelViewModelPropertyChanged;

            this.AllPrograms = new ObservableCollection<ProgramViewModel>(all);     //再转换成Observable
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the ProgramModelViewModel objects.
        /// </summary>
        public ObservableCollection<ProgramViewModel> AllPrograms { get; private set; }

        public ProgramViewModel SelectedProgram    //绑定选中项，从而改变subprograms
        {
            get
            {
                return _selectedProgram;
            }
            set
            {
                if (_selectedProgram != value)
                {
                    _selectedProgram = value;
                    //OnPropertyChanged("SelectedType");
                    OnPropertyChanged("SubPrograms"); //通知SubPrograms改变
                }
            }
        }
        
        /// <summary>
        /// Returns a collection of all the ProgramModelViewModel objects.
        /// </summary>
        public ObservableCollection<SubProgramViewModel> SubPrograms
        {
            get
            {
                if (_selectedProgram != null)
                    return _selectedProgram.SubPrograms;
                else
                    return null;
            }
        }

        public SubProgramViewModel SelectedSubProgram    //绑定选中项，从而改变Test
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
                    //OnPropertyChanged("SelectedType");
                    //OnPropertyChanged("Test1"); //通知Test1改变
                    //OnPropertyChanged("Test2"); //通知Test2改变
                }
            }
        }
        
        public ICommand CreateCommand
        {
            get
            {
                if (_createCommand == null)
                {
                    _createCommand = new RelayCommand(
                        param => { this.Create();}
                        );
                }
                return _createCommand;
            }
        }
        public ICommand EditCommand
        {
            get
            {
                if (_editCommand == null)
                {
                    _editCommand = new RelayCommand(
                        param => { this.Edit(); },
                        param => this.CanEdit
                        );
                }
                return _editCommand;
            }
        }
        public ICommand SaveAsCommand
        {
            get
            {
                if (_saveAsCommand == null)
                {
                    _saveAsCommand = new RelayCommand(
                        param => { this.SaveAs(); },
                        param => this.CanSaveAs
                        );
                }
                return _saveAsCommand;
            }
        }

        #endregion // Public Interface

        #region Private Helper
        private void Create()
        {
            ProgramClass model = new ProgramClass();      //实例化一个新的model
            ProgramViewModel viewmodel = new ProgramViewModel(model, _programRepository, _subprogramRepository);      //实例化一个新的view model
            viewmodel.DisplayName = "Program-Create";
            viewmodel.commandType = CommandType.Create;
            var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            ProgramViewInstance.DataContext = viewmodel;
            ProgramViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                _programRepository.AddItem(model);
            }
        }
        private void Edit()
        {
            ProgramClass model = new ProgramClass();      //实例化一个新的model
            ProgramViewModel viewmodel = new ProgramViewModel(model, _programRepository, _subprogramRepository);      //实例化一个新的view model
            viewmodel.Name = _selectedProgram.Name;
            viewmodel.Requester = _selectedProgram.Requester;   //给viewmodel的属性赋值，实际上是对model进行赋值
            viewmodel.RequestDate = _selectedProgram.RequestDate;
            viewmodel.Description = _selectedProgram.Description;   //虽然string也是引用类型，但是string比较特殊，编译器有特殊处理
            viewmodel.SubPrograms = _selectedProgram.SubPrograms;   //这是引用类型，不应该这样赋值过去
            viewmodel.DisplayName = "Program-Edit";
            viewmodel.commandType = CommandType.Edit;
            var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            ProgramViewInstance.DataContext = viewmodel;
            ProgramViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _selectedProgram.Name = viewmodel.Name;
                _selectedProgram.Requester = viewmodel.Requester;
                _selectedProgram.RequestDate = viewmodel.RequestDate;
                _selectedProgram.Description = viewmodel.Description;
            }
        }
        private bool CanEdit
        {
            get { return _selectedProgram != null; }
        }
        private void SaveAs()
        {
            /*ProgramClass model = new ProgramClass();      //实例化一个新的model
            ProgramViewModel viewmodel = new ProgramViewModel(model, _programRepository, _subprogramRepository);      //实例化一个新的view model
            viewmodel.Name = _selectedProgram.Name;
            viewmodel.TestCount = _selectedProgram.TestCount;
            viewmodel.DisplayName = "Program-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            ProgramViewInstance.DataContext = viewmodel;
            ProgramViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _programRepository.AddItem(model);
            }*/
        }
        private bool CanSaveAs
        {
            get { return _selectedProgram != null; }
        }
        #endregion //Private Helper
        #region  Base Class Overrides

        protected override void OnDispose()
        {
            foreach (ProgramViewModel viewmodel in this.AllPrograms)
                viewmodel.Dispose();

            this.AllPrograms.Clear();
            //this.AllProgramModels.CollectionChanged -= this.OnCollectionChanged;

            _programRepository.ItemAdded -= this.OnProgramAddedToRepository;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        void OnProgramAddedToRepository(object sender, ItemAddedEventArgs<ProgramClass> e)
        {
            var viewModel = new ProgramViewModel(e.NewItem, _programRepository, _subprogramRepository);
            this.AllPrograms.Add(viewModel);
        }

        #endregion // Event Handling Methods
    }
}
