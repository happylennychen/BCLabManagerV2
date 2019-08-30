using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace BCLabManager.ViewModel
{
    public class AllProgramsViewModel : ViewModelBase
    {
        #region Fields

        readonly ProgramRepository _programRepository;
        readonly SubProgramRepository _subprogramRepository;
        ProgramViewModel _selectedProgram;
        SubProgramViewModel _selectedSubProgram;
        TestRecordViewModel _selectedFirstTestRecord;
        TestRecordViewModel _selectedSecondTestRecord;
        //TestViewModel _selectedTest1;     //Test1区域选中项
        //TestViewModel _selectedTest2;     //Test2区域选中项
        //TestViewModel _selectedTest;      //Test1,Test2选中项中，拥有焦点的那一个
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _executeCommand;
        RelayCommand _commitCommand;
        RelayCommand _invalidateCommand;
        RelayCommand _viewCommand;
        RelayCommand _executeCommand2;
        RelayCommand _commitCommand2;
        RelayCommand _invalidateCommand2;
        RelayCommand _viewCommand2;

        #endregion // Fields

        #region Constructor

        public AllProgramsViewModel()
        {

            _programRepository = Repositories._programRepository;

            _subprogramRepository = Repositories._subprogramRepository;

            // Subscribe for notifications of when a new customer is saved.
            _programRepository.ItemAdded += this.OnProgramAddedToRepository;

            // Populate the AllProgramTypes collection with _programtypeRepository.
            this.CreateAllPrograms();
        }

        void CreateAllPrograms()
        {
            using (var dbContext = new AppDbContext())
            {
                List<ProgramViewModel> all =
                    (from program in dbContext.Programs
                     .Include(pro=>pro.SubPrograms)
                        .ThenInclude(sub=>sub.FirstTestRecords)
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub=> sub.SecondTestRecords)
                     select new ProgramViewModel(program, _programRepository, _subprogramRepository)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

                //foreach (ProgramModelViewModel batmod in all)
                //batmod.PropertyChanged += this.OnProgramModelViewModelPropertyChanged;

                this.AllPrograms = new ObservableCollection<ProgramViewModel>(all);     //再转换成Observable
                                                                                        //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
            }
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
                    OnPropertyChanged("FirstTestRecords"); //通知Test1改变
                    OnPropertyChanged("SecondTestRecords"); //通知Test2改变
                }
            }
        }

        public ObservableCollection<TestRecordViewModel> FirstTestRecords
        {
            get
            {
                if (_selectedSubProgram != null)
                    return _selectedSubProgram.Test1Records;
                else
                    return null;
            }
        }

        public ObservableCollection<TestRecordViewModel> SecondTestRecords
        {
            get
            {
                if (_selectedSubProgram != null)
                    return _selectedSubProgram.Test2Records;
                else
                    return null;
            }
        }

        public TestRecordViewModel SelectedFirstTestRecord
        {
            get
            {
                return _selectedFirstTestRecord;
            }
            set
            {
                if (_selectedFirstTestRecord != value)
                {
                    _selectedFirstTestRecord = value;
                }
            }
        }

        public TestRecordViewModel SelectedSecondTestRecord
        {
            get
            {
                return _selectedSecondTestRecord;
            }
            set
            {
                if (_selectedSecondTestRecord != value)
                {
                    _selectedSecondTestRecord = value;
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
        public ICommand ExecuteCommand
        {
            get
            {
                if (_executeCommand == null)
                {
                    _executeCommand = new RelayCommand(
                        param => { this.Execute(); },
                        param => this.CanExecute
                        );
                }
                return _executeCommand;
            }
        }
        public ICommand CommitCommand
        {
            get
            {
                if (_commitCommand == null)
                {
                    _commitCommand = new RelayCommand(
                        param => { this.Commit(); },
                        param => this.CanCommit
                        );
                }
                return _commitCommand;
            }
        }

        public ICommand InvalidateCommand
        {
            get
            {
                if (_invalidateCommand == null)
                {
                    _invalidateCommand = new RelayCommand(
                        param => { this.Invalidate(); },
                        param => this.CanInvalidate
                        );
                }
                return _invalidateCommand;
            }
        }

        public ICommand ViewCommand
        {
            get
            {
                if (_viewCommand == null)
                {
                    _viewCommand = new RelayCommand(
                        param => { this.View(); },
                        param => this.CanView
                        );
                }
                return _viewCommand;
            }
        }


        public ICommand ExecuteCommand2
        {
            get
            {
                if (_executeCommand2 == null)
                {
                    _executeCommand2 = new RelayCommand(
                        param => { this.Execute2(); },
                        param => this.CanExecute2
                        );
                }
                return _executeCommand2;
            }
        }
        public ICommand CommitCommand2
        {
            get
            {
                if (_commitCommand2 == null)
                {
                    _commitCommand2 = new RelayCommand(
                        param => { this.Commit2(); },
                        param => this.CanCommit2
                        );
                }
                return _commitCommand2;
            }
        }

        public ICommand InvalidateCommand2
        {
            get
            {
                if (_invalidateCommand2 == null)
                {
                    _invalidateCommand2 = new RelayCommand(
                        param => { this.Invalidate2(); },
                        param => this.CanInvalidate2
                        );
                }
                return _invalidateCommand2;
            }
        }

        public ICommand ViewCommand2
        {
            get
            {
                if (_viewCommand2 == null)
                {
                    _viewCommand2 = new RelayCommand(
                        param => { this.View2(); },
                        param => this.CanView2
                        );
                }
                return _viewCommand2;
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
                //_programRepository.AddItem(model);
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Programs.Add(model);
                    dbContext.SaveChanges();
                    this.AllPrograms.Add(viewmodel);
                }
            }
        }
        private void Edit()
        {
            ProgramClass model = _selectedProgram._program.Clone();
            ProgramViewModel viewmodel = new ProgramViewModel(model, _programRepository, _subprogramRepository);      //实例化一个新的view model
            viewmodel.DisplayName = "Program-Edit";
            viewmodel.commandType = CommandType.Edit;
            var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            ProgramViewInstance.DataContext = viewmodel;
            ProgramViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                /*SelectedProgram._program.Update(model);   //修改model中的主项

                SelectedProgram.UpdateSubPrograms();        //修改viewmodel中的子项
                OnPropertyChanged("SubPrograms");*/
                using (var dbContext = new AppDbContext())
                {
                    var program = dbContext.Programs.SingleOrDefault(i => i.Id == SelectedProgram.Id);  //没有完全取出
                    dbContext.Entry(program)
                        .Collection(p => p.SubPrograms)
                        .Load();
                    //program.Update(model);
                    bool isTgtNotContainSrc = false;    //Add to target
                    bool isSrcNotContainTgt = false;    //Remove from target
                    List<SubProgramClass> TobeRemoved = new List<SubProgramClass>();
                    List<SubProgramClass> TobeAdded = new List<SubProgramClass>();
                    foreach (var sub_target in program.SubPrograms)     //看看在不在source中，不在则删掉
                    {
                        isSrcNotContainTgt = true;
                        foreach (var sub_source in model.SubPrograms)
                        {
                            if (sub_target.Id == sub_source.Id)
                            {
                                isSrcNotContainTgt = false;
                                break;
                            }
                        }
                        if (isSrcNotContainTgt == true)
                            TobeRemoved.Add(sub_target);
                    }
                    foreach (var sub_source in model.SubPrograms)
                    {
                        isTgtNotContainSrc = true;
                        foreach (var sub_target in program.SubPrograms)
                        {
                            if (sub_target.Id == sub_source.Id)
                            {
                                isTgtNotContainSrc = false;
                                break;
                            }
                        }
                        if (isTgtNotContainSrc == true)
                            TobeAdded.Add(sub_source);
                    }
                    foreach (var sub in TobeRemoved)
                    {
                        program.SubPrograms.Remove(sub);
                        //var subs = dbContext.SubPrograms;       //手动递归删除
                        //subs.Remove(sub);
                    }
                    foreach (var sub in TobeAdded)
                    {
                        program.SubPrograms.Add(sub);
                    }

                    //SelectedProgram.UpdateSubPrograms();        //修改viewmodel中的子项

                    List<SubProgramViewModel> all =
                        (from sub in program.SubPrograms
                         select new SubProgramViewModel(sub, _subprogramRepository, "")).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

                    //foreach (SubProgramModelViewModel batmod in all)
                    //batmod.PropertyChanged += this.OnSubProgramModelViewModelPropertyChanged;

                    SelectedProgram.SubPrograms = new ObservableCollection<SubProgramViewModel>(all);     //再转换成Observable
                    OnPropertyChanged("SubPrograms");
                    dbContext.SaveChanges();
                }
            }
        }
        private bool CanEdit
        {
            get { return _selectedProgram != null; }
        }
        private void SaveAs()
        {
            ProgramClass model = _selectedProgram._program.Clone();
            ProgramViewModel viewmodel = new ProgramViewModel(model, _programRepository, _subprogramRepository);      //实例化一个新的view model
            viewmodel.DisplayName = "Program-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            ProgramViewInstance.DataContext = viewmodel;
            ProgramViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _programRepository.AddItem(model);
            }
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
        private void Execute()
        {
            /*ProgramClass model = _selectedProgram._program.Clone();
            ProgramViewModel viewmodel = new ProgramViewModel(model, _programRepository, _subprogramRepository);      //实例化一个新的view model
            viewmodel.DisplayName = "Program-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            ProgramViewInstance.DataContext = viewmodel;
            ProgramViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _programRepository.AddItem(model);
            }*/

            TestRecordClass model = new TestRecordClass();
            TestRecordViewModel viewmodel = new TestRecordViewModel(model,SelectedProgram.Name, SelectedSubProgram.Name);
            viewmodel.DisplayName = "Test-Execute";
            var TestRecordViewInstance = new ExecuteView();
            TestRecordViewInstance.DataContext = viewmodel;
            TestRecordViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                SelectedFirstTestRecord.BatteryType = viewmodel.BatteryType;
                SelectedFirstTestRecord.Battery = viewmodel.Battery;
                SelectedFirstTestRecord.Chamber = viewmodel.Chamber;
                SelectedFirstTestRecord.Tester = viewmodel.Tester;
                SelectedFirstTestRecord.Channel = viewmodel.Channel;
                SelectedFirstTestRecord.StartTime = viewmodel.StartTime;
                SelectedFirstTestRecord.Steps = viewmodel.Steps;
                SelectedFirstTestRecord.Execute();
            }
        }
        private bool CanExecute
        {
            get { return _selectedFirstTestRecord != null && _selectedFirstTestRecord.Status == TestStatus.Waiting; }
        }
        private void Commit()
        {
            TestRecordClass model = new TestRecordClass();
            TestRecordViewModel viewmodel = new TestRecordViewModel(model, SelectedProgram.Name, SelectedSubProgram.Name);
            viewmodel.DisplayName = "Test-Commit";
            var TestRecordCommitViewInstance = new CommitView();
            TestRecordCommitViewInstance.DataContext = viewmodel;
            TestRecordCommitViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                SelectedFirstTestRecord.EndTime = viewmodel.EndTime;
                SelectedFirstTestRecord.NewCycle = viewmodel.NewCycle;
                SelectedFirstTestRecord.Comment = viewmodel.Comment;
                SelectedFirstTestRecord.Commit();
            }
        }
        private bool CanCommit
        {
            get { return _selectedFirstTestRecord != null && _selectedFirstTestRecord.Status == TestStatus.Executing; }
        }
        private void Invalidate()
        {
            TestRecordClass model = new TestRecordClass();
            TestRecordViewModel viewmodel = new TestRecordViewModel(model, SelectedProgram.Name, SelectedSubProgram.Name);
            viewmodel.DisplayName = "Test-Invalidate";
            var TestRecordInvalidateViewInstance = new InvalidateView();
            TestRecordInvalidateViewInstance.DataContext = viewmodel;
            TestRecordInvalidateViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                SelectedFirstTestRecord.Comment = viewmodel.Comment;
                SelectedFirstTestRecord.Invalidate();
            }
        }
        private bool CanInvalidate
        {
            get { return _selectedFirstTestRecord != null && _selectedFirstTestRecord.Status == TestStatus.Completed; }
        }
        private void View()
        {
            /*TestRecordClass model = new TestRecordClass();
            TestRecordViewModel viewmodel = new TestRecordViewModel(model, SelectedProgram.Name, SelectedSubProgram.Name);
            viewmodel.DisplayName = "Test-Invalidate";
            var TestRecordInvalidateViewInstance = new InvalidateView();
            TestRecordInvalidateViewInstance.DataContext = viewmodel;
            TestRecordInvalidateViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                SelectedFirstTestRecord.Comment = viewmodel.Comment;
                SelectedFirstTestRecord.Invalidate();
            }*/
        }
        private bool CanView
        {
            get { return _selectedFirstTestRecord != null && (_selectedFirstTestRecord.Status == TestStatus.Completed || _selectedFirstTestRecord.Status == TestStatus.Invalid); }
        }
        private void Execute2()
        {
            TestRecordClass model = new TestRecordClass();
            TestRecordViewModel viewmodel = new TestRecordViewModel(model, SelectedProgram.Name, SelectedSubProgram.Name);
            viewmodel.DisplayName = "Test-Execute";
            var TestRecordViewInstance = new ExecuteView();
            TestRecordViewInstance.DataContext = viewmodel;
            TestRecordViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                SelectedSecondTestRecord.BatteryType = viewmodel.BatteryType;
                SelectedSecondTestRecord.Battery = viewmodel.Battery;
                SelectedSecondTestRecord.Chamber = viewmodel.Chamber;
                SelectedSecondTestRecord.Tester = viewmodel.Tester;
                SelectedSecondTestRecord.Channel = viewmodel.Channel;
                SelectedSecondTestRecord.StartTime = viewmodel.StartTime;
                SelectedSecondTestRecord.Steps = viewmodel.Steps;
                SelectedSecondTestRecord.Execute();
            }
        }
        private bool CanExecute2
        {
            get { return _selectedSecondTestRecord != null && _selectedSecondTestRecord.Status == TestStatus.Waiting; }
        }
        private void Commit2()
        {
            TestRecordClass model = new TestRecordClass();
            TestRecordViewModel viewmodel = new TestRecordViewModel(model, SelectedProgram.Name, SelectedSubProgram.Name);
            viewmodel.DisplayName = "Test-Commit";
            var TestRecordCommitViewInstance = new CommitView();
            TestRecordCommitViewInstance.DataContext = viewmodel;
            TestRecordCommitViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                SelectedSecondTestRecord.EndTime = viewmodel.EndTime;
                SelectedSecondTestRecord.NewCycle = viewmodel.NewCycle;
                SelectedSecondTestRecord.Comment = viewmodel.Comment;
                SelectedSecondTestRecord.Commit();
            }
        }
        private bool CanCommit2
        {
            get { return _selectedSecondTestRecord != null && _selectedSecondTestRecord.Status == TestStatus.Executing; }
        }
        private void Invalidate2()
        {
            TestRecordClass model = new TestRecordClass();
            TestRecordViewModel viewmodel = new TestRecordViewModel(model, SelectedProgram.Name, SelectedSubProgram.Name);
            viewmodel.DisplayName = "Test-Invalidate";
            var TestRecordInvalidateViewInstance = new InvalidateView();
            TestRecordInvalidateViewInstance.DataContext = viewmodel;
            TestRecordInvalidateViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                SelectedSecondTestRecord.Comment = viewmodel.Comment;
                SelectedSecondTestRecord.Invalidate();
            }
        }
        private bool CanInvalidate2
        {
            get { return _selectedSecondTestRecord != null && _selectedSecondTestRecord.Status == TestStatus.Completed; }
        }
        private void View2()
        {
            /*TestRecordClass model = new TestRecordClass();
            TestRecordViewModel viewmodel = new TestRecordViewModel(model, SelectedProgram.Name, SelectedSubProgram.Name);
            viewmodel.DisplayName = "Test-Invalidate";
            var TestRecordInvalidateViewInstance = new InvalidateView();
            TestRecordInvalidateViewInstance.DataContext = viewmodel;
            TestRecordInvalidateViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                SelectedSecondTestRecord.Comment = viewmodel.Comment;
                SelectedSecondTestRecord.Invalidate();
            }*/
        }
        private bool CanView2
        {
            get { return _selectedSecondTestRecord != null && (_selectedSecondTestRecord.Status == TestStatus.Completed || _selectedSecondTestRecord.Status == TestStatus.Invalid); }
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
