using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCLabManager.Model
{
    public enum AssetStatusEnum
    {
        IDLE,
        USING,
    }
    public class AssetUsageRecordClass : ModelBase
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public AssetStatusEnum Status { get; set; }
        public String ProgramName { get; set; }
        public String SubProgramName { get; set; }
        //public AssetUsageRecordClass()
        //{

        //}
        public AssetUsageRecordClass(DateTime Timestamp, AssetStatusEnum Status, String ProgramName, String SubProgramName)
        {
            this.Timestamp = Timestamp;
            this.Status = Status;
            this.ProgramName = ProgramName;
            this.SubProgramName = SubProgramName;
        }
    }
    public class AssetClass : ModelBase
    {
        private AssetStatusEnum status = new AssetStatusEnum();
        public AssetStatusEnum Status
        {
            get
            {
                return status;
            }
            set
            {
                if (value != status)
                {
                    status = value;
                    OnPropertyChanged("Status");
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
            this.Status = AssetStatusEnum.IDLE;
            Records = new List<AssetUsageRecordClass>();
        }

        public void AddRecord(DateTime Timestamp, AssetStatusEnum Status, String ProgramName, String SubProgramName)
        {
            Records.Add(new AssetUsageRecordClass(Timestamp, Status, ProgramName, SubProgramName));
        }
    }
}

