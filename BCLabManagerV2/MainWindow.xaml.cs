//#define Migrate
//#define Seed
#define Show
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BCLabManager.ViewModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Collections.ObjectModel;
using BCLabManager.View;

namespace BCLabManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private Repositories _repositories = new Repositories();    //model实例全部放在这里。viewmodel和database的source

        public AllBatteryTypesViewModel allBatteryTypesViewModel { get; set; }  //其中需要显示BatteryTypes和Batteries
        public AllProjectsViewModel allProjectsViewModel { get; set; }  //其中需要显示Projects
        public AllProjectSettingsViewModel allProjectSettingsViewModel { get; set; }  //其中需要显示Projects
        public AllProgramTypesViewModel allProgramTypesViewModel { get; set; }  //其中需要显示Projects
        public AllTableMakerProductTypesViewModel allTableMakerProductTypesViewModel { get; set; }  //
        public AllTableMakerProductsViewModel allTableMakerProductsViewModel { get; set; }  //
        public AllBatteriesViewModel allBatteriesViewModel { get; set; }  //其中需要显示Batteries和Records

        public AllTestersViewModel allTestersViewModel { get; set; }  //其中需要显示Testers和Channels
        public AllChannelsViewModel allChannelsViewModel { get; set; }  //其中需要显示Channels和Records

        public AllChambersViewModel allChambersViewModel { get; set; }  //其中需要显示Chambers和Records

        public AllStepTemplatesViewModel allStepTemplatesViewModel { get; set; }  //其中需要显示Recipes

        public AllRecipeTemplatesViewModel allRecipeTemplatesViewModel { get; set; }  //其中需要显示Recipes

        public AllProgramsViewModel allProgramsViewModel { get; set; }  //其中需要显示Programs, Recipes, Test1, Test2, TestSteps

        public DashBoardViewModel dashBoardViewModel { get; set; }

        //public ObservableCollection<BatteryTypeClass> BatteryTypes { get; set; }
        //public ObservableCollection<BatteryClass> Batteries { get; set; }
        //public ObservableCollection<Tester> Testers { get; set; }
        //public ObservableCollection<Channel> Channels { get; set; }
        //public ObservableCollection<ChamberClass> Chambers { get; set; }
        //public List<RecipeTemplate> RecipeTemplates { get; set; }
        //public List<ChargeTemperatureClass> ChargeTemperatures { get; set; }
        //public List<ChargeCurrentClass> ChargeCurrents { get; set; }
        //public List<DischargeTemperatureClass> DischargeTemperatures { get; set; }
        //public List<DischargeCurrentClass> DischargeCurrents { get; set; }
        //public ObservableCollection<ProgramClass> Programs { get; set; }

        //private DomainDataClass _domainData;

        public BatteryTypeServiceClass BatteryTypeService { get; set; } = new BatteryTypeServiceClass();
        public BatteryServiceClass BatteryService { get; set; } = new BatteryServiceClass();
        public TesterServiceClass TesterService { get; set; } = new TesterServiceClass();
        public ChannelServiceClass ChannelService { get; set; } = new ChannelServiceClass();
        public ChamberServiceClass ChamberService { get; set; } = new ChamberServiceClass();
        //public TestRecordServiceClass TestRecordService { get; set; } = new TestRecordServiceClass();
        public RecipeTemplateServiceClass RecipeTemplateService { get; set; } = new RecipeTemplateServiceClass();
        public StepServiceClass StepService { get; set; } = new StepServiceClass();
        public StepTemplateServiceClass StepTemplateService { get; set; } = new StepTemplateServiceClass();
        public ProgramServiceClass ProgramService { get; set; } = new ProgramServiceClass();
        public ProjectServiceClass ProjectService { get; set; } = new ProjectServiceClass();
        public ProjectSettingServiceClass ProjectSettingService { get; set; } = new ProjectSettingServiceClass();
        public ProgramTypeServiceClass ProgramTypeService { get; set; } = new ProgramTypeServiceClass();
        public TableMakerProductTypeServiceClass TableMakerProductTypeService { get; set; } = new TableMakerProductTypeServiceClass();
        public TableMakerProductServiceClass TableMakerProductService { get; set; } = new TableMakerProductServiceClass();
        public TestRecordServiceClass FreeTestRecordService { get; set; } = new TestRecordServiceClass();

        public MainWindow()
        {
            try
            {
                InitializeDatabase();
#if Show
                InitializeComponent();
                InitializeTempFileFolder();
                LoadFromDB();
                CreateProcesserForTesters();
                InitializeNavigator();
                CreateViewModels();
                BindingVMandView();
                ProgramService.UpdateEstimatedTimeChain();
#else
                App.Current.Shutdown();
#endif
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                App.Current.Shutdown();
            }
        }

        private void InitializeTempFileFolder()
        {
            string tempFilePath = $@"{GlobalSettings.RootPath}{GlobalSettings.TempDataFolderName}";
            if (!Directory.Exists(tempFilePath))
                Directory.CreateDirectory(tempFilePath); 

            tempFilePath = $@"{GlobalSettings.TempraryFolder}{GlobalSettings.TempDataFolderName}";
            if (!Directory.Exists(tempFilePath))
                Directory.CreateDirectory(tempFilePath);
        }

        private void CreateProcesserForTesters()
        {
            foreach (var tester in TesterService.Items)
            {
                tester.BuildProcesser();
            }
        }

        private void InitializeNavigator()
        {
            Navigator.Initialize(this);
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

                RecipeTemplateService.Items = new ObservableCollection<RecipeTemplate>(uow.RecipeTemplates.GetAll());
                StepService.Items = new ObservableCollection<Step>(uow.Steps.GetAll());
                StepTemplateService.Items = new ObservableCollection<StepTemplate>(uow.StepTemplates.GetAll());

                ProjectService.Items = new ObservableCollection<Project>(uow.Projects.GetAll());
                ProjectSettingService.Items = new ObservableCollection<ProjectSetting>(uow.ProjectSettings.GetAll());
                ProgramTypeService.Items = new ObservableCollection<ProgramType>(uow.ProgramTypes.GetAll());
                TableMakerProductTypeService.Items = new ObservableCollection<TableMakerProductType>(uow.TableMakerProductTypes.GetAll());
                TableMakerProductService.Items = new ObservableCollection<TableMakerProduct>(uow.TableMakerProducts.GetAll());
                //ProgramService.RecipeService.StepRuntimeService.StepService.Items = new ObservableCollection<StepClass>(uow.Steps.GetAll());
                //ProgramService.RecipeService.StepRuntimeService.StepTemplateService.Items = new ObservableCollection<StepTemplate>(uow.StepTemplates.GetAll());
                FreeTestRecordService.Items = new ObservableCollection<TestRecord>(uow.TestRecords.GetAllFreeTestRecords());
            }
        }
        void CreateViewModels()
        {
            allBatteryTypesViewModel = new AllBatteryTypesViewModel(BatteryTypeService, BatteryService);    //ViewModel初始化

            allProjectsViewModel = new AllProjectsViewModel(ProjectService, BatteryTypeService);    //ViewModel初始化

            allProjectSettingsViewModel = new AllProjectSettingsViewModel(ProjectSettingService, ProjectService);    //ViewModel初始化

            allProgramTypesViewModel = new AllProgramTypesViewModel(ProgramTypeService);    //ViewModel初始化

            allTableMakerProductTypesViewModel = new AllTableMakerProductTypesViewModel(TableMakerProductTypeService);    //ViewModel初始化

            allTableMakerProductsViewModel = new AllTableMakerProductsViewModel(TableMakerProductService, TableMakerProductTypeService, ProjectService);    //ViewModel初始化

            allBatteriesViewModel = new AllBatteriesViewModel(BatteryService, BatteryTypeService);    //ViewModel初始化

            allTestersViewModel = new AllTestersViewModel(TesterService, ChannelService);    //ViewModel初始化

            allChannelsViewModel = new AllChannelsViewModel(ChannelService, TesterService);    //ViewModel初始化

            allChambersViewModel = new AllChambersViewModel(ChamberService);    //ViewModel初始化

            allRecipeTemplatesViewModel =
                new AllRecipeTemplatesViewModel(
                    RecipeTemplateService, StepTemplateService
                    //ChargeCurrents,
                    //DischargeTemperatures,
                    //DischargeCurrents
                    );    //ViewModel初始化

            allStepTemplatesViewModel = new AllStepTemplatesViewModel(StepTemplateService);

            allProgramsViewModel = new AllProgramsViewModel
                (
                ProgramService,
                RecipeTemplateService,
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

            dashBoardViewModel = new DashBoardViewModel(BatteryService, ChannelService, ChamberService, ProgramService);
        }
        void BindingVMandView()
        {

            this.AllBatteryTypesViewInstance.DataContext = allBatteryTypesViewModel;

            this.AllProjectsViewInstance.DataContext = allProjectsViewModel;                                                           //ViewModel跟View绑定

            this.AllProjectSettingsViewInstance.DataContext = allProjectSettingsViewModel;                                                           //ViewModel跟View绑定

            this.AllProgramTypesViewInstance.DataContext = allProgramTypesViewModel;                                                           //ViewModel跟View绑定

            this.AllTableMakerProductTypesViewInstance.DataContext = allTableMakerProductTypesViewModel;

            this.AllTableMakerProductsViewInstance.DataContext = allTableMakerProductsViewModel;                                                         //ViewModel跟View绑定


            this.AllBatteriesViewInstance.DataContext = allBatteriesViewModel;                                                            //ViewModel跟View绑定


            this.AllTestersViewInstance.DataContext = allTestersViewModel;                                                            //ViewModel跟View绑定


            this.AllChannelsViewInstance.DataContext = allChannelsViewModel;                                                            //ViewModel跟View绑定


            this.AllChambersViewInstance.DataContext = allChambersViewModel;                                                            //ViewModel跟View绑定

            this.AllStepTemplateViewInstance.DataContext = allStepTemplatesViewModel;

            this.AllRecipeTemplatesViewInstance.DataContext = allRecipeTemplatesViewModel;                                                            //ViewModel跟View绑定

            this.AllProgramsViewInstance.DataContext = allProgramsViewModel;                                                            //ViewModel跟View绑定

            this.DashBoardViewInstance.DataContext = dashBoardViewModel;
        }
    }
}
