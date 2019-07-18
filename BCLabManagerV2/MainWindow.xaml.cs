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
        public MainWindow()
        {
            InitializeComponent();

            allBatteryTypesViewModel = new AllBatteryTypesViewModel(_repositories._batterytypeRepository, _repositories._batteryRepository);    //ViewModel初始化
            this.AllBatteryTypesViewInstance.DataContext = allBatteryTypesViewModel;                                                            //ViewModel跟View绑定
        }
    }
}
