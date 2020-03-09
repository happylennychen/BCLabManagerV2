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
    public class StepViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        public readonly StepClass _step;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public StepViewModel(StepClass step)
        {
            _step = step;
            _step.PropertyChanged += _Step_PropertyChanged;
        }

        private void _Step_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region RecipeClass Properties

        public int Id
        {
            get { return _step.Id; }
            set
            {
                if (value == _step.Id)
                    return;

                _step.Id = value;

                RaisePropertyChanged("Id");
            }
        }
        public string Name
        {
            get
            {
                return _step.StepTemplate.ToString();
            }
        }

        public string LoopLabel
        {
            get
            {
                return _step.LoopLabel;
            }
            set
            {
                if (value == _step.LoopLabel)
                    return;
                _step.LoopLabel = value;

                RaisePropertyChanged();
            }
        }

        public string LoopTarget
        {
            get
            {
                return _step.LoopTarget;
            }
            set
            {
                if (value == _step.LoopTarget)
                    return;
                _step.LoopTarget = value;

                RaisePropertyChanged();
            }
        }

        public ushort LoopCount
        {
            get
            {
                return _step.LoopCount;
            }
            set
            {
                if (value == _step.LoopCount)
                    return;
                _step.LoopCount = value;

                RaisePropertyChanged();
            }
        }

        public CompareMarkEnum CompareMark
        {
            get
            {
                return _step.CompareMark;
            }
            set
            {
                if (value == _step.CompareMark)
                    return;
                _step.CompareMark = value;

                RaisePropertyChanged();
            }
        }

        public double Capacity
        {
            get
            {
                return _step.CRate;
            }
            set
            {
                if (value == _step.CRate)
                    return;
                _step.CRate = value;

                RaisePropertyChanged();
            }
        }


        #endregion // Customer Properties
    }
}