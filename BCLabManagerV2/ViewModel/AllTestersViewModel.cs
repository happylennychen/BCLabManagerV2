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
    public class AllTestersViewModel : ViewModelBase
    {
        #region Fields

        readonly TesterRepository _testerRepository;
        readonly ChannelRepository _channelRepository;
        TesterViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllTestersViewModel(TesterRepository testerRepository, ChannelRepository channelRepository)
        {
            if (testerRepository == null)
                throw new ArgumentNullException("testerRepository");

            if (channelRepository == null)
                throw new ArgumentNullException("channelRepository");

            _testerRepository = testerRepository;

            _channelRepository = channelRepository;

            // Subscribe for notifications of when a new customer is saved.
            _testerRepository.ItemAdded += this.OnTesterAddedToRepository;

            // Populate the AllTesters collection with _testerRepository.
            this.CreateAllTesters();
        }

        void CreateAllTesters()
        {
            List<TesterViewModel> all =
                (from tster in _testerRepository.GetItems()
                 select new TesterViewModel(tster, _testerRepository)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //foreach (ChannelModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnChannelModelViewModelPropertyChanged;

            this.AllTesters = new ObservableCollection<TesterViewModel>(all);     //再转换成Observable
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the ChannelModelViewModel objects.
        /// </summary>
        public ObservableCollection<TesterViewModel> AllTesters { get; private set; }

        public TesterViewModel SelectedItem    //绑定选中项，从而改变batteries
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
                    OnPropertyChanged("Channels"); //通知Channels改变
                }
            }
        }
        public List<ChannelViewModel> Channels //绑定选中type的batteries。只显示，所以只有get没有set。每次改选type，都要重新做一次查询    //不需要ObservableCollection，因为每次变化都已经被通知了
        //如果不是用查询，那么需要维护一个二维List。每一个Tester，对应一个List。用空间换时间。
        {
            get
            {
                if (SelectedItem == null)
                    return null;
                List<ChannelViewModel> all =
                  (from bat in _channelRepository.GetItems()
                   where bat.Tester.Name == SelectedItem.Name
                   select new ChannelViewModel(bat, _channelRepository, _testerRepository)).ToList();
                return all;
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
            TesterClass btc = new TesterClass();      //实例化一个新的model
            TesterViewModel btvm = new TesterViewModel(btc, _testerRepository);      //实例化一个新的view model
            btvm.DisplayName = "Channel Type-Create";
            var TesterViewInstance = new TesterView();      //实例化一个新的view
            TesterViewInstance.DataContext = btvm;
            TesterViewInstance.ShowDialog();                   //设置viewmodel属性
            if (btvm.IsOK == true)
            {
                _testerRepository.AddItem(btc);
            }
        }
        private void Edit()
        {
            TesterClass btc = new TesterClass();      //实例化一个新的model
            TesterViewModel btvm = new TesterViewModel(btc, _testerRepository);      //实例化一个新的view model
            btvm.Manufactor = _selectedItem.Manufactor;
            btvm.Name = _selectedItem.Name;
            btvm.DisplayName = "Channel Type-Edit";
            var TesterViewInstance = new TesterView();      //实例化一个新的view
            TesterViewInstance.DataContext = btvm;
            TesterViewInstance.ShowDialog();
            if (btvm.IsOK == true)
            {
                _selectedItem.Manufactor = btvm.Manufactor;
                _selectedItem.Name = btvm.Name;
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            TesterClass btc = new TesterClass();      //实例化一个新的model
            TesterViewModel btvm = new TesterViewModel(btc, _testerRepository);      //实例化一个新的view model
            btvm.Manufactor = _selectedItem.Manufactor;
            btvm.Name = _selectedItem.Name;
            btvm.DisplayName = "Channel Type-Save As";
            var TesterViewInstance = new TesterView();      //实例化一个新的view
            TesterViewInstance.DataContext = btvm;
            TesterViewInstance.ShowDialog();
            if (btvm.IsOK == true)
            {
                _testerRepository.AddItem(btc);
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
            foreach (TesterViewModel custVM in this.AllTesters)
                custVM.Dispose();

            this.AllTesters.Clear();
            //this.AllChannelModels.CollectionChanged -= this.OnCollectionChanged;

            _testerRepository.ItemAdded -= this.OnTesterAddedToRepository;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        void OnTesterAddedToRepository(object sender, ItemAddedEventArgs<TesterClass> e)
        {
            var viewModel = new TesterViewModel(e.NewItem, _testerRepository);
            this.AllTesters.Add(viewModel);
        }

        #endregion // Event Handling Methods
    }
}
