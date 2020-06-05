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
    public class ProjectSettingClass : BindableBase
    {

        public int Id { get; set; }

        private Int32 _designCapacity;
        public Int32 design_capacity_mahr
        {
            get { return _designCapacity; }
            set { SetProperty(ref _designCapacity, value); }
        }
        private Int32 _limitedChargeVoltage;
        public Int32 limited_charge_voltage_mv
        {
            get { return _limitedChargeVoltage; }
            set { SetProperty(ref _limitedChargeVoltage, value); }
        }
        private Int32 _fullyChargedEndCurrent;
        public Int32 fully_charged_end_current_ma
        {
            get { return _fullyChargedEndCurrent; }
            set { SetProperty(ref _fullyChargedEndCurrent, value); }
        }
        private Int32 _fullyChargedEndingTimeout;
        public Int32 fully_charged_ending_time_ms
        {
            get { return _fullyChargedEndingTimeout; }
            set { SetProperty(ref _fullyChargedEndingTimeout, value); }
        }
        private Int32 _dischargeEndVoltage;
        public Int32 discharge_end_voltage_mv
        {
            get { return _dischargeEndVoltage; }
            set { SetProperty(ref _dischargeEndVoltage, value); }
        }
        //threshold1st_facc_mv threshold2nd_facc_mv threshold3rd_facc_mv threshold4th_facc_mv initialratio_fcc
        //accumulatedcapacity_mahr dsglowvolt_mv dsglowtemp_01dc initialsoc_startocv systemline_impedance isvalid
        private int _threshold1st_facc_mv;
        public int threshold_1st_facc_mv
        {
            get { return _threshold1st_facc_mv; }
            set { SetProperty(ref _threshold1st_facc_mv, value); }
        }
        private int _threshold2nd_facc_mv;
        public int threshold_2nd_facc_mv
        {
            get { return _threshold2nd_facc_mv; }
            set { SetProperty(ref _threshold2nd_facc_mv, value); }
        }
        private int _threshold3rd_facc_mv;
        public int threshold_3rd_facc_mv
        {
            get { return _threshold3rd_facc_mv; }
            set { SetProperty(ref _threshold3rd_facc_mv, value); }
        }
        private int _threshold4th_facc_mv;
        public int threshold_4th_facc_mv
        {
            get { return _threshold4th_facc_mv; }
            set { SetProperty(ref _threshold4th_facc_mv, value); }
        }
        private int _initialratio_fcc;
        public int initial_ratio_fcc
        {
            get { return _initialratio_fcc; }
            set { SetProperty(ref _initialratio_fcc, value); }
        }
        private int _accumulatedcapacity_mahr;
        public int accumulated_capacity_mahr
        {
            get { return _accumulatedcapacity_mahr; }
            set { SetProperty(ref _accumulatedcapacity_mahr, value); }
        }
        private int _dsglowvolt_mv;
        public int dsg_low_volt_mv
        {
            get { return _dsglowvolt_mv; }
            set { SetProperty(ref _dsglowvolt_mv, value); }
        }
        private int _dsglowtemp_01dc;
        public int dsg_low_temp_01dc
        {
            get { return _dsglowtemp_01dc; }
            set { SetProperty(ref _dsglowtemp_01dc, value); }
        }
        private int _initialsoc_startocv;
        public int initial_soc_start_ocv
        {
            get { return _initialsoc_startocv; }
            set { SetProperty(ref _initialsoc_startocv, value); }
        }
        private int _systemline_impedance;
        public int system_line_impedance
        {
            get { return _systemline_impedance; }
            set { SetProperty(ref _systemline_impedance, value); }
        }
        private bool _isvalid;
        public bool is_valid
        {
            get { return _isvalid; }
            set { SetProperty(ref _isvalid, value); }
        }
    }
}
