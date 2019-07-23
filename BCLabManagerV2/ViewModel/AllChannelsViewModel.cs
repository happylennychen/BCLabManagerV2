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
    public class AllChannelsViewModel : ViewModelBase
    {
        #region Fields

        //readonly TesterRepository _testerRepository;
        readonly ChannelRepository _channelRepository;
        readonly TesterRepository _testerRepository;
        ChannelViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;

        #endregion // Fields

        #region Constructor

        public AllChannelsViewModel(ChannelRepository channelRepository, TesterRepository testerRepository)
        {
            if (channelRepository == null)
                throw new ArgumentNullException("channelRepository");

            _channelRepository = channelRepository;

            if (testerRepository == null)
                throw new ArgumentNullException("testerRepository");

            _channelRepository = channelRepository;
            _testerRepository = testerRepository;

            // Subscribe for notifications of when a new customer is saved.
            _channelRepository.ItemAdded += this.OnChannelAddedToRepository;

            // Populate the AllTesters collection with _testerRepository.
            this.CreateAllChannels();
        }

        void CreateAllChannels()
        {
            List<ChannelViewModel> all =
                (from bat in _channelRepository.GetItems()
                 select new ChannelViewModel(bat,_channelRepository, _testerRepository)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //foreach (ChannelModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnChannelModelViewModelPropertyChanged;

            this.AllChannels = new ObservableCollection<ChannelViewModel>(all);     //再转换成Observable
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the ChannelModelViewModel objects.
        /// </summary>
        public ObservableCollection<ChannelViewModel> AllChannels { get; private set; }

        public ChannelViewModel SelectedItem    //绑定选中项，从而改变batteries
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
                    OnPropertyChanged("Records"); //通知Records改变
                }
            }
        }

        public List<RecordViewModel> Records //绑定选中channel的Records。只显示，所以只有get没有set。每次改选type，都要重新做一次查询    //不需要ObservableCollection，因为每次变化都已经被通知了
        //如果不是用查询，那么需要维护一个二维List。每一个Channel，对应一个List。用空间换时间。
        {
            get
            {
                if (SelectedItem == null)
                    return null;
                List<RecordViewModel> all =
                  (from bat in _channelRepository.GetItems()
                   where bat.Name == SelectedItem.Name
                   from record in bat.Records
                   select new RecordViewModel(record)).ToList();
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
            ChannelClass bc = new ChannelClass();      //实例化一个新的model
            ChannelViewModel bvm = new ChannelViewModel(bc, _channelRepository, _testerRepository);      //实例化一个新的view model
            bvm.DisplayName = "Channel-Create";
            bvm.commandType = CommandType.Create;
            var ChannelViewInstance = new ChannelView();      //实例化一个新的view
            ChannelViewInstance.DataContext = bvm;
            ChannelViewInstance.ShowDialog();                   //设置viewmodel属性
            if (bvm.IsOK == true)
            {
                _channelRepository.AddItem(bc);
            }
        }
        private void Edit()
        {
            ChannelClass bc = new ChannelClass();      //实例化一个新的model
            ChannelViewModel bvm = new ChannelViewModel(bc, _channelRepository, _testerRepository);      //实例化一个新的view model
            bvm.Name = _selectedItem.Name;
            bvm.Tester = _selectedItem.Tester;
            bvm.Status = _selectedItem.Status;
            bvm.DisplayName = "Channel-Edit";
            bvm.commandType = CommandType.Edit;
            var ChannelViewInstance = new ChannelView();      //实例化一个新的view
            ChannelViewInstance.DataContext = bvm;
            ChannelViewInstance.ShowDialog();
            if (bvm.IsOK == true)
            {
                _selectedItem.Name = bvm.Name;
                _selectedItem.Tester = bvm.Tester;
                _selectedItem.Status = bvm.Status;
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            ChannelClass bc = new ChannelClass();      //实例化一个新的model
            ChannelViewModel bvm = new ChannelViewModel(bc, _channelRepository, _testerRepository);      //实例化一个新的view model
            bvm.Name = _selectedItem.Name;
            bvm.Tester = _selectedItem.Tester;
            bvm.Status = _selectedItem.Status;
            bvm.DisplayName = "Channel-Edit";
            bvm.commandType = CommandType.SaveAs;
            var ChannelViewInstance = new ChannelView();      //实例化一个新的view
            ChannelViewInstance.DataContext = bvm;
            ChannelViewInstance.ShowDialog();
            if (bvm.IsOK == true)
            {
                _channelRepository.AddItem(bc);
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
            foreach (ChannelViewModel custVM in this.AllChannels)
                custVM.Dispose();

            this.AllChannels.Clear();
            //this.AllChannelModels.CollectionChanged -= this.OnCollectionChanged;

            _channelRepository.ItemAdded -= this.OnChannelAddedToRepository;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        void OnChannelAddedToRepository(object sender, ItemAddedEventArgs<ChannelClass> e)
        {
            var viewModel = new ChannelViewModel(e.NewItem, _channelRepository, _testerRepository);
            this.AllChannels.Add(viewModel);
        }

        #endregion // Event Handling Methods
    }
}
