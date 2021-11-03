//#define TemplateUpgrade
//#define Migrate
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
using Prism.Mvvm;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Text.Json;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class MainWindowViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields
        public BatteryTypeServiceClass BatteryTypeService { get; set; } = new BatteryTypeServiceClass();
        public BatteryServiceClass BatteryService { get; set; } = new BatteryServiceClass();
        public TesterServiceClass TesterService { get; set; } = new TesterServiceClass();
        public ChannelServiceClass ChannelService { get; set; } = new ChannelServiceClass();
        public ChamberServiceClass ChamberService { get; set; } = new ChamberServiceClass();
        //public TestRecordServiceClass TestRecordService { get; set; } = new TestRecordServiceClass();
        public RecipeTemplateServiceClass RecipeTemplateService { get; set; } = new RecipeTemplateServiceClass();
        public ProtectionServiceClass ProtectionService { get; set; } = new ProtectionServiceClass();
        public StepServiceClass StepService { get; set; } = new StepServiceClass();
        public StepV2ServiceClass StepV2Service { get; set; } = new StepV2ServiceClass();
        public TesterActionServiceClass TesterActionService { get; set; } = new TesterActionServiceClass();
        public CutOffConditionServiceClass CutOffConditionService { get; set; } = new CutOffConditionServiceClass();
        public CutOffBehaviorServiceClass CutOffBehaviorService { get; set; } = new CutOffBehaviorServiceClass();
        public JumpBehaviorServiceClass JumpBehaviorService { get; set; } = new JumpBehaviorServiceClass();
        public StepTemplateServiceClass StepTemplateService { get; set; } = new StepTemplateServiceClass();
        public ProgramServiceClass ProgramService { get; set; } = new ProgramServiceClass();
        public ProjectServiceClass ProjectService { get; set; } = new ProjectServiceClass();
        public ProjectSettingServiceClass ProjectSettingService { get; set; } = new ProjectSettingServiceClass();
        public ProgramTypeServiceClass ProgramTypeService { get; set; } = new ProgramTypeServiceClass();
        public TableMakerProductTypeServiceClass TableMakerProductTypeService { get; set; } = new TableMakerProductTypeServiceClass();
        public TableMakerRecordServiceClass TableMakerRecordService { get; set; } = new TableMakerRecordServiceClass();
        public TestRecordServiceClass FreeTestRecordService { get; set; } = new TestRecordServiceClass();

        #endregion // Fields

        #region Constructor

        public MainWindowViewModel(
            )     //
        {
            //try
            {
                InitializeRuningLogFolder();
                LoadConfigration();
                Stopwatch sw = new Stopwatch();
                sw.Start();
                InitializeDatabase();
                RuningLog.Write($"InitializeDatabase spend {sw.ElapsedMilliseconds} milliseconds\n");
                sw.Restart();
                try
                {
                    LoadFromDB();
                }
                catch (Exception e)
                {
                    throw new DatabaseAccessException(e.Message, e.InnerException);
                }
                RuningLog.Write($"LoadFromDB spend {sw.ElapsedMilliseconds} milliseconds\n");
                sw.Restart();
                InitializeFolder();
                RuningLog.Write($"InitializeFolder spend {sw.ElapsedMilliseconds} milliseconds\n");
                sw.Restart();
                //InitializeTempFileFolder();
                CreateProcesserForTesters();
                RuningLog.Write($"CreateProcesserForTesters spend {sw.ElapsedMilliseconds} milliseconds\n");
                sw.Restart();
                CreateViewModels();
                RuningLog.Write($"CreateViewModels spend {sw.ElapsedMilliseconds} milliseconds\n");
                sw.Restart();
                //以下三个函数只用调用一次即可
                //UpdateStatus();
                //UpdateTime();
                //UpdateEditable();
            }
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //}
        }

        private void LoadConfigration()
        {
            if (!File.Exists(GlobalSettings.ConfigurationFilePath))
            {
                Configuration conf = new Configuration();
                conf.RemotePath = GlobalSettings.RemotePath;
                conf.EnableTest = GlobalSettings.EnableTest;
                conf.MappingPath = GlobalSettings.MappingPath;
                conf.DatabaseHost = GlobalSettings.DatabaseHost;
                conf.DatabaseName = GlobalSettings.DatabaseName;
                conf.DatabaseUser = GlobalSettings.DatabaseUser;
                conf.DatabasePassword = GlobalSettings.DatabasePassword;
                string jsonString = JsonSerializer.Serialize(conf);
                File.WriteAllText(GlobalSettings.ConfigurationFilePath, jsonString);
            }
            else
            {
                string jsonString = File.ReadAllText(GlobalSettings.ConfigurationFilePath);
                Configuration conf = JsonSerializer.Deserialize<Configuration>(jsonString);
                GlobalSettings.RemotePath = conf.RemotePath;
                GlobalSettings.EnableTest = conf.EnableTest;
                GlobalSettings.MappingPath = conf.MappingPath;
                GlobalSettings.DatabaseHost = conf.DatabaseHost;
                GlobalSettings.DatabaseName = conf.DatabaseName;
                GlobalSettings.DatabaseUser = conf.DatabaseUser;
                GlobalSettings.DatabasePassword = conf.DatabasePassword;
            }
        }

        private void UpdateEditable()
        {
            List<RecipeTemplate> UsedTemplates = new List<RecipeTemplate>();
            foreach (var pro in ProgramService.Items)
            {
                foreach (var rec in pro.Recipes)
                {
                    if (!UsedTemplates.Contains(rec.RecipeTemplate))
                        UsedTemplates.Add(rec.RecipeTemplate);
                }
            }
            foreach (var rectemp in ProgramService.RecipeService.RecipeTemplateService.Items)
            {
                if (!UsedTemplates.Contains(rectemp))
                {
                    rectemp.Editable = true;
                    ProgramService.RecipeService.RecipeTemplateService.SuperUpdate(rectemp);
                }
            }
        }

        private void UpdateStatus()
        {
            foreach (var rec in ProgramService.RecipeService.Items)
            {
                if (rec.IsAbandoned == false)
                    if (rec.TestRecords.All(o => o.Status == TestStatus.Abandoned))
                    {
                        rec.IsAbandoned = true;
                        ProgramService.RecipeService.SuperUpdate(rec);
                    }
            }
            foreach (var pro in ProgramService.Items)
            {
                if (pro.IsInvalid == false)
                    if (pro.Recipes.All(o => o.IsAbandoned == true))
                    {
                        pro.IsInvalid = true;
                        ProgramService.SuperUpdate(pro);
                    }
            }
        }

        private void UpdateTime()
        {
            foreach (var rec in ProgramService.RecipeService.Items)
            {
                if (rec.IsAbandoned == false)
                    if (rec.StartTime == null || rec.StartTime == DateTime.MinValue)
                        ProgramService.RecipeService.UpdateTime(rec);
            }
            foreach (var pro in ProgramService.Items)
            {
                if (pro.IsInvalid == false)
                    if (pro.StartTime == null || pro.StartTime == DateTime.MinValue)
                        ProgramService.UpdateTime(pro);
            }
        }

        private void InitializeFolder()
        {
            InitializeLocalFolder();
            //InitializeRemoteFolder();
            InitializeProjectFolder();
            InitializeTempFileFolder();
            //foreach (var prj in ProjectService.Items)
            //{
            //    ProjectService.CreateFolder(prj.BatteryType.Name, prj.Name);
            //}
        }

        private void InitializeLocalFolder()
        {
            if (!Directory.Exists(GlobalSettings.LocalFolder))
                Directory.CreateDirectory(GlobalSettings.LocalFolder);
        }

        private void InitializeRemoteFolder()
        {
            try
            {
                if (!Directory.Exists(GlobalSettings.RemotePath))
                    Directory.CreateDirectory(GlobalSettings.RemotePath);
            }
            catch (Exception e)
            {
                Event evt = new Event();
                evt.Module = Module.FileOperation;
                evt.Timestamp = DateTime.Now;
                evt.Type = EventType.Error;
                evt.Description = $"Cannot access NAS {GlobalSettings.RemotePath}.";
                EventService.SuperAdd(evt);
                throw e;
            }
        }

        private void InitializeProjectFolder()
        {
            foreach (var prj in ProjectService.Items)
            {
                ProjectService.CreateFolder(prj.BatteryType.Name, prj.Name);
            }
        }

        private void InitializeTempFileFolder()
        {
            string tempFilePath = $@"{GlobalSettings.UniversalPath}{GlobalSettings.TempDataFolderName}";
            if (!Directory.Exists(tempFilePath))
                Directory.CreateDirectory(tempFilePath);

            tempFilePath = $@"{GlobalSettings.LocalFolder}{GlobalSettings.TempDataFolderName}";
            if (!Directory.Exists(tempFilePath))
                Directory.CreateDirectory(tempFilePath);
        }

        private void InitializeRuningLogFolder()
        {
            if (!Directory.Exists(GlobalSettings.RunningLogFolder))
                Directory.CreateDirectory(GlobalSettings.RunningLogFolder);
        }

        private void CreateProcesserForTesters()
        {
            foreach (var tester in TesterService.Items)
            {
                tester.BuildProcesser();
            }
        }
        void InitializeDatabase()
        {
#if Migrate
            if (MessageBox.Show("Do you want to re-build DB?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Database.EnsureDeleted();
                    dbContext.Database.Migrate();
                }
                MessageBox.Show("DB Data Migration Completed!");
            }
#endif
#if Seed
            if (MessageBox.Show("Do you want to populate DB?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DatabasePopulator.PopulateHistoricData();
                MessageBox.Show("DB Data Seeding Completed!");
            }
#endif
        }

        void LoadFromDB()
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                BatteryTypeService.Items = new ObservableCollection<BatteryType>(uow.BatteryTypes.GetAll());
                BatteryService.Items = new ObservableCollection<Battery>(uow.Batteries.GetAll("BatteryType,Records"));

                TesterService.Items = new ObservableCollection<Tester>(uow.Testers.GetAll());
                ChannelService.Items = new ObservableCollection<Channel>(uow.Channels.GetAll("Tester,Records"));

                ChamberService.Items = new ObservableCollection<Chamber>(uow.Chambers.GetAll("Records"));

                //RecipeTemplateService.Items = new ObservableCollection<RecipeTemplate>(uow.RecipeTemplates.GetAll());
                //StepService.Items = new ObservableCollection<Step>(uow.Steps.GetAll());
                //StepTemplateService.Items = new ObservableCollection<StepTemplate>(uow.StepTemplates.GetAll());

                ProgramService.Items = new ObservableCollection<Program>(uow.Programs.GetAll());
                ProgramService.RecipeService.Items = new ObservableCollection<Recipe>(uow.Recipies.GetAll());
                ProgramService.RecipeService.TestRecordService.Items = new ObservableCollection<TestRecord>(uow.TestRecords.GetAll());
                ProgramService.RecipeService.StepRuntimeService.Items = new ObservableCollection<StepRuntime>(uow.StepRuntimes.GetAll());

                ProgramService.RecipeService.RecipeTemplateService.Items = new ObservableCollection<RecipeTemplate>(uow.RecipeTemplates.GetAll());
                ProgramService.RecipeService.RecipeTemplateService.RecipeTemplateGroupService.Items = new ObservableCollection<RecipeTemplateGroup>(uow.RecipeTemplateGroups.GetAll());
                ProtectionService.Items = new ObservableCollection<Protection>(uow.Protections.GetAll());
                StepService.Items = new ObservableCollection<Step>(uow.Steps.GetAll());
                StepV2Service.Items = new ObservableCollection<StepV2>(uow.StepV2s.GetAll());
                TesterActionService.Items = new ObservableCollection<TesterAction>(uow.TesterActions.GetAll());
                CutOffConditionService.Items = new ObservableCollection<CutOffCondition>(uow.CutOffConditions.GetAll());
                CutOffBehaviorService.Items = new ObservableCollection<CutOffBehavior>(uow.CutOffBehaviors.GetAll("Condition"));
                JumpBehaviorService.Items = new ObservableCollection<JumpBehavior>(uow.JumpBehaviors.GetAll("Condition"));
                StepTemplateService.Items = new ObservableCollection<StepTemplate>(uow.StepTemplates.GetAll());

                ProjectService.Items = new ObservableCollection<Project>(uow.Projects.GetAll("EmulatorResults"));
                ProjectSettingService.Items = new ObservableCollection<ProjectSetting>(uow.ProjectSettings.GetAll());
                ProgramTypeService.Items = new ObservableCollection<ProgramType>(uow.ProgramTypes.GetAll());
                TableMakerProductTypeService.Items = new ObservableCollection<TableMakerProductType>(uow.TableMakerProductTypes.GetAll());
                //ProgramService.RecipeService.StepRuntimeService.StepService.Items = new ObservableCollection<StepClass>(uow.Steps.GetAll());
                //ProgramService.RecipeService.StepRuntimeService.StepTemplateService.Items = new ObservableCollection<StepTemplate>(uow.StepTemplates.GetAll());
                TableMakerRecordService.Items = new ObservableCollection<TableMakerRecord>(uow.TableMakerRecords.GetAll("Products"));
                //FreeTestRecordService.Items = new ObservableCollection<TestRecord>(uow.TestRecords.GetAllFreeTestRecords());

                //ProgramService.RecipeTemplateService = RecipeTemplateService;
                EventService.Items = new ObservableCollection<Event>(uow.Events.GetAll());
            }
        }
