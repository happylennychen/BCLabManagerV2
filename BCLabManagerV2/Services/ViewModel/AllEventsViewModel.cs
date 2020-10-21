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
    /// <summary>
    /// Editable: True
    /// Updateable: 1. Events: True, 2. Records: True
    /// </summary>
    public class AllEventsViewModel : BindableBase
    {
        #region Fields
        //private EventService _eventService;

        #endregion // Fields

        #region Constructor

        public AllEventsViewModel(/*EventService eventService*/)
        {
            //_eventService = eventService;
            //this.CreateAllEvents(_eventService.Items);
            //_eventService.Items.CollectionChanged += Items_CollectionChanged;
            this.CreateAllEvents(EventService.Items);
            EventService.Items.CollectionChanged += Items_CollectionChanged;
        }
        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var Event = item as Event;
                        this.AllEvents.Add(new EventViewModel(Event));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var Event = item as Event;
                        var deletetarget = this.AllEvents.SingleOrDefault(o => o.Id == Event.Id);
                        this.AllEvents.Remove(deletetarget);
                    }
                    break;
            }
        }

        void CreateAllEvents(ObservableCollection<Event> Events)
        {
            List<EventViewModel> all =
                (from cmb in Events
                 select new EventViewModel(cmb)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllEvents = new ObservableCollection<EventViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the EventModelViewModel objects.
        /// </summary>
        public ObservableCollection<EventViewModel> AllEvents { get; private set; }

        #endregion // Public Interface
    }
}
