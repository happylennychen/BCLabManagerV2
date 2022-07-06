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
    public class TestRecordTestDataViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields
        readonly TestRecord _record;

        #endregion // Fields

        #region Constructor

        public TestRecordTestDataViewModel(
            TestRecord record
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
                return _record.StdFilePath;
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
