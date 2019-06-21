using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.Model;
using System.Diagnostics;

namespace BCLabManager.DataAccess
{
    /// <summary>
    /// Event arguments used by RepositoryBase's IItemAdded event.
    /// </summary>
    public class ItemAddedEventArgs<T> : EventArgs
    {
        public ItemAddedEventArgs(T newItem)
        {
            this.NewItem = newItem;
        }

        public T NewItem { get; private set; }
    }
    public abstract class RepositoryBase<T>
    {
        #region Fields

        readonly List<T> _items;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Creates a new repository of items.
        /// </summary>
        /// <param name="customerDataFile">The relative path to an XML resource file that contains customer data.</param>
        /*public CustomerRepository(string customerDataFile)
        {
            _batterymodels = LoadCustomers(customerDataFile);
        }*/

        public RepositoryBase()
        {
            _items = new List<T>();
        }

        public RepositoryBase(List<T> items)
        {
            _items = items;
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Raised when a customer is placed into the repository.
        /// </summary>
        public event EventHandler<ItemAddedEventArgs<T>> ItemAdded;

        /// <summary>
        /// Places the specified customer into the repository.
        /// If the customer is already in the repository, an
        /// exception is not thrown.
        /// </summary>
        public void AddItem(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (!_items.Contains(item))
            {
                _items.Add(item);

                if (this.ItemAdded != null)
                    this.ItemAdded(this, new ItemAddedEventArgs<T>(item));
            }
        }

        /// <summary>
        /// Returns true if the specified customer exists in the
        /// repository, or false if it is not.
        /// </summary>
        public bool ContainsItem(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return _items.Contains(item);
        }

        /// <summary>
        /// Returns a shallow-copied list of all customers in the repository.
        /// </summary>
        public List<T> GetItems()
        {
            return new List<T>(_items);
        }

        public abstract void SerializeToDatabase();
        public abstract void DeserializeFromDatabase();
        #endregion // Public Interface

        #region Private Helpers

        /*static List<T> LoadCustomers(string customerDataFile)
        {
            // In a real application, the data would come from an external source,
            // but for this demo let's keep things simple and use a resource file.
            using (Stream stream = GetResourceStream(customerDataFile))
            using (XmlReader xmlRdr = new XmlTextReader(stream))
                return
                    (from customerElem in XDocument.Load(xmlRdr).Element("customers").Elements("customer")
                     select Customer.CreateCustomer(
                        (double)customerElem.Attribute("totalSales"),
                        (string)customerElem.Attribute("firstName"),
                        (string)customerElem.Attribute("lastName"),
                        (bool)customerElem.Attribute("isCompany"),
                        (string)customerElem.Attribute("email")
                         )).ToList();
        }*/

        /*static Stream GetResourceStream(string resourceFile)
        {
            Uri uri = new Uri(resourceFile, UriKind.RelativeOrAbsolute);

            StreamResourceInfo info = Application.GetResourceStream(uri);
            if (info == null || info.Stream == null)
                throw new ApplicationException("Missing resource file: " + resourceFile);

            return info.Stream;
        }*/

        #endregion // Private Helpers
    }

    public class BatteryTypeRepository : RepositoryBase<BatteryTypeClass>
    {
        public BatteryTypeRepository(List<BatteryTypeClass> items)
            : base(items)
        {
        }
        public override void SerializeToDatabase()
        {
            throw new NotImplementedException();
        }
        public override void DeserializeFromDatabase()
        {
            throw new NotImplementedException();
        }
    }

    public class BatteryRepository : RepositoryBase<BatteryClass>
    {
        public BatteryRepository(List<BatteryClass> items)
            : base(items)
        {
        }
        public override void SerializeToDatabase()
        {
            throw new NotImplementedException();
        }
        public override void DeserializeFromDatabase()
        {
            throw new NotImplementedException();
        }
    }
    public class Repositories
    {
        public BatteryTypeRepository _batterytypeRepository;
        public BatteryRepository _batteryRepository;
        //public ChamberRepository _chamberRepository;
        //public TesterRepository _testerRepository;

        //public SubProgramRepository _subprogramRepository;
        //public RecipeRepository _recipeRepository;
        //public ChamberRecipeRepository _chamberrecipeRepository;
        //public TesterRecipeRepository _testerrecipeRepository;
        //public ProgramRepository _programRepository;
        //public RequestRepository _requestRepository;// = new RequestRepository();
        //public ExecutorRepository _executorRepository;


