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
