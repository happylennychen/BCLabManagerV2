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
        //public ObservableCollection<TesterClass> Testers { get; set; }
        //public ObservableCollection<ChannelClass> Channels { get; set; }
        //public ObservableCollection<ChamberClass> Chambers { get; set; }
        //public List<RecipeTemplate> RecipeTemplates { get; set; }
        //public List<ChargeTemperatureClass> ChargeTemperatures { get; set; }
        //public List<ChargeCurrentClass> ChargeCurrents { get; set; }
        //public List<DischargeTemperatureClass> DischargeTemperatures { get; set; }
        //public List<DischargeCurrentClass> DischargeCurrents { get; set; }
        //public ObservableCollection<ProgramClass> Programs { get; set; }

        //private DomainDataClass _domainData;

        public BatteryTypeServieClass BatteryTypeService { get; set; } = new BatteryTypeServieClass();
        public BatteryServieClass BatteryService { get; set; } = new BatteryServieClass();
        public TesterServieClass TesterService { get; set; } = new TesterServieClass();
        public ChannelServieClass ChannelService { get; set; } = new ChannelServieClass();
        public ChamberServieClass ChamberService { get; set; } = new ChamberServieClass();
        //public TestRecordServiceClass TestRecordService { get; set; } = new TestRecordServiceClass();
        public RecipeTemplateServiceClass RecipeTemplateService { get; set; } = new RecipeTemplateServiceClass();
        public StepServiceClass StepService { get; set; } = new StepServiceClass();
        public StepTemplateServiceClass StepTemplateService { get; set; } = new StepTemplateServiceClass();
        public ProgramServiceClass ProgramService { get; set; } = new ProgramServiceClass();

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                InitializeConfiguration();
                InitializeDatabase();
                //_domainData = new DomainDataClass();
                LoadFromDB();
                InitializeNavigator();
                CreateViewModels();
                BindingVMandView();
                ProgramService.UpdateEstimatedTimeChain();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                App.Current.Shutdown();
            }
        }

        private void InitializeNavigator()
        {
            Navigator.Initialize(this);
        }

        void InitializeConfiguration()
        {
            if (!File.Exists(GlobalSettings.ConfigurationFilePath))
            {
                ConfigureDBPath();
                CreateConfigurationFile();
            }
            else
            {
                LoadDBPathFromConfigurationFile();
            }
        }
        void InitializeDatabase()
        {
            //if (!File.Exists(GlobalSettings.DbPath))
            //{
                using (var dbContext = new AppDbContext())
                {
                dbContext.Database.Migrate();
                //dbContext.Database.EnsureCreated();   //用了这个，Migrate就不好使了
                }
                DatabasePopulator.PopulateHistoricData();
            //}
        }

        private void ConfigureDBPath()
        {
            DBConfigureWindow dBConfigureWindow = new DBConfigureWindow();
            //dBConfigureWindow.Owner = this;
            dBConfigureWindow.ShowDialog();
            if (dBConfigureWindow.DialogResult == false)
            {
                throw new Exception("Database is not ready.");
            }

        }

        private void CreateConfigurationFile()
        {
            var fs = File.Create(GlobalSettings.ConfigurationFilePath);
            var sw = new StreamWriter(fs);
            sw.WriteLine(GlobalSettings.DbPath);
            sw.Close();
            fs.Close();
        }

        private void LoadDBPathFromConfigurationFile()
        {
            var fs = new FileStream(GlobalSettings.ConfigurationFilePath, FileMode.Open);
            var sr = new StreamReader(fs);
            GlobalSettings.DbPath = sr.ReadLine();
        }

        void LoadFromDB()
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                BatteryTypeService.Items = new ObservableCollection<BatteryTypeClass>(uow.BatteryTypes.GetAll());
                BatteryService.Items = new ObservableCollection<BatteryClass>(uow.Batteries.GetAll("BatteryType,Records"));

                TesterService.Items = new ObservableCollection<TesterClass>(uow.Testers.GetAll());
                ChannelService.Items = new ObservableCollection<ChannelClass>(uow.Channels.GetAll("Tester,Records"));

                ChamberService.Items = new ObservableCollection<ChamberClass>(uow.Chambers.GetAll("Records"));

                RecipeTemplateService.Items = new ObservableCollection<RecipeTemplate>(uow.RecipeTemplates.GetAll());
                StepService.Items = new ObservableCollection<StepClass>(uow.Steps.GetAll());
                StepTemplateService.Items = new ObservableCollection<StepTemplate>(uow.StepTemplates.GetAll());

                ProgramService.Items = new ObservableCollection<ProgramClass>(uow.Programs.GetAll());
                ProgramService.RecipeService.Items = new ObservableCollection<RecipeClass>(uow.Recipies.GetAll());
                ProgramService.RecipeService.TestRecordService.Items = new ObservableCollection<TestRecordClass>(uow.TestRecords.GetAll());
                ProgramService.RecipeService.TestRecordService.RawDataService.Items = new ObservableCollection<RawDataClass>(uow.RawDataList.GetAll());
                ProgramService.RecipeService.StepRuntimeService.Items = new ObservableCollection<StepRuntimeClass>(uow.StepRuntimes.GetAll());
                //ProgramService.RecipeService.StepRuntimeService.StepService.Items = new ObservableCollection<StepClass>(uow.Steps.GetAll());
                //ProgramService.RecipeService.StepRuntimeService.StepTemplateService.Items = new ObservableCollection<StepTemplate>(uow.StepTemplates.GetAll());
            }
        }
        void CreateViewModels()
        {
            allBatteryTypesViewModel = new AllBatteryTypesViewModel(BatteryTypeService, BatteryService);    //ViewModel初始化

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
                BatteryTypeService,
                BatteryService,
                TesterService,
                ChannelService,
                ChamberService
                );    //ViewModel初始化

            dashBoardViewModel = new DashBoardViewModel(BatteryService, ChannelService, ChamberService, ProgramService);
        }
        void BindingVMandView()
        {

            this.AllBatteryTypesViewInstance.DataContext = allBatteryTypesViewModel;                                                            //ViewModel跟View绑定


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
