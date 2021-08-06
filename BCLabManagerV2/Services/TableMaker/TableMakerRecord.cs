using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace BCLabManager.Model
{
    public class TableMakerRecord : BindableBase
    {
        public int Id { get; set; }

        private string _tablemakerversion;
        public string TableMakerVersion
        {
            get { return _tablemakerversion; }
            set { SetProperty(ref _tablemakerversion, value); }
        }
        private Project _project;
        public Project Project
        {
            get { return _project; }
            set { SetProperty(ref _project, value); }
        }
        private uint _eod;
        public uint EOD
        {
            get { return _eod; }
            set { SetProperty(ref _eod, value); }
        }
        private List<int> _voltagePoints = new List<int>();
        //[NotMapped]
        public List<int> VoltagePoints
        {
            get { return _voltagePoints; }
            set { SetProperty(ref _voltagePoints, value); }
        }
        private bool _isvalid;
        public bool IsValid
        {
            get { return _isvalid; }
            set { SetProperty(ref _isvalid, value); }
        }
        private List<TestRecord> _ocvSources = new List<TestRecord>();
        public List<TestRecord> OCVSources 
        { 
            get { return _ocvSources; } 
            set { SetProperty(ref _ocvSources, value); }
        }
        private List<TestRecord> _rcSources = new List<TestRecord>();
        public List<TestRecord> RCSources
        {
            get { return _rcSources; }
            set { SetProperty(ref _rcSources, value); }
        }
        private List<TableMakerProduct> _products = new List<TableMakerProduct>();
        public List<TableMakerProduct> Products
        {
            get { return _products; }
            set { SetProperty(ref _products, value); }
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
}