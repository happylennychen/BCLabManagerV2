﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.Model;
using BCLabManager.View;
using BCLabManager.DataAccess;
using System.Windows.Input;
using BCLabManager.Properties;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// Editable: no need
    /// Updateable: true
    /// </summary>
    public class TesterViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        readonly Tester _tester;

        #endregion // Fields

        #region Constructor

        public TesterViewModel(Tester tester)  //构造函数里面之所以要testerrepository,是因为IsNewBattery需要用此进行判断
        {
            _tester = tester;
            _tester.PropertyChanged += _tester_PropertyChanged;
        }

        private void _tester_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region TesterClass Properties
        public int Id
        {
            get { return _tester.Id; }
        }
        public string Manufacturer
        {
            get { return _tester.Manufacturer; }
        }

        public string Name
        {
            get { return _tester.Name; }
        }

        #endregion // Customer Properties
    }
}
