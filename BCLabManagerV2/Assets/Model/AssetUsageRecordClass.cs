using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCLabManager.Model
{
    public class AssetUsageRecordClass : BindableBase
    {
        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { SetProperty(ref _timestamp, value); }
        }
        private int _assetUseCount;
        public int AssetUseCount
        {
            get { return _assetUseCount; }
            set { SetProperty(ref _assetUseCount, value); }
        }
        private string _programName;
        public string ProgramName
        {
            get { return _programName; }
            set { SetProperty(ref _programName, value); }
        }
        private string _recipeName;
        public string RecipeName
        {
            get { return _recipeName; }
            set { SetProperty(ref _recipeName, value); }
        }
        public int Id { get; set; }
        //public DateTime Timestamp { get; set; }
        //public int AssetUseCount { get; set; }
        //public String ProgramName { get; set; }
        //public String RecipeName { get; set; }
        //public AssetUsageRecordClass()
        //{

        //}
        public AssetUsageRecordClass(DateTime Timestamp, int AssetUseCount, String ProgramName, String RecipeName)
        {
            this.Timestamp = Timestamp;
            this.AssetUseCount = AssetUseCount;
            this.ProgramName = ProgramName;
            this.RecipeName = RecipeName;
        }
    }
}

