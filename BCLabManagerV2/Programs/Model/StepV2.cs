using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.DataAccess;
using Prism.Mvvm;

namespace BCLabManager.Model
{
    public class StepV2 : BindableBase
    {
        public int Id { get; set; }
        private int _index;
        public int Index
        {
            get { return _index; }
            set { SetProperty(ref _index, value); }
        }
        private int _rest;
        public int Rest
        {
            get { return _rest; }
            set { SetProperty(ref _rest, value); }
        }
        private int _preRest;
        public int Prerest
        {
            get { return _preRest; }
            set { SetProperty(ref _preRest, value); }
        }
        private string _loop1Label;
        public string Loop1Label
        {
            get { return _loop1Label; }
            set { SetProperty(ref _loop1Label, value); }
        }
        private string _loop2Label;
        public string Loop2Label
        {
            get { return _loop2Label; }
            set { SetProperty(ref _loop2Label, value); }
        }
        private TesterAction _action = new TesterAction();
        public TesterAction Action
        {
            get { return _action; }
            set { SetProperty(ref _action, value); }
        }
        private ObservableCollection<CutOffCondition> _cutOffConditions = new ObservableCollection<CutOffCondition>();
        public ObservableCollection<CutOffCondition> CutOffConditions
        {
            get { return _cutOffConditions; }
            set { SetProperty(ref _cutOffConditions, value); }
        }
        private ObservableCollection<CutOffBehavior> _cutOffBehaviors = new ObservableCollection<CutOffBehavior>();
        public ObservableCollection<CutOffBehavior> CutOffBehaviors
        {
            get { return _cutOffBehaviors; }
            set { SetProperty(ref _cutOffBehaviors, value); }
        }
        public ObservableCollection<Protection> Protections { get; set; } = new ObservableCollection<Protection>();
        public StepV2()
        {
        }

        internal StepV2 Clone()
        {
            StepV2 output = new StepV2();
            output.Action = this.Action.Clone();
            //foreach (var coc in this.CutOffConditions)
            //{
            //    CutOffCondition newcoc = coc.Clone();
            //    output.CutOffConditions.Add(newcoc);
            //}
            foreach (var cob in this.CutOffBehaviors)
            {
                CutOffBehavior newcob = cob.Clone();
                output.CutOffBehaviors.Add(newcob);
            }
            foreach (var protection in this.Protections)
            {
                var newprotection = protection.Clone();
                output.Protections.Add(newprotection);
            }
            output.Index = this.Index;
            output.Loop1Label = this.Loop1Label;
            output.Loop2Label = this.Loop2Label;
            return output;
        }
    }
}
