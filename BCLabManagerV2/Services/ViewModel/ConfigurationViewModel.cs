using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using Prism.Mvvm;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text.Json;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// Editable: True
    /// Updateable: 1. Events: True, 2. Records: True
    /// </summary>
    public class ConfigurationViewModel : BindableBase
    {
        #region Fields
        //private EventService _eventService;
        RelayCommand _okCommand;
        private Configuration _configuration;
        #endregion // Fields

        #region Constructor

        public ConfigurationViewModel(Configuration conf)
        {
            _configuration = conf;
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the EventModelViewModel objects.
        /// </summary>
        public string RemotePath
        {
            get { return _configuration.RemotePath; }
            set { _configuration.RemotePath = value; }
        }
        public bool EnableTest
        {
            get { return _configuration.EnableTest; }
            set { _configuration.EnableTest = value; }
        }
        public string MappingPath
        {
            get { return _configuration.MappingPath; }
            set { _configuration.MappingPath = value; }
        }
        public string LocalPath
        {
            get { return _configuration.LocalPath; }
            set { _configuration.LocalPath = value; }
        }
        public string DatabaseHost
        {
            get { return _configuration.DatabaseHost; }
            set { _configuration.DatabaseHost = value; }
        }
        public string DatabaseName
        {
            get { return _configuration.DatabaseName; }
            set { _configuration.DatabaseName = value; }
        }
        public string DatabaseUser
        {
            get { return _configuration.DatabaseUser; }
            set { _configuration.DatabaseUser = value; }
        }
        public string DatabasePassword
        {
            get { return _configuration.DatabasePassword; }
            set { _configuration.DatabasePassword = value; }
        }

        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new RelayCommand(
                        param => { this.OK(); }
                        );
                }
                return _okCommand;
            }
        }

        private void OK()
        {
            Configuration _confBuffer;
            _confBuffer = SaveConfigToBuffer();
            UpdateConfig(_configuration);

            if (InputCheck())
            {
                string jsonString = JsonSerializer.Serialize(_configuration);
                File.WriteAllText(GlobalSettings.ConfigurationFilePath, jsonString);
                //_mainWindowViewModel = new MainWindowViewModel();
                //MessageBox.Show("Please restart BCLM!");
                Thread t = new Thread(() =>
                {
                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                });
                t.Start();
                Application.Current.Shutdown();
            }
            else
            {
                MessageBox.Show("Configuration Error. Please reset.");
                UpdateConfig(_confBuffer);
            }
        }

        private void UpdateConfig(Configuration config)
        {
            GlobalSettings.RemotePath = config.RemotePath;
            GlobalSettings.EnableTest = config.EnableTest;
            GlobalSettings.MappingPath = config.MappingPath;
            GlobalSettings.LocalPath = config.LocalPath;
            GlobalSettings.DatabaseHost = config.DatabaseHost;
            GlobalSettings.DatabaseName = config.DatabaseName;
            GlobalSettings.DatabaseUser = config.DatabaseUser;
            GlobalSettings.DatabasePassword = config.DatabasePassword;
        }

        private Configuration SaveConfigToBuffer()
        {
            Configuration output = new Configuration();
            output.RemotePath = GlobalSettings.RemotePath;
            output.EnableTest = GlobalSettings.EnableTest;
            output.MappingPath = GlobalSettings.MappingPath;
            output.LocalPath = GlobalSettings.LocalPath;
            output.DatabaseHost = GlobalSettings.DatabaseHost;
            output.DatabaseName = GlobalSettings.DatabaseName;
            output.DatabaseUser = GlobalSettings.DatabaseUser;
            output.DatabasePassword = GlobalSettings.DatabasePassword;
            return output;
        }

        private bool InputCheck()
        {
            if (_configuration.EnableTest)
            {
                if (!Directory.Exists(_configuration.MappingPath))
                    return false;
            }
            if (!IsDatabaseAccessable())
                return false;
            return true;
        }

        private bool IsDatabaseAccessable()
        {
            try
            {
                using (var uow = new UnitOfWork(new AppDbContext()))
                {
                    var a = uow.BatteryTypes.GetAll();
                }
            }
            catch { return false; }
            return true;
        }

        private void Cancel()
        {

        }
        #endregion // Public Interface
    }
}
