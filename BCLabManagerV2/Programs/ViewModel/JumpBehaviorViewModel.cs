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
    public class JumpBehaviorViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        public readonly JumpBehavior _jpb;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public JumpBehaviorViewModel(JumpBehavior jpb)
        {
            _jpb = jpb;
            _jpb.PropertyChanged += _jpb_PropertyChanged;
        }

        private void _jpb_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region JPB Properties

        public int Id
        {
            get { return _jpb.Id; }
        }
        public Condition Condition { get { return _jpb.Condition; } }
        public JumpType JumpType
        {
            get { return _jpb.JumpType; }
        }
        public int Index
        {
            get { return _jpb.Index; }
        }
        #endregion // Customer Properties
    }
}