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
    public class TableMakerViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields

        TableMakerModel _tableMakerModel;
        RelayCommand _generateCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public TableMakerViewModel(TableMakerModel tableMakerModel)
        {
            _tableMakerModel = tableMakerModel;
            TableMakerService.GetFilePaths(ref _tableMakerModel);
        }
        #endregion // Constructor

        #region Presentation Properties
        public bool OCVReady
        {
            get
            {

                var project = _tableMakerModel.Project;

                var programs = _tableMakerModel.Programs.Select(o => o).Where(o => o.Project.Id == project.Id && o.IsCompleted == true && (o.Type.Name == "OCV")).ToList();

                if (programs.Count >= 1)
                {
                    return true;    //需要进一步改进
                }

                return false;
            }
        }

        public bool RCReady
        {
            get
            {
                var project = _tableMakerModel.Project;

                var programs = _tableMakerModel.Programs.Select(o => o).Where(o => o.Project.Id == project.Id && o.IsCompleted == true && (o.Type.Name == "RC")).ToList();

                if (programs.Count >= 1)
                {
                    return true;    //需要进一步改进
                }

                return false;
            }
        }
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

        public string OCVFileName
        {
            get { return _tableMakerModel.OCVModel.FilePath; }
        }
        public string RCFileName
        {
            get { return _tableMakerModel.RCModel.FilePath; }
        }
        public string StandardDriverCFileName
        {
            get { return _tableMakerModel.StandardModel.FilePaths[0]; }
        }
        public string StandardDriverHFileName
        {
            get { return _tableMakerModel.StandardModel.FilePaths[1]; }
        }
        public string AndroidDriverCFileName
        {
            get { return _tableMakerModel.AndroidModel.FilePaths[0]; }
        }
        public string AndroidDriverHFileName
        {
            get { return _tableMakerModel.AndroidModel.FilePaths[1]; }
        }
        public string MiniDriverCFileName
        {
            get { return _tableMakerModel.MiniModel.FilePaths[0]; }
        }
        public string MiniDriverHFileName
        {
            get { return _tableMakerModel.MiniModel.FilePaths[1]; }
        }
        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
        public ICommand GenerateCommand
        {
            get
            {
                if (_generateCommand == null)
                {
                    _generateCommand = new RelayCommand(
                        param => { this.Generate(); }
                        );
                }
                return _generateCommand;
            }
        }


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
                if (MessageBox.Show("It will take a while to get the work done, Continue?", "Generate Tables and Drivers.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();
                    TableMakerService.Build(ref _tableMakerModel, 2800, false, false);
                    var project = _tableMakerModel.Project;
                    var folder = $@"{GlobalSettings.LocalFolder}{project.BatteryType.Name}\{project.Name}\{GlobalSettings.ProductFolderName}";
                    string time = Math.Round(stopwatch.Elapsed.TotalSeconds, 0).ToString() + "S";
                    MessageBox.Show($"Completed. It took {time} to get the job done.");
                    Process.Start(folder);
                }
            });
            t.Start();
        }

        #endregion // Public Methods

        #region Private Helpers

        #endregion // Private Helpers
    }
}