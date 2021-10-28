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

        internal ObservableCollection<StepV2> GetNormalSteps()  //将StepV2s中的特殊项变成一般项
        {
            if (StepV2s.Any(step => step.Action.Mode == ActionMode.CCR_DISCHARGE || step.Action.Mode == ActionMode.CCR_DISCHARGE_EOD || step.Action.Mode == ActionMode.CPR_DISCHARGE || step.Action.Mode == ActionMode.CPR_DISCHARGE_EOD || step.Action.Mode == ActionMode.STANDARD_CHARGE))
            {
                ObservableCollection<StepV2> output = new ObservableCollection<StepV2>();
                foreach (var step in StepV2s)
                {
                    switch(step.Action.Mode)
                    {
                        case ActionMode.CCR_DISCHARGE:
                            StepV2 newStep = new StepV2();
                            newStep.Index = step.Index;
                            var action = new TesterAction();
                            action.Mode = ActionMode.CC_DISCHARGE;
                            //action.Current =
                            //newStep.Action = new TesterAction()
                            break;
                        case ActionMode.CCR_DISCHARGE_EOD:break;
                        case ActionMode.CPR_DISCHARGE:break;
                        case ActionMode.CPR_DISCHARGE_EOD:break;
                        case ActionMode.STANDARD_CHARGE:break;
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
