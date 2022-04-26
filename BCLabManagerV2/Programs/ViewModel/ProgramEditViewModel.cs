﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;
using Prism.Mvvm;
using System.Windows;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Program object.
    /// </summary>
    public class ProgramEditViewModel : BindableBaseWithName, IDataErrorInfo
    {
        #region Fields

        public Program _program;            //为了AllProgramsViewModel中的Edit，不得不开放给viewmodel。以后再想想有没有别的办法。
        ObservableCollection<Project> _projects;
        ObservableCollection<ProgramType> _programTypes;
        private ObservableCollection<RecipeTemplate> _recipeTemplates;
        private ObservableCollection<Program> _programs;

        //RecipeTemplateViewModel _selectedRecipeTemplate;
        //RecipeViewModel _selectedRecipe;
        //RecipeTemplateViewModel _selectedChoosenRecipeTemplate;
        RelayCommand _okCommand;
        //RelayCommand _addCommand;
        //RelayCommand _removeCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public ProgramEditViewModel(
            Program programmodel,
            ObservableCollection<Project> projects,
            ObservableCollection<RecipeTemplate> RecipeTemplates,
            ObservableCollection<ProgramType> programTypes,
            ObservableCollection<Program> programs)
        {
            _program = programmodel;
            _projects = projects;
            _programTypes = programTypes;
            _recipeTemplates = RecipeTemplates;
            _programs = programs;
            this.CreateAllRecipeTemplates(RecipeTemplates);
            this.CreateRecipes();
        }


        void CreateAllRecipeTemplates(ObservableCollection<RecipeTemplate> RecipeTemplates)
        {
            List<RecipeTemplateViewModel> all =
                (from sub in RecipeTemplates
                 where (sub.Group == null || sub.Group.Name != "Abandoned")
                 select new RecipeTemplateViewModel(sub)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllRecipeTemplates = new ObservableCollection<RecipeTemplateViewModel>(all);     //再转换成Observable
        }
        void CreateRecipes()
        {
            List<RecipeViewModel> all =
                (from sub in _program.Recipes
                 select new RecipeViewModel(sub)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //foreach (RecipeModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnRecipeModelViewModelPropertyChanged;

            this.Recipes = new ObservableCollection<RecipeViewModel>(all);     //再转换成Observable
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }
        #endregion // Constructor

        #region ProgramClass Properties

        public int Id
        {
            get { return _program.Id; }
            set
            {
                if (value == _program.Id)
                    return;

                _program.Id = value;

                RaisePropertyChanged("Id");
            }
        }

        public string Name
        {
            get { return _program.Name; }
            set
            {
                if (value == _program.Name)
                    return;

                _program.Name = value;

                RaisePropertyChanged("Name");
            }
        }
        private string _temperatures;
        public string Temperatures
        {
            get { return _temperatures; }
            set
            {
                if (value == _temperatures)
                    return;

                _temperatures = value;

                RaisePropertyChanged("Temperatures");
            }
        }

        public Project Project       //选中项
        {
            get
            {
                //if (_batteryType == null)
                //return null;
                return _program.Project;
            }
            set
            {
                if (value == _program.Project)
                    return;

                _program.Project = value;

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

        public ProgramType ProgramType       //选中项
        {
            get
            {
                //if (_batteryType == null)
                //return null;
                return _program.Type;
            }
            set
            {
                if (value == _program.Type)
                    return;

                _program.Type = value;

                RaisePropertyChanged("Type");
            }
        }

        public ObservableCollection<ProgramType> AllProgramTypes //供选项
        {
            get
            {
                ObservableCollection<ProgramType> all = _programTypes;

                return new ObservableCollection<ProgramType>(all);
            }
        }
        public string Requester
        {
            get { return _program.Requester; }
            set
            {
                if (value == _program.Requester)
                    return;

                _program.Requester = value;

                RaisePropertyChanged("Requester");
            }
        }

        public string Description
        {
            get { return _program.Description; }
            set
            {
                if (value == _program.Description)
                    return;

                _program.Description = value;

                RaisePropertyChanged("Description");
            }
        }

        public DateTime RequestDate
        {
            get { return _program.RequestTime; }
            set
            {
                if (value == _program.RequestTime)
                    return;

                _program.RequestTime = value;

                RaisePropertyChanged("RequestDate");
            }
        }

        public ObservableCollection<RecipeViewModel> Recipes { get; set; }        //这个是当前program所拥有的Recipes
        /*{
            get
            {
                if (_program.Recipes == null)
                    return new ObservableCollection<RecipeViewModel>();
                List<RecipeViewModel> all =
                    (from bat in _program.Recipes
                     select new RecipeViewModel(bat, _RecipeRepository)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

                //foreach (RecipeModelViewModel batmod in all)
                //batmod.PropertyChanged += this.OnRecipeModelViewModelPropertyChanged;

                return new ObservableCollection<RecipeViewModel>(all);     //再转换成Observable
            }
        }*/

        #endregion // Customer Properties

        #region Presentation Properties

        public ObservableCollection<RecipeTemplateViewModel> AllRecipeTemplates { get; private set; }   //展示所有RecipeTemplate以便选用,跟Recipes是不一样的

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
            List<int> tempList;
            if (GetTemperatureList(Temperatures, out tempList))
                _program.Temperatures = tempList;
            else
            {
                MessageBox.Show($"Parsing Temperature failed.");
                IsOK = false;
                return;
            }
            if (_program.Temperatures == null)
            {
                IsOK = false;
                return;
            }
            _program.RecipeTemplates = AllRecipeTemplates.Where(o => o.IsSelected).Select(o => o._recipeTemplate.Name).ToList();
            var dic = new Dictionary<int, int>();
            foreach (var recVM in AllRecipeTemplates)
            {
                if (recVM.IsSelected)
                {
                    if (recVM.Count == 0)
                        recVM.Count = 1;
                    dic.Add(recVM.Id, recVM.Count);
                }
            }
            if (_program.Type.Name == "Stage2RC")
            {
                var stage1program = _programs.SingleOrDefault(o => o.Project.Id == _program.Project.Id && o.Type.Name == "Stage1RC");
                if (stage1program != null)
                {
                    _program.BuildRecipes(_recipeTemplates.ToList(), dic, stage1program.Recipes.ToList());
                }
                else
                {
                    MessageBox.Show("Please create Stage1RC program before creating Stage2RC program.");
                    IsOK = false;
                    return;
                }
            }
            else
                _program.BuildRecipes(_recipeTemplates.ToList(), dic);
            IsOK = true;
        }

        private bool GetTemperatureList(string temperatures, out List<int> tempList)
        {
            try
            {
                tempList = temperatures.Trim(',').Split(',').Select(o => Convert.ToInt32(o)).ToList();
            }
            catch (Exception e)
            {
                tempList = null;
                return false;
            }
            return true;
        }


        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewProgram
        {
            get
            {
                //var dbContext = new AppDbContext();
                //int number = (
                //    from bat in dbContext.Programs
                //    where bat.Name == _program.Name     //名字（某一个属性）一样就认为是一样的
                //    select bat).Count();
                //if (number != 0)
                //    return false;
                //else
                return true;
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanCreate
        {
            get 
            {
                if (Project == null)
                    return false;
                if (ProgramType == null)
                    return false;
                if(!AllRecipeTemplates.Any(rt=>rt.IsSelected))
                    return false;
                if (Temperatures == null || Temperatures.Length == 0)
                    return false;
                if (Name == null || Name.Length == 0)
                    return false;
                if (Description == null || Description.Length == 0)
                    return false;
                if (Requester == null || Requester.Length == 0)
                    return false;
                List<int> tempList;
                if (!GetTemperatureList(Temperatures, out tempList))
                    return false;
                return true; 
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSaveAs
        {
            get { return IsNewProgram; }
        }
        public string Error
        {
            get
            {
                return null;
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "Name")
                {
                    if (Name != null && Name.Length > 30 )
                    {
                        return "长度超出范围";
                    }

                    if(Name != null && Name.Contains("_"))
                        {
                            return "Do not use \"_\"";
                        }
                    if (Name == string.Empty)
                    {
                        return "不能为空 ";
                    }
                }

                if (columnName == "Requester")
                {
                    if (Requester != null && Requester.Length > 30)
                    {
                        return "长度超出范围";
                    }

                  
                    if (Requester == string.Empty)
                    {
                        return "不能为空 ";
                    }
                }

                if(columnName == "Temperatures")
                {
                    if (Temperatures != null && Temperatures.Length > 30)
                    {
                        return "长度超出范围";
                    }


                    if (Temperatures == string.Empty)
                    {
                        return "不能为空 ";
                    }
                }
                return string.Empty;



            }


            #endregion // Private Helpers
        }
    }
}