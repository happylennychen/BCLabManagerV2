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

        public MainWindow()
        {
            InitializeComponent();
            InitializeDatabase();
            List<BatteryTypeClass> batteryTypes;
            using (var dbContext = new AppDbContext())
            {
                batteryTypes = new List<BatteryTypeClass>(dbContext.BatteryTypes.ToList());
            }

            allBatteryTypesViewModel = new AllBatteryTypesViewModel(batteryTypes);    //ViewModel初始化
            this.AllBatteryTypesViewInstance.DataContext = allBatteryTypesViewModel;                                                            //ViewModel跟View绑定


            List<BatteryClass> batteries;
            using (var dbContext = new AppDbContext())
            {
                batteries = new List<BatteryClass>(
                    dbContext.Batteries
                    .Include(i=>i.BatteryType)
                    .Include(o=>o.Records)
                    .ToList()
                    );
            }

            allBatteriesViewModel = new AllBatteriesViewModel(batteries, batteryTypes);    //ViewModel初始化
            this.AllBatteriesViewInstance.DataContext = allBatteriesViewModel;                                                            //ViewModel跟View绑定


            List<TesterClass> testers;
            using (var dbContext = new AppDbContext())
            {
                testers = new List<TesterClass>(dbContext.Testers.ToList());
            }

            allTestersViewModel = new AllTestersViewModel(testers);    //ViewModel初始化
            this.AllTestersViewInstance.DataContext = allTestersViewModel;                                                            //ViewModel跟View绑定


            List<ChannelClass> channels;
            using (var dbContext = new AppDbContext())
            {
                channels = new List<ChannelClass>(
                    dbContext.Channels
                    .Include(i=>i.Tester)
                    .Include(o=>o.Records)
                    .ToList()
                    );
            }

            allChannelsViewModel = new AllChannelsViewModel(channels, testers);    //ViewModel初始化
            this.AllChannelsViewInstance.DataContext = allChannelsViewModel;                                                            //ViewModel跟View绑定


            List<ChamberClass> chambers;
            using (var dbContext = new AppDbContext())
            {
                chambers = new List<ChamberClass>(
                    dbContext.Chambers
                    .Include(o=>o.Records)
                    .ToList()
                    );
            }

            allChambersViewModel = new AllChambersViewModel(chambers);    //ViewModel初始化
            this.AllChambersViewInstance.DataContext = allChambersViewModel;                                                            //ViewModel跟View绑定


            List<SubProgramTemplate> subProgramTemplates;
            using (var dbContext = new AppDbContext())
            {
                subProgramTemplates = new List<SubProgramTemplate>(dbContext.SubProgramTemplates.ToList());
            }
            allSubProgramTemplatesViewModel = new AllSubProgramTemplatesViewModel(subProgramTemplates);    //ViewModel初始化
            this.AllSubProgramTemplatesViewInstance.DataContext = allSubProgramTemplatesViewModel;                                                            //ViewModel跟View绑定


            List<ProgramClass> programClasses;
            using (var dbContext = new AppDbContext())
            {
                programClasses = new List<ProgramClass>(dbContext.Programs
                     .Include(pro => pro.SubPrograms)
                        .ThenInclude(sub => sub.FirstTestRecords)
                            .ThenInclude(tr=>tr.RawData)
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
            }

            allProgramsViewModel = new AllProgramsViewModel
                (
                programClasses, 
                subProgramTemplates,
                batteryTypes,
                batteries,
                testers,
                channels,
                chambers
                );    //ViewModel初始化
            this.AllProgramsViewInstance.DataContext = allProgramsViewModel;                                                            //ViewModel跟View绑定
        }
        void InitializeDatabase()
        {
            string defaultfilepath = "D://BCLab.db3";
            if(!File.Exists(defaultfilepath))
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Database.Migrate();
                }
            }
        }
    }
}
