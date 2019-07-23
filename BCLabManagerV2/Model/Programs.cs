﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BCLabManager.Model
{
    public enum TestCountEnum
    {
        One,
        Two,
    }
    public enum TestStatus
    {
        Waiting,
        Executing,
        Completed,
        Invalid,
        Abandoned,
    }
    public class RawDataClass
    { }
    public class TestRecordClass
    {
        private TestStatus status = TestStatus.Waiting;
        public TestStatus Status
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
                    OnRasieStatusChangedEvent();
                }
            }
        }
        public BatteryClass Battery { get; set; }
        public ChannelClass TesterChannel { get; set; }
        public String Steps { get; set; }
        public ChamberClass Chamber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public String Description { get; set; }
        public RawDataClass RawData { get; set; }
        public Double NewCycle { get; set; }
        public SubProgramClass SubProgram { get; set; }

        public event EventHandler StatusChanged;

        protected virtual void OnRasieStatusChangedEvent()
        {
            EventHandler handler = StatusChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public TestRecordClass(SubProgramClass SubProgram)
        {
            this.SubProgram = SubProgram;
            this.Status = TestStatus.Waiting;
            this.Description = String.Empty;
            this.Battery = null;
            this.TesterChannel = null;
            this.Chamber = null;
        }

        public void Execute(BatteryClass Battery, ChamberClass Chamber, ChannelClass TesterChannel, String Steps, DateTime StartTime)
        {
        }

        public void Commit(TestStatus Status, DateTime EndTime, RawDataClass RawData, Double NewCycle, String Description = "")  //Need to check the Executor Status to make sure it is executing
        {
        }

        public void Abandon(String Description = "")
        {
        }

        public void Invalidate(String Description = "")
        {
        }
    }
    // Summary:
    //     Represents a sub program which can be united to form a program
    public class SubProgramClass
    {
        public String Name { get; set; }
        public TestCountEnum TestCount { get; set; }
        public List<TestRecordClass> FirstTest { get; set; }
        public List<TestRecordClass> SecondTest { get; set; }

        public SubProgramClass(String Name, TestCountEnum TestCount)
        {
            this.Name = Name;
            this.TestCount = TestCount;
        }

        public void Update(String Name, TestCountEnum TestCount)
        {
            this.Name = Name;
            this.TestCount = TestCount;
        }
        public SubProgramClass Clone(SubProgramClass sub)
        {
            return new SubProgramClass(sub.Name, sub.TestCount);
        }
        public void Abandon(String Description = "")
        { }
    }

    public class ProgramClass
    {
        public String Name { get; set; }
        public String Requester { get; set; }
        public DateTime RequestDate { get; set; }
        public String Description { get; set; }
        public DateTime CompleteDate { get; set; }
        public List<SubProgramClass> SubPrograms { get; set; }

        public ProgramClass(String Name, String Requester, DateTime RequestDate, List<SubProgramClass> SubPrograms)
        {
            this.Name = Name;
            this.Requester = Requester;
            this.RequestDate = RequestDate;
            this.SubPrograms = SubPrograms;
        }

        public void Update(String Name, String Requester, DateTime RequestDate, List<SubProgramClass> SubPrograms)
        {
            this.Name = Name;
            this.Requester = Requester;
            this.RequestDate = RequestDate;
            this.SubPrograms = SubPrograms;
        }
    }
}
