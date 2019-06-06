using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace O2Micro.BCLabManager.Model
{
    public enum AssetStatusEnum
    {
        IDLE,
        USING,
    }
    public class AssetClass
    {
        private static Int32 nextID = 1;
        private Int32 NextID
        {
            get
            {
                nextID += 1;
                return nextID - 1;
            }
        }
        public Int32 AssetID { get; set; }

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
                    status = value;
                else
                {
                    //Todo: throw exception here
                }
            }
        }

        public class Record
        {
            DateTime Timestamp;
            AssetStatusEnum Status;
            String ProgramName;
            String SubProgramName;

            public Record(DateTime Timestamp, AssetStatusEnum Status, String ProgramName, String SubProgramName)
            {
                this.Timestamp = Timestamp;
                this.Status = Status;
                this.ProgramName = ProgramName;
                this.SubProgramName = SubProgramName;
            }
        }
        public List<Record> Records { get; set; }

        public AssetClass()
        {
            this.AssetID = NextID;
            this.Status = AssetStatusEnum.IDLE;
            Records = new List<Record>();
        }

        public void UpdateRecords(DateTime Timestamp, AssetStatusEnum Status, String ProgramName, String SubProgramName)
        {
            Records.Add(new Record(Timestamp, Status, ProgramName, SubProgramName));
        }
    }
    public class BatteryTypeClass
    {
        private static Int32 nextID = 1;
        private Int32 NextID
        {
            get
            {
                nextID += 1;
                return nextID - 1;
            }
        }
        public Int32 BatteryTypeID { get; set; }
        public String Manufactor { get; set; }
        public String Name { get; set; }
        public String Material { get; set; }
        public Int32 LimitedChargeVoltage { get; set; }
        public Int32 RatedCapacity { get; set; }
        public Int32 NominalVoltage { get; set; }
        public Int32 TypicalCapacity { get; set; }
        public Int32 CutoffDischargeVoltage { get; set; }
        //public List<Battery> Batteries { get; set; }
        //public List<Program> TestPlans { get; set; }

        public BatteryTypeClass()
        { }

        public BatteryTypeClass(Int32 BatteryTypeID, String Manufactor, String Name, String Material, Int32 LimitedChargeVoltage, Int32 RatedCapacity, Int32 NominalVoltage, Int32 TypicalCapacity, Int32 CutoffDischargeVoltage)
        {
            this.BatteryTypeID = BatteryTypeID;
            this.Manufactor = Manufactor;
            this.Name = Name;
            this.Material = Material;
            this.LimitedChargeVoltage = LimitedChargeVoltage;
            this.RatedCapacity = RatedCapacity;
            this.NominalVoltage = NominalVoltage;
            this.TypicalCapacity = TypicalCapacity;
            this.CutoffDischargeVoltage = CutoffDischargeVoltage;
        }
        public BatteryTypeClass(String Manufactor, String Name, String Material, Int32 LimitedChargeVoltage, Int32 RatedCapacity, Int32 NominalVoltage, Int32 TypicalCapacity, Int32 CutoffDischargeVoltage)
        {
            this.BatteryTypeID = NextID;
            this.Manufactor = Manufactor;
            this.Name = Name;
            this.Material = Material;
            this.LimitedChargeVoltage = LimitedChargeVoltage;
            this.RatedCapacity = RatedCapacity;
            this.NominalVoltage = NominalVoltage;
            this.TypicalCapacity = TypicalCapacity;
            this.CutoffDischargeVoltage = CutoffDischargeVoltage;
        }
    }

    public class BatteryClass : AssetClass
    {
        private static Int32 nextID = 1;
        private Int32 NextID
        {
            get
            {
                nextID += 1;
                return nextID - 1;
            }
        }
        public Int32 BatteryID { get; set; }
        public String Name { get; set; }
        public BatteryTypeClass BatteryType { get; set; }
        public Double CycleCount { get; set; }
        //public AssestStatus Stauts { get; set; }

        public BatteryClass()
        { }

        public BatteryClass(Int32 BatteryID, String Name, BatteryTypeClass BatteryType, Double CycleCount = 0)
        {
            this.BatteryID = BatteryID;
            this.Name = Name;
            this.BatteryType = BatteryType;
            this.CycleCount = CycleCount;
        }
        public BatteryClass(String Name, BatteryTypeClass BatteryType, Double CycleCount = 0)
        {
            this.BatteryID = NextID;
            this.Name = Name;
            this.BatteryType = BatteryType;
            this.CycleCount = CycleCount;
        }
    }


    public class TesterClass
    {
        private static Int32 nextID = 1;
        private Int32 NextID
        {
            get
            {
                nextID += 1;
                return nextID - 1;
            }
        }
        public Int32 TesterID { get; set; }
        public String Manufactor { get; set; }
        public String Name { get; set; }
        public List<TesterChannelClass> TesterChannels { get; set; }

        public TesterClass(Int32 TesterID, String Manufactor, String Name, List<TesterChannelClass> TesterChannels)
        {
            this.TesterID = TesterID;
            this.Manufactor = Manufactor;
            this.Name = Name;
            this.TesterChannels = TesterChannels;
        }
        public TesterClass(String Manufactor, String Name, List<TesterChannelClass> TesterChannels)
        {
            this.TesterID = NextID;
            this.Manufactor = Manufactor;
            this.Name = Name;
            this.TesterChannels = TesterChannels;
        }
    }
    public class TesterChannelClass : AssetClass
    {
        private static Int32 nextID = 1;
        private Int32 NextID
        {
            get
            {
                nextID += 1;
                return nextID - 1;
            }
        }
        public Int32 TesterChannelID { get; set; }
        public TesterClass Tester { get; set; }

        public TesterChannelClass(Int32 TesterChannelID, TesterClass Tester)
        {
            this.TesterChannelID = TesterChannelID;
            this.Tester = Tester;
        }
        public TesterChannelClass(TesterClass Tester)
        {
            this.TesterChannelID = NextID;
            this.Tester = Tester;
        }
    }

    public class ChamberClass : AssetClass
    {
        private static Int32 nextID = 1;
        private Int32 NextID
        {
            get
            {
                nextID += 1;
                return nextID - 1;
            }
        }
        public Int32 ChamberID { get; set; }
        public String Manufactor { get; set; }
        public String Name { get; set; }
        public Double LowestTemperature { get; set; }
        public Double HighestTemperature { get; set; }

        public ChamberClass(Int32 ChamberID, String Manufactor, String Name, Double LowestTemperature, Double HighestTemperature)
        {
            this.ChamberID = ChamberID;
            this.Manufactor = Manufactor;
            this.Name = Name;
            this.LowestTemperature = LowestTemperature;
            this.HighestTemperature = HighestTemperature;
        }
        public ChamberClass(String Manufactor, String Name, String TemperatureRange)
        {
            this.ChamberID = NextID;
            this.Manufactor = Manufactor;
            this.Name = Name;
            this.LowestTemperature = LowestTemperature;
            this.HighestTemperature = HighestTemperature;
        }
    }
}

