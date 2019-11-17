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
    public class StepRuntimeViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        private StepRuntimeClass _stepRuntime;

        public StepRuntimeClass StepRuntime
        {
            get { return _stepRuntime; }
            set { _stepRuntime = value; }
        }


        #endregion // Fields

        #region Constructor

        public StepRuntimeViewModel(
            StepRuntimeClass stepRuntime)     //
        {
            _stepRuntime = stepRuntime;
            _stepRuntime.PropertyChanged += _stepRuntime_PropertyChanged;
            //_stepRuntime.StatusChanged += _record_StatusChanged;
        }

        private void _stepRuntime_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region Presentation Properties

        public int Id
        {
            get { return _stepRuntime.Id; }
            set
            {
                if (value == _stepRuntime.Id)
                    return;

                _stepRuntime.Id = value;

                RaisePropertyChanged("Id");
            }
        }

        public DateTime StartTime
        {
            get
            {
                if (_stepRuntime.StartTime == DateTime.MinValue)
                    return _stepRuntime.EST;
                else
                    return _stepRuntime.StartTime;
            }
        }

        public DateTime EndTime
        {
            get
            {
                if (_stepRuntime.EndTime == DateTime.MinValue)
                    return _stepRuntime.EET;
                else
                    return _stepRuntime.EndTime;
            }
        }

        public string Step
        {
            get
            {
                return _stepRuntime.Step.StepTemplate.ToString();
            }
        }
        #endregion // Presentation Properties

        #region Presentation logic
        #endregion
    }
}
