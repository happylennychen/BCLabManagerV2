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

namespace BCLabManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //AllBatteryTypesViewModel allBatteryTypesVM = new AllBatteryTypesViewModel();
            //AllBatteryTypesViewInstance.DataContext = 
            var viewModel = new MainWindowViewModel();


            // Allow all controls in the window to 
            // bind to the ViewModel by setting the 
            // DataContext, which propagates down 
            // the element tree.
            //DataContext = viewModel;
            this.AllBatteryTypesViewInstance.DataContext = viewModel.allBatteryTypesViewModel;
        }
    }
}
