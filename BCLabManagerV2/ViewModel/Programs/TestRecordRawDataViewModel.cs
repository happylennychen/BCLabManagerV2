﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TestRecordRawDataViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields
        readonly TestRecordClass _record;

        #endregion // Fields

        #region Constructor

        public TestRecordRawDataViewModel(
            TestRecordClass record
            )     //
        {
            if (record == null)
                throw new ArgumentNullException("record");

            _record = record;
        }

        #endregion // Constructor

        #region Presentation Properties

        public String FilePath
        {
            get
            {
                return _record.FilePath;
            }
            set
            {
                _record.FilePath = value;
            }
        }

        #endregion // Presentation Properties
    }
}