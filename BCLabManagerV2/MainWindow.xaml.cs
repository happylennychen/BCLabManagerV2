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

namespace BCLabManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Repositories _repositories = new Repositories();    //model实例全部放在这里。viewmodel和database的source

        public AllBatteryTypesViewModel allBatteryTypesViewModel { get; set; }  //其中需要显示BatteryTypes和Batteries
        public AllBatteriesViewModel allBatteriesViewModel { get; set; }  //其中需要显示Batteries和Records

        public AllTestersViewModel allTestersViewModel { get; set; }  //其中需要显示Testers和Channels
        public AllChannelsViewModel allChannelsViewModel { get; set; }  //其中需要显示Channels和Records

        public AllChambersViewModel allChambersViewModel { get; set; }  //其中需要显示Chambers和Records

        public AllSubProgramsViewModel allSubProgramsViewModel { get; set; }  //其中需要显示SubPrograms

        public AllProgramsViewModel allProgramsViewModel { get; set; }  //其中需要显示Programs, SubPrograms, Test1, Test2, TestSteps

        public MainWindow()
        {
            InitializeComponent();

            allBatteryTypesViewModel = new AllBatteryTypesViewModel(_repositories._batterytypeRepository, _repositories._batteryRepository);    //ViewModel初始化
            this.AllBatteryTypesViewInstance.DataContext = allBatteryTypesViewModel;                                                            //ViewModel跟View绑定

            allBatteriesViewModel = new AllBatteriesViewModel(_repositories._batteryRepository, _repositories._batterytypeRepository);    //ViewModel初始化
            this.AllBatteriesViewInstance.DataContext = allBatteriesViewModel;                                                            //ViewModel跟View绑定

            allTestersViewModel = new AllTestersViewModel(_repositories._testerRepository, _repositories._channelRepository);    //ViewModel初始化
            this.AllTestersViewInstance.DataContext = allTestersViewModel;                                                            //ViewModel跟View绑定

            allChannelsViewModel = new AllChannelsViewModel(_repositories._channelRepository, _repositories._testerRepository);    //ViewModel初始化
            this.AllChannelsViewInstance.DataContext = allChannelsViewModel;                                                            //ViewModel跟View绑定

            allChambersViewModel = new AllChambersViewModel(_repositories._chamberRepository);    //ViewModel初始化
            this.AllChambersViewInstance.DataContext = allChambersViewModel;                                                            //ViewModel跟View绑定

            allSubProgramsViewModel = new AllSubProgramsViewModel(_repositories._subprogramRepository);    //ViewModel初始化
            this.AllSubProgramsViewInstance.DataContext = allSubProgramsViewModel;                                                            //ViewModel跟View绑定

            allProgramsViewModel = new AllProgramsViewModel(_repositories._programRepository, _repositories._subprogramRepository);    //ViewModel初始化
            this.AllProgramsViewInstance.DataContext = allProgramsViewModel;                                                            //ViewModel跟View绑定
        }
    }
}
