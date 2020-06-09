using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BCLabManager.Model
{
    public class Asset : BindableBase, IAsset
    {
        private int assetUseCount = 0;
        public int AssetUseCount
        {
            get { return assetUseCount; }
            set { SetProperty(ref assetUseCount, value); }
        }
        //private int assetUseCount = 0;
        //public int AssetUseCount
        //{
        //    get
        //    {
        //        return assetUseCount;
        //    }
        //    set
        //    {
        //        if (value != assetUseCount)
        //        {
        //            assetUseCount = value;
        //            RaisePropertyChanged("AssetUseCount");
        //        }
        //        else
        //        {
        //            //Todo: throw exception here
        //        }
        //    }
        //}

        public ObservableCollection<AssetUsageRecord> Records { get; set; }

        public Asset()
        {
            this.AssetUseCount = 0;
            Records = new ObservableCollection<AssetUsageRecord>();
        }

        public void AddRecord(DateTime Timestamp, int AssetUseCount, String ProgramName, String RecipeName)
        {
            Records.Add(new AssetUsageRecord(Timestamp, AssetUseCount, ProgramName, RecipeName));
        }
    }
}

