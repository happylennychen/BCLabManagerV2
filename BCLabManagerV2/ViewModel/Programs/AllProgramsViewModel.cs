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
using System.IO;

namespace BCLabManager.ViewModel
{
    public class AllProgramsViewModel : ViewModelBase
    {
        #region Fields
        List<SubProgramTemplate> _subProgramTemplates;
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

        List<BatteryTypeClass> _batteryTypes;
        ObservableCollection<BatteryClass> _batteries;
        List<TesterClass> _testers;
        ObservableCollection<ChannelClass> _channels;
        ObservableCollection<ChamberClass> _chambers;
        ObservableCollection<ProgramClass> _programs;
        #endregion // Fields

        #region Constructor

        public AllProgramsViewModel
            (
            ObservableCollection<ProgramClass> programs,
            List<SubProgramTemplate> subProgramTemplates,
            List<BatteryTypeClass> batteryTypes,
            ObservableCollection<BatteryClass> batteries,
            List<TesterClass> testers,
            ObservableCollection<ChannelClass> channels,
            ObservableCollection<ChamberClass> chambers
            )
        {
            _subProgramTemplates = subProgramTemplates;
            _programs = programs;
            this.CreateAllPrograms(programs);

            _batteryTypes = batteryTypes;
            _batteries = batteries;
            _testers = testers;
            _channels = channels;
            _chambers = chambers;
        }

        void CreateAllPrograms(ObservableCollection<ProgramClass> programClasses)
        {
            List<ProgramViewModel> all =
                (from program in programClasses
                 select new ProgramViewModel(program)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllPrograms = new ObservableCollection<ProgramViewModel>(all);     //再转换成Observable
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
                        param => { this.Create(); }
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
            ProgramClass m = new ProgramClass();      //实例化一个新的model
            ProgramEditViewModel evm = new ProgramEditViewModel(m, _subProgramTemplates);      //实例化一个新的view model
            evm.DisplayName = "Program-Create";
            evm.commandType = CommandType.Create;
            var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            ProgramViewInstance.DataContext = evm;
            ProgramViewInstance.ShowDialog();                   //设置viewmodel属性
            if (evm.IsOK == true)
            {
                //_programRepository.AddItem(model);
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Programs.Add(m);
                    dbContext.SaveChanges();
                }
                _programs.Add(m);
                this.AllPrograms.Add(new ProgramViewModel(m));
            }
        }
        private void Edit()
        {
            var oldpro = _selectedProgram._program;
            var oldsubs = oldpro.SubPrograms;

            ProgramClass model = new ProgramClass();    //Edit Window要用到的model

            model.Name = oldpro.Name;
            model.Requester = oldpro.Requester;
            model.RequestDate = oldpro.RequestDate;
            model.Description = oldpro.Description;
            model.SubPrograms = new ObservableCollection<SubProgramClass>(oldsubs);          //这里并不希望在edit window里面修改原本的subprograms，而是想编辑一个新的subprogram,只是这个新的，是旧集合的浅复制

            ProgramEditViewModel viewmodel = new ProgramEditViewModel(model, _subProgramTemplates);      //实例化一个新的view model
            viewmodel.DisplayName = "Program-Edit";
            viewmodel.commandType = CommandType.Edit;
            var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            ProgramViewInstance.DataContext = viewmodel;
            ProgramViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)     //Add Remove操作，就是将model.SubPrograms里面的集合内容改变了
            {
                List<SubProgramClass> TobeRemoved = new List<SubProgramClass>();
                List<SubProgramClass> TobeAdded = new List<SubProgramClass>();
                //先改数据库，这样可以使得Id准确
                using (var dbContext = new AppDbContext())
                {
                    var dbProgram = dbContext.Programs.SingleOrDefault(i => i.Id == SelectedProgram.Id);  //没有完全取出
                    dbContext.Entry(dbProgram)
                        .Collection(p => p.SubPrograms)
                        .Load();

                    bool isTgtNotContainSrc = false;    //Add to target
                    bool isSrcNotContainTgt = false;    //Remove from target
                    foreach (var sub_target in dbProgram.SubPrograms)     //看看在不在source中，不在则删掉
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
                        foreach (var sub_target in dbProgram.SubPrograms)
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
                        dbProgram.SubPrograms.Remove(sub);
                    }
                    foreach (var sub in TobeAdded)
                    {
                        dbProgram.SubPrograms.Add(sub);
                    }
                    dbContext.SaveChanges();        //TobeAdded中的元素Id改变了
                }
                //再改model
                foreach (var sub in TobeRemoved)
                {
                    oldsubs.Remove(oldsubs.SingleOrDefault(o => o.Id == sub.Id));
                }
                foreach (var sub in TobeAdded)
                {
                    oldsubs.Add(sub);
                }
                //再改view model
                foreach (var sub in TobeRemoved)
                {
                    oldsubs.Remove(oldsubs.SingleOrDefault(o => o.Id == sub.Id));
                    SelectedProgram.SubPrograms.Remove(SelectedProgram.SubPrograms.SingleOrDefault(o => o.Id == sub.Id));
                }
                foreach (var sub in TobeAdded)
                {
                    SelectedProgram.SubPrograms.Add(new SubProgramViewModel(sub));
                }
            }

