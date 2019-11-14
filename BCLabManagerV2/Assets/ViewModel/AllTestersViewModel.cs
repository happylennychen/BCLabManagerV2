﻿using System;
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
    public class AllTestersViewModel : BindableBase
    {
        #region Fields

        TesterViewModel _selectedItem;
        RelayCommand _createCommand;
        RelayCommand _editCommand;
        RelayCommand _saveAsCommand;
        RelayCommand _deleteCommand;
        //ObservableCollection<TesterClass> _testers;
        private TesterServieClass _testerService;
        private ChannelServieClass _channelService;
        #endregion // Fields

        #region Constructor

        public AllTestersViewModel(TesterServieClass testerService, ChannelServieClass channelService)
        {
            _testerService = testerService;
            _channelService = channelService;
            this.CreateAllTesters(_testerService.Items);
            _testerService.Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var tester = item as TesterClass;
                        this.AllTesters.Add(new TesterViewModel(tester));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var tester = item as TesterClass;
                        var deletetarget = this.AllTesters.SingleOrDefault(o => o.Id == tester.Id);
                        this.AllTesters.Remove(deletetarget);
                    }
                    break;
            }
        }

        void CreateAllTesters(ObservableCollection<TesterClass> testers)
        {
            List<TesterViewModel> all =
                (from tster in testers
                 select new TesterViewModel(tster)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllTesters = new ObservableCollection<TesterViewModel>(all);     //再转换成Observable
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
                    RaisePropertyChanged("Channels"); //通知Channels改变
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
                using (var dbContext = new AppDbContext())
                {
                    List<ChannelViewModel> all =
                      (from chn in dbContext.Channels
                       where chn.Tester.Id == SelectedItem.Id
                       select new ChannelViewModel(chn)).ToList();
                    return all;
                }
                //return null;
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
            TesterClass m = new TesterClass();      //实例化一个新的model
            TesterEditViewModel evm = new TesterEditViewModel(m);      //实例化一个新的view model
            //evm.DisplayName = "Tester-Create";
            var TesterViewInstance = new TesterView();      //实例化一个新的view
            TesterViewInstance.DataContext = evm;
            TesterViewInstance.ShowDialog();                   //设置viewmodel属性
            if (evm.IsOK == true)
            {
                _testerService.Add(m);
            }
        }
        private void Edit()
        {
            TesterClass m = new TesterClass();      //实例化一个新的model
            TesterEditViewModel evm = new TesterEditViewModel(m);      //实例化一个新的view model
            evm.Id = SelectedItem.Id;
            evm.Manufactor = _selectedItem.Manufactor;
            evm.Name = _selectedItem.Name;
            //evm.DisplayName = "Tester-Edit";
            var TesterViewInstance = new TesterView();      //实例化一个新的view
            TesterViewInstance.DataContext = evm;
            TesterViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                _testerService.Update(m);
            }
        }
        private bool CanEdit
        {
            get { return _selectedItem != null; }
        }
        private void SaveAs()
        {
            TesterClass m = new TesterClass();      //实例化一个新的model
            TesterEditViewModel evm = new TesterEditViewModel(m);      //实例化一个新的view model
            evm.Manufactor = _selectedItem.Manufactor;
            evm.Name = _selectedItem.Name;
            //evm.DisplayName = "Tester-Save As";
            var TesterViewInstance = new TesterView();      //实例化一个新的view
            TesterViewInstance.DataContext = evm;
            TesterViewInstance.ShowDialog();
            if (evm.IsOK == true)
            {
                _testerService.Add(m);
            }
        }
        private bool CanSaveAs
        {
            get { return _selectedItem != null; }
        }
        private void Delete()
        {
            if (_channelService.Items.Count(o => o.Tester.Id == _selectedItem.Id) != 0)
            {
                MessageBox.Show("Before deleting this tester, please delete all channels that belong to it.");
                return;
            }
            if (MessageBox.Show("Are you sure?", "Delete Tester", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _testerService.Remove(SelectedItem.Id);
            }
        }
        private bool CanDelete
        {
            get { return _selectedItem != null; }
        }
        #endregion //Private Helper
    }
}
