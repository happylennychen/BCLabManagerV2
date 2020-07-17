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
using Microsoft.Win32;

namespace BCLabManager.ViewModel
{

    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TableMakerProductEditViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        readonly TableMakerProduct _tableMakerProduct;
        ObservableCollection<Project> _projects;
        RelayCommand _okCommand;
        RelayCommand _openFilesCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public TableMakerProductEditViewModel(
            TableMakerProduct tableMakerProduct,
            ObservableCollection<Project> projects,
            ObservableCollection<TableMakerProductType> tableMakerProductTypes)
        {
            if (tableMakerProduct == null)
                throw new ArgumentNullException("tableMakerProduct");

            _tableMakerProduct = tableMakerProduct;
            _projects = projects;

            CreateAllTableMakerProductTypes(tableMakerProductTypes);
        }

        void CreateAllTableMakerProductTypes(ObservableCollection<TableMakerProductType> tableMakerProductTypes)
        {

            this.AllTableMakerProductTypes = tableMakerProductTypes;
        }

        #endregion // Constructor

        #region TableMakerProductClass Properties

        public int Id
        {
            get { return _tableMakerProduct.Id; }
            set
            {
                if (value == _tableMakerProduct.Id)
                    return;

                _tableMakerProduct.Id = value;

                RaisePropertyChanged("Id");
            }
        }
        public string FilePath
        {
            get { return _tableMakerProduct.FilePath; }
            set
            {
                if (value == _tableMakerProduct.FilePath)
                    return;

                _tableMakerProduct.FilePath = value;

                RaisePropertyChanged("FilePath");
            }
        }
        public bool IsValid
        {
            get { return _tableMakerProduct.IsValid; }
            set
            {
                if (value == _tableMakerProduct.IsValid)
                    return;

                _tableMakerProduct.IsValid = value;

                RaisePropertyChanged("IsValid");
            }
        }

        public Project Project       //选中项
        {
            get
            {
                //if (_batteryType == null)
                //return null;
                return _tableMakerProduct.Project;
            }
            set
            {
                if (value == _tableMakerProduct.Project)
                    return;

                _tableMakerProduct.Project = value;

                RaisePropertyChanged("Project");
            }
        }

        public ObservableCollection<Project> AllProjects //供选项
        {
            get
            {
                ObservableCollection<Project> all = _projects;

                return new ObservableCollection<Project>(all);
            }
        }

        #endregion // Customer Properties

        #region Presentation Properties

        public TableMakerProductType TableMakerProductType
        {
            get { return _tableMakerProduct.Type; }
            set
            {
                if (value == _tableMakerProduct.Type)
                    return;

                _tableMakerProduct.Type = value;

                RaisePropertyChanged("Type");
            }
        }

        public ObservableCollection<TableMakerProductType> AllTableMakerProductTypes
        {
            get;set;
        }

        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    switch (commandType)
                    {
                        case CommandType.Create:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); },
                                param => this.CanCreate
                                );
                            break;
                        case CommandType.Edit:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); }
                                );
                            break;
                        case CommandType.SaveAs:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); },
                                param => this.CanSaveAs
                                );
                            break;
                    }
                }
                return _okCommand;
            }
        }

        public ICommand OpenFilesCommand
        {
            get
            {
                if (_openFilesCommand == null)
                {
                    _openFilesCommand = new RelayCommand(
                        param => { this.OpenFiles(); }//,
                                                      //param => this.CanExecute
                        );
                }
                return _openFilesCommand;
            }
        }
        public void OpenFiles()
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            //dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.NetworkShortcuts); No use, cannot see network driver
            //dialog.InitialDirectory = $@"Q:\807\Software\WH BC Lab\Data\{_record.BatteryTypeStr}\{_record.ProjectStr}\Raw Data";
            //dialog.InitialDirectory = $@"Q:\807\Software\WH BC Lab\Data\{_record.BatteryTypeStr}\High Power\Raw Data";
            //dialog.InitialDirectory = $@"{GlobalSettings.RootPath}{_record.BatteryTypeStr}\High Power\{GlobalSettings.TestDataFolderName}";
            if (dialog.ShowDialog() == true)
            {
                FilePath = dialog.FileName;
            }
        }

        public CommandType commandType
        { get; set; }

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
        public void OK()
        {
            //if (!_tableMakerProducttype.IsValid)
            //throw new InvalidOperationException(Resources.TableMakerProductTypeViewModel_Exception_CannotSave);

            //if (this.IsNewTableMakerProductType)
            //_tableMakerProducttypeRepository.AddItem(_tableMakerProducttype);

            //RaisePropertyChanged("DisplayName");
            IsOK = true;
        }

        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewTableMakerProduct
        {
            get
            {
                //var dbContext = new AppDbContext();
                //int number = (
                //    from bat in dbContext.TableMakerProducts
                //    where bat.Name == _tableMakerProduct.Name     //名字（某一个属性）一样就认为是一样的
                //    select bat).Count();
                //if (number != 0)
                //    return false;
                //else
                //    return true;
                return true;
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanCreate
        {
            get { return IsNewTableMakerProduct; }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSaveAs
        {
            get { return IsNewTableMakerProduct; }
        }

        #endregion // Private Helpers
    }
}