                //model.SubPrograms = _selectedProgram._program.SubPrograms;
                /*
                ProgramClass model = _selectedProgram._program.Clone();
                ProgramEditViewModel viewmodel = new ProgramEditViewModel(model, _subProgramTemplates);      //实例化一个新的view model
                viewmodel.DisplayName = "Program-Edit";
                viewmodel.commandType = CommandType.Edit;
                var ProgramViewInstance = new ProgramView();      //实例化一个新的view
                ProgramViewInstance.DataContext = viewmodel;
                ProgramViewInstance.ShowDialog();
                if (viewmodel.IsOK == true)
                {
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
                        dbContext.SaveChanges();
                        SelectedProgram._program = program;

                        //SelectedProgram.UpdateSubPrograms();        //修改viewmodel中的子项

                        List<SubProgramViewModel> all =
                            (from sub in program.SubPrograms
                             select new SubProgramViewModel(sub)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

                        //foreach (SubProgramModelViewModel batmod in all)
                        //batmod.PropertyChanged += this.OnSubProgramModelViewModelPropertyChanged;

                        SelectedProgram.SubPrograms = new ObservableCollection<SubProgramViewModel>(all);     //再转换成Observable
                        OnPropertyChanged("SubPrograms");
                    }
                }
                    */
            }
        private bool CanEdit
        {
            get { return _selectedProgram != null; }
        }
        private void SaveAs()
        {
            ProgramClass m = _selectedProgram._program.Clone();
            ProgramEditViewModel evm = new ProgramEditViewModel(m, _subProgramTemplates);      //实例化一个新的view model
            evm.DisplayName = "Program-Save As";
            evm.commandType = CommandType.SaveAs;
            var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            ProgramViewInstance.DataContext = evm;
            ProgramViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                //_programRepository.AddItem(model);
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Programs.Add(m);
                    dbContext.SaveChanges();
                }
                _programs.Add(m);
                this.AllPrograms.Add(new ProgramViewModel(m));
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedProgram != null; }
        }
        private void Execute()
        {
            Execute(SelectedFirstTestRecord);
        }
        private bool CanExecute
        {
            get { return _selectedFirstTestRecord != null && _selectedFirstTestRecord.Status == TestStatus.Waiting; }
        }
        private void Commit()
        {
            Commit(SelectedFirstTestRecord);
        }
        private bool CanCommit
        {
            get { return _selectedFirstTestRecord != null && _selectedFirstTestRecord.Status == TestStatus.Executing; }
        }
        private void Invalidate()
        {
            Invalidate(SelectedFirstTestRecord);
        }
        private bool CanInvalidate
        {
            get { return _selectedFirstTestRecord != null && _selectedFirstTestRecord.Status == TestStatus.Completed; }
        }
        private void View()
        {
            ViewRawData(SelectedFirstTestRecord);
        }
        private bool CanView
        {
            get { return _selectedFirstTestRecord != null && (_selectedFirstTestRecord.Status == TestStatus.Completed || _selectedFirstTestRecord.Status == TestStatus.Invalid); }
        }
        private void Execute2()
        {
            Execute(SelectedSecondTestRecord);
        }
        private bool CanExecute2
        {
            get { return _selectedSecondTestRecord != null && _selectedSecondTestRecord.Status == TestStatus.Waiting; }
        }
        private void Commit2()
        {
            Commit(SelectedSecondTestRecord);
        }
        private bool CanCommit2
        {
            get { return _selectedSecondTestRecord != null && _selectedSecondTestRecord.Status == TestStatus.Executing; }
        }
        private void Invalidate2()
        {
            Invalidate(SelectedSecondTestRecord);
        }
        private bool CanInvalidate2
        {
            get { return _selectedSecondTestRecord != null && _selectedSecondTestRecord.Status == TestStatus.Completed; }
        }
        private void View2()
        {
            ViewRawData(SelectedSecondTestRecord);
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
        }

        #endregion // Base Class Overrides


        private void Execute(TestRecordViewModel testRecord)                                      
            //相当于Edit，需要修改TestRecord的属性（vm和m层面都要修改），保存到数据库。还需要修改Assets的属性（vm和m层面都要修改），保存到数据库
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

            TestRecordExecuteViewModel evm = new TestRecordExecuteViewModel
                (
                testRecord.Record,      //将model传给ExecuteViewModel      //??????????????????????????
                _batteryTypes,
                _batteries,
                _testers,
                _channels,
                _chambers
                );
            evm.DisplayName = "Test-Execute";
            var TestRecordViewInstance = new ExecuteView();
            TestRecordViewInstance.DataContext = evm;
            TestRecordViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                testRecord.BatteryTypeStr = evm.BatteryType.Name;
                testRecord.BatteryStr = evm.Battery.Name;
                testRecord.ChamberStr = evm.Chamber.Name;
                testRecord.TesterStr = evm.Tester.Name;
                testRecord.ChannelStr = evm.Channel.Name;
                testRecord.StartTime = evm.StartTime;
                testRecord.Steps = evm.Steps;
                testRecord.Status = TestStatus.Executing;

                using (var dbContext = new AppDbContext())
                {
                    var tr = dbContext.TestRecords.SingleOrDefault(i => i.Id == testRecord.Id);
                    tr.BatteryTypeStr = testRecord.BatteryTypeStr;
                    tr.BatteryStr = testRecord.BatteryStr;
                    tr.ChamberStr = testRecord.ChamberStr;
                    tr.TesterStr = testRecord.TesterStr;
                    tr.ChannelStr = testRecord.ChannelStr;
                    tr.StartTime = testRecord.StartTime;
                    tr.Steps = testRecord.Steps;
                    tr.Status = testRecord.Status;
                    tr.AssignedBattery = dbContext.Batteries.SingleOrDefault(o=>o.Id == evm.Battery.Id);
                    tr.AssignedChamber = dbContext.Chambers.SingleOrDefault(o => o.Id == evm.Chamber.Id);
                    tr.AssignedChannel = dbContext.Channels.SingleOrDefault(o => o.Id == evm.Channel.Id);
                    dbContext.SaveChanges();
                }
                testRecord.ExecuteOnAssets(evm.Battery, evm.Chamber, evm.Channel,SelectedProgram.Name, SelectedSubProgram.Name);      //将evm的Assets传给testRecord
            }
        }
        private void Commit(TestRecordViewModel testRecord)
        //相当于Edit，需要修改TestRecord的属性（vm和m层面都要修改），保存到数据库。还需要修改Assets的属性（vm和m层面都要修改），保存到数据库
        {
            //viewmodel.DisplayName = "Test-Commit";
            //var TestRecordCommitViewInstance = new CommitView();
            //TestRecordCommitViewInstance.DataContext = viewmodel;
            //TestRecordCommitViewInstance.ShowDialog();
            //if (viewmodel.IsOK == true)
            //{
            //    SelectedFirstTestRecord.EndTime = viewmodel.EndTime;
            //    SelectedFirstTestRecord.NewCycle = viewmodel.NewCycle;
            //    SelectedFirstTestRecord.Comment = viewmodel.Comment;
            //    SelectedFirstTestRecord.Commit();
            //}

            //TestRecordClass m = new TestRecordClass();
            TestRecordCommitViewModel evm = new TestRecordCommitViewModel
                (
                testRecord.Record      //??????????????????????????
                );
            evm.DisplayName = "Test-Commit";
            var TestRecordCommitViewInstance = new CommitView();
            TestRecordCommitViewInstance.DataContext = evm;
            TestRecordCommitViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                testRecord.EndTime = evm.EndTime;
                testRecord.NewCycle = evm.NewCycle;
                testRecord.Comment = evm.Comment;
                testRecord.Status = TestStatus.Completed;
                //testRecord.FilePath = evm.FileList;
                testRecord.CommitOnAssets();
                using (var dbContext = new AppDbContext())
                {
                    var tr = dbContext.TestRecords.SingleOrDefault(i => i.Id == testRecord.Id);
                    tr.EndTime = testRecord.EndTime;
                    tr.NewCycle = testRecord.NewCycle;
                    tr.Comment = testRecord.Comment;
                    tr.Status = testRecord.Status;
                    tr.RawDataList = CreateRawDataList(evm.FileList);
                    tr.AssignedBattery = null;
                    tr.AssignedChamber = null;
                    tr.AssignedChannel = null;
                    dbContext.SaveChanges();
                }
            }
        }
        private List<RawDataClass> CreateRawDataList(ObservableCollection<string> fileList)
        {
            List<RawDataClass> output = new List<RawDataClass>();
            foreach (var filepath in fileList)
            {
                var rd = new RawDataClass();
                rd.FileName = Path.GetFileName(filepath);
                rd.BinaryData = File.ReadAllBytes(filepath);
                output.Add(rd);
            }
            return output;
        }
        private void Invalidate(TestRecordViewModel testRecord)
        {
            TestRecordInvalidateViewModel evm = new TestRecordInvalidateViewModel
                (
                testRecord.Record      //??????????????????????????
                );
            evm.DisplayName = "Test-Invalidate";
            var TestRecordInvalidateViewInstance = new InvalidateView();
            TestRecordInvalidateViewInstance.DataContext = evm;
            TestRecordInvalidateViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                testRecord.Comment = evm.Comment;
                testRecord.Status = TestStatus.Invalid;

                using (var dbContext = new AppDbContext())
                {
                    var tr = dbContext.TestRecords.SingleOrDefault(i => i.Id == testRecord.Id);
                    tr.Comment = testRecord.Comment;
                    tr.Status = testRecord.Status;
                    dbContext.SaveChanges();
                }
                testRecord.Invalidate();
            }
        }
        private void ViewRawData(TestRecordViewModel testRecordVM)
        {
            TestRecordRawDataViewModel evm = new TestRecordRawDataViewModel
                (
                testRecordVM.Record      //??????????????????????????
                );
            evm.DisplayName = "Test-View Raw Data";
            var TestRecordRawDataViewInstance = new RawDataView();
            TestRecordRawDataViewInstance.DataContext = evm;
            TestRecordRawDataViewInstance.ShowDialog();
        }
    }
}