        private List<BatteryTypeClass> BatteryTypes;// { get; set; }
        private List<BatteryClass> Batteries;// { get; set; }
        private List<TesterClass> Testers;// { get; set; }
        private List<TesterChannelClass> TesterChannels;// { get; set; }
        private List<ChamberClass> Chambers;// { get; set; }
        private List<ProgramClass> Programs;// { get; set; }
        private List<SubProgramClass> SubPrograms;// { get; set; }
        //private List<RecipeClass> Recipes;// { get; set; }
        //private List<ChamberRecipeClass> ChamberRecipes;// { get; set; }
        //private List<TesterRecipeClass> TesterRecipes;// { get; set; }
        //private List<RequestClass> Requests;// { get; set; }
        //private List<ExecutorClass> Executors;

        public Repositories()
        {
            //string folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BCLabManager Documents\\");
            //dbmanager.DBInit(folder);
            //HistoricData();
            //TestCase4_1();
            //TestCase4_2();
            //TestCase4_3();
            //TestCase5_1();
            //TestCase5_2();
            //TestCase5_3();
            //TestCase5_4();
            //TestCase5_5();
            //TestCase6_1();
            //TestCase6_2();
            //TestCase6_3();
            //TestCase7_1();
            //TestCase7_2();
            //TestCase7_3();
            //TestCase9();
            HistoricRegistration();

            // Subscribe for notifications of when a new customer is saved.
            //_requestRepository.ItemAdded += this.OnRequestAddedToRepository;    //对于添加新Request的事件，应该由_executorRepository来订阅
        }

        #region Event Handler


        /*void OnRequestAddedToRepository(object sender, ItemAddedEventArgs<RequestClass> e)
        {
            //var viewModel = new RequestViewModel(e.NewItem, _requestRepository);
            //this.AllRequests.Add(viewModel);
            var executors = (from sp in e.NewItem.RequestedProgram.RequestedSubPrograms
                             from rec in sp.RequestedRecipes
                             from exe in rec.Executors
                             select exe).ToList();
            foreach (var exe in executors)
                _executorRepository.AddItem(exe);
        }*/
        #endregion  // Event Handler
        #region debugger
        [Conditional("DEBUG")]
        private void HistoricData()
        {
            HistoricRegistration();
            //HistoricOperation();
            //FakeView();
        }

        private void HistoricRegistration()
        {
            InitAssets();
            //InitPrograms();
            //InitRequests();
            //InitExecutors();  //用Requests就够了，如非必要勿增实体
        }

        private void InitAssets()
        {
            BatteryTypes = new List<BatteryTypeClass>();
            BatteryTypeClass bm = new BatteryTypeClass("Oppo", "BLP663", "Li-on", 4400, 3350, 3700, 3450, 3200);
            BatteryTypes.Add(bm);
            bm = new BatteryTypeClass("Apple", "NxT3", "Li-on", 4400, 3350, 3700, 3450, 3200);
            BatteryTypes.Add(bm);

            Batteries = new List<BatteryClass>();
            BatteryClass bat = new BatteryClass("pack1", BatteryTypes[0]);
            Batteries.Add(bat);
            bat = new BatteryClass("pack2", BatteryTypes[0]);
            Batteries.Add(bat);
            bat = new BatteryClass("pack3", BatteryTypes[0]);
            Batteries.Add(bat);
            bat = new BatteryClass("pack4", BatteryTypes[1]);
            Batteries.Add(bat);
            bat = new BatteryClass("pack5", BatteryTypes[1]);
            Batteries.Add(bat);

            Chambers = new List<ChamberClass>();
            ChamberClass chm = new ChamberClass("Hongzhan", "PUL-80", "40, 150");
            Chambers.Add(chm);

            Testers = new List<TesterClass>();
            TesterClass Tester = new TesterClass("Chroma", "17200");
            TesterChannels = new List<TesterChannelClass> { 
                new TesterChannelClass("1",Tester),
                new TesterChannelClass("2",Tester),
                new TesterChannelClass("3",Tester),
                new TesterChannelClass("4",Tester)};
            Testers.Add(Tester);

            _batterytypeRepository = new BatteryTypeRepository(BatteryTypes);
            _batteryRepository = new BatteryRepository(Batteries);
            //_chamberRepository = new ChamberRepository(Chambers);
            //_testerRepository = new TesterRepository(Testers);

        }
        #endregion //debugger
    }
}
