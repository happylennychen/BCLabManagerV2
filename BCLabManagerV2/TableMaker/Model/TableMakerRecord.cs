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
        private Stage _stage;
        public Stage Stage
        {
            get { return _stage; }
            set { SetProperty(ref _stage, value); }
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
        private List<string> _ocvSources = new List<string>();
        public List<string> OCVSources 
        { 
            get { return _ocvSources; } 
            set { SetProperty(ref _ocvSources, value); }
        }
        private List<string> _rcSources = new List<string>();
        public List<string> RCSources
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