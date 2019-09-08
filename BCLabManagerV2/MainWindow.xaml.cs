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

        public AllSubProgramTemplatesViewModel allSubProgramTemplatesViewModel { get; set; }  //其中需要显示SubPrograms

        public AllProgramsViewModel allProgramsViewModel { get; set; }  //其中需要显示Programs, SubPrograms, Test1, Test2, TestSteps

        public DashBoardViewModel dashBoardViewModel { get; set; }

        List<BatteryTypeClass> BatteryTypes { get; set; }
        ObservableCollection<BatteryClass> Batteries { get; set; }
        List<TesterClass> Testers { get; set; }
        ObservableCollection<ChannelClass> Channels { get; set; }
        ObservableCollection<ChamberClass> Chambers { get; set; }
        List<SubProgramTemplate> SubProgramTemplates { get; set; }
        ObservableCollection<ProgramClass> Programs { get; set; }
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                GlobalSettings.DbPath = "C://BCLab.db3";
                InitializeDatabase();
                LoadingFromDB();
                CreateViewModels();
                BindingVMandView();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                App.Current.Shutdown();
            }
        }
        void InitializeDatabase()
        {
            if(!File.Exists(GlobalSettings.DbPath))
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Database.Migrate();
                }
            }
        }
        void LoadingFromDB()
        {
            using (var dbContext = new AppDbContext())
            {
                BatteryTypes = new List<BatteryTypeClass>(dbContext.BatteryTypes.ToList());

                Batteries = new ObservableCollection<BatteryClass>(
                    dbContext.Batteries
                    .Include(i => i.BatteryType)
                    .Include(o => o.Records)
                    .ToList()
                    );

                Testers = new List<TesterClass>(dbContext.Testers.ToList());

                Channels = new ObservableCollection<ChannelClass>(
                    dbContext.Channels
                    .Include(i => i.Tester)
                    .Include(o => o.Records)
                    .ToList()
                    );

                Chambers = new ObservableCollection<ChamberClass>(
                    dbContext.Chambers
                    .Include(o => o.Records)
                    .ToList()
                    );

                SubProgramTemplates = new List<SubProgramTemplate>(dbContext.SubProgramTemplates.ToList());

                Programs = new ObservableCollection<ProgramClass>(dbContext.Programs
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.FirstTestRecords)
                            .ThenInclude(tr => tr.RawData)
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.FirstTestRecords)
                            .ThenInclude(tr => tr.AssignedBattery)
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.FirstTestRecords)
                            .ThenInclude(tr => tr.AssignedChamber)
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.FirstTestRecords)
                            .ThenInclude(tr => tr.AssignedChannel)
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.SecondTestRecords)
                            .ThenInclude(tr => tr.RawData)
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.SecondTestRecords)
                            .ThenInclude(tr => tr.AssignedBattery)
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.SecondTestRecords)
                            .ThenInclude(tr => tr.AssignedChamber)
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.SecondTestRecords)
                            .ThenInclude(tr => tr.AssignedChannel)
                    .ToList());

                foreach (var pro in Programs)
                {
                    foreach (var sub in pro.SubPrograms)
                    {
                        foreach (var tr in sub.FirstTestRecords)
                            sub.AssociateEvent(tr);
                        foreach (var tr in sub.SecondTestRecords)
                            sub.AssociateEvent(tr);
                    }
                }
            }
        }
        void CreateViewModels()
        {
            allBatteryTypesViewModel = new AllBatteryTypesViewModel(BatteryTypes);    //ViewModel初始化

            allBatteriesViewModel = new AllBatteriesViewModel(Batteries, BatteryTypes);    //ViewModel初始化

            allTestersViewModel = new AllTestersViewModel(Testers);    //ViewModel初始化

            allChannelsViewModel = new AllChannelsViewModel(Channels, Testers);    //ViewModel初始化

            allChambersViewModel = new AllChambersViewModel(Chambers);    //ViewModel初始化

            allSubProgramTemplatesViewModel = new AllSubProgramTemplatesViewModel(SubProgramTemplates);    //ViewModel初始化

            allProgramsViewModel = new AllProgramsViewModel
                (
                Programs,
                SubProgramTemplates,
                BatteryTypes,
                Batteries,
                Testers,
                Channels,
                Chambers
                );    //ViewModel初始化

            dashBoardViewModel = new DashBoardViewModel(Programs, Batteries, Channels, Chambers);
        }
        void BindingVMandView()
        {

            this.AllBatteryTypesViewInstance.DataContext = allBatteryTypesViewModel;                                                            //ViewModel跟View绑定


            this.AllBatteriesViewInstance.DataContext = allBatteriesViewModel;                                                            //ViewModel跟View绑定


            this.AllTestersViewInstance.DataContext = allTestersViewModel;                                                            //ViewModel跟View绑定


            this.AllChannelsViewInstance.DataContext = allChannelsViewModel;                                                            //ViewModel跟View绑定


            this.AllChambersViewInstance.DataContext = allChambersViewModel;                                                            //ViewModel跟View绑定

            this.AllSubProgramTemplatesViewInstance.DataContext = allSubProgramTemplatesViewModel;                                                            //ViewModel跟View绑定


            this.AllProgramsViewInstance.DataContext = allProgramsViewModel;                                                            //ViewModel跟View绑定

            this.DashBoardViewInstance.DataContext = dashBoardViewModel;
        }
    }
}
