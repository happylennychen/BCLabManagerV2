using System;
using System.Collections.ObjectModel;

namespace BCLabManager.Model
{
    public interface IAsset
    {
        int AssetUseCount { get; set; }
        ObservableCollection<AssetUsageRecordClass> Records { get; set; }

        void AddRecord(DateTime Timestamp, int AssetUseCount, string ProgramName, string RecipeName);
    }
}