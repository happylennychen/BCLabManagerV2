using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BCLabManager.Model
{
    public class AssetClass : BindBase, IAsset
    {
        private int assetUseCount = 0;
        public int AssetUseCount
        {
            get
            {
                return assetUseCount;
            }
            set
            {
                if (value != assetUseCount)
                {
                    assetUseCount = value;
                    OnPropertyChanged("AssetUseCount");
                }
                else
                {
                    //Todo: throw exception here
                }
            }
        }

        public ObservableCollection<AssetUsageRecordClass> Records { get; set; }

        public AssetClass()
        {
            this.AssetUseCount = 0;
            Records = new ObservableCollection<AssetUsageRecordClass>();
        }

        public void AddRecord(DateTime Timestamp, int AssetUseCount, String ProgramName, String RecipeName)
        {
            Records.Add(new AssetUsageRecordClass(Timestamp, AssetUseCount, ProgramName, RecipeName));
        }
    }
}

