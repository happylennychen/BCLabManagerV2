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
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    public class AllChannelsViewModel : BindableBase
    {
        #region Fields
        //ObservableCollection<ChannelClass> _channels;
        //ObservableCollection<TesterClass> _testers;
        private ChannelServieClass _channelService;
        private TesterServieClass _testerService;
        ChannelViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _deleteCommand;

        #endregion // Fields

        #region Constructor

        public AllChannelsViewModel(ChannelServieClass channelService, TesterServieClass testerService)
        {
            _channelService = channelService;
            _testerService = testerService;
            // Populate the AllTesters collection with _testerRepository.
            this.CreateAllChannels(_channelService.Items);
            _channelService.Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var channel = item as ChannelClass;
                        this.AllChannels.Add(new ChannelViewModel(channel));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var channel = item as ChannelClass;
                        var deletetarget = this.AllChannels.SingleOrDefault(o => o.Id == channel.Id);
                        this.AllChannels.Remove(deletetarget);
                    }
                    break;
            }
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
                    //RaisePropertyChanged("SelectedType");
                    RaisePropertyChanged("Records"); //通知Records改变
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
                        param => { this.Create(); }
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
            ChannelEditViewModel evm = new ChannelEditViewModel(m, _testerService.Items);      //实例化一个新的view model
            //evm.DisplayName = "Channel-Create";
            evm.commandType = CommandType.Create;
            var ChannelViewInstance = new ChannelView();      //实例化一个新的view
            ChannelViewInstance.DataContext = evm;
            ChannelViewInstance.ShowDialog();                   //设置viewmodel属性
            if (evm.IsOK == true)
            {
                _channelService.SuperAdd(m);
            }
        }
        private void Edit()
        {
            ChannelClass m = new ChannelClass();      //实例化一个新的model
            ChannelEditViewModel evm = new ChannelEditViewModel(m, _testerService.Items);      //实例化一个新的view model
            evm.Id = SelectedItem.Id;
            evm.Name = _selectedItem.Name;
            evm.Tester = evm.AllTesters.SingleOrDefault(i => i.Id == _selectedItem.Tester.Id);   //所以改用Id来找到List里的item
            evm.AssetUseCount = _selectedItem.AssetUseCount;
            //evm.DisplayName = "Channel-Edit";
            evm.commandType = CommandType.Edit;
            var ChannelViewInstance = new ChannelView();      //实例化一个新的view
            ChannelViewInstance.DataContext = evm;
            ChannelViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                _channelService.SuperUpdate(m);
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            ChannelClass m = new ChannelClass();      //实例化一个新的model
            ChannelEditViewModel evm = new ChannelEditViewModel(m, _testerService.Items);      //实例化一个新的view model
            evm.Name = _selectedItem.Name;
            //evm.Tester = _selectedItem.Tester;
            evm.Tester = evm.AllTesters.SingleOrDefault(i => i.Id == _selectedItem.Tester.Id);   //所以改用Id来找到List里的item
            evm.AssetUseCount = _selectedItem.AssetUseCount;
            //evm.DisplayName = "Channel-Save As";
            evm.commandType = CommandType.SaveAs;
            var ChannelViewInstance = new ChannelView();      //实例化一个新的view
            ChannelViewInstance.DataContext = evm;
            ChannelViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                _channelService.SuperAdd(m);
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedItem != null; }
        }
        private void Delete()
        {
            var model = _channelService.Items.SingleOrDefault(o => o.Id == _selectedItem.Id);
            if (model.AssetUseCount > 0)
            {
                MessageBox.Show("Cannot delete using battery.");
                return;
            }
            if (MessageBox.Show("Are you sure?", "Delete Channel", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _channelService.SuperRemove(SelectedItem.Id);
            }
        }
        private bool CanDelete
        {
            get { return _selectedItem != null; }
        }
        #endregion //Private Helper
    }
}
