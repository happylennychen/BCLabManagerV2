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
        StepRuntimeViewModel _selectedStepRuntime;
        RelayCommand _createCommand;
        RelayCommand _createRCProgramCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _programInvalidateCommand;
        RelayCommand _abandonCommand;
        RelayCommand _directCommitCommand;
        RelayCommand _executeCommand;
        RelayCommand _commitCommand;
        RelayCommand _invalidateCommand;
        RelayCommand _viewCommand;
        RelayCommand _startCommand;
        RelayCommand _endCommand;
        RelayCommand _addCommand;

        //ObservableCollection<BatteryTypeClass> _batteryTypes;
        //ObservableCollection<Tester> _testers;
        //ObservableCollection<Channel> _channels;
        //ObservableCollection<ChamberClass> _chambers;
        //ObservableCollection<ProgramClass> _programs;
        private BatteryServiceClass _batteryService;
        private ProjectServiceClass _projectService;
        private ProgramTypeServiceClass _programTypeService;
        private TesterServiceClass _testerService;
        private ChannelServiceClass _channelService;
        private ChamberServiceClass _chamberService;
        private ProgramServiceClass _programService;
        private RecipeTemplateServiceClass _recipeTemplateService;
        private StepTemplateServiceClass _stepTemplateService;
        private BatteryTypeServiceClass _batteryTypeService;
        private TestRecordServiceClass _freeTestRecordService;
        #endregion // Fields

        #region Constructor

        public AllProgramsViewModel
            (
            ProgramServiceClass programService,
            RecipeTemplateServiceClass recipeTemplateService,
            StepTemplateServiceClass stepTemplateService,
            ProjectServiceClass projectService,
            ProgramTypeServiceClass programTypeService,
            BatteryServiceClass batteryService,
            TesterServiceClass testerService,
            ChannelServiceClass channelService,
            ChamberServiceClass chamberService,
            BatteryTypeServiceClass batteryTypeService,
            TestRecordServiceClass freeTestRecordService
            )
        {
            _recipeTemplateService = recipeTemplateService;
            _stepTemplateService = stepTemplateService;
            _programService = programService;
            this.CreateAllPrograms(_programService.Items);

            _projectService = projectService;
            _programTypeService = programTypeService;
            _batteryService = batteryService;
            _testerService = testerService;
            _channelService = channelService;
            _chamberService = chamberService;
            _batteryTypeService = batteryTypeService;
            _freeTestRecordService = freeTestRecordService;

            foreach (var recipe in _programService.RecipeService.Items)
                recipe.TestRecords.CollectionChanged += TestRecords_CollectionChanged;

            _programService.Items.CollectionChanged += Items_CollectionChanged;

            CreateAllFreeTestRecords(_freeTestRecordService.Items);
            _freeTestRecordService.Items.CollectionChanged += TRItems_CollectionChanged;
        }

        private void TRItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var tr = item as TestRecord;
                        this.FreeTestRecords.Add(new TestRecordViewModel(tr));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var tr = item as TestRecord;
                        var deletetarget = this.FreeTestRecords.SingleOrDefault(o => o.Id == tr.Id);
                        this.FreeTestRecords.Remove(deletetarget);
                    }
                    break;
            }
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var program = item as Program;
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
                        var testRecord = item as TestRecord;
                        SelectedRecipe.TestRecords.Add(new TestRecordViewModel(testRecord));
                    }
                    break;
            }
        }

        void CreateAllPrograms(ObservableCollection<Program> programClasses)
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
                    return new ObservableCollection<RecipeViewModel>(_selectedProgram.Recipes.OrderBy(o => o.Temperature).ThenBy(o => o.Name).ToList());
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

        public List<StepRuntimeViewModel> StepRuntimes
        {
            get
            {
                if (_selectedRecipe != null)
                    return _selectedRecipe.StepRuntimes.OrderBy(o => o.StepRuntime.Order).ToList();
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

        public StepRuntimeViewModel SelectedStepRuntime
        {
            get
            {
                return _selectedStepRuntime;
            }
            set
            {
                if (_selectedStepRuntime != value)
                {
                    _selectedStepRuntime = value;
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

        public ICommand CreateRCProgramCommand
        {
            get
            {
                if (_createRCProgramCommand == null)
                {
                    _createRCProgramCommand = new RelayCommand(
                        param => { this.CreateRCProgram(); }
                        );
                }
                return _createRCProgramCommand;
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
        public ICommand ProgramInvalidateCommand
        {
            get
            {
                if (_programInvalidateCommand == null)
                {
                    _programInvalidateCommand = new RelayCommand(
                        param => { this.ProgramInvalidate(); },
                        param => this.CanSaveAs
                        );
                }
                return _programInvalidateCommand;
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
        public ICommand DirectCommitCommand
        {
            get
            {
                if (_directCommitCommand == null)
                {
                    _directCommitCommand = new RelayCommand(
                        param => { this.DirectCommit(); },
                        param => this.CanExecute
                        );
                }
                return _directCommitCommand;
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

        public ICommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                {
                    _addCommand = new RelayCommand(
                        param => { this.AddTestRecord(); },
                        param => this.CanView
                        );
                }
                return _addCommand;
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
            Program m = new Program();      //实例化一个新的model
            ProgramEditViewModel evm = new ProgramEditViewModel(m, _projectService.Items, _recipeTemplateService.Items, _programTypeService.Items);      //实例化一个新的view model
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
        private void CreateRCProgram()
        {
            Program m = new Program();      //实例化一个新的model
            m.Type = _programTypeService.Items.SingleOrDefault(o => o.Name == "RC");
            RCProgramEditViewModel evm = new RCProgramEditViewModel(m, _projectService.Items);      //实例化一个新的view model
            //evm.DisplayName = "Program-Create";
            var RCProgramViewInstance = new RCProgramView();      //实例化一个新的view
            RCProgramViewInstance.DataContext = evm;
            RCProgramViewInstance.ShowDialog();                   //设置viewmodel属性
            if (evm.IsOK == true)
            {
                _programService.RCSuperAdd(m, evm.ChargeRate, evm.IdleTime, evm.Currents.Select(o => o.Current).ToList(), evm.Temperatures.Select(o => o.Temperature).ToList(), _recipeTemplateService, _stepTemplateService);
            }
        }
        private void Edit()
        {
            //    var oldpro = _selectedProgram._program;
            //    var oldsubs = oldpro.Recipes;

            //    ProgramClass model = new ProgramClass();    //Edit Window要用到的model
            //    model.Id = oldpro.Id;
            //    model.Name = oldpro.Name;
            //    model.Requester = oldpro.Requester;
            //    model.RequestTime = oldpro.RequestTime;
            //    model.Description = oldpro.Description;
            //    model.Recipes = new ObservableCollection<RecipeClass>(oldsubs);          //这里并不希望在edit window里面修改原本的Recipes，而是想编辑一个新的Recipe,只是这个新的，是旧集合的浅复制

            //    ProgramEditViewModel viewmodel = new ProgramEditViewModel(model, _projectService.Items, _recipeTemplateService.Items);      //实例化一个新的view model
            //    //viewmodel.DisplayName = "Program-Edit";
            //    viewmodel.commandType = CommandType.Edit;
            //    var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            //    ProgramViewInstance.DataContext = viewmodel;
            //    ProgramViewInstance.ShowDialog();
            //    if (viewmodel.IsOK == true)     //Add Remove操作，就是将model.Recipes里面的集合内容改变了
            //    {
            //        List<RecipeClass> TobeRemoved = new List<RecipeClass>();
            //        List<RecipeClass> TobeAdded = new List<RecipeClass>();
            //        //先改数据库，这样可以使得Id准确
            //        using (var dbContext = new AppDbContext())
            //        {
            //            var dbProgram = dbContext.Programs.SingleOrDefault(i => i.Id == SelectedProgram.Id);  //没有完全取出
            //            dbContext.Entry(dbProgram)
            //                .Collection(p => p.Recipes)
            //                .Load();

            //            bool isTgtNotContainSrc = false;    //Add to target
            //            bool isSrcNotContainTgt = false;    //Remove from target
            //            foreach (var sub_target in dbProgram.Recipes)     //看看在不在source中，不在则删掉
            //            {
            //                isSrcNotContainTgt = true;
            //                foreach (var sub_source in model.Recipes)
            //                {
            //                    if (sub_target.Id == sub_source.Id)
            //                    {
            //                        isSrcNotContainTgt = false;
            //                        break;
            //                    }
            //                }
            //                if (isSrcNotContainTgt == true)
            //                    TobeRemoved.Add(sub_target);
            //            }
            //            foreach (var sub_source in model.Recipes)
            //            {
            //                isTgtNotContainSrc = true;
            //                foreach (var sub_target in dbProgram.Recipes)
            //                {
            //                    if (sub_target.Id == sub_source.Id)
            //                    {
            //                        isTgtNotContainSrc = false;
            //                        break;
            //                    }
            //                }
            //                if (isTgtNotContainSrc == true)
            //                    TobeAdded.Add(sub_source);
            //            }

            //            //foreach (var sub in TobeRemoved)
            //            //{
            //            //    sub.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Id == sub.ChargeTemperature.Id);
            //            //    sub.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Id == sub.ChargeCurrent.Id);
            //            //    sub.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Id == sub.DischargeTemperature.Id);
            //            //    sub.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Id == sub.DischargeCurrent.Id);
            //            //    //newP.Recipes.Add(dbContext.Recipes.SingleOrDefault(o => o.Id == sub.Id));
            //            //}
            //            foreach (var sub in TobeRemoved)
            //            {
            //                dbProgram.Recipes.Remove(sub);
            //            }

            //            foreach (var sub in TobeAdded)
            //            {
            //                //sub.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Id == sub.ChargeTemperature.Id);
            //                //sub.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Id == sub.ChargeCurrent.Id);
            //                //sub.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Id == sub.DischargeTemperature.Id);
            //                //sub.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Id == sub.DischargeCurrent.Id);
            //                //newP.Recipes.Add(dbContext.Recipes.SingleOrDefault(o => o.Id == sub.Id));
            //            }
            //            foreach (var sub in TobeAdded)
            //            {
            //                dbProgram.Recipes.Add(sub);
            //            }
            //            dbContext.SaveChanges();        //TobeAdded中的元素Id改变了
            //        }
            //        //再改model
            //        foreach (var sub in TobeRemoved)
            //        {
            //            oldsubs.Remove(oldsubs.SingleOrDefault(o => o.Id == sub.Id));
            //        }
            //        foreach (var sub in TobeAdded)
            //        {
            //            oldsubs.Add(sub);
            //        }
            //        //再改view model
            //        foreach (var sub in TobeRemoved)
            //        {
            //            oldsubs.Remove(oldsubs.SingleOrDefault(o => o.Id == sub.Id));
            //            SelectedProgram.Recipes.Remove(SelectedProgram.Recipes.SingleOrDefault(o => o.Id == sub.Id));
            //        }
            //        foreach (var sub in TobeAdded)
            //        {
            //            SelectedProgram.Recipes.Add(new RecipeViewModel(sub));
            //        }
            //    }

            //    //model.Recipes = _selectedProgram._program.Recipes;
            //    /*
            //    ProgramClass model = _selectedProgram._program.Clone();
            //    ProgramEditViewModel viewmodel = new ProgramEditViewModel(model, _recipeTemplateService.Items);      //实例化一个新的view model
            //    viewmodel.DisplayName = "Program-Edit";
            //    viewmodel.commandType = CommandType.Edit;
            //    var ProgramViewInstance = new ProgramView();      //实例化一个新的view
            //    ProgramViewInstance.DataContext = viewmodel;
            //    ProgramViewInstance.ShowDialog();
            //    if (viewmodel.IsOK == true)
            //    {
            //        using (var dbContext = new AppDbContext())
            //        {
            //            var program = dbContext.Programs.SingleOrDefault(i => i.Id == SelectedProgram.Id);  //没有完全取出
            //            dbContext.Entry(program)
            //                .Collection(p => p.Recipes)
            //                .Load();
            //            //program.Update(model);
            //            bool isTgtNotContainSrc = false;    //Add to target
            //            bool isSrcNotContainTgt = false;    //Remove from target
            //            List<RecipeClass> TobeRemoved = new List<RecipeClass>();
            //            List<RecipeClass> TobeAdded = new List<RecipeClass>();
            //            foreach (var sub_target in program.Recipes)     //看看在不在source中，不在则删掉
            //            {
            //                isSrcNotContainTgt = true;
            //                foreach (var sub_source in model.Recipes)
            //                {
            //                    if (sub_target.Id == sub_source.Id)
            //                    {
            //                        isSrcNotContainTgt = false;
            //                        break;
            //                    }
            //                }
            //                if (isSrcNotContainTgt == true)
            //                    TobeRemoved.Add(sub_target);
            //            }
            //            foreach (var sub_source in model.Recipes)
            //            {
            //                isTgtNotContainSrc = true;
            //                foreach (var sub_target in program.Recipes)
            //                {
            //                    if (sub_target.Id == sub_source.Id)
            //                    {
            //                        isTgtNotContainSrc = false;
            //                        break;
            //                    }
            //                }
            //                if (isTgtNotContainSrc == true)
            //                    TobeAdded.Add(sub_source);
            //            }
            //            foreach (var sub in TobeRemoved)
            //            {
            //                program.Recipes.Remove(sub);
            //                //var subs = dbContext.Recipes;       //手动递归删除
            //                //subs.Remove(sub);
            //            }
            //            foreach (var sub in TobeAdded)
            //            {
            //                program.Recipes.Add(sub);
            //            }
            //            dbContext.SaveChanges();
            //            SelectedProgram._program = program;

            //            //SelectedProgram.UpdateRecipes();        //修改viewmodel中的子项

            //            List<RecipeViewModel> all =
            //                (from sub in program.Recipes
            //                 select new RecipeViewModel(sub)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //            //foreach (RecipeModelViewModel batmod in all)
            //            //batmod.PropertyChanged += this.OnRecipeModelViewModelPropertyChanged;

            //            SelectedProgram.Recipes = new ObservableCollection<RecipeViewModel>(all);     //再转换成Observable
            //            RaisePropertyChanged("Recipes");
            //        }
            //    }
            //        */
        }
        private bool CanEdit
        {
            get { return _selectedProgram != null; }
        }
        private void SaveAs()
        {
            Program m = _selectedProgram._program.Clone();
            ProgramEditViewModel evm = new ProgramEditViewModel(m, _projectService.Items, _recipeTemplateService.Items, _programTypeService.Items);      //实例化一个新的view model
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
        private void ProgramInvalidate()
        {
            if (MessageBox.Show("Are you sure?", "Invalidate Program", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _programService.Invalidate(_selectedProgram._program);
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
            var model = new TestRecord();
            TestRecordExecuteViewModel evm = new TestRecordExecuteViewModel
                (
                //testRecord.Record,      //将model传给ExecuteViewModel      //??????????????????????????
                model,
                SelectedProgram.Project.BatteryType,
                _batteryService.Items,
                _testerService.Items,
                _channelService.Items,
                _chamberService.Items,
                _selectedProgram.Type
                );
            evm.Temperature = _selectedRecipe.Temperature;
            if (_selectedProgram._program.Type.Name == "EV")
            {
                if (_selectedProgram.Name.Contains("Dynamic"))
                    evm.Current = 0;
            }
            else
            {
                var index = _selectedRecipe.Name.IndexOf('A');
                if (index != -1)
                {
                    var currStr = _selectedRecipe.Name.Remove(index);
                    try
                    {
                        var currInA = Convert.ToDouble(currStr);
                        evm.Current = currInA * 1000;
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show("Error when converting");
                        //return;
                    }
                }
            }
            //evm.DisplayName = "Test-Execute";
            var TestRecordViewInstance = new ExecuteView();
            TestRecordViewInstance.DataContext = evm;
            TestRecordViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                _programService.RecipeService.TestRecordService.Execute(
                    SelectedTestRecord.Record,
                    SelectedProgram.Project.BatteryType.Name,
                    SelectedProgram.Project.Name,
                    evm.Battery, evm.Chamber,
                    evm.Tester.Name,
                    evm.Channel,
                    evm.Current,
                    evm.Temperature,
                    evm.StartTime,
                    evm.MeasurementGain,
                    evm.MeasurementOffset,
                    evm.TraceResistance,
                    evm.CapacityDifference,
                    evm.Operator,
                    SelectedProgram.Name,
                    $"{SelectedRecipe.Temperature}Deg-{SelectedRecipe.Name}"    //Use this to represent RecipeStr
                    );
                if (!evm.IsSkip)
                {
                    _batteryService.Execute(evm.Battery, evm.StartTime, SelectedProgram.Name, SelectedRecipe.Name);
                    _channelService.Execute(evm.Channel, evm.StartTime, SelectedProgram.Name, SelectedRecipe.Name);
                    if (evm.Chamber != null)
                        _chamberService.Execute(evm.Chamber, evm.StartTime, SelectedProgram.Name, SelectedRecipe.Name);
                }
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
            //TestRecordClass m = new TestRecordClass();
            //m.BatteryTypeStr = testRecord.Record.BatteryTypeStr;
            //m.ProjectStr = testRecord.Record.ProjectStr;
            //m.ProgramStr = testRecord.Record.ProgramStr;
            //m.RecipeStr = testRecord.Record.RecipeStr;
            TestRecordCommitViewModel evm = new TestRecordCommitViewModel
                (
                testRecord.Record      //??????????????????????????
                                       //m
                );
            //evm.DisplayName = "Test-Commit";
            var TestRecordCommitViewInstance = new CommitView();
            TestRecordCommitViewInstance.DataContext = evm;
            TestRecordCommitViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                if (!evm.IsSkip)
                {
                    _batteryService.Commit(testRecord.Record.AssignedBattery, evm.EndTime, SelectedProgram.Name, SelectedRecipe.Name, evm.NewCycle);
                    _channelService.Commit(testRecord.Record.AssignedChannel, evm.EndTime, SelectedProgram.Name, SelectedRecipe.Name);
                    if (testRecord.Record.AssignedChamber != null)
                        _chamberService.Commit(testRecord.Record.AssignedChamber, evm.EndTime, SelectedProgram.Name, SelectedRecipe.Name);
                }
                try
                {
                    DateTime[] time = _testerService.GetTimeFromRawData(testRecord.Record.AssignedChannel.Tester.ITesterProcesser, evm.FileList);
                    if (time != null)
                    {
                        Header header = new Header();
                        header.Type = SelectedProgram.Type.ToString();
                        header.TestTime = time[0].ToString("yyyy-MM-dd");
                        header.Equipment = testRecord.Record.AssignedChannel.Tester.Manufacturer + " " + testRecord.TesterStr;
                        header.ManufactureFactory = SelectedProgram.Project.BatteryType.Manufacturer;
                        header.BatteryModel = SelectedProgram.Project.BatteryType.Name;
                        header.CycleCount = evm.NewCycle.ToString();
                        header.Temperature = testRecord.Record.Temperature.ToString();
                        header.Current = testRecord.Record.Current.ToString();
                        header.MeasurementGain = testRecord.MeasurementGain.ToString();
                        header.MeasurementOffset = testRecord.MeasurementOffset.ToString();
                        header.TraceResistance = testRecord.TraceResistance.ToString();
                        header.CapacityDifference = testRecord.CapacityDifference.ToString();
                        header.AbsoluteMaxCapacity = SelectedProgram.Project.AbsoluteMaxCapacity.ToString();//.BatteryType.TypicalCapacity.ToString();
                        header.LimitedChargeVoltage = SelectedProgram.Project.LimitedChargeVoltage.ToString();
                        //header.CutoffDischargeVoltage = SelectedProgram.Project.CutoffDischargeVoltage.ToString();
                        header.CutoffDischargeVoltage = SelectedProgram.Project.BatteryType.CutoffDischargeVoltage.ToString();
                        header.Tester = testRecord.Operator;
                        _programService.RecipeService.TestRecordService.Commit(testRecord.Record, evm.Comment, evm.FileList.ToList(), evm.IsRename, evm.NewName, time[0], time[1], SelectedProgram.Project.BatteryType.Name, SelectedProgram.Project.Name, header);
                    }
                    else
                    {
                        Header header = new Header();
                        header.Type = string.Empty;
                        _programService.RecipeService.TestRecordService.Commit(testRecord.Record, evm.Comment, evm.FileList.ToList(), evm.IsRename, evm.NewName, DateTime.MinValue, DateTime.MinValue, SelectedProgram.Project.BatteryType.Name, SelectedProgram.Project.Name, header);
                    }
                }
                catch (Exception e)
                {
                    //_programService.RecipeService.TestRecordService.Commit(testRecord.Record, evm.Comment, CreateRawDataList(evm.FileList), DateTime.MinValue, DateTime.MinValue, SelectedProgram.Project.BatteryType.Name, SelectedProgram.Project.Name, SelectedProgram.Description);
                    MessageBox.Show(e.Message);
                }
            }
        }
        private bool CanCommit
        {
            get { return _selectedTestRecord != null && _selectedTestRecord.Status == TestStatus.Executing; }
        }
        private void DirectCommit()
        //相当于Edit，需要修改TestRecord的属性（vm和m层面都要修改），保存到数据库。还需要修改Assets的属性（vm和m层面都要修改），保存到数据库
        {
            var model = new TestRecord();
            TestRecordDirectCommitViewModel evm = new TestRecordDirectCommitViewModel
                (
                model,
                SelectedProgram.Project.BatteryType,
                _batteryService.Items,
                _testerService.Items,
                _channelService.Items,
                _chamberService.Items,
                _selectedProgram.Type,
                    SelectedProgram.Name,
                    $"{SelectedRecipe.Temperature}Deg-{SelectedRecipe.Name}"
                );
            evm.Temperature = _selectedRecipe.Temperature;
            if (_selectedProgram._program.Type.Name == "EV" && _selectedProgram.Name.Contains("Dynamic"))   //Issue 2321
            {
                //if (_selectedProgram.Name.Contains("Dynamic"))
                evm.Current = 0;
            }
            else
            {
                var index = _selectedRecipe.Name.IndexOf('A');
                if (index != -1)
                {
                    var currStr = _selectedRecipe.Name.Remove(index);
                    try
                    {
                        var currInA = Convert.ToDouble(currStr);
                        evm.Current = currInA * 1000;
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show("Error when converting");
                        //return;
                    }
                }
            }
            //evm.DisplayName = "Test-Commit";
            var TestRecordDirectCommitViewInstance = new DirectCommitView();
            TestRecordDirectCommitViewInstance.DataContext = evm;
            TestRecordDirectCommitViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                try
                {

                    DateTime[] time = _testerService.GetTimeFromRawData(evm.Channel.Tester.ITesterProcesser, evm.FileList);
                    var st = new DateTime();
                    var et = new DateTime();
                    if (time != null)
                    {
                        st = time[0];
                        et = time[1];
                    }
                    else
                    {
                        st = DateTime.MinValue;
                        et = DateTime.MinValue;
                    }
                    var filePath = _programService.RecipeService.TestRecordService.DataPreProcess(SelectedTestRecord.Record, evm.FileList.ToList(),
                        evm.IsRename,
                        evm.NewName,
                        evm.StartIndex,
                        st,
                        et,
                        SelectedProgram.Project.BatteryType.Name,
                        SelectedProgram.Project.Name,
                        SelectedProgram._program,
                        SelectedRecipe._recipe,
                        evm.Tester.ITesterProcesser);
                    if (filePath == string.Empty)
                        return;
                    SelectedTestRecord.Record.TestFilePath = filePath;

                    _programService.RecipeService.TestRecordService.Execute(
                    SelectedTestRecord.Record,
                    SelectedProgram.Project.BatteryType.Name,
                    SelectedProgram.Project.Name,
                    evm.Battery, evm.Chamber,
                    evm.Tester.Name,
                    evm.Channel,
                    evm.Current,
                    evm.Temperature,
                    st,
                    evm.MeasurementGain,
                    evm.MeasurementOffset,
                    evm.TraceResistance,
                    evm.CapacityDifference,
                    evm.Operator,
                    SelectedProgram.Name,
                    $"{SelectedRecipe.Temperature}Deg-{SelectedRecipe.Name}"    //Use this to represent RecipeStr
                    );
                    _batteryService.Execute(evm.Battery, st, SelectedProgram.Name, SelectedRecipe.Name);
                    _channelService.Execute(evm.Channel, st, SelectedProgram.Name, SelectedRecipe.Name);
                    if (evm.Chamber != null)
                        _chamberService.Execute(evm.Chamber, st, SelectedProgram.Name, SelectedRecipe.Name);

                    _batteryService.Commit(evm.Battery, et, SelectedProgram.Name, SelectedRecipe.Name, evm.NewCycle);
                    _channelService.Commit(evm.Channel, et, SelectedProgram.Name, SelectedRecipe.Name);
                    if (evm.Chamber != null)
                        _chamberService.Commit(evm.Chamber, et, SelectedProgram.Name, SelectedRecipe.Name);
                    Header header = new Header();
                    if (time != null)
                    {
                        header.Type = SelectedProgram.Type.ToString();
                        header.TestTime = time[0].ToString("yyyy-MM-dd");
                        header.Equipment = evm.Channel.Tester.Manufacturer + " " + evm.Channel.Tester.Name;
                        header.ManufactureFactory = SelectedProgram.Project.BatteryType.Manufacturer;
                        header.BatteryModel = SelectedProgram.Project.BatteryType.Name;
                        header.CycleCount = evm.NewCycle.ToString();
                        header.Temperature = model.Temperature.ToString();
                        header.Current = model.Current.ToString();
                        header.MeasurementGain = model.MeasurementGain.ToString();
                        header.MeasurementOffset = model.MeasurementOffset.ToString();
                        header.TraceResistance = model.TraceResistance.ToString();
                        header.CapacityDifference = model.CapacityDifference.ToString();
                        header.AbsoluteMaxCapacity = SelectedProgram.Project.AbsoluteMaxCapacity.ToString();//.BatteryType.TypicalCapacity.ToString();
                        header.LimitedChargeVoltage = SelectedProgram.Project.LimitedChargeVoltage.ToString();
                        //header.CutoffDischargeVoltage = SelectedProgram.Project.CutoffDischargeVoltage.ToString();
                        header.CutoffDischargeVoltage = SelectedProgram.Project.BatteryType.CutoffDischargeVoltage.ToString();
                        header.Tester = model.Operator;
                    }
                    else
                    {
                        header.Type = string.Empty;
                    }
                    SelectedTestRecord.NewCycle = evm.NewCycle;
                    _programService.RecipeService.TestRecordService.CommitV2(
                    SelectedTestRecord.Record, evm.Comment, filePath, st, et);
                }
                catch (Exception e)
                {
                    //_programService.RecipeService.TestRecordService.Commit(testRecord.Record, evm.Comment, CreateRawDataList(evm.FileList), DateTime.MinValue, DateTime.MinValue, SelectedProgram.Project.BatteryType.Name, SelectedProgram.Project.Name, SelectedProgram.Description);
                    MessageBox.Show(e.Message);
                }
            }
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
            TestRecordTestDataViewModel evm = new TestRecordTestDataViewModel
                (
                testRecordVM.Record      //??????????????????????????
                );
            //evm.DisplayName = "Test-View Raw Data";
            var TestRecordRawDataViewInstance = new TestDataView();
            TestRecordRawDataViewInstance.DataContext = evm;
            TestRecordRawDataViewInstance.ShowDialog();
        }
        private bool CanView
        {
            get { return _selectedTestRecord != null && (_selectedTestRecord.Record.TestFilePath != string.Empty); }
        }
        private void AddTestRecord()
        {
            _programService.RecipeService.AddTestRecord(SelectedRecipe._recipe);
        }
        private void Start()
        {
            StepStartViewModel viewModel = new StepStartViewModel();
            StartView StartViewInstance = new StartView();
            StartViewInstance.DataContext = viewModel;
            StartViewInstance.ShowDialog();
            if (viewModel.IsOK == true)
            {
                _programService.StepStart(SelectedProgram._program, SelectedRecipe._recipe, SelectedStepRuntime.StepRuntime, viewModel.StartTime);
            }
        }
        private bool CanStart
        {
            get
            {
                if (_selectedStepRuntime != null)
                {
                    //    //var sr = _selectedStepRuntime.StepRuntime;
                    //    if (_selectedProgram == AllPrograms.First())
                    //    {
                    //        if (_selectedRecipe == _selectedProgram.Recipes.First())
                    //        {
                    if (_selectedStepRuntime == _selectedRecipe.StepRuntimes.First())
                    {
                        if (_selectedStepRuntime.StepRuntime.StartTime == DateTime.MinValue)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        var index = _selectedRecipe.StepRuntimes.IndexOf(_selectedStepRuntime);
                        var previous = _selectedRecipe.StepRuntimes[index - 1];
                        if (previous.StepRuntime.EndTime == DateTime.MinValue)
                            return false;
                        else
                        {
                            if (_selectedStepRuntime.StepRuntime.StartTime == DateTime.MinValue)
                                return true;
                            else
                                return false;
                        }
                    }
                    //    }
                    //    else
                    //    {

                    //    }
                    //}
                    //else
                    //{

                    //}
                }
                else
                    return false;
            }
        }
        private void End()
        {
            StepEndViewModel viewModel = new StepEndViewModel();
            EndView EndViewInstance = new EndView();
            EndViewInstance.DataContext = viewModel;
            EndViewInstance.ShowDialog();
            if (viewModel.IsOK == true)
            {
                _programService.StepEnd(SelectedProgram._program, SelectedRecipe._recipe, SelectedStepRuntime.StepRuntime, viewModel.EndTime);
            }
        }
        private bool CanEnd
        {
            get
            {
                if (_selectedStepRuntime != null && _selectedStepRuntime.StepRuntime.StartTime != DateTime.MinValue && _selectedStepRuntime.StepRuntime.EndTime == DateTime.MinValue)
                    return true;
                else
                    return false;
            }
        }

        #endregion //Private Helper


        #region Free Test Records



        RelayCommand _addFreeCommand;
        RelayCommand _directCommitFreeCommand;
        RelayCommand _executeFreeCommand;
        RelayCommand _commitFreeCommand;
        RelayCommand _attachCommand;


        private void CreateAllFreeTestRecords(ObservableCollection<TestRecord> items)
        {
            List<TestRecordViewModel> all =
                (from tr in items
                 select new TestRecordViewModel(tr)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.FreeTestRecords = new ObservableCollection<TestRecordViewModel>(all);     //再转换成Observable
        }
        public ObservableCollection<TestRecordViewModel> FreeTestRecords
        {
            get; set;
        }

        public TestRecordViewModel SelectedFreeTestRecord
        {
            get;
            set;
        }
        public ICommand ExecuteFreeCommand
        {
            get
            {
                if (_executeFreeCommand == null)
                {
                    _executeFreeCommand = new RelayCommand(
                        param => { this.ExecuteFree(); },
                        param => this.CanExecuteFree
                        );
                }
                return _executeFreeCommand;
            }
        }
        public ICommand CommitFreeCommand
        {
            get
            {
                if (_commitFreeCommand == null)
                {
                    _commitFreeCommand = new RelayCommand(
                        param => { this.CommitFree(); },
                        param => this.CanCommitFree
                        );
                }
                return _commitFreeCommand;
            }
        }
        public ICommand DirectCommitFreeCommand
        {
            get
            {
                if (_directCommitFreeCommand == null)
                {
                    _directCommitFreeCommand = new RelayCommand(
                        param => { this.DirectCommitFree(); },
                        param => this.CanExecuteFree
                        );
                }
                return _directCommitFreeCommand;
            }
        }

        public ICommand AddFreeCommand
        {
            get
            {
                if (_addFreeCommand == null)
                {
                    _addFreeCommand = new RelayCommand(
                        param => { this.AddFreeTestRecord(); },
                        param => this.CanAddFree
                        );
                }
                return _addFreeCommand;
            }
        }

        public ICommand AttachCommand
        {
            get
            {
                if (_attachCommand == null)
                {
                    _attachCommand = new RelayCommand(
                        param => { this.AttachTestRecord(); },
                        param => this.CanAttach
                        );
                }
                return _attachCommand;
            }
        }
        private void ExecuteFree()
        //相当于Edit，需要修改TestRecord的属性（vm和m层面都要修改），保存到数据库。还需要修改Assets的属性（vm和m层面都要修改），保存到数据库
        {
            var model = new TestRecord();
            TestRecordExecuteFreeViewModel evm = new TestRecordExecuteFreeViewModel
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
            var FreeTestRecordViewInstance = new ExecuteFreeView();
            FreeTestRecordViewInstance.DataContext = evm;
            FreeTestRecordViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                _freeTestRecordService.ExecuteFree(
                    SelectedFreeTestRecord.Record,
                    evm.Battery,
                    evm.Chamber,
                    evm.Tester.Name,
                    evm.Channel,
                    evm.StartTime,
                    evm.MeasurementGain,
                    evm.MeasurementOffset,
                    evm.TraceResistance,
                    evm.CapacityDifference,
                    evm.Operator
                    );
            }
        }
        private bool CanExecuteFree
        {
            get { return SelectedFreeTestRecord != null && SelectedFreeTestRecord.Status == TestStatus.Waiting; }
        }
        private void CommitFree()
        //相当于Edit，需要修改TestRecord的属性（vm和m层面都要修改），保存到数据库。还需要修改Assets的属性（vm和m层面都要修改），保存到数据库
        {
            TestRecordViewModel testRecord = SelectedFreeTestRecord;
            //TestRecordClass m = new TestRecordClass();
            //m.BatteryTypeStr = testRecord.Record.BatteryTypeStr;
            //m.ProjectStr = testRecord.Record.ProjectStr;
            //m.ProgramStr = testRecord.Record.ProgramStr;
            //m.RecipeStr = testRecord.Record.RecipeStr;
            TestRecordCommitFreeViewModel evm = new TestRecordCommitFreeViewModel
                (
                testRecord.Record      //??????????????????????????
                                       //m
                );
            //evm.DisplayName = "Test-Commit";
            var TestRecordCommitFreeViewInstance = new CommitFreeView();
            TestRecordCommitFreeViewInstance.DataContext = evm;
            TestRecordCommitFreeViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                try
                {
                    DateTime[] time = _testerService.GetTimeFromRawData(testRecord.Record.AssignedChannel.Tester.ITesterProcesser, evm.FileList);
                    if (time != null)
                    {
                        Header header = new Header();
                        header.TestTime = time[0].ToString("yyyy-MM-dd");
                        header.Equipment = testRecord.Record.AssignedChannel.Tester.Manufacturer + " " + testRecord.TesterStr;
                        header.ManufactureFactory = SelectedProgram.Project.BatteryType.Manufacturer;
                        header.BatteryModel = SelectedProgram.Project.BatteryType.Name;
                        header.CycleCount = evm.NewCycle.ToString();
                        header.MeasurementGain = testRecord.MeasurementGain.ToString();
                        header.MeasurementOffset = testRecord.MeasurementOffset.ToString();
                        header.TraceResistance = testRecord.TraceResistance.ToString();
                        header.CapacityDifference = testRecord.CapacityDifference.ToString();
                        header.Tester = testRecord.Operator;
                        _freeTestRecordService.CommitFree(testRecord.Record, evm.Comment, evm.FileList.ToList(), evm.IsRename, evm.NewName, time[0], time[1]);
                    }
                    else
                    {
                        Header header = new Header();
                        header.Type = string.Empty;
                        _freeTestRecordService.CommitFree(testRecord.Record, evm.Comment, evm.FileList.ToList(), evm.IsRename, evm.NewName, DateTime.MinValue, DateTime.MinValue);
                    }
                }
                catch (Exception e)
                {
                    //_programService.RecipeService.TestRecordService.Commit(testRecord.Record, evm.Comment, CreateRawDataList(evm.FileList), DateTime.MinValue, DateTime.MinValue, SelectedProgram.Project.BatteryType.Name, SelectedProgram.Project.Name, SelectedProgram.Description);
                    MessageBox.Show(e.Message);
                }
            }
        }
        private bool CanCommitFree
        {
            get { return SelectedFreeTestRecord != null && SelectedFreeTestRecord.Status == TestStatus.Executing; }
        }
        private void DirectCommitFree()
        //相当于Edit，需要修改TestRecord的属性（vm和m层面都要修改），保存到数据库。还需要修改Assets的属性（vm和m层面都要修改），保存到数据库
        {
            var model = new TestRecord();
            TestRecordDirectCommitViewModel evm = new TestRecordDirectCommitViewModel
                (
                model,
                SelectedProgram.Project.BatteryType,
                _batteryService.Items,
                _testerService.Items,
                _channelService.Items,
                _chamberService.Items,
                _selectedProgram.Type,
                    SelectedProgram.Name,
                    $"{SelectedRecipe.Temperature}Deg-{SelectedRecipe.Name}"
                );
            evm.Temperature = _selectedRecipe.Temperature;
            if (_selectedProgram._program.Type.Name == "EV")
            {
                if (_selectedProgram.Name.Contains("Dynamic"))
                    evm.Current = 0;
            }
            else
            {
                var index = _selectedRecipe.Name.IndexOf('A');
                if (index != -1)
                {
                    var currStr = _selectedRecipe.Name.Remove(index);
                    try
                    {
                        var currInA = Convert.ToDouble(currStr);
                        evm.Current = currInA * 1000;
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show("Error when converting");
                        //return;
                    }
                }
            }
            //evm.DisplayName = "Test-Commit";
            var TestRecordDirectCommitViewInstance = new DirectCommitView();
            TestRecordDirectCommitViewInstance.DataContext = evm;
            TestRecordDirectCommitViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                try
                {
                    DateTime[] time = _testerService.GetTimeFromRawData(evm.Channel.Tester.ITesterProcesser, evm.FileList);
                    var st = new DateTime();
                    var et = new DateTime();
                    if (time != null)
                    {
                        st = time[0];
                        et = time[1];
                    }
                    else
                    {
                        st = DateTime.MinValue;
                        et = DateTime.MinValue;
                    }
                    _programService.RecipeService.TestRecordService.Execute(
                    SelectedTestRecord.Record,
                    SelectedProgram.Project.BatteryType.Name,
                    SelectedProgram.Project.Name,
                    evm.Battery, evm.Chamber,
                    evm.Tester.Name,
                    evm.Channel,
                    evm.Current,
                    evm.Temperature,
                    st,
                    evm.MeasurementGain,
                    evm.MeasurementOffset,
                    evm.TraceResistance,
                    evm.CapacityDifference,
                    evm.Operator,
                    SelectedProgram.Name,
                    $"{SelectedRecipe.Temperature}Deg-{SelectedRecipe.Name}"    //Use this to represent RecipeStr
                    );
                    _batteryService.Execute(evm.Battery, st, SelectedProgram.Name, SelectedRecipe.Name);
                    _channelService.Execute(evm.Channel, st, SelectedProgram.Name, SelectedRecipe.Name);
                    if (evm.Chamber != null)
                        _chamberService.Execute(evm.Chamber, st, SelectedProgram.Name, SelectedRecipe.Name);

                    _batteryService.Commit(evm.Battery, et, SelectedProgram.Name, SelectedRecipe.Name, evm.NewCycle);
                    _channelService.Commit(evm.Channel, et, SelectedProgram.Name, SelectedRecipe.Name);
                    if (evm.Chamber != null)
                        _chamberService.Commit(evm.Chamber, et, SelectedProgram.Name, SelectedRecipe.Name);
                    Header header = new Header();
                    if (time != null)
                    {
                        header.Type = SelectedProgram.Type.ToString();
                        header.TestTime = time[0].ToString("yyyy-MM-dd");
                        header.Equipment = evm.Channel.Tester.Manufacturer + " " + evm.Channel.Tester.Name;
                        header.ManufactureFactory = SelectedProgram.Project.BatteryType.Manufacturer;
                        header.BatteryModel = SelectedProgram.Project.BatteryType.Name;
                        header.CycleCount = evm.NewCycle.ToString();
                        header.Temperature = model.Temperature.ToString();
                        header.Current = model.Current.ToString();
                        header.MeasurementGain = model.MeasurementGain.ToString();
                        header.MeasurementOffset = model.MeasurementOffset.ToString();
                        header.TraceResistance = model.TraceResistance.ToString();
                        header.CapacityDifference = model.CapacityDifference.ToString();
                        header.AbsoluteMaxCapacity = SelectedProgram.Project.AbsoluteMaxCapacity.ToString();//.BatteryType.TypicalCapacity.ToString();
                        header.LimitedChargeVoltage = SelectedProgram.Project.LimitedChargeVoltage.ToString();
                        //header.CutoffDischargeVoltage = SelectedProgram.Project.CutoffDischargeVoltage.ToString();
                        header.CutoffDischargeVoltage = SelectedProgram.Project.BatteryType.CutoffDischargeVoltage.ToString();
                        header.Tester = model.Operator;
                    }
                    else
                    {
                        header.Type = string.Empty;
                    }
                    SelectedTestRecord.NewCycle = evm.NewCycle;
                    _programService.RecipeService.TestRecordService.Commit(
                    SelectedTestRecord.Record, evm.Comment, evm.FileList.ToList(), evm.IsRename, evm.NewName, st, et, SelectedProgram.Project.BatteryType.Name, SelectedProgram.Project.Name, header);
                }
                catch (Exception e)
                {
                    //_programService.RecipeService.TestRecordService.Commit(testRecord.Record, evm.Comment, CreateRawDataList(evm.FileList), DateTime.MinValue, DateTime.MinValue, SelectedProgram.Project.BatteryType.Name, SelectedProgram.Project.Name, SelectedProgram.Description);
                    MessageBox.Show(e.Message);
                }
            }
        }
        private bool CanAddFree
        {
            get { return true; }
        }
        private void AddFreeTestRecord()
        {
            var tr = new TestRecord();
            tr.NewCycle = 0;
            _freeTestRecordService.SuperAdd(tr);
            _programService.RecipeService.TestRecordService.Items.Add(tr);
        }
        private void AttachTestRecord()
        {
            TestRecordViewModel testRecord = SelectedFreeTestRecord;
            TestRecordAttachViewModel evm = new TestRecordAttachViewModel
                (
                testRecord.Record,
                SelectedFreeTestRecord.BatteryTypeStr,
                _projectService.Items,
                _programService.Items,
                _programService.RecipeService.Items
                );
            //evm.DisplayName = "Test-Commit";
            var TestRecordAttachViewInstance = new AttachView();
            TestRecordAttachViewInstance.DataContext = evm;
            TestRecordAttachViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                try
                {
                    //_freeTestRecordService.Attach(testRecord.Record, evm.IsRename, evm.NewName, evm.Recipe);
                    SelectedRecipe = SelectedProgram.Recipes.SingleOrDefault(o => o.Id == evm.Recipe.Id);
                    _freeTestRecordService.Detach(testRecord.Record);
                    _programService.RecipeService.Attach(evm.Recipe, testRecord.Record);
                    _programService.RecipeService.TestRecordService.UpdateFreeTestRecord(testRecord.Record, evm.IsRename, evm.NewName, testRecord.Record.BatteryTypeStr, evm.Project.Name, evm.Program.Name, evm.Recipe.Name);
                }
                catch (Exception e)
                {
                    //_programService.RecipeService.TestRecordService.Commit(testRecord.Record, evm.Comment, CreateRawDataList(evm.FileList), DateTime.MinValue, DateTime.MinValue, SelectedProgram.Project.BatteryType.Name, SelectedProgram.Project.Name, SelectedProgram.Description);
                    MessageBox.Show(e.Message);
                }
            }
        }
        private bool CanAttach
        {
            get { return SelectedFreeTestRecord != null && SelectedFreeTestRecord.Status == TestStatus.Completed; }
        }
        #endregion
    }
}
