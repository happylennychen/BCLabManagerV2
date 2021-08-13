using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;
using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;
using System.Threading;
using System.Diagnostics;
using System.Windows;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TableMakerConfirmationViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields
        RelayCommand _okCommand;

        #endregion // Fields

        #region Constructor

        public TableMakerConfirmationViewModel()
        {
        }
        #endregion // Constructor

        #region Presentation Properties
        public bool OCVReady { get; set; }

        public bool RCReady { get; set; }
        public bool SDReady
        {
            get { return OCVReady & RCReady; }
        }
        public bool ADReady
        {
            get { return OCVReady & RCReady; }
        }
        public bool MiniReady
        {
            get { return OCVReady & RCReady; }
        }
        public bool LiteReady
        {
            get { return OCVReady & RCReady; }
        }

        public string OCVFileName { get; set; }
        public string RCFileName { get; set; }
        public string StandardDriverCFileName { get; set; }
        public string StandardDriverHFileName { get; set; }
        public string AndroidDriverCFileName { get; set; }
        public string AndroidDriverHFileName { get; set; }
        public string MiniDriverCFileName { get; set; }
        public string MiniDriverHFileName { get; set; }
        public string LiteDriverCFileName { get; set; }
        public string LiteDriverHFileName { get; set; }
        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
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
            IsOK = true;
        }

        private bool _isOK;
        public bool IsOK
        {
            get { return _isOK; }
            set { _isOK = value; }
        }

        #endregion // Presentation Properties

        #region Public Methods

        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        public void Generate()
        {
            //TableMakerService.Make(_tableMakerModel.Project, _tableMakerModel.Programs, _tableMakerModel.Testers, OCVReady, RCReady, SDReady, ADReady, MiniReady);
            Thread t = new Thread(() =>
            {
                //if (MessageBox.Show("It will take a while to get the work done, Continue?", "Generate Tables and Drivers.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //{
                //    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                //    stopwatch.Start();
                //    TableMakerService.Build(ref _tableMakerModel, 800, "Some description", _tableMakerRecordService);
                //    var project = _tableMakerModel.Project;
                //    var folder = $@"{GlobalSettings.LocalFolder}{project.BatteryType.Name}\{project.Name}\{GlobalSettings.ProductFolderName}";
                //    string time = Math.Round(stopwatch.Elapsed.TotalSeconds, 0).ToString() + "S";
                //    MessageBox.Show($"Completed. It took {time} to get the job done.");
                //    Process.Start(folder);
                //}
            });
            t.Start();
        }

        #endregion // Public Methods

        #region Private Helpers

        #endregion // Private Helpers
    }
}