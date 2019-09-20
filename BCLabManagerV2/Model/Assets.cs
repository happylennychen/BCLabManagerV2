using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCLabManager.Model
{
    //public enum AssetStatusEnum
    //{
    //    IDLE,
    //    USING,
    //}
    public class AssetUsageRecordClass : ModelBase
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int AssetUseCount { get; set; }
        public String ProgramName { get; set; }
        public String SubProgramName { get; set; }
        //public AssetUsageRecordClass()
        //{

        //}
        public AssetUsageRecordClass(DateTime Timestamp, int AssetUseCount, String ProgramName, String SubProgramName)
        {
            this.Timestamp = Timestamp;
            this.AssetUseCount = AssetUseCount;
            this.ProgramName = ProgramName;
            this.SubProgramName = SubProgramName;
        }
    }
    public class AssetClass : ModelBase
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

        public List<AssetUsageRecordClass> Records { get; set; }

        public AssetClass()
        {
            this.AssetUseCount = 0;
            Records = new List<AssetUsageRecordClass>();
        }

        public void AddRecord(DateTime Timestamp, int AssetUseCount, String ProgramName, String SubProgramName)
        {
            Records.Add(new AssetUsageRecordClass(Timestamp, AssetUseCount, ProgramName, SubProgramName));
        }
    }
}

