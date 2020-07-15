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
    public class EmulatorResult : BindableBase
    {

        public int Id { get; set; }

        private TestRecord _test_record;
        public TestRecord test_record
        {
            get { return _test_record; }
            set { SetProperty(ref _test_record, value); }
        }

        private ProjectSetting _project_setting;
        public ProjectSetting project_setting
        {
            get { return _project_setting; }
            set { SetProperty(ref _project_setting, value); }
        }

        private LibFG _lib_fg;
        public LibFG lib_fg
        {
            get { return _lib_fg; }
            set { SetProperty(ref _lib_fg, value); }
        }

        private string _em_temperature;
        public string em_temperature
        {
            get { return _em_temperature; }
            set { SetProperty(ref _em_temperature, value); }
        }

        private string _em_current;
        public string em_current
        {
            get { return _em_current; }
            set { SetProperty(ref _em_current, value); }
        }

        private string _error_rsoc;
        public string error_rsoc
        {
            get { return _error_rsoc; }
            set { SetProperty(ref _error_rsoc, value); }
        }

        private string _error_tte;
        public string error_tte
        {
            get { return _error_tte; }
            set { SetProperty(ref _error_tte, value); }
        }

        private string _log_file_path;
        public string log_file_path
        {
            get { return _log_file_path; }
            set { SetProperty(ref _log_file_path, value); }
        }

        private string _excel_file_path;
        public string excel_file_path
        {
            get { return _excel_file_path; }
            set { SetProperty(ref _excel_file_path, value); }
        }

        private string _rsoc_png_file_path;
        public string rsoc_png_file_path
        {
            get { return _rsoc_png_file_path; }
            set { SetProperty(ref _rsoc_png_file_path, value); }
        }

        private string _tte_png_file_path;
        public string tte_png_file_path
        {
            get { return _tte_png_file_path; }
            set { SetProperty(ref _tte_png_file_path, value); }
        }

        private string _emulator_info;
        public string emulator_info
        {
            get { return _emulator_info; }
            set { SetProperty(ref _emulator_info, value); }
        }
        private bool _isvalid;
        public bool is_valid
        {
            get { return _isvalid; }
            set { SetProperty(ref _isvalid, value); }
        }
    }
}
