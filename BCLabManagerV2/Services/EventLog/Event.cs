using BCLabManager.Model;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace BCLabManager
{
    public enum EventType
    {
        CommitError
    }
    internal class Event:BindableBase
    {
        private DateTime _timestamp;
        public DateTime TimeStamp
        {
            get { return _timestamp; }
            set { SetProperty(ref _timestamp, value); }
        }
        private EventType _type;
        public EventType EventType
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
    }
}