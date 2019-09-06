using System;
using System.Collections.Generic;
using System.Globalization;
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
using BCLabManager.Model;
using BCLabManager.ViewModel;

namespace BCLabManager.View
{
    /// <summary>
    /// Interaction logic for DashBoardView.xaml
    /// </summary>
    public partial class DashBoardView : UserControl
    {
        public DashBoardView()
        {
            InitializeComponent();
        }
    }

    public class HeightMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type typetarget, object param, CultureInfo culture)
        {
            /*
            double res;
            if (!Double.TryParse(value[0].ToString(), out res))
                value[0] = (double)5000;
            if (!Double.TryParse(value[1].ToString(), out res))
                value[1] = (double)0;
            if (!Double.TryParse(value[2].ToString(), out res))
                value[2] = (double)0;
            double a, b, y;
            a = ((double)value[3] * 0.80) / ((double)value[0] - (double)value[1]);
            b = ((double)value[3] * 0.1) - a * (double)value[1];
            y = a * (double)value[2] + b;
            if (y < 0)
                y = 0;
            else if (y > (double)value[3])
                y = (double)value[3];
            return y;*/
            double? total = value[0] as double?;
            double? val = value[1] as double?;
            double? actualheight = value[2] as double?;
            if (total != null && val != null && actualheight != null)
                return val / total * actualheight;
            else
                return null;
        }
        public object[] ConvertBack(object value, Type[] typetarget, object param, CultureInfo culture)
        {
            return null;
        }
    }

    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((double)value).ToString("#.#% " + parameter);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
