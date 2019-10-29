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

        #endregion // Fields

        #region Constructor

        public ChannelViewModel(ChannelClass channel)
        {
            _channel = channel;
            _channel.PropertyChanged += _channel_PropertyChanged;
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

        #endregion // Customer Properties
    }
}