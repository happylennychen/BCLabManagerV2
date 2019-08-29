using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BCLabManager.ViewModel
{
    public class AllSubProgramTemplatesViewModel : ViewModelBase
    {
        #region Fields

        readonly SubProgramRepository _subprogramRepository;
        SubProgramTemplateViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllSubProgramTemplatesViewModel()
        {

            _subprogramRepository = Repositories._subprogramRepository;

            // Populate the AllSubProgramTypes collection with _subprogramtypeRepository.
            this.CreateAllSubProgramTemplates();
        }

        void CreateAllSubProgramTemplates()
        {
            using (var dbContext = new AppDbContext())
            {
                List<SubProgramTemplateViewModel> all =
                    (from spt in dbContext.SubProgramTemplates
                     select new SubProgramTemplateViewModel(spt, _subprogramRepository, "")).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

                //foreach (SubProgramModelViewModel batmod in all)
                //batmod.PropertyChanged += this.OnSubProgramModelViewModelPropertyChanged;

                this.AllSubProgramTemplates = new ObservableCollection<SubProgramTemplateViewModel>(all);     //再转换成Observable
                                                                                              //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
            }
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the SubProgramModelViewModel objects.
        /// </summary>
        public ObservableCollection<SubProgramTemplateViewModel> AllSubProgramTemplates { get; private set; }

        public SubProgramTemplateViewModel SelectedItem    //绑定选中项，从而改变subprograms
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    //OnPropertyChanged("SelectedType");
                    //OnPropertyChanged("Records"); //通知Records改变
                }
            }
        }

        public ICommand CreateCommand
        {
            get
            {
                if (_createCommand == null)
                {
                    _createCommand = new RelayCommand(
                        param => { this.Create();}
                        );
                }
                return _createCommand;
            }
        }
        public ICommand EditCommand
        {
            get
            {
                if (_editCommand == null)
                {
                    _editCommand = new RelayCommand(
                        param => { this.Edit(); },
                        param => this.CanEdit
                        );
                }
                return _editCommand;
            }
        }
        public ICommand SaveAsCommand
        {
            get
            {
                if (_saveAsCommand == null)
                {
                    _saveAsCommand = new RelayCommand(
                        param => { this.SaveAs(); },
                        param => this.CanSaveAs
                        );
                }
                return _saveAsCommand;
            }
        }

        #endregion // Public Interface

        #region Private Helper
        private void Create()
        {
            SubProgramTemplate model = new SubProgramTemplate();      //实例化一个新的model
            SubProgramTemplateViewModel viewmodel = new SubProgramTemplateViewModel(model, _subprogramRepository, "");      //实例化一个新的view model
            viewmodel.DisplayName = "SubProgramTemplate-Create";
            viewmodel.commandType = CommandType.Create;
            var SubProgramViewInstance = new SubProgramTemplateView();      //实例化一个新的view
            SubProgramViewInstance.DataContext = viewmodel;
            SubProgramViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                //_subprogramRepository.AddItem(model);
                using (var dbContext = new AppDbContext())
                {
                    dbContext.SubProgramTemplates.Add(model);
                    dbContext.SaveChanges();
                    this.AllSubProgramTemplates.Add(viewmodel);
                }
            }
        }
        private void Edit()
        {
            SubProgramTemplate model = new SubProgramTemplate();      //实例化一个新的model
            SubProgramTemplateViewModel viewmodel = new SubProgramTemplateViewModel(model, _subprogramRepository, "");      //实例化一个新的view model
            viewmodel.Name = _selectedItem.Name;
            viewmodel.TestCount = _selectedItem.TestCount;
            viewmodel.DisplayName = "SubProgram-Edit";
            viewmodel.commandType = CommandType.Edit;
            var SubProgramViewInstance = new SubProgramTemplateView();      //实例化一个新的view
            SubProgramViewInstance.DataContext = viewmodel;
            SubProgramViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _selectedItem.Name = viewmodel.Name;
                _selectedItem.TestCount = viewmodel.TestCount;
                using (var dbContext = new AppDbContext())
                {
                    var sub = dbContext.SubProgramTemplates.SingleOrDefault(i => i.Id == _selectedItem.Id);
                    sub.Name = _selectedItem.Name;
                    sub.TestCount = _selectedItem.TestCount;
                    dbContext.SaveChanges();
                }
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            SubProgramTemplate model = new SubProgramTemplate();      //实例化一个新的model
            SubProgramTemplateViewModel viewmodel = new SubProgramTemplateViewModel(model, _subprogramRepository, "");      //实例化一个新的view model
            viewmodel.Name = _selectedItem.Name;
            viewmodel.TestCount = _selectedItem.TestCount;
            viewmodel.DisplayName = "SubProgram-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var SubProgramViewInstance = new SubProgramTemplateView();      //实例化一个新的view
            SubProgramViewInstance.DataContext = viewmodel;
            SubProgramViewInstance.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.SubProgramTemplates.Add(model);
                    dbContext.SaveChanges();
                    this.AllSubProgramTemplates.Add(viewmodel);
                }
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedItem != null; }
        }
        #endregion //Private Helper
        #region  Base Class Overrides

        protected override void OnDispose()
        {
            foreach (SubProgramTemplateViewModel viewmodel in this.AllSubProgramTemplates)
                viewmodel.Dispose();

            this.AllSubProgramTemplates.Clear();
            //this.AllSubProgramModels.CollectionChanged -= this.OnCollectionChanged;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        #endregion // Event Handling Methods
    }
}
