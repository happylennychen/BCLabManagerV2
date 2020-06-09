using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// Editable: no need
    /// Updateable: true
    /// </summary>
    public class ChannelViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        readonly Channel _channel;

        #endregion // Fields

        #region Constructor

        public ChannelViewModel(Channel channel)
        {
            _channel = channel;
            _channel.PropertyChanged += _channel_PropertyChanged;
        }

        private void _channel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region ChannelClass Properties

        public int Id
        {
            get { return _channel.Id; }
        }

        public string Name
        {
            get { return _channel.Name; }
        }

        public int AssetUseCount
        {
            get { return _channel.AssetUseCount; }
        }
        public Tester Tester
        {
            get { return _channel.Tester; }
        }

        #endregion // Customer Properties
    }
}