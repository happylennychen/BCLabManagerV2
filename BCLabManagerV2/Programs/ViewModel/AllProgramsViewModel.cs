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
        //List<RecipeTemplate> _recipeTemplateService.Items;
        ProgramViewModel _selectedProgram;
        RecipeViewModel _selectedRecipe;
        TestRecordViewModel _selectedTestRecord;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _abandonCommand;
        RelayCommand _executeCommand;
        RelayCommand _commitCommand;
        RelayCommand _invalidateCommand;
        RelayCommand _viewCommand;
        RelayCommand _startCommand;
        RelayCommand _endCommand;

        //ObservableCollection<BatteryTypeClass> _batteryTypes;
        //ObservableCollection<TesterClass> _testers;
        //ObservableCollection<ChannelClass> _channels;
        //ObservableCollection<ChamberClass> _chambers;
        //ObservableCollection<ProgramClass> _programs;
        private BatteryServieClass _batteryService;
        private BatteryTypeServieClass _batteryTypeService;
        private TesterServieClass _testerService;
        private ChannelServieClass _channelService;
        private ChamberServieClass _chamberService;
        private ProgramServiceClass _programService;
        private RecipeTemplateServiceClass _recipeTemplateService;
        #endregion // Fields

        #region Constructor

        public AllProgramsViewModel
            (
            ProgramServiceClass programService, 
            RecipeTemplateServiceClass recipeTemplateService,
            BatteryTypeServieClass batteryTypeService,
            BatteryServieClass batteryService,
            TesterServieClass testerService,
            ChannelServieClass channelService,
            ChamberServieClass chamberService
            )
        {
            _recipeTemplateService = recipeTemplateService;
            _programService = programService;
            this.CreateAllPrograms(_programService.Items);

            _batteryTypeService = batteryTypeService;
            _batteryService = batteryService;
            _testerService = testerService;
            _channelService = channelService;
            _chamberService = chamberService;

            foreach(var recipe in _programService.RecipeService.Items)
                recipe.TestRecords.CollectionChanged += TestRecords_CollectionChanged;

            _programService.Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var program = item as ProgramClass;
                        this.AllPrograms.Add(new ProgramViewModel(program));
                    }
                    break;
            }
        }

        private void TestRecords_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var testRecord = item as TestRecordClass; 
                        SelectedRecipe.TestRecords.Add(new TestRecordViewModel(testRecord));
                    }
                    break;
            }
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
                    RaisePropertyChanged("TestRecords"); //通知TestRecords改变
                    RaisePropertyChanged("StepRuntimes"); //通知TestRecords改变
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

        public ObservableCollection<StepRuntimeViewModel> StepRuntimes
        {
            get
            {
                if (_selectedRecipe != null)
                    return _selectedRecipe.StepRuntimes;
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
        public ICommand StartCommand
        {
            get
            {
                if (_startCommand == null)
                {
                    _startCommand = new RelayCommand(
                        param => { this.Start(); },
                        param => this.CanStart
                        );
                }
                return _startCommand;
            }
        }
        public ICommand EndCommand
        {
            get
            {
                if (_endCommand == null)
                {
                    _endCommand = new RelayCommand(
                        param => { this.End(); },
                        param => this.CanEnd
                        );
                }
                return _endCommand;
            }
        }
        #endregion // Public Interface

        #region Private Helper
        private void Create()
        {
            ProgramClass m = new ProgramClass();      //实例化一个新的model
            ProgramEditViewModel evm = new ProgramEditViewModel(m,_batteryTypeService.Items, _recipeTemplateService.Items);      //实例化一个新的view model
            //evm.DisplayName = "Program-Create";
            evm.commandType = CommandType.Create;
            var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            ProgramViewInstance.DataContext = evm;
            ProgramViewInstance.ShowDialog();                   //设置viewmodel属性
            if (evm.IsOK == true)
            {
                _programService.SuperAdd(m);
            }
        }
        private void Edit()
        {
            var oldpro = _selectedProgram._program;
            var oldsubs = oldpro.Recipes;

            ProgramClass model = new ProgramClass();    //Edit Window要用到的model
            model.Id = oldpro.Id;
            model.Name = oldpro.Name;
            model.Requester = oldpro.Requester;
            model.RequestTime = oldpro.RequestTime;
            model.Description = oldpro.Description;
            model.Recipes = new ObservableCollection<RecipeClass>(oldsubs);          //这里并不希望在edit window里面修改原本的Recipes，而是想编辑一个新的Recipe,只是这个新的，是旧集合的浅复制

            ProgramEditViewModel viewmodel = new ProgramEditViewModel(model, _batteryTypeService.Items, _recipeTemplateService.Items);      //实例化一个新的view model
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
                ProgramEditViewModel viewmodel = new ProgramEditViewModel(model, _recipeTemplateService.Items);      //实例化一个新的view model
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
            ProgramClass m = _selectedProgram._program.Clone();
            ProgramEditViewModel evm = new ProgramEditViewModel(m, _batteryTypeService.Items, _recipeTemplateService.Items);      //实例化一个新的view model
            ////evm.DisplayName = "Program-Save As";
            //evm.commandType = CommandType.SaveAs;
            var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            ProgramViewInstance.DataContext = evm;
            ProgramViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                _programService.SuperAdd(m);
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedProgram != null; }
        }
        private void Abandon()
        {
            if (MessageBox.Show("Are you sure?", "Abandon Recipe", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _programService.RecipeService.Abandon(SelectedRecipe._recipe);
            }
        }
        private bool CanAbandon
        {
            get { return _selectedRecipe != null; }
        }
        private void Execute()
        //相当于Edit，需要修改TestRecord的属性（vm和m层面都要修改），保存到数据库。还需要修改Assets的属性（vm和m层面都要修改），保存到数据库
        {
            var model = new TestRecordClass();
            TestRecordExecuteViewModel evm = new TestRecordExecuteViewModel
                (
                //testRecord.Record,      //将model传给ExecuteViewModel      //??????????????????????????
                model,
                _batteryTypeService.Items,
                _batteryService.Items,
                _testerService.Items,
                _channelService.Items,
                _chamberService.Items
                );
            //evm.DisplayName = "Test-Execute";
            var TestRecordViewInstance = new ExecuteView();
            TestRecordViewInstance.DataContext = evm;
            TestRecordViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                _programService.RecipeService.TestRecordService.Execute(SelectedTestRecord.Record, evm.BatteryType.Name, evm.Battery, evm.Chamber, evm.Tester.Name, evm.Channel, evm.StartTime);
                _batteryService.Execute(evm.Battery, evm.StartTime, SelectedProgram.Name, SelectedRecipe.Name);
                _channelService.Execute(evm.Channel, evm.StartTime, SelectedProgram.Name, SelectedRecipe.Name);
                _chamberService.Execute(evm.Chamber, evm.StartTime, SelectedProgram.Name, SelectedRecipe.Name);
            }
        }
        private bool CanExecute
        {
            get { return _selectedTestRecord != null && _selectedTestRecord.Status == TestStatus.Waiting; }
        }
        private void Commit()
        //相当于Edit，需要修改TestRecord的属性（vm和m层面都要修改），保存到数据库。还需要修改Assets的属性（vm和m层面都要修改），保存到数据库
        {
            TestRecordViewModel testRecord = SelectedTestRecord;
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
                _batteryService.Commit(testRecord.Record.AssignedBattery, evm.CompleteTime, SelectedProgram.Name, SelectedRecipe.Name, evm.NewCycle);
                _channelService.Commit(testRecord.Record.AssignedChannel, evm.CompleteTime, SelectedProgram.Name, SelectedRecipe.Name);
                _chamberService.Commit(testRecord.Record.AssignedChamber, evm.CompleteTime, SelectedProgram.Name, SelectedRecipe.Name);
                _programService.RecipeService.TestRecordService.Commit(testRecord.Record, evm.Comment, CreateRawDataList(evm.FileList), evm.CompleteTime, SelectedProgram.Name, SelectedRecipe.Name);
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
        private bool CanCommit
        {
            get { return _selectedTestRecord != null && _selectedTestRecord.Status == TestStatus.Executing; }
        }
        private void Invalidate()
        {
            TestRecordViewModel testRecord = SelectedTestRecord;
            TestRecordInvalidateViewModel evm = new TestRecordInvalidateViewModel();
            //evm.DisplayName = "Test-Invalidate";
            var TestRecordInvalidateViewInstance = new InvalidateView();
            TestRecordInvalidateViewInstance.DataContext = evm;
            TestRecordInvalidateViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                _programService.RecipeService.Invalidate(SelectedRecipe._recipe, testRecord.Record, evm.Comment);
            }
        }
        private bool CanInvalidate
        {
            get { return _selectedTestRecord != null && _selectedTestRecord.Status == TestStatus.Completed; }
        }
        private void View()
        {
            TestRecordViewModel testRecordVM = SelectedTestRecord;
            TestRecordRawDataViewModel evm = new TestRecordRawDataViewModel
                (
                testRecordVM.Record      //??????????????????????????
                );
            //evm.DisplayName = "Test-View Raw Data";
            var TestRecordRawDataViewInstance = new RawDataView();
            TestRecordRawDataViewInstance.DataContext = evm;
            TestRecordRawDataViewInstance.ShowDialog();
        }
        private bool CanView
        {
            get { return _selectedTestRecord != null && (_selectedTestRecord.Record.RawDataList.Count!=0); }
        }
        private void Start()
        {
        }
        private bool CanStart
        {
            get { return true; }
        }
        private void End()
        {
        }
        private bool CanEnd
        {
            get { return true; }
        }
        #endregion //Private Helper
    }
}
