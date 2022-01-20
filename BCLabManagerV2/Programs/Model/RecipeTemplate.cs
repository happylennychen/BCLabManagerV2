using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class RecipeTemplate : BindableBase
    {
        public int Id { get; set; }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        private RecipeTemplateGroup _group;
        public RecipeTemplateGroup Group
        {
            get { return _group; }
            set { SetProperty(ref _group, value); }
        }

        private bool _editable = true;
        public bool Editable
        {
            get { return _editable; }
            set { SetProperty(ref _editable, value); }
        }

        public ObservableCollection<Step> Steps { get; set; } = new ObservableCollection<Step>();
        public ObservableCollection<StepV2> StepV2s { get; set; } = new ObservableCollection<StepV2>();
        public ObservableCollection<Protection> Protections { get; set; } = new ObservableCollection<Protection>();

        public RecipeTemplate()
        {
        }

        internal ObservableCollection<StepV2> GetNormalSteps(Project project)  //将StepV2s中的特殊项变成一般项
        {
            if (StepV2s.Any(step => step.Action.Mode == ActionMode.EX_CCR_DISCHARGE_EOD_LAST || step.Action.Mode == ActionMode.EX_CCR_DISCHARGE_EOD_NEXT || step.Action.Mode == ActionMode.EX_CPR_DISCHARGE_EOD_LAST || step.Action.Mode == ActionMode.EX_CPR_DISCHARGE_EOD_NEXT || step.Action.Mode == ActionMode.EX_STANDARD_CHARGE))
            {
                StepV2 newStep;
                TesterAction action;
                CutOffBehavior cob;
                var capacity = project.BatteryType.RatedCapacity;
                ObservableCollection<StepV2> output = new ObservableCollection<StepV2>();
                foreach (var step in StepV2s)
                {
                    switch(step.Action.Mode)
                    {
                        case ActionMode.EX_CCR_DISCHARGE_EOD_LAST:
                            newStep = new StepV2();
                            newStep.Index = step.Index;
                            action = new TesterAction();
                            action.Mode = ActionMode.CC_DISCHARGE;
                            action.Current = step.Action.Current * capacity / 100;
                            newStep.Action = action;
                            cob = new CutOffBehavior();
                            cob.Condition = new Condition() { Parameter = Parameter.VOLTAGE, Mark = CompareMarkEnum.SmallerThan, Value = project.BatteryType.CutoffDischargeVoltage };
                            cob.JumpBehaviors.Add(new JumpBehavior() { JumpType = JumpType.INDEX, Index = StepV2s.Count });
                            newStep.CutOffBehaviors.Add(cob);
                            foreach (var ocob in step.CutOffBehaviors)
                            {
                                newStep.CutOffBehaviors.Add(ocob);
                            }
                            output.Add(newStep);
                            break;
                        case ActionMode.EX_CCR_DISCHARGE_EOD_NEXT:
                            newStep = new StepV2();
                            newStep.Index = step.Index;
                            action = new TesterAction();
                            action.Mode = ActionMode.CC_DISCHARGE;
                            action.Current = step.Action.Current * capacity / 100;
                            newStep.Action = action;
                            cob = new CutOffBehavior();
                            cob.Condition = new Condition() { Parameter = Parameter.VOLTAGE, Mark = CompareMarkEnum.SmallerThan, Value = project.BatteryType.CutoffDischargeVoltage };
                            cob.JumpBehaviors.Add(new JumpBehavior() { JumpType = JumpType.NEXT });
                            newStep.CutOffBehaviors.Add(cob);
                            foreach (var ocob in step.CutOffBehaviors)
                            {
                                newStep.CutOffBehaviors.Add(ocob);
                            }
                            output.Add(newStep);
                            break; ;
                        case ActionMode.EX_CPR_DISCHARGE_EOD_LAST:
                            newStep = new StepV2();
                            newStep.Index = step.Index;
                            action = new TesterAction();
                            action.Mode = ActionMode.CP_DISCHARGE;
                            action.Power = step.Action.Power * capacity;
                            newStep.Action = action;
                            cob = new CutOffBehavior();
                            cob.Condition = new Condition() { Parameter = Parameter.VOLTAGE, Mark = CompareMarkEnum.SmallerThan, Value = project.BatteryType.CutoffDischargeVoltage };
                            cob.JumpBehaviors.Add(new JumpBehavior() { JumpType = JumpType.INDEX, Index = StepV2s.Count });
                            newStep.CutOffBehaviors.Add(cob);
                            foreach (var ocob in step.CutOffBehaviors)
                            {
                                newStep.CutOffBehaviors.Add(ocob);
                            }
                            output.Add(newStep);
                            break; ;
                        case ActionMode.EX_CPR_DISCHARGE_EOD_NEXT:
                            newStep = new StepV2();
                            newStep.Index = step.Index;
                            action = new TesterAction();
                            action.Mode = ActionMode.CP_DISCHARGE;
                            action.Power = step.Action.Power * capacity;
                            newStep.Action = action;
                            cob = new CutOffBehavior();
                            cob.Condition = new Condition() { Parameter = Parameter.VOLTAGE, Mark = CompareMarkEnum.SmallerThan, Value = project.BatteryType.CutoffDischargeVoltage };
                            cob.JumpBehaviors.Add(new JumpBehavior() { JumpType = JumpType.NEXT });
                            newStep.CutOffBehaviors.Add(cob);
                            foreach (var ocob in step.CutOffBehaviors)
                            {
                                newStep.CutOffBehaviors.Add(ocob);
                            }
                            output.Add(newStep);
                            break;
                        case ActionMode.EX_STANDARD_CHARGE:
                            newStep = new StepV2();
                            newStep.Index = step.Index;
                            action = new TesterAction();
                            action.Mode = ActionMode.CC_CV_CHARGE;
                            action.Voltage = project.BatteryType.LimitedChargeVoltage;
                            action.Current = project.BatteryType.ChargeCurrent;
                            newStep.Action = action;
                            cob = new CutOffBehavior();
                            cob.Condition = new Condition() { Parameter = Parameter.CURRENT, Mark = CompareMarkEnum.SmallerThan, Value = project.BatteryType.FullyChargedEndCurrent };
                            cob.JumpBehaviors.Add(new JumpBehavior() { JumpType = JumpType.NEXT });
                            newStep.CutOffBehaviors.Add(cob);
                            foreach (var ocob in step.CutOffBehaviors)
                            {
                                newStep.CutOffBehaviors.Add(ocob);
                            }
                            output.Add(newStep);
                            break; ;
                        default:
                            output.Add(step);
                            break;
                    }
                }
                return output;
            }
            else
                return StepV2s;
        }
        //internal StepV2 GetStepByActionMode(ActionMode actionMode)
        //{
        //    return StepV2s.SingleOrDefault(o => o.Action.Mode == actionMode);
        //}
        //internal StepV2 GetStepByIndex(int index)
        //{
        //    return StepV2s.SingleOrDefault(o => o.Index == index);
        //}
    }
}
