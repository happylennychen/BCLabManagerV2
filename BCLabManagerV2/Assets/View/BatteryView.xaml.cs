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
using System.Windows.Shapes;

namespace BCLabManager.View
{
    /// <summary>
    /// Interaction logic for BatteryTypeView.xaml
    /// </summary>
    public partial class BatteryView : Window
    {
        public BatteryView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CycleTxt_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
