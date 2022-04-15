using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TestViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields
        private TestRecord _record;

        public TestRecord Record
        {
            get { return _record; }
            set { _record = value; }
        }


        #endregion // Fields

        #region Constructor

        public TestViewModel(
            TestRecord record)     //
        {
            _record = record;
        }

        #endregion // Constructor

        #region Presentation Properties


        public double Temperature
        {
            get
            {
                return _record.Temperature;
            }
        }

        public double Current
        {
            get
            {
                return _record.Current;
            }
        }


        public string RecipeStr
        {
            get
            {
                return _record.RecipeStr;
            }
        }

        public double DischargeCapacity
        {
            get
            {
                return _record.DischargeCapacity;
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
        #endregion // Presentation Properties
    }
}
