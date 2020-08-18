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
    public class ProtectionViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        public readonly Protection _prot;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public ProtectionViewModel(Protection prot)
        {
            _prot = prot;
            _prot.PropertyChanged += _prot_PropertyChanged;
        }

        private void _prot_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region Protection Properties

        public int Id
        {
            get { return _prot.Id; }
            //set
            //{
            //    if (value == _prot.Id)
            //        return;

            //    _prot.Id = value;

            //    RaisePropertyChanged("Id");
            //}
        }
        public Parameter Parameter
        {
            get { return _prot.Parameter; }
            //set
            //{
            //    if (value == _prot.Parameter)
            //        return;

            //    _prot.Parameter = value;

            //    RaisePropertyChanged("Parameter");
            //}
        }
        public CompareMarkEnum Mark
        {
            get { return _prot.Mark; }
        }
        public int Value
        {
            get { return _prot.Value; }
        }
        #endregion // Customer Properties
    }
}