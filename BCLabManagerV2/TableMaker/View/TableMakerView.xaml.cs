using BCLabManager.Model;
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
    public partial class TableMakerView : UserControl
    {
        public TableMakerView()
        {
            InitializeComponent();
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            TableMakerRecord tmr = e.Item as TableMakerRecord;
            if (tmr != null)
            {
                if (tmr.IsValid)
                    e.Accepted = true;
                else
                    e.Accepted = false;
            }
        }
    }
}