#if TemplateUpgrade
        private void UpgradeTemplate()
        {
            foreach (var recT in ProgramService.RecipeService.RecipeTemplateService.Items.Where(o => o.StepV2s.Count != 0 && o.StepV2s.All(s => s.CutOffConditions.Count != 0 && s.CutOffBehaviors.Count == 0)))
            {
                foreach (var step in recT.StepV2s)
                {
                    foreach (var prot in recT.Protections)
                    {
                        var newprot = new Protection() { Parameter = prot.Parameter, Mark = prot.Mark, Value = prot.Value};
                        step.Protections.Add(newprot);
                    }
                    foreach (var coc in step.CutOffConditions)
                    {
                        var cob = ConvertCOC2COB(coc);
                        if (cob != null)
                        {
                            step.CutOffBehaviors.Add(cob);
                        }
                    }
                }
                RecipeTemplateService.SuperUpdate(recT);
            }
            //foreach (var recT in ProgramService.RecipeService.RecipeTemplateService.Items.Where(o => o.StepV2s.Count != 0 && o.StepV2s.All(s => s.CutOffBehaviors.All(c => c.JumpBehaviors.All(j=>j.Condition.Parameter == Parameter.VOLTAGE && j.Condition.Mark == CompareMarkEnum.NA)))))
            //{
            //    foreach (var step in recT.StepV2s)
            //    {
            //        foreach (var cob in step.CutOffBehaviors)
            //        {
            //            foreach (var jpb in cob.JumpBehaviors)
            //            {
            //                jpb.Condition.Parameter = Parameter.NA;
            //            }
            //        }
            //    }
            //    RecipeTemplateService.SuperUpdate(recT);
            //}
        }

        private CutOffBehavior ConvertCOC2COB(CutOffCondition coc)
        {
            CutOffBehavior cob = new CutOffBehavior();
            cob.Condition.Parameter = coc.Parameter;
            cob.Condition.Mark = coc.Mark;
            cob.Condition.Value = coc.Value;
            JumpBehavior jpb = new JumpBehavior();
            jpb.JumpType = coc.JumpType;
            jpb.Index = coc.Index;
            cob.JumpBehaviors.Add(jpb);
            return cob;
        }
