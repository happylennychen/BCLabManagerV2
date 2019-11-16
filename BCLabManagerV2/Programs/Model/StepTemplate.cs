using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public enum CurrentUnitEnum
    {
        mA,
        C,
    }
    public enum CutOffConditionTypeEnum
    {
        Time_s,
        C_mAH,
        CRate
    }
    public class StepTemplate : BindableBase    //不需要跳转比较符
    {
        public int Id { get; set; }
        private double _temperatureDeg;
        public double Temperature
        {
            get { return _temperatureDeg; }
            set { SetProperty(ref _temperatureDeg, value); }
        }
        private double _currentInput;
        public double CurrentInput
        {
            get { return _currentInput; }
            set { SetProperty(ref _currentInput, value); }
        }
        private CurrentUnitEnum _currentUnit;
        public CurrentUnitEnum CurrentUnit
        {
            get { return _currentUnit; }
            set { SetProperty(ref _currentUnit, value); }
        }
        private double _cutOffConditionValue;
        public double CutOffConditionValue
        {
            get { return _cutOffConditionValue; }
            set { SetProperty(ref _cutOffConditionValue, value); }
        }
        private CutOffConditionTypeEnum _cutOffConditionType;
        public CutOffConditionTypeEnum CutOffConditionType
        {
            get { return _cutOffConditionType; }
            set { SetProperty(ref _cutOffConditionType, value); }
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
        public StepTemplate()
        {
        }
    }
}
