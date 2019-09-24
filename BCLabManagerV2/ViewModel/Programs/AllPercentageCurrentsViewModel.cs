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
    public class AllPercentageCurrentsViewModel : ViewModelBase
    {
        #region Fields
        List<PercentageCurrentClass> _percentageCurrents;
        PercentageCurrentViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllPercentageCurrentsViewModel(List<PercentageCurrentClass> PercentageCurrents)
        {
            this.CreateAllPercentageCurrents(PercentageCurrents);
        }

        void CreateAllPercentageCurrents(List<PercentageCurrentClass> PercentageCurrents)
        {
            _percentageCurrents = PercentageCurrents;
            List<PercentageCurrentViewModel> all =
                (from ct in PercentageCurrents
                 select new PercentageCurrentViewModel(ct)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllPercentageCurrents = new ObservableCollection<PercentageCurrentViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the SubProgramModelViewModel objects.
        /// </summary>
        public ObservableCollection<PercentageCurrentViewModel> AllPercentageCurrents { get; private set; }

        public PercentageCurrentViewModel SelectedItem    //绑定选中项，从而改变subprograms
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
            PercentageCurrentClass model = new PercentageCurrentClass();      //实例化一个新的model
            PercentageCurrentEditViewModel viewmodel = new PercentageCurrentEditViewModel(model);      //实例化一个新的view model
            viewmodel.DisplayName = "ChargeCurrent-Create";
            viewmodel.commandType = CommandType.Create;
            var view = new PercentageCurrentEditView();      //实例化一个新的view
            view.DataContext = viewmodel;
            view.ShowDialog();                   //设置viewmodel属性
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.PercentageCurrents.Add(model);
                    dbContext.SaveChanges();
                }
                _percentageCurrents.Add(model);
                this.AllPercentageCurrents.Add(new PercentageCurrentViewModel(model));
            }
        }
        private void Edit()
        {
            PercentageCurrentClass model = new PercentageCurrentClass();      //实例化一个新的model
            PercentageCurrentEditViewModel viewmodel = new PercentageCurrentEditViewModel(model);      //实例化一个新的view model
            viewmodel.Value = _selectedItem.Value;
            viewmodel.DisplayName = "ChargeCurrent-Edit";
            viewmodel.commandType = CommandType.Edit;
            var view = new PercentageCurrentEditView();      //实例化一个新的view
            view.DataContext = viewmodel;
            view.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                _selectedItem.Value = viewmodel.Value;
                using (var dbContext = new AppDbContext())
                {
                    var ct = dbContext.PercentageCurrents.SingleOrDefault(i => i.Id == _selectedItem.Id);
                    ct.Value = _selectedItem.Value;
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
            PercentageCurrentClass model = new PercentageCurrentClass();      //实例化一个新的model
            PercentageCurrentEditViewModel viewmodel = new PercentageCurrentEditViewModel(model);      //实例化一个新的view model
            viewmodel.Value = _selectedItem.Value;
            viewmodel.DisplayName = "ChargeCurrent-Save As";
            viewmodel.commandType = CommandType.SaveAs;
            var view = new PercentageCurrentEditView();      //实例化一个新的view
            view.DataContext = viewmodel;
            view.ShowDialog();
            if (viewmodel.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.PercentageCurrents.Add(model);
                    dbContext.SaveChanges();
                }
                _percentageCurrents.Add(model);
                this.AllPercentageCurrents.Add(new PercentageCurrentViewModel(model));
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
            foreach (PercentageCurrentViewModel viewmodel in this.AllPercentageCurrents)
                viewmodel.Dispose();

            this.AllPercentageCurrents.Clear();
            //this.AllSubProgramModels.CollectionChanged -= this.OnCollectionChanged;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        #endregion // Event Handling Methods
    }
}
