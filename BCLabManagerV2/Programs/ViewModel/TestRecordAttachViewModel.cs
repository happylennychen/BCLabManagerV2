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

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TestRecordAttachViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        //string _programName;
        //string _RecipeName;
        readonly TestRecord _record;
        //readonly BatteryTypeRepository _batterytypeRepository;
        //readonly BatteryRepository _batteryRepository;
        //readonly ChamberRepository _chamberRepository;
        //readonly TesterRepository _testerRepository;
        //readonly ChannelRepository _channelRepository;
        ObservableCollection<Project> _projects;
        ObservableCollection<Program> _programs;
        ObservableCollection<Recipe> _recipes;

        //ObservableCollection<BatteryTypeClass> _allBatteryTypes;
        String _batteryType;
        List<Project> _allProjects;
        Project _project;
        ObservableCollection<Program> _allPrograms;
        Program _program;
        ObservableCollection<Recipe> _allRecipes;
        Recipe _recipe;
        //ObservableCollection<ChamberClass> _allChambers;
        //RelayCommand _executeCommand;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public TestRecordAttachViewModel(
            TestRecord record,
        string batteryType,
        ObservableCollection<Project> projects,
            ObservableCollection<Program> programs,
            ObservableCollection<Recipe> recipes)     //
        {
            if (record == null)
                throw new ArgumentNullException("record");

            _record = record;
            _batteryType = batteryType;
            _projects = projects;
            _programs = programs;
            _recipes = recipes;

            //_record.PropertyChanged += _record_PropertyChanged;
        }

        //private void _record_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion // Constructor

        #region Presentation Properties

        public Project Project   //选中项
        {
            get
            {
                //if (_record.BatteryStr == null)
                //return "/";
                return _project;
            }
            set
            {
                if (value == _project)
                    return;

                _project = value;

                RaisePropertyChanged("Project");
                AllPrograms = new ObservableCollection<Program>(_programs.Where(o => o.Project.Id == _project.Id).ToList());
            }
        }

        public List<Project> AllProjects    //供选项
        {
            get
            {
                if (_allProjects == null)
                {
                    _allProjects = _projects.Where(o => o.BatteryType.Name == _batteryType).ToList();
                }
                return _allProjects;
            }
        }

        public Program Program
        {
            get
            {
                //if (_record.ChannelStr == null)
                //return "/";
                return _program;
            }
            set
            {
                if (value == _program)
                    return;

                _program = value;

                RaisePropertyChanged("Program");


                //ObservableCollection<Recipe> all = _recipes;
                //List<Recipe> allstring = (
                //    from i in all
                //    where (i.Program.Id == Program.Id) && i.AssetUseCount == 0
                //    select i).ToList();
                //AllRecipes = new ObservableCollection<Recipe>(allstring);
                AllRecipes = _program.Recipes;

            }
        }

        public ObservableCollection<Program> AllPrograms    //供选项
        {
            get
            {
                return _allPrograms;
            }
            set
            {
                if (value != _allPrograms)
                {
                    _allPrograms = value;
                    RaisePropertyChanged("AllPrograms");
                }
            }
        }

        public Recipe Recipe
        {
            get
            {
                //if (_channel == null)
                //return "/";
                return _recipe;
            }
            set
            {
                if (value == _recipe)
                    return;

                _recipe = value;

                RaisePropertyChanged("Recipe");
                this.Temperature = _recipe.Temperature;
                if (_program.Type.Name == "EV")
                {
                    if (_program.Name.Contains("Dynamic"))
                        this.Current = 0;
                }
                else
                {
                    var index = _recipe.Name.IndexOf('A');
                    if (index != -1)
                    {
                        var currStr = _recipe.Name.Remove(index);
                        try
                        {
                            var currInA = Convert.ToDouble(currStr);
                            this.Current = currInA * 1000;
                        }
                        catch (Exception e)
                        {
                            //MessageBox.Show("Error when converting");
                            //return;
                        }
                    }
                }
                var array = Path.GetFileName(_record.TestFilePath).Split('_').ToList();
                if (array.Count == 5)
                    NewName = $@"{_program.Name}_{_recipe.Temperature}Deg-{_recipe.Name}_{array[2]}_{array[3]}_{array[4]}";
                else if (array.Count == 6)
                    NewName = $@"{_program.Name}_{_recipe.Temperature}Deg-{_recipe.Name}_{array[2]}_{array[3]}_{array[4]}_{array[5]}";
            }
        }

        public ObservableCollection<Recipe> AllRecipes    //供选项
        {
            get
            {
                if (_allRecipes == null)
                    _allRecipes = new ObservableCollection<Recipe>();
                return _allRecipes;
            }
            set
            {
                if (value != _allRecipes)
                {
                    _allRecipes = value;
                    RaisePropertyChanged("AllRecipes");
                }
            }
        }

        public String Comment
        {
            get
            {
                return _record.Comment;
            }
            set
            {
                if (value == _record.Comment)
                    return;

                _record.Comment = value;

                RaisePropertyChanged("Comment");
            }
        }

        public double Current
        {
            get
            {
                return _record.Current;
            }
            set
            {
                if (value == _record.Current)
                    return;

                _record.Current = value;

                RaisePropertyChanged("Current");
            }
        }

        public double Temperature
        {
            get
            {
                return _record.Temperature;
            }
            set
            {
                if (value == _record.Temperature)
                    return;

                _record.Temperature = value;

                RaisePropertyChanged("Temperature");
            }
        }

        private bool _isRename = true;
        public bool IsRename
        {
            get
            {
                return _isRename;
            }
            set
            {
                if (value == _isRename)
                    return;

                _isRename = value;

                RaisePropertyChanged("IsRename");
            }
        }


        private string _newName;
        public string NewName
        {
            get
            {
                return _newName;
            }
            set
            {
                if (value == _newName)
                    return;

                _newName = value;

                RaisePropertyChanged("NewName");
            }
        }

        #endregion // Presentation Properties


        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    switch (operationType)
                    {
                        case OperationType.Execute:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); }//,
                                //param => this.CanExecute
                                );
                            break;
                    }
                }
                return _okCommand;
            }
        }
        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        public void OK()
        {
            //if (!_programtype.IsValid)
            //throw new InvalidOperationException(Resources.ProgramTypeViewModel_Exception_CannotSave);

            //if (this.IsNewProgramType)
            //_programtypeRepository.AddItem(_programtype);

            //RaisePropertyChanged("DisplayName");
            IsOK = true;
        }

        public OperationType operationType
        { get; set; }

        public bool IsOK
        {
            get { return _isOK; }
            set { _isOK = value; }
        }
    }
}
