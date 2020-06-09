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
    public class LibFG : BindableBase
    {

        public int Id { get; set; }

        private int _libfg_model_code;
        public int libfg_model_code
        {
            get { return _libfg_model_code; }
            set { SetProperty(ref _libfg_model_code, value); }
        }

        private int _libfg_version;
        public int libfg_version
        {
            get { return _libfg_version; }
            set { SetProperty(ref _libfg_version, value); }
        }

        private string _libfg_sample_file_path;
        public string libfg_sample_file_path
        {
            get { return _libfg_sample_file_path; }
            set { SetProperty(ref _libfg_sample_file_path, value); }
        }

        private string _libfg_dev_pack_file_path;
        public string libfg_dev_pack_file_path
        {
            get { return _libfg_dev_pack_file_path; }
            set { SetProperty(ref _libfg_dev_pack_file_path, value); }
        }

        private string _libfg_dll_path;
        public string libfg_dll_path
        {
            get { return _libfg_dll_path; }
            set { SetProperty(ref _libfg_dll_path, value); }
        }
        private bool _isvalid;
        public bool is_valid
        {
            get { return _isvalid; }
            set { SetProperty(ref _isvalid, value); }
        }
    }
}
