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
            TableMakerService.GetFileNames(ref _tableMakerModel);
        }
        #endregion // Constructor

        #region Presentation Properties
        public bool OCV1Ready
        {
            get
            {

                //var project = _tableMakerModel.Project;

                //var programs = _tableMakerModel.Programs.Select(o => o).Where(o => o.Project.Id == project.Id && o.IsCompleted == true && (o.Type.Name == "OCV")).ToList();

                //if (programs.Count >= 1)
                //{
                //    return true;    //需要进一步改进
                //}

                return false;
            }
        }
        public bool OCV2Ready
        {
            get
            {

                var project = _tableMakerModel.Project;

                var programs = _tableMakerModel.Stage2OCVPrograms.Select(o => o).Where(o => o.Project.Id == project.Id && o.IsCompleted == true).ToList();

                var recipesList = programs.Select(o => o.Recipes);
                List<Recipe> recipes = new List<Recipe>();
                foreach (var recs in recipesList)
                {
                    recipes = recipes.Concat(recs).ToList();
                }

                if (recipes.Count == 2)
                {
                    return true;    //需要进一步改进
                }

                return false;
            }
        }

        public bool RC1Ready
        {
            get
            {
                var project = _tableMakerModel.Project;

                var programs = _tableMakerModel.Stage1RCPrograms.Select(o => o).Where(o => o.Project.Id == project.Id && o.IsCompleted == true).ToList();

                if (programs.Count == 1)
                {
                    return true;    //需要进一步改进
                }

                return false;
            }
        }

        public bool RC2Ready
        {
            get
            {
                var project = _tableMakerModel.Project;

                var programs = _tableMakerModel.Stage2RCPrograms.Select(o => o).Where(o => o.Project.Id == project.Id && o.IsCompleted == true).ToList();

                if (programs.Count == 1 && RC1Ready)
                {
                    return true;    //需要进一步改进
                }

                return false;
            }
        }
        public bool SD1Ready
        {
            get { return (OCV1Ready|OCV2Ready) & RC1Ready; }
        }
        public bool SD2Ready
        {
            get { return OCV2Ready & RC2Ready; }
        }
        public bool AD1Ready
        {
            get { return (OCV1Ready | OCV2Ready) & RC1Ready; }
        }
        public bool AD2Ready
        {
            get { return OCV2Ready & RC2Ready; }
        }
        public bool Mini1Ready
        {
            get { return (OCV1Ready | OCV2Ready) & RC1Ready; }
        }
        public bool Mini2Ready
        {
            get { return OCV2Ready & RC2Ready; }
        }
        public bool Lite1Ready
        {
            get { return (OCV1Ready | OCV2Ready) & RC1Ready; }
        }
        public bool Lite2Ready
        {
            get { return OCV2Ready & RC2Ready; }
        }

        public string OCV1FileName
        {
            get { return _tableMakerModel.OCVModel.FileName; }
        }

        public string OCV2FileName
        {
            get { return _tableMakerModel.OCVModel.FileName; }
        }
        public string RC1FileName
        {
            get { return _tableMakerModel.RCModel.FileName; }
        }
        public string RC2FileName
        {
            get { return _tableMakerModel.RCModel.FileName; }
        }
        public string StandardDriverC1FileName
        {
            get { return _tableMakerModel.StandardModel.FileNames[0]; }
        }
        public string StandardDriverC2FileName
        {
            get { return _tableMakerModel.StandardModel.FileNames[0]; }
        }
        public string StandardDriverH1FileName
        {
            get { return _tableMakerModel.StandardModel.FileNames[1]; }
        }
        public string StandardDriverH2FileName
        {
            get { return _tableMakerModel.StandardModel.FileNames[1]; }
        }
        public string AndroidDriverC1FileName
        {
            get { return _tableMakerModel.AndroidModel.FileNames[0]; }
        }
        public string AndroidDriverC2FileName
        {
            get { return _tableMakerModel.AndroidModel.FileNames[0]; }
        }
        public string AndroidDriverH1FileName
        {
            get { return _tableMakerModel.AndroidModel.FileNames[1]; }
        }
        public string AndroidDriverH2FileName
        {
            get { return _tableMakerModel.AndroidModel.FileNames[1]; }
        }
        public string MiniDriverC1FileName
        {
            get { return _tableMakerModel.MiniModel.FileNames[0]; }
        }
        public string MiniDriverC2FileName
        {
            get { return _tableMakerModel.MiniModel.FileNames[0]; }
        }
        public string MiniDriverH1FileName
        {
            get { return _tableMakerModel.MiniModel.FileNames[1]; }
        }
        public string MiniDriverH2FileName
        {
            get { return _tableMakerModel.MiniModel.FileNames[1]; }
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
                    TableMakerService.Build(ref _tableMakerModel, 2800, "Some description");
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