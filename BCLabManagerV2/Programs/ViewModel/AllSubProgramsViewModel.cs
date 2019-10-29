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
    public class AllSubProgramsViewModel : ViewModelBase
    {
        #region Fields

        readonly SubProgramRepository _subprogramRepository;
        SubProgramViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllSubProgramsViewModel()
        {

            _subprogramRepository = Repositories._subprogramRepository;

            // Subscribe for notifications of when a new customer is saved.
            _subprogramRepository.ItemAdded += this.OnSubProgramAddedToRepository;

            // Populate the AllSubProgramTypes collection with _subprogramtypeRepository.
            this.CreateAllSubPrograms();
        }

        void CreateAllSubPrograms()
        {
            using (var dbContext = new AppDbContext())
            {
                List<SubProgramViewModel> all =
                    (from bat in dbContext.SubPrograms
                     select new SubProgramViewModel(bat, _subprogramRepository, "")).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

                //foreach (SubProgramModelViewModel batmod in all)
                //batmod.PropertyChanged += this.OnSubProgramModelViewModelPropertyChanged;

                this.AllSubPrograms = new ObservableCollection<SubProgramViewModel>(all);     //再转换成Observable
                                                                                              //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
            }
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the SubProgramModelViewModel objects.
        /// </summary>
        public ObservableCollection<SubProgramViewModel> AllSubPrograms { get; private set; }

        public SubProgramViewModel SelectedItem    //绑定选中项，从而改变subprograms
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
            SubProgramClass model = new SubProgramClass();      //实例化一个新的model
            SubProgramViewModel viewmodel = new SubProgramViewModel(model, _subprogramRepository, "");      //实例化一个新的view model
            viewmodel.DisplayName = "SubProgram-Create";
            viewmodel.commandType = CommandType.Create;
            var SubProgramViewInstance = new SubProgramTemplateView();      //实例化一个新的view
            SubProgramViewInstance.DataContext = viewmodel;
            SubProgramViewInstance.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                //_subprogramRepository.AddItem(model);
                using (var dbContext = new AppDbContext())
                {
                    dbContext.SubPrograms.Add(model);
                    dbContext.SaveChanges();
                    this.AllSubPrograms.Add(viewmodel);
                }
            }
        }
        private void Edit()
        {
            SubProgramClass model = new SubProgramClass();      //实例化一个新的model
            SubProgramViewModel viewmodel = new SubProgramViewModel(model, _subprogramRepository, "");      //实例化一个新的view model
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
                    var sub = dbContext.SubPrograms.SingleOrDefault(i => i.Id == _selectedItem.Id);
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
            SubProgramClass model = new SubProgramClass();      //实例化一个新的model
            SubProgramViewModel viewmodel = new SubProgramViewModel(model, _subprogramRepository, "");      //实例化一个新的view model
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
                    dbContext.SubPrograms.Add(model);
                    dbContext.SaveChanges();
                    this.AllSubPrograms.Add(viewmodel);
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
            foreach (SubProgramViewModel viewmodel in this.AllSubPrograms)
                viewmodel.Dispose();

            this.AllSubPrograms.Clear();
            //this.AllSubProgramModels.CollectionChanged -= this.OnCollectionChanged;

            _subprogramRepository.ItemAdded -= this.OnSubProgramAddedToRepository;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        void OnSubProgramAddedToRepository(object sender, ItemAddedEventArgs<SubProgramClass> e)
        {
            var viewModel = new SubProgramViewModel(e.NewItem, _subprogramRepository, "");
            this.AllSubPrograms.Add(viewModel);
        }

        #endregion // Event Handling Methods
    }
}
