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
    public class CutOffConditionViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        public readonly CutOffCondition _coc;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public CutOffConditionViewModel(CutOffCondition coc)
        {
            _coc = coc;
            _coc.PropertyChanged += _coc_PropertyChanged;
        }

        private void _coc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region COC Properties

        public int Id
        {
            get { return _coc.Id; }
            set
            {
                if (value == _coc.Id)
                    return;

                _coc.Id = value;

                RaisePropertyChanged("Id");
            }
        }
        public Parameter Parameter
        {
            get { return _coc.Parameter; }
            set
            {
                if (value == _coc.Parameter)
                    return;

                _coc.Parameter = value;

                RaisePropertyChanged("Parameter");
            }
        }
        #endregion // Customer Properties
    }
}