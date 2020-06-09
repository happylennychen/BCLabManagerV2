using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class Coefficient : BindableBase    //不需要跳转比较符
    {
        public int Id { get; set; }
        private double _temperatureDeg;
        public double Temperature
        {
            get { return _temperatureDeg; }
            set { SetProperty(ref _temperatureDeg, value); }
        }
        private double _slope = 1;
        public double Slope
        {
            get { return _slope; }
            set { SetProperty(ref _slope, value); }
        }
        private double _offset = 0;
        public double Offset
        {
            get { return _offset; }
            set { SetProperty(ref _offset, value); }
        }
        public Coefficient()
        {
        }
    }
}
