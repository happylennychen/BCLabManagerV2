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
    public class ReleasePackageClass : BindableBase
    {

        public int Id { get; set; }

        private ProjectSettingClass _project_setting;
        public ProjectSettingClass project_setting
        {
            get { return _project_setting; }
            set { SetProperty(ref _project_setting, value); }
        }

        private LibFGClass _lib_fg;
        public LibFGClass lib_fg
        {
            get { return _lib_fg; }
            set { SetProperty(ref _lib_fg, value); }
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

        private string _result_file_path;
        public string result_file_path
        {
            get { return _result_file_path; }
            set { SetProperty(ref _result_file_path, value); }
        }

        private string _package_file_path;
        public string package_file_path
        {
            get { return _package_file_path; }
            set { SetProperty(ref _package_file_path, value); }
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
