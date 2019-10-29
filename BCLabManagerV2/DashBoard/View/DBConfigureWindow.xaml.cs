using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// DBConfigureWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DBConfigureWindow : Window
    {
        public DBConfigureWindow()
        {
            InitializeComponent();
            ret = false;
        }
        public bool ret { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if(saveFileDialog.ShowDialog()==true)
            {
                FilePath.Text = saveFileDialog.FileName;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (FilePath.Text.Length > 0)
            {
                GlobalSettings.DbPath = FilePath.Text;
                this.DialogResult = true;
            }
        }
    }
}
