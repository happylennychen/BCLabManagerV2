﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.DataAccess;
using Prism.Mvvm;

namespace BCLabManager.Model
{
    public class StepRuntime : BindableBase
    {
        public int Id { get; set; }
        public StepTemplate StepTemplate { get; set; }
        public double DesignCapacityInmAH { get; set; }
        private DateTime _startTime;
        public DateTime StartTime
        {
            get { return _startTime; }
            set { SetProperty(ref _startTime, value); }
        }

        private DateTime _endTime;
        public DateTime EndTime
        {
            get { return _endTime; }
            set { SetProperty(ref _endTime, value); }
        }

        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get { return _duration; }
            set { SetProperty(ref _duration, value); }
        }

        private DateTime _est;
        public DateTime EST //Estimated Start Time
        {
            get { return _est; }
            set { SetProperty(ref _est, value); }
        }

        private DateTime _eet;
        public DateTime EET //Estimated End Time
        {
            get { return _eet; }
            set { SetProperty(ref _eet, value); }
        }

        private TimeSpan _ed;
        public TimeSpan ED //Estimated Duration
        {
            get { return _ed; }
            set { SetProperty(ref _ed, value); }
        }
        public int Order { get; set; }
        public StepRuntime()
        {
        }

        public double GetCurrentInmA()
        {
            var st = StepTemplate;
            if (st.CurrentUnit == CurrentUnitEnum.mA)
                return st.CurrentInput;
            else if (st.CurrentUnit == CurrentUnitEnum.C)
                return st.CurrentInput * DesignCapacityInmAH;
            return 0;
        }
    }
}
