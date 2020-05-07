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
        private CoefficientClass _coef = new CoefficientClass();
        public CoefficientClass Coefficient
        {
            get { return _coef; }
            set { SetProperty(ref _coef, value); }
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
        public StepTemplate()
        {
        }

        public double GetCurrentInmA(double typicalCapacity)
        {
            if (CurrentUnit == CurrentUnitEnum.mA)
                return CurrentInput;
            else if (CurrentUnit == CurrentUnitEnum.C)
                return CurrentInput * typicalCapacity;
            return 0;
        }
        public double GetEndCapacity(double typicalCapacity, double CBegin)
        {
            TimeSpan duration;
            double Cend=0;
            if (CutOffConditionType == CutOffConditionTypeEnum.Time_s)
            {
                duration = TimeSpan.FromSeconds(CutOffConditionValue);
                Cend = CBegin + GetCurrentInmA(typicalCapacity) * duration.TotalHours;
            }
            else
            {
                if (CutOffConditionType == CutOffConditionTypeEnum.CRate)
                    Cend = CutOffConditionValue * typicalCapacity;
                else if (CutOffConditionType == CutOffConditionTypeEnum.C_mAH)
                    Cend = CutOffConditionValue;
            }
            return Cend;
        }

        public override string ToString()
        {
            return $"{CurrentInput} {CurrentUnit}, {CutOffConditionValue} {CutOffConditionType}";
        }
    }
}
