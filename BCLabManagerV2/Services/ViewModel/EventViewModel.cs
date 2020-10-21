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
    public class EventViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        readonly Event _event;

        #endregion // Fields

        #region Constructor

        public EventViewModel(Event Event)
        {
            _event = Event;
            _event.PropertyChanged += _Event_PropertyChanged;
        }

        private void _Event_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region EventClass Properties

        public int Id
        {
            get { return _event.Id; }
        }
        public string Module
        {
            get { return _event.Module; }
        }

        public EventType Type
        {
            get { return _event.Type; }
        }
        public string Description
        {
            get { return _event.Description; }
        }
        public DateTime Timestamp
        {
            get { return _event.Timestamp; }
        }

        #endregion
    }
}