using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using System.Collections.ObjectModel;

namespace BCLabManager.ViewModel
{
    public class AllBatteryTypesViewModel : ViewModelBase
    {
        #region Fields

        readonly BatteryTypeRepository _batterytypeRepository;

        #endregion // Fields

        #region Constructor

        public AllBatteryTypesViewModel(BatteryTypeRepository batterytypeRepository)
        {
            if (batterytypeRepository == null)
                throw new ArgumentNullException("batterytypeRepository");

            _batterytypeRepository = batterytypeRepository;

            // Subscribe for notifications of when a new customer is saved.
            _batterytypeRepository.ItemAdded += this.OnBatteryModelAddedToRepository;

            // Populate the AllCustomers collection with BatteryModelViewModels.
            //this.CreateAllBatteryModels();
        }

        /*void CreateAllBatteryModels()
        {
            List<BatteryTypeViewModel> all =
                (from bat in _batterytypeRepository.GetItems()
                 select new BatteryTypeViewModel(base.mainWindowViewModel, bat, _batterytypeRepository)).ToList();

            //foreach (BatteryModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnBatteryModelViewModelPropertyChanged;

            this.AllBatteryModels = new ObservableCollection<BatteryTypeViewModel>(all);
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }*/

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the BatteryModelViewModel objects.
        /// </summary>
        public ObservableCollection<BatteryTypeViewModel> AllBatteryModels { get; private set; }


        #endregion // Public Interface

        #region  Base Class Overrides

        protected override void OnDispose()
        {
            foreach (BatteryTypeViewModel custVM in this.AllBatteryModels)
                custVM.Dispose();

            this.AllBatteryModels.Clear();
            //this.AllBatteryModels.CollectionChanged -= this.OnCollectionChanged;

            _batterytypeRepository.ItemAdded -= this.OnBatteryModelAddedToRepository;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        void OnBatteryModelAddedToRepository(object sender, ItemAddedEventArgs<BatteryTypeClass> e)
        {
            var viewModel = new BatteryTypeViewModel(e.NewItem, _batterytypeRepository);
            this.AllBatteryModels.Add(viewModel);
        }

        #endregion // Event Handling Methods
    }
}
