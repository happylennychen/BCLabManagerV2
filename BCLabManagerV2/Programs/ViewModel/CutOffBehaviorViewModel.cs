using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;
using Prism.Mvvm;
using System.Windows.Media;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class CutOffBehaviorViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        public readonly CutOffBehavior _cob;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public CutOffBehaviorViewModel(CutOffBehavior cob)
        {
            _cob = cob;
            _cob.PropertyChanged += _cob_PropertyChanged;
            //CreateCondition();
            CreateJPBs();
        }

        private void CreateJPBs()
        {
            List<JumpBehaviorViewModel> all =
                (from sub in _cob.JumpBehaviors
                 select new JumpBehaviorViewModel(sub)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.JumpBehaviors = new ObservableCollection<JumpBehaviorViewModel>(all);     //再转换成Observable
        }

        private void _cob_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region COB Properties

        public int Id
        {
            get { return _cob.Id; }
        }
        public Condition Condition { get { return _cob.Condition; } }
        public ObservableCollection<JumpBehaviorViewModel> JumpBehaviors { get; set; }
        public string Loop1Target
        {
            get { return _cob.Loop1Target; }
        }
        public int Loop1Count
        {
            get { return _cob.Loop1Count; }
        }
        public string Loop2Target
        {
            get { return _cob.Loop2Target; }
        }
        public int Loop2Count
        {
            get { return _cob.Loop2Count; }
        }
        #endregion // Customer Properties
    }
}