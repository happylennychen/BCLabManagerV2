using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace O2Micro.BCLabManager.Model
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
    public class TestClass
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
        public Int32 TestID { get; set; }
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
        //public TesterClass Tester { get; set; }
        public TesterChannelClass TesterChannel { get; set; }
        public String Steps { get; set; }
        public ChamberClass Chamber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public String Description { get; set; }
        public RawDataClass RawData { get; set; }
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

        public TestClass(SubProgramClass SubProgram)
        {
            this.TestID = NextID;
            this.SubProgram = SubProgram;
            this.Status = TestStatus.Waiting;
            this.Description = String.Empty;
            this.Battery = null;
            this.TesterChannel = null;
            this.Chamber = null;
            //this.StartTime = null;
            //this.EndTime = DateTime.;
        }

        public void AssignAssets(BatteryClass Battery, ChamberClass Chamber, TesterChannelClass TesterChannel)
        {

            if (this.Status != TestStatus.Waiting)
            {
                //Todo: prompt warning message
                System.Windows.MessageBox.Show("Only waiting task can be assign assets to!");
                return;
            }

            if (Battery.Status != AssetStatusEnum.IDLE)
            {
                //Todo: prompt warning message
                System.Windows.MessageBox.Show("Battery is in use!");
                return;
            }

            if (Chamber != null)
            {
                if (Chamber.Status != AssetStatusEnum.IDLE)
                {
                    //Todo: prompt warning message
                    System.Windows.MessageBox.Show("Chamber is in use!");
                    return;
                }
                if (this.RequestedRecipe.Recipe.ChamberRecipe == null)
                {
                    //Todo: prompt warning message but do not return
                    System.Windows.MessageBox.Show("No chamber recipe but chamber is provided!");
                    //return;
                }
            }
            else if (this.RequestedRecipe.Recipe.ChamberRecipe != null)
            {
                //Todo: prompt warning message
                System.Windows.MessageBox.Show("No chamber to be assigned to chamber recipe!");
                return;
            }

            if (TesterChannel.Status != AssetStatusEnum.IDLE)
            {
                //Todo: prompt warning message
                System.Windows.MessageBox.Show("Test Channel is in use!");
                return;
            }

            if (TesterChannel.Tester != this.RequestedRecipe.Recipe.TesterRecipe.Tester)
            {
                //Todo: prompt warning message
                System.Windows.MessageBox.Show("Recipe and Tester are not compatible!");
                return;
            }

            if (Battery.BatteryType != this.RequestedRecipe.Recipe.TesterRecipe.BatteryType)
            {
                //Todo: prompt warning message
                System.Windows.MessageBox.Show("Recipe and Battery are not compatible!");
                return;
            }

            this.Battery = Battery;
            Battery.Status = AssetStatusEnum.ASSIGNED;
            if (this.RequestedRecipe.Recipe.ChamberRecipe != null) //If there is no chamber recipe, then we don't need to assign chamber at all.
            {
                this.Chamber = Chamber;
                Chamber.Status = AssetStatusEnum.ASSIGNED;
            }
            this.TesterChannel = TesterChannel;
            TesterChannel.Status = AssetStatusEnum.ASSIGNED;
            this.Status = TestStatus.Ready;
        }

        public void Execute(DateTime StartTime)
        {
            if (this.Status == TestStatus.Ready)
            {
                this.StartTime = StartTime;
                Battery.Status = AssetStatusEnum.USING;
                Battery.UpdateRecords(StartTime, Battery.Status);
                if (Chamber != null)
                {
                    Chamber.Status = AssetStatusEnum.USING;
                    Chamber.UpdateRecords(StartTime, Chamber.Status);
                }
                TesterChannel.Status = AssetStatusEnum.USING;
                TesterChannel.UpdateRecords(StartTime, TesterChannel.Status);
                this.Status = TestStatus.Executing;
            }
            else
            {
                System.Windows.MessageBox.Show("Only ready tasks can be executed!");
            }
        }

        public void Commit(TestStatus Status, DateTime EndTime, String Description = "")  //Need to check the Executor Status to make sure it is executing
        {
            if (Status != TestStatus.Completed && Status != TestStatus.Invalid)
            {
                System.Windows.MessageBox.Show("Only Completed and Invalid are available status to be commited");
                return;
            }
            if (this.Status == TestStatus.Executing)
            {
                this.Description = Description;
                this.EndTime = EndTime;
                if (Chamber != null)
                {
                    Chamber.Status = AssetStatusEnum.IDLE;
                    Chamber.UpdateRecords(StartTime, Chamber.Status);
                }
                TesterChannel.Status = AssetStatusEnum.IDLE;
                TesterChannel.UpdateRecords(StartTime, TesterChannel.Status);
                Battery.Status = AssetStatusEnum.IDLE;
                Battery.UpdateRecords(StartTime, Battery.Status);
                this.Status = Status;   //this has to be the last assignment because it will raise StatusChanged event so that duration will be updated using StartTime and EndTime
            }
            else
            {
                System.Windows.MessageBox.Show("Only executing tasks can be commited!");
            }
        }

        public void Abandon()
        {
            if (this.Status == TestStatus.Abandoned)
            {
                System.Windows.MessageBox.Show("Abandoning Abandoned tasks is meaningless!");
                return;
            }
            else if (this.Status == TestStatus.Executing)
            {
                System.Windows.MessageBox.Show("Commit running task before abandon it!");
                return;
            }
            if (this.Battery != null)
                this.Battery.Status = AssetStatusEnum.IDLE;
            if (this.Chamber != null)
                this.Chamber.Status = AssetStatusEnum.IDLE;
            if (this.TesterChannel != null)
                this.TesterChannel.Status = AssetStatusEnum.IDLE;
            this.Status = TestStatus.Abandoned;
        }

        public void Invalidate()
        {
            if (this.Status == TestStatus.Completed || this.Status == TestStatus.Executing)
            {
                this.Battery.Status = AssetStatusEnum.IDLE;
                if (this.Chamber != null)
                    this.Chamber.Status = AssetStatusEnum.IDLE;
                this.TesterChannel.Status = AssetStatusEnum.IDLE;
                this.Status = TestStatus.Invalid;
                this.RequestedRecipe.AddExecutor();
            }
            else
            {
                System.Windows.MessageBox.Show("Only completed or executing tasks can be Invalidated!");
            }
        }
    }
    // Summary:
    //     Represents a sub program which can be united to form a program
    public class SubProgramClass
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
        public Int32 SubProgramID { get; set; }
        public String Name { get; set; }
        public TestCountEnum TestCount { get; set; }
        private TestClass[] test = new TestClass[2];
        public TestClass[] Test 
        {
            get
            { 
                return test; 
            }
            set
            {
                test = value;
            }
        }

        public SubProgramClass(String Name, TestCountEnum TestCount)
        {
            this.SubProgramID = NextID;
            this.Name = Name;
            this.TestCount = TestCount;
        }

        public SubProgramClass(SubProgramClass old)
        {
            this.SubProgramID = NextID;
            this.Name = old.Name;
            this.TestCount = old.TestCount;
        }
    }

    public class ProgramClass
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
        public Int32 ProgramID { get; set; }
        public String Name { get; set; }
        public String Requester { get; set; }
        public DateTime RequestDate { get; set; }
        public String Description { get; set; }
        //public Int32 Priority { get; set; }
        public DateTime CompleteDate { get; set; }
        public List<SubProgramClass> SubPrograms { get; set; }

        public ProgramClass()
        { }

        public ProgramClass(String Name, String Requester, DateTime RequestDate, List<SubProgramClass> SubPrograms)
        {
            this.ProgramID = NextID;
            this.Name = Name;
            this.Requester = Requester;
            this.RequestDate = RequestDate;
            this.SubPrograms = SubPrograms;
        }
    }
}