#endif
        void CreateViewModels()
        {
            allBatteryTypesViewModel = new AllBatteryTypesViewModel(BatteryTypeService, BatteryService);    //ViewModel初始化

            allProjectsViewModel = new AllProjectsViewModel(TesterService, ProjectService, BatteryTypeService, ProgramService, ProjectSettingService, TableMakerRecordService);    //ViewModel初始化

            allProjectSettingsViewModel = new AllProjectSettingsViewModel(ProjectSettingService, ProjectService);    //ViewModel初始化

            allProgramTypesViewModel = new AllProgramTypesViewModel(ProgramTypeService);    //ViewModel初始化

            allTableMakerProductTypesViewModel = new AllTableMakerProductTypesViewModel(TableMakerProductTypeService);    //ViewModel初始化

            allBatteriesViewModel = new AllBatteriesViewModel(BatteryService, BatteryTypeService);    //ViewModel初始化

            allTestersViewModel = new AllTestersViewModel(TesterService, ChannelService);    //ViewModel初始化

            allChannelsViewModel = new AllChannelsViewModel(ChannelService, TesterService);    //ViewModel初始化

            allChambersViewModel = new AllChambersViewModel(ChamberService);    //ViewModel初始化

            allRecipeTemplatesViewModel =
                new AllRecipeTemplatesViewModel(
                    ProgramService.RecipeService.RecipeTemplateService, StepTemplateService, ProgramService.RecipeService.RecipeTemplateService.RecipeTemplateGroupService
                    //ChargeCurrents,
                    //DischargeTemperatures,
                    //DischargeCurrents
                    );    //ViewModel初始化

            allProgramsViewModel = new AllProgramsViewModel
                (
                ProgramService,
                ProgramService.RecipeService.RecipeTemplateService,
                StepTemplateService,
                ProjectService,
                ProgramTypeService,
                BatteryService,
                TesterService,
                ChannelService,
                ChamberService,
                BatteryTypeService,
                FreeTestRecordService
                );    //ViewModel初始化

            dashBoardViewModel = new DashBoardViewModel(BatteryTypeService, ProjectService, BatteryService, ChannelService, ChamberService, ProgramService);
            tableMakerViewModel = new TableMakerViewModel(ProjectService, TableMakerRecordService, ProgramService, TesterService);
        }
        #endregion // Constructor

        #region Presentation Properties
        public AllBatteryTypesViewModel allBatteryTypesViewModel { get; set; }  //其中需要显示BatteryTypes和Batteries
        public AllProjectsViewModel allProjectsViewModel { get; set; }  //其中需要显示Projects
        public AllProjectSettingsViewModel allProjectSettingsViewModel { get; set; }  //其中需要显示Projects
        public AllProgramTypesViewModel allProgramTypesViewModel { get; set; }  //其中需要显示Projects
        public AllTableMakerProductTypesViewModel allTableMakerProductTypesViewModel { get; set; }  //
        public AllBatteriesViewModel allBatteriesViewModel { get; set; }  //其中需要显示Batteries和Records

        public AllTestersViewModel allTestersViewModel { get; set; }  //其中需要显示Testers和Channels
        public AllChannelsViewModel allChannelsViewModel { get; set; }  //其中需要显示Channels和Records

        public AllChambersViewModel allChambersViewModel { get; set; }  //其中需要显示Chambers和Records

        public AllRecipeTemplatesViewModel allRecipeTemplatesViewModel { get; set; }  //其中需要显示Recipes

        public AllProgramsViewModel allProgramsViewModel { get; set; }  //其中需要显示Programs, Recipes, Test1, Test2, TestSteps

        public DashBoardViewModel dashBoardViewModel { get; set; }
        public TableMakerViewModel tableMakerViewModel { get; set; }
        #endregion // Presentation Properties

    }
}
