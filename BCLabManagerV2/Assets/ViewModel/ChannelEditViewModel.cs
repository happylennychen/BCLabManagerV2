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
    public class ChannelEditViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        readonly ChannelClass _channel;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public ChannelEditViewModel(ChannelClass channel, List<TesterClass> testers)
        {
            _channel = channel;
            _channel.PropertyChanged += _channel_PropertyChanged;
            CreateAllTesters(testers);
        }

        private void _channel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        void CreateAllTesters(List<TesterClass> testers)
        {
            this.AllTesters = testers;
        }

        #endregion // Constructor

        #region ChannelClass Properties

        public int Id
        {
            get { return _channel.Id; }
            set
            {
                if (value == _channel.Id)
                    return;

                _channel.Id = value;

                base.OnPropertyChanged("Id");
            }
        }

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

        public int AssetUseCount
        {
            get { return _channel.AssetUseCount; }
            set
            {
                if (value == _channel.AssetUseCount)
                    return;

                _channel.AssetUseCount = value;

                base.OnPropertyChanged("AssetUseCount");
            }
        }

        #endregion // Customer Properties

        #region Presentation Properties


        public TesterClass Tester
        {
            get { return _channel.Tester; }
            set
            {
                if (value == _channel.Tester)
                    return;

                _channel.Tester = value;

                base.OnPropertyChanged("Tester");
            }
        }

        public List<TesterClass> AllTesters { get; set; }

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
                //int number = (
                //    from bat in _channelRepository.GetItems()
                //    where bat.Name == _channel.Name     //名字（某一个属性）一样就认为是一样的
                //    select bat).Count();
                //if (number != 0)
                //    return false;
                //return !_channelRepository.ContainsItem(_channel);
                return true;
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