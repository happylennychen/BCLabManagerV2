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
    public class AssetClass
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
        public String Manufactor { get; set; }
        public String Name { get; set; }
        public String Material { get; set; }
        public Int32 LimitedChargeVoltage { get; set; }
        public Int32 RatedCapacity { get; set; }
        public Int32 NominalVoltage { get; set; }
        public Int32 TypicalCapacity { get; set; }
        public Int32 CutoffDischargeVoltage { get; set; }

        public BatteryTypeClass()
        { }

        public BatteryTypeClass(String Manufactor, String Name, String Material, Int32 LimitedChargeVoltage, Int32 RatedCapacity, Int32 NominalVoltage, Int32 TypicalCapacity, Int32 CutoffDischargeVoltage)
        {
            this.Manufactor = Manufactor;
            this.Name = Name;
            this.Material = Material;
            this.LimitedChargeVoltage = LimitedChargeVoltage;
            this.RatedCapacity = RatedCapacity;
            this.NominalVoltage = NominalVoltage;
            this.TypicalCapacity = TypicalCapacity;
            this.CutoffDischargeVoltage = CutoffDischargeVoltage;
        }
        public void Update(String Manufactor, String Name, String Material, Int32 LimitedChargeVoltage, Int32 RatedCapacity, Int32 NominalVoltage, Int32 TypicalCapacity, Int32 CutoffDischargeVoltage)
        {
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
        public String Name { get; set; }
        public BatteryTypeClass BatteryType { get; set; }
        public Double CycleCount { get; set; }

        public BatteryClass(String Name, BatteryTypeClass BatteryType, Double CycleCount = 0)
        {
            //this.BatteryID = NextID;
            this.Name = Name;
            this.BatteryType = BatteryType;
            this.CycleCount = CycleCount;
        }

        public void Update(String Name, BatteryTypeClass BatteryType, Double CycleCount = 0)
        {
            this.Name = Name;
            this.BatteryType = BatteryType;
            this.CycleCount = CycleCount;
        }
    }


    public class TesterClass
    {
        public String Manufactor { get; set; }
        public String Name { get; set; }

        public TesterClass(String Manufactor, String Name)
        {
            //this.TesterID = NextID;
            this.Manufactor = Manufactor;
            this.Name = Name;
        }

        public void Update(String Manufactor, String Name)
        {
            this.Manufactor = Manufactor;
            this.Name = Name;
        }
    }
    public class TesterChannelClass : AssetClass
    {
        public TesterClass Tester { get; set; }
        public String Name { get; set; }

        public TesterChannelClass(String Name, TesterClass Tester)
        {
            this.Tester = Tester;
            this.Name = Name;
        }

        public void Update(String Name, TesterClass Tester)
        {
            this.Tester = Tester;
            this.Name = Name;
        }
    }

    public class ChamberClass : AssetClass
    {
        public String Manufactor { get; set; }
        public String Name { get; set; }
        public Double LowestTemperature { get; set; }
        public Double HighestTemperature { get; set; }
        
        public ChamberClass(String Manufactor, String Name, String TemperatureRange)
        {
            this.Manufactor = Manufactor;
            this.Name = Name;
            this.LowestTemperature = LowestTemperature;
            this.HighestTemperature = HighestTemperature;
        }

        public void Update(String Manufactor, String Name, String TemperatureRange)
        {
            this.Manufactor = Manufactor;
            this.Name = Name;
            this.LowestTemperature = LowestTemperature;
            this.HighestTemperature = HighestTemperature;
        }
    }
}

