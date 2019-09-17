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

namespace BCLabManager.ViewModel
{
    public class AllChannelsViewModel : ViewModelBase
    {
        #region Fields
        ObservableCollection<ChannelClass> _channels;
        List<TesterClass> _testers;
        ChannelViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _deleteCommand;

        #endregion // Fields

        #region Constructor

        public AllChannelsViewModel(ObservableCollection<ChannelClass> channels, List<TesterClass> testers)
        {
            _channels = channels;
            _testers = testers;
            // Populate the AllTesters collection with _testerRepository.
            this.CreateAllChannels(channels);
        }

        void CreateAllChannels(ObservableCollection<ChannelClass> channels)
        {
            List<ChannelViewModel> all =
                (from c in channels
                 select new ChannelViewModel(c)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllChannels = new ObservableCollection<ChannelViewModel>(all);     //再转换成Observable
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

        public List<AssetUsageRecordViewModel> Records //绑定选中channel的Records。只显示，所以只有get没有set。每次改选type，都要重新做一次查询    //不需要ObservableCollection，因为每次变化都已经被通知了
        //如果不是用查询，那么需要维护一个二维List。每一个Channel，对应一个List。用空间换时间。
        {
            get
            {
                if (SelectedItem == null)
                    return null;
                //List<AssetUsageRecordViewModel> all =
                //  (from bat in _channelRepository.GetItems()
                //   where bat.Name == SelectedItem.Name
                //   from record in bat.Records
                //   select new AssetUsageRecordViewModel(record)).ToList();
                //return all;
                using (var dbContext = new AppDbContext())
                {
                    List<AssetUsageRecordViewModel> all =
                      (from chn in dbContext.Channels
                       where chn.Id == SelectedItem.Id
                       from record in chn.Records
                       select new AssetUsageRecordViewModel(record)).ToList();
                    return all;
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
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(
                        param => { this.Delete(); },
                        param => this.CanDelete
                        );
                }
                return _deleteCommand;
            }
        }

        #endregion // Public Interface

        #region Private Helper
        private void Create()
        {
            ChannelClass m = new ChannelClass();      //实例化一个新的model
            ChannelEditViewModel evm = new ChannelEditViewModel(m, _testers);      //实例化一个新的view model
            evm.DisplayName = "Channel-Create";
            evm.commandType = CommandType.Create;
            var ChannelViewInstance = new ChannelView();      //实例化一个新的view
            ChannelViewInstance.DataContext = evm;
            ChannelViewInstance.ShowDialog();                   //设置viewmodel属性
            if (evm.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    //dbContext.Channels.Add(bc);    //不能直接这样写，不然会报错。这里不是添加一个全新的graph，而是添加一个新的m，然后修改关系
                    var newc = new ChannelClass()
                    {
                        Name = m.Name,
                    };
                    newc.Tester = dbContext.Testers.SingleOrDefault(i => i.Id == m.Tester.Id);
                    dbContext.Channels.Add(newc);
                    dbContext.SaveChanges();    //side effect是会更新newc的id
                    m = newc;                  //所以把newc存到using语句外面的bc里
                }
                _channels.Add(m);
                this.AllChannels.Add(new ChannelViewModel(m));    //然后用m生成vm，这样ID就会更新
            }
        }
        private void Edit()
        {
            ChannelClass m = new ChannelClass();      //实例化一个新的model
            ChannelEditViewModel evm = new ChannelEditViewModel(m, _testers);      //实例化一个新的view model
            evm.Name = _selectedItem.Name;
            evm.Tester = evm.AllTesters.SingleOrDefault(i => i.Id == _selectedItem.Tester.Id);   //所以改用Id来找到List里的item
            evm.Status = _selectedItem.Status;
            evm.DisplayName = "Channel-Edit";
            evm.commandType = CommandType.Edit;
            var ChannelViewInstance = new ChannelView();      //实例化一个新的view
            ChannelViewInstance.DataContext = evm;
            ChannelViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                _selectedItem.Name = evm.Name;
                _selectedItem.Tester = evm.Tester;
                using (var dbContext = new AppDbContext())
                {
                    var c = dbContext.Channels.SingleOrDefault(i => i.Id == _selectedItem.Id);
                    c.Name = m.Name;
                    c.Tester = dbContext.Testers.SingleOrDefault(i => i.Id == m.Tester.Id);

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
            ChannelClass m = new ChannelClass();      //实例化一个新的model
            ChannelEditViewModel evm = new ChannelEditViewModel(m, _testers);      //实例化一个新的view model
            evm.Name = _selectedItem.Name;
            //evm.Tester = _selectedItem.Tester;
            evm.Tester = evm.AllTesters.SingleOrDefault(i => i.Id == _selectedItem.Tester.Id);   //所以改用Id来找到List里的item
            evm.Status = _selectedItem.Status;
            evm.DisplayName = "Channel-Save As";
            evm.commandType = CommandType.SaveAs;
            var ChannelViewInstance = new ChannelView();      //实例化一个新的view
            ChannelViewInstance.DataContext = evm;
            ChannelViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                using (var dbContext = new AppDbContext())
                {
                    //dbContext.Channels.Add(bc);    //不能直接这样写，不然会报错。这里不是添加一个全新的graph，而是添加一个新的m，然后修改关系
                    var newc = new ChannelClass()
                    {
                        Name = m.Name,
                    };
                    newc.Tester = dbContext.Testers.SingleOrDefault(i => i.Id == m.Tester.Id);
                    dbContext.Channels.Add(newc);
                    dbContext.SaveChanges();    //side effect是会更新newc的id
                    m = newc;                  //所以把newc存到using语句外面的bc里
                }
                _channels.Add(m);
                this.AllChannels.Add(new ChannelViewModel(m));    //然后用m生成vm，这样ID就会更新
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedItem != null; }
        }
        private void Delete()
        {
            using (var dbContext = new AppDbContext())
            {
                var model = dbContext.Channels.SingleOrDefault(o => o.Id == _selectedItem.Id);
                if (model.Status == AssetStatusEnum.USING)
                {
                    MessageBox.Show("Cannot delete using battery.");
                    return;
                }
                if (MessageBox.Show("Are you sure?", "Delete Channel", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    dbContext.Channels.Remove(model);
                    dbContext.SaveChanges();

                    model = _channels.SingleOrDefault(o => o.Id == _selectedItem.Id);
                    _channels.Remove(model);
                    this.AllChannels.Remove(_selectedItem);
                }
            }
        }
        private bool CanDelete
        {
            get { return _selectedItem != null; }
        }
        #endregion //Private Helper
        #region  Base Class Overrides

        protected override void OnDispose()
        {
            foreach (ChannelViewModel viewmodel in this.AllChannels)
                viewmodel.Dispose();

            this.AllChannels.Clear();
            //this.AllChannelModels.CollectionChanged -= this.OnCollectionChanged;

            //_channelRepository.ItemAdded -= this.OnChannelAddedToRepository;
        }

        #endregion // Base Class Overrides

        #region Event Handling Methods

        //void OnChannelAddedToRepository(object sender, ItemAddedEventArgs<ChannelClass> e)
        //{
        //    var viewModel = new ChannelViewModel(e.NewItem, _channelRepository, _testerRepository);
        //    this.AllChannels.Add(viewModel);
        //}

        #endregion // Event Handling Methods
    }
}
