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
    public class AllBatteryTypesViewModel : ViewModelBase
    {
        #region Fields

        readonly BatteryTypeRepository _batterytypeRepository;
        readonly BatteryRepository _batteryRepository;
        BatteryTypeViewModel _selectedType;
        RelayCommand _createCommand;

        #endregion // Fields

        #region Constructor

        public AllBatteryTypesViewModel(BatteryTypeRepository batterytypeRepository, BatteryRepository batteryRepository)
        {
            if (batterytypeRepository == null)
                throw new ArgumentNullException("batterytypeRepository");

            if (batteryRepository == null)
                throw new ArgumentNullException("batteryRepository");

            _batterytypeRepository = batterytypeRepository;

            _batteryRepository = batteryRepository;

            // Subscribe for notifications of when a new customer is saved.
            _batterytypeRepository.ItemAdded += this.OnBatteryModelAddedToRepository;

            // Populate the AllBatteryTypes collection with _batterytypeRepository.
            this.CreateAllBatteryTypes();
        }

        void CreateAllBatteryTypes()
        {
            List<BatteryTypeViewModel> all =
                (from batT in _batterytypeRepository.GetItems()
                 select new BatteryTypeViewModel(batT, _batterytypeRepository)).ToList();

            //foreach (BatteryModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnBatteryModelViewModelPropertyChanged;

            this.AllBatteryTypes = new ObservableCollection<BatteryTypeViewModel>(all);
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the BatteryModelViewModel objects.
        /// </summary>
        public ObservableCollection<BatteryTypeViewModel> AllBatteryTypes { get; private set; }
        public BatteryTypeViewModel SelectedType 
        {
            get
            {
                return _selectedType;
            }
            set
            {
                _selectedType = value;
                OnPropertyChanged("SelectedType");
                OnPropertyChanged("Batteries");
            }
        }
        public ObservableCollection<BatteryViewModel> Batteries 
        {
            get
            {
                if (SelectedType == null)
                    return null;
                List<BatteryViewModel> all =
                  (from bat in _batteryRepository.GetItems()
                   where bat.BatteryType.Name == SelectedType.Name
                   select new BatteryViewModel(bat, _batteryRepository)).ToList();
                return new ObservableCollection<BatteryViewModel>(all);
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

        #endregion // Public Interface

        #region Private Helper
        private void Create()
        {
            BatteryTypeClass btc = new BatteryTypeClass();
            BatteryTypeViewModel btvm = new BatteryTypeViewModel(btc, _batterytypeRepository);
            btvm.DisplayName = "Battery Type-Create";
            var BatteryTypeViewInstance = new BatteryTypeView();
            BatteryTypeViewInstance.DataContext = btvm;
            BatteryTypeViewInstance.Show();
        }
        #endregion //Private Helper
        #region  Base Class Overrides

        protected override void OnDispose()
        {
            foreach (BatteryTypeViewModel custVM in this.AllBatteryTypes)
                custVM.Dispose();

            this.AllBatteryTypes.Clear();
            //this.AllBatteryModels.CollectionChanged -= this.OnCollectionChanged;

            _batterytypeRepository.ItemAdded -= this.OnBatteryModelAddedToRepository;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        void OnBatteryModelAddedToRepository(object sender, ItemAddedEventArgs<BatteryTypeClass> e)
        {
            var viewModel = new BatteryTypeViewModel(e.NewItem, _batterytypeRepository);
            this.AllBatteryTypes.Add(viewModel);
        }

        #endregion // Event Handling Methods
    }
}
