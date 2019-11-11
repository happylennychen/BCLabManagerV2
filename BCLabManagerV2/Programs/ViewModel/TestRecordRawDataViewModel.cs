﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TestRecordRawDataViewModel : BindableBase//, IDataErrorInfo
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

        public List<String> FileList
        {
            get
            {
                return _record.RawDataList.Select(o=>o.FileName).ToList();
            }
        }

        public string Steps
        {
            get
            {
                return _record.Steps;
            }
        }

        public string Comment
        {
            get
            {
                return _record.Comment;
            }
        }

        #endregion // Presentation Properties
    }
}
