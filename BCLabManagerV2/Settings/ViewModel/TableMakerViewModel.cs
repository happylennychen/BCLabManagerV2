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
                TableMakerService.Build(ref _tableMakerModel);
            });
            t.Start();
        }

        #endregion // Public Methods

        #region Private Helpers

        #endregion // Private Helpers
    }
}