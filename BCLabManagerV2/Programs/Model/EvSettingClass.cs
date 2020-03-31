using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BCLabManager.DataAccess;
using Prism.Mvvm;

namespace BCLabManager.Model
{
    public class EvSettingClass : BindableBase
    {

        public int Id { get; set; }

        private Int32 _typicalCapacity;
        public Int32 TypicalCapacity
        {
            get { return _typicalCapacity; }
            set { SetProperty(ref _typicalCapacity, value); }
        }
        private Int32 _fullyChargedEndCurrent;
        public Int32 FullyChargedEndCurrent
        {
            get { return _fullyChargedEndCurrent; }
            set { SetProperty(ref _fullyChargedEndCurrent, value); }
        }
        private Int32 _fullyChargedEndingTimeout;
        public Int32 FullyChargedEndingTimeout
        {
            get { return _fullyChargedEndingTimeout; }
            set { SetProperty(ref _fullyChargedEndingTimeout, value); }
        }
        private Int32 _dischargeEndVoltage;
        public Int32 DischargeEndVoltage
        {
            get { return _dischargeEndVoltage; }
            set { SetProperty(ref _dischargeEndVoltage, value); }
        }
    }
}
