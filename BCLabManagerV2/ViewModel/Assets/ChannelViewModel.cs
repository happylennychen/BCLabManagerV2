using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class ChannelViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        readonly ChannelClass _channel;
        readonly ChannelRepository _channelRepository;
        readonly TesterRepository _testerRepository;
        //bool _isSelected;
        string _tester;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public ChannelViewModel(ChannelClass channelmodel, ChannelRepository channelRepository, TesterRepository testerRepository)
        {
            if (channelmodel == null)
                throw new ArgumentNullException("channelmodel");

            if (channelRepository == null)
                throw new ArgumentNullException("channelRepository");

            if (testerRepository == null)
                throw new ArgumentNullException("channelmodelRepository");

            _channel = channelmodel;
            _channelRepository = channelRepository;
            _testerRepository = testerRepository;
            _channel.PropertyChanged += _channel_PropertyChanged;
            // Populate the AllCustomers collection with TesterViewModels.
            //this.CreateAllTesters();      
        }

        private void _channel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        /*void CreateAllTesters()
        {
            List<TesterClass> all = _testerRepository.GetItems();

            this.AllTesters = new ObservableCollection<TesterClass>(all);
        }*/

        #endregion // Constructor

        #region ChannelClass Properties

        public string Name
        {
            get { return _channel.Name; }
            set
            {
                if (value == _channel.Name)
                    return;

                _channel.Name = value;

                base.OnPropertyChanged("Name");
            }
        }

        public AssetStatusEnum Status
        {
            get { return _channel.Status; }
            set
            {
                if (value == _channel.Status)
                    return;

                _channel.Status = value;

                base.OnPropertyChanged("Status");
            }
        }

        #endregion // Customer Properties

        #region Presentation Properties


        public String Tester
        {
            get
            {
                if (_tester == null)
                {
                    if (_channel.Tester == null)
                        _tester = string.Empty;
                    else
                        _tester = _channel.Tester.Name;
                }
                return _tester;
            }
            set
            {
                if (value == _tester || String.IsNullOrEmpty(value))
                    return;

                _tester = value;

                _channel.Tester = _testerRepository.GetItems().First(i => i.Name == _tester);

                base.OnPropertyChanged("Tester");
            }
        }

        public ObservableCollection<string> AllTesters
        {
            get
            {
                List<TesterClass> all = _testerRepository.GetItems();
                List<string> allstring = (
                    from i in all
                    select i.Name).ToList();

                return new ObservableCollection<string>(allstring);
            }
        }

        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    switch (commandType)
                    {
                        case CommandType.Create:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); },
                                param => this.CanCreate
                                );
                            break;
                        case CommandType.Edit:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); }
                                );
                            break;
                        case CommandType.SaveAs:
                            _okCommand = new RelayCommand(
                                param => { this.OK(); },
                                param => this.CanSaveAs
                                );
                            break;
                    }
                }
                return _okCommand;
            }
        }

        public CommandType commandType
        { get; set; }

        public bool IsOK
        {
            get { return _isOK; }
            set { _isOK = value; }
        }

        #endregion // Presentation Properties

        #region Public Methods

        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        public void OK()
        {
            //if (!_tester.IsValid)
            //throw new InvalidOperationException(Resources.TesterViewModel_Exception_CannotSave);

            //if (this.IsNewTester)
            //_testerRepository.AddItem(_tester);

            //base.OnPropertyChanged("DisplayName");
            IsOK = true;
        }

        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewChannel
        {
            get
            {
                int number = (
                    from bat in _channelRepository.GetItems()
                    where bat.Name == _channel.Name     //名字（某一个属性）一样就认为是一样的
                    select bat).Count();
                if (number != 0)
                    return false;
                return !_channelRepository.ContainsItem(_channel);
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanCreate
        {
            get { return IsNewChannel; }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanSaveAs
        {
            get { return IsNewChannel; }
        }

        #endregion // Private Helpers
    }
}