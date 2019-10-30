using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCLabManager.Model
{
    public class AssetUsageRecordClass : ModelBase
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int AssetUseCount { get; set; }
        public String ProgramName { get; set; }
        public String RecipeName { get; set; }
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

