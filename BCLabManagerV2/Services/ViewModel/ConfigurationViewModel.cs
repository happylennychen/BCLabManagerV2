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
            set { _configuration.RemotePath = value;}
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
            Thread t = new Thread(() =>
            {
                GlobalSettings.RemotePath = _configuration.RemotePath;
                GlobalSettings.EnableTest = _configuration.EnableTest;
                GlobalSettings.MappingPath = _configuration.MappingPath;
                GlobalSettings.DatabaseHost = _configuration.DatabaseHost;
                GlobalSettings.DatabaseName = _configuration.DatabaseName;
                GlobalSettings.DatabaseUser = _configuration.DatabaseUser;
                GlobalSettings.DatabasePassword = _configuration.DatabasePassword;
                string jsonString = JsonSerializer.Serialize(_configuration);
                File.WriteAllText(GlobalSettings.ConfigurationFilePath, jsonString);
                //_mainWindowViewModel = new MainWindowViewModel();
                //MessageBox.Show("Please restart BCLM!");
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            });
            t.Start();
            Application.Current.Shutdown();
        }
        private void Cancel()
        {
            
        }
        #endregion // Public Interface
    }
}
