using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class Event: BindableBase
    {
        public int Id { get; set; }

        private string _module;
        public string Module
        {
            get { return _module; }
            set { SetProperty(ref _module, value); }
        }
        private EventType eventType;
        public EventType Type
        {
            get { return eventType; }
            set { SetProperty(ref eventType, value); }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { SetProperty(ref _timestamp, value); }
        }
    }
    public enum EventType
    {
        Information,
        Warning,
        Error
    }
}
