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
        public StepV2()
        {
        }
    }
}
