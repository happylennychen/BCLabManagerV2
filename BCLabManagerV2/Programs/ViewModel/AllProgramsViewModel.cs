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
using System.Windows;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    public class AllProgramsViewModel : BindableBase
    {
        #region Fields
        List<RecipeTemplate> _RecipeTemplates;
        ProgramViewModel _selectedProgram;
        RecipeViewModel _selectedRecipe;
        TestRecordViewModel _selectedTestRecord;
        TestRecordViewModel _selectedSecondTestRecord;
        //TestViewModel _selectedTest1;     //Test1区域选中项
        //TestViewModel _selectedTest2;     //Test2区域选中项
        //TestViewModel _selectedTest;      //Test1,Test2选中项中，拥有焦点的那一个
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _abandonCommand;
        RelayCommand _executeCommand;
        RelayCommand _commitCommand;
        RelayCommand _invalidateCommand;
        RelayCommand _viewCommand;
        RelayCommand _executeCommand2;
        RelayCommand _commitCommand2;
        RelayCommand _invalidateCommand2;
        RelayCommand _viewCommand2;

        ObservableCollection<BatteryTypeClass> _batteryTypes;
        ObservableCollection<BatteryClass> _batteries;
        ObservableCollection<TesterClass> _testers;
        ObservableCollection<ChannelClass> _channels;
        ObservableCollection<ChamberClass> _chambers;
        ObservableCollection<ProgramClass> _programs;
        #endregion // Fields

        #region Constructor

        public AllProgramsViewModel
            (
            ObservableCollection<ProgramClass> programs,
            List<RecipeTemplate> RecipeTemplates,
            ObservableCollection<BatteryTypeClass> batteryTypes,
            ObservableCollection<BatteryClass> batteries,
            ObservableCollection<TesterClass> testers,
            ObservableCollection<ChannelClass> channels,
            ObservableCollection<ChamberClass> chambers
            )
        {
            _RecipeTemplates = RecipeTemplates;
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

        public ProgramViewModel SelectedProgram    //绑定选中项，从而改变Recipes
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
                    RaisePropertyChanged("Recipes"); //通知Recipes改变
                }
            }
        }

        /// <summary>
        /// Returns a collection of all the ProgramModelViewModel objects.
        /// </summary>
        public ObservableCollection<RecipeViewModel> Recipes
        {
            get
            {
                if (_selectedProgram != null)
                    return _selectedProgram.Recipes;
                else
                    return null;
            }
        }

        public RecipeViewModel SelectedRecipe    //绑定选中项，从而改变Test
        {
            get
            {
                return _selectedRecipe;
            }
            set
            {
                if (_selectedRecipe != value)
                {
                    _selectedRecipe = value;
                    //RaisePropertyChanged("SelectedType");
                    RaisePropertyChanged("TestRecords"); //通知Test1改变
                    RaisePropertyChanged("SecondTestRecords"); //通知Test2改变
                }
            }
        }

        public ObservableCollection<TestRecordViewModel> TestRecords
        {
            get
            {
                if (_selectedRecipe != null)
                    return _selectedRecipe.TestRecords;
                else
                    return null;
            }
        }

        public TestRecordViewModel SelectedTestRecord
        {
            get
            {
                return _selectedTestRecord;
            }
            set
            {
                if (_selectedTestRecord != value)
                {
                    _selectedTestRecord = value;
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
        public ICommand AbandonCommand
        {
            get
            {
                if (_abandonCommand == null)
                {
                    _abandonCommand = new RelayCommand(
                        param => { this.Abandon(); },
                        param => this.CanAbandon
                        );
                }
                return _abandonCommand;
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
        #endregion // Public Interface

        #region Private Helper
        private void Create()
        {
            ProgramClass m = new ProgramClass();      //实例化一个新的model
            ProgramEditViewModel evm = new ProgramEditViewModel(m,_batteryTypes, _RecipeTemplates);      //实例化一个新的view model
            //evm.DisplayName = "Program-Create";
            evm.commandType = CommandType.Create;
            var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            ProgramViewInstance.DataContext = evm;
            ProgramViewInstance.ShowDialog();                   //设置viewmodel属性
            if (evm.IsOK == true)
            {
                //_programRepository.AddItem(model);
                using (var dbContext = new AppDbContext())
                {
                    foreach (var sub in m.Recipes)
                    {
                        //sub.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Id == sub.ChargeTemperature.Id);
                        //sub.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Id == sub.ChargeCurrent.Id);
                        //sub.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Id == sub.DischargeTemperature.Id);
                        //sub.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Id == sub.DischargeCurrent.Id);
                        //newP.Recipes.Add(dbContext.Recipes.SingleOrDefault(o => o.Id == sub.Id));
                    }
                    m.BatteryType = dbContext.BatteryTypes.SingleOrDefault(o => o.Id == m.BatteryType.Id);
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
            var oldsubs = oldpro.Recipes;

            ProgramClass model = new ProgramClass();    //Edit Window要用到的model

            model.Name = oldpro.Name;
            model.Requester = oldpro.Requester;
            model.RequestTime = oldpro.RequestTime;
            model.Description = oldpro.Description;
            model.Recipes = new ObservableCollection<RecipeClass>(oldsubs);          //这里并不希望在edit window里面修改原本的Recipes，而是想编辑一个新的Recipe,只是这个新的，是旧集合的浅复制

            ProgramEditViewModel viewmodel = new ProgramEditViewModel(model, _batteryTypes, _RecipeTemplates);      //实例化一个新的view model
            //viewmodel.DisplayName = "Program-Edit";
            viewmodel.commandType = CommandType.Edit;
            var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            ProgramViewInstance.DataContext = viewmodel;
            ProgramViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)     //Add Remove操作，就是将model.Recipes里面的集合内容改变了
            {
                List<RecipeClass> TobeRemoved = new List<RecipeClass>();
                List<RecipeClass> TobeAdded = new List<RecipeClass>();
                //先改数据库，这样可以使得Id准确
                using (var dbContext = new AppDbContext())
                {
                    var dbProgram = dbContext.Programs.SingleOrDefault(i => i.Id == SelectedProgram.Id);  //没有完全取出
                    dbContext.Entry(dbProgram)
                        .Collection(p => p.Recipes)
                        .Load();

                    bool isTgtNotContainSrc = false;    //Add to target
                    bool isSrcNotContainTgt = false;    //Remove from target
                    foreach (var sub_target in dbProgram.Recipes)     //看看在不在source中，不在则删掉
                    {
                        isSrcNotContainTgt = true;
                        foreach (var sub_source in model.Recipes)
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
                    foreach (var sub_source in model.Recipes)
                    {
                        isTgtNotContainSrc = true;
                        foreach (var sub_target in dbProgram.Recipes)
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

                    //foreach (var sub in TobeRemoved)
                    //{
                    //    sub.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Id == sub.ChargeTemperature.Id);
                    //    sub.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Id == sub.ChargeCurrent.Id);
                    //    sub.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Id == sub.DischargeTemperature.Id);
                    //    sub.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Id == sub.DischargeCurrent.Id);
                    //    //newP.Recipes.Add(dbContext.Recipes.SingleOrDefault(o => o.Id == sub.Id));
                    //}
                    foreach (var sub in TobeRemoved)
                    {
                        dbProgram.Recipes.Remove(sub);
                    }

                    foreach (var sub in TobeAdded)
                    {
                        //sub.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Id == sub.ChargeTemperature.Id);
                        //sub.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Id == sub.ChargeCurrent.Id);
                        //sub.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Id == sub.DischargeTemperature.Id);
                        //sub.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Id == sub.DischargeCurrent.Id);
                        //newP.Recipes.Add(dbContext.Recipes.SingleOrDefault(o => o.Id == sub.Id));
                    }
                    foreach (var sub in TobeAdded)
                    {
                        dbProgram.Recipes.Add(sub);
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
                    SelectedProgram.Recipes.Remove(SelectedProgram.Recipes.SingleOrDefault(o => o.Id == sub.Id));
                }
                foreach (var sub in TobeAdded)
                {
                    SelectedProgram.Recipes.Add(new RecipeViewModel(sub));
                }
            }

                //model.Recipes = _selectedProgram._program.Recipes;
                /*
                ProgramClass model = _selectedProgram._program.Clone();
                ProgramEditViewModel viewmodel = new ProgramEditViewModel(model, _RecipeTemplates);      //实例化一个新的view model
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
                            .Collection(p => p.Recipes)
                            .Load();
                        //program.Update(model);
                        bool isTgtNotContainSrc = false;    //Add to target
                        bool isSrcNotContainTgt = false;    //Remove from target
                        List<RecipeClass> TobeRemoved = new List<RecipeClass>();
                        List<RecipeClass> TobeAdded = new List<RecipeClass>();
                        foreach (var sub_target in program.Recipes)     //看看在不在source中，不在则删掉
                        {
                            isSrcNotContainTgt = true;
                            foreach (var sub_source in model.Recipes)
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
                        foreach (var sub_source in model.Recipes)
                        {
                            isTgtNotContainSrc = true;
                            foreach (var sub_target in program.Recipes)
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
                            program.Recipes.Remove(sub);
                            //var subs = dbContext.Recipes;       //手动递归删除
                            //subs.Remove(sub);
                        }
                        foreach (var sub in TobeAdded)
                        {
                            program.Recipes.Add(sub);
                        }
                        dbContext.SaveChanges();
                        SelectedProgram._program = program;

                        //SelectedProgram.UpdateRecipes();        //修改viewmodel中的子项

                        List<RecipeViewModel> all =
                            (from sub in program.Recipes
                             select new RecipeViewModel(sub)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

                        //foreach (RecipeModelViewModel batmod in all)
                        //batmod.PropertyChanged += this.OnRecipeModelViewModelPropertyChanged;

                        SelectedProgram.Recipes = new ObservableCollection<RecipeViewModel>(all);     //再转换成Observable
                        RaisePropertyChanged("Recipes");
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
            //ProgramClass m = _selectedProgram._program.Clone();
            //ProgramEditViewModel evm = new ProgramEditViewModel(m, _batteryTypes, _RecipeTemplates);      //实例化一个新的view model
            ////evm.DisplayName = "Program-Save As";
            //evm.commandType = CommandType.SaveAs;
            //var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            //ProgramViewInstance.DataContext = evm;
            //ProgramViewInstance.ShowDialog();
            //if (evm.IsOK == true)
            //{
            //    //_programRepository.AddItem(model);
            //    using (var dbContext = new AppDbContext())
            //    {
            //        foreach (var sub in m.Recipes)
            //        {
            //            sub.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Id == sub.ChargeTemperature.Id);
            //            sub.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Id == sub.ChargeCurrent.Id);
            //            sub.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Id == sub.DischargeTemperature.Id);
            //            sub.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Id == sub.DischargeCurrent.Id);
            //            newP.Recipes.Add(dbContext.Recipes.SingleOrDefault(o => o.Id == sub.Id));
            //        }
            //        m.BatteryType = dbContext.BatteryTypes.SingleOrDefault(o => o.Id == m.BatteryType.Id);
            //        dbContext.Programs.Add(m);
            //        dbContext.SaveChanges();
            //    }
            //    _programs.Add(m);
            //    this.AllPrograms.Add(new ProgramViewModel(m));
            //}
        }
        private bool CanSaveAs
        {
            get { return _selectedProgram != null; }
        }
        private void Abandon()
        {
            if (MessageBox.Show("Are you sure?", "Abandon Recipe", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //_selectedRecipe._Recipe.Abandon();    //不需要。因为下面一行代码会做一样的事情。如果这里做了，反而会导致UI无法更新
                _selectedRecipe.Abandon();
                using (var dbContext = new AppDbContext())
                {
                    var model = dbContext.Recipes.SingleOrDefault(o => o.Id == _selectedRecipe.Id);
                    dbContext.Entry(model)
                        .Collection(o => o.TestRecords)
                        .Load();
                    model.Abandon();
                    dbContext.SaveChanges();
                }
            }
        }
        private bool CanAbandon
        {
            get { return _selectedRecipe != null; }
        }
        private void Execute()
        {
            Execute(SelectedTestRecord);
        }
        private bool CanExecute
        {
            get { return _selectedTestRecord != null && _selectedTestRecord.Status == TestStatus.Waiting; }
        }
        private void Commit()
        {
            Commit(SelectedTestRecord);
        }
        private bool CanCommit
        {
            get { return _selectedTestRecord != null && _selectedTestRecord.Status == TestStatus.Executing; }
        }
        private void Invalidate()
        {
            Invalidate(SelectedTestRecord);
        }
        private bool CanInvalidate
        {
            get { return _selectedTestRecord != null && _selectedTestRecord.Status == TestStatus.Completed; }
        }
        private void View()
        {
            ViewRawData(SelectedTestRecord);
        }
        private bool CanView
        {
            get { return _selectedTestRecord != null && (_selectedTestRecord.Record.RawDataList.Count!=0); }
        }
        #endregion //Private Helper


        private void Execute(TestRecordViewModel testRecord)                                      
            //相当于Edit，需要修改TestRecord的属性（vm和m层面都要修改），保存到数据库。还需要修改Assets的属性（vm和m层面都要修改），保存到数据库
        {
            /*ProgramClass model = _selectedProgram._program.Clone();
            ProgramViewModel viewmodel = new ProgramViewModel(model, _programRepository, _RecipeRepository);      //实例化一个新的view model
            viewmodel.DisplayName = "Program-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            ProgramViewInstance.DataContext = viewmodel;
            ProgramViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _programRepository.AddItem(model);
            }*/
            var model = new TestRecordClass();
            TestRecordExecuteViewModel evm = new TestRecordExecuteViewModel
                (
                //testRecord.Record,      //将model传给ExecuteViewModel      //??????????????????????????
                model,
                _batteryTypes,
                _batteries,
                _testers,
                _channels,
                _chambers
                );
            //evm.DisplayName = "Test-Execute";
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
                testRecord.ExecuteOnAssets(evm.Battery, evm.Chamber, evm.Channel,SelectedProgram.Name, SelectedRecipe.Name);      //将evm的Assets传给testRecord
                testRecord.ExecuteUpdateTime(_selectedProgram._program, _selectedRecipe._recipe);
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
            //    SelectedTestRecord.CompleteTime = viewmodel.CompleteTime;
            //    SelectedTestRecord.NewCycle = viewmodel.NewCycle;
            //    SelectedTestRecord.Comment = viewmodel.Comment;
            //    SelectedTestRecord.Commit();
            //}

            TestRecordClass m = new TestRecordClass();
            TestRecordCommitViewModel evm = new TestRecordCommitViewModel
                (
                //testRecord.Record      //??????????????????????????
                m
                );
            //evm.DisplayName = "Test-Commit";
            var TestRecordCommitViewInstance = new CommitView();
            TestRecordCommitViewInstance.DataContext = evm;
            TestRecordCommitViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                testRecord.CompleteTime = evm.CompleteTime;
                testRecord.NewCycle = evm.NewCycle;
                testRecord.Comment = evm.Comment;
                testRecord.Record.RawDataList = CreateRawDataList(evm.FileList);
                testRecord.Status = TestStatus.Completed;
                testRecord.CommitOnAssets();
                using (var dbContext = new AppDbContext())
                {
                    var tr = dbContext.TestRecords.SingleOrDefault(i => i.Id == testRecord.Id);
                    tr.CompleteTime = testRecord.CompleteTime;
                    tr.NewCycle = testRecord.NewCycle;
                    tr.Comment = testRecord.Comment;
                    tr.Status = testRecord.Status;
                    tr.RawDataList = testRecord.Record.RawDataList;
                    tr.AssignedBattery = null;
                    tr.AssignedChamber = null;
                    tr.AssignedChannel = null;
                    dbContext.SaveChanges();
                }
                testRecord.CommitUpdateTime(_selectedProgram._program, _selectedRecipe._recipe);
            }
        }

        private List<RawDataClass> CreateRawDataList(ObservableCollection<string> fileList)
        {
            List<RawDataClass> output = new List<RawDataClass>();
            foreach (var filepath in fileList)
            {
                var rd = new RawDataClass();
                //rd.FileName = Path.GetFileName(filepath);
                rd.FileName = filepath;
                //rd.BinaryData = File.ReadAllBytes(filepath);
                output.Add(rd);
            }
            return output;
        }
        private void Invalidate(TestRecordViewModel testRecord)
        {
            TestRecordInvalidateViewModel evm = new TestRecordInvalidateViewModel();
            //evm.DisplayName = "Test-Invalidate";
            var TestRecordInvalidateViewInstance = new InvalidateView();
            TestRecordInvalidateViewInstance.DataContext = evm;
            TestRecordInvalidateViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                testRecord.Comment += "\r\n" + evm.Comment;
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
            //evm.DisplayName = "Test-View Raw Data";
            var TestRecordRawDataViewInstance = new RawDataView();
            TestRecordRawDataViewInstance.DataContext = evm;
            TestRecordRawDataViewInstance.ShowDialog();
        }
    }
}
