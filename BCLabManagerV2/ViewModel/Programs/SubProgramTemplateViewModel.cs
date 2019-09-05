using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class SubProgramTemplateViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields
        public readonly SubProgramTemplate _subProgramTemplate;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public SubProgramTemplateViewModel(SubProgramTemplate subProgramTemplate)
        {
            _subProgramTemplate = subProgramTemplate;
            _subProgramTemplate.PropertyChanged += _subProgramTemplate_PropertyChanged;
        }

        private void _subProgramTemplate_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region SubProgramTemplate Properties

        public int Id
        {
            get { return _subProgramTemplate.Id; }
            set
            {
                if (value == _subProgramTemplate.Id)
                    return;

                _subProgramTemplate.Id = value;

                base.OnPropertyChanged("Id");
            }
        }
        public string Name
        {
            get { return _subProgramTemplate.Name; }
            set
            {
                if (value == _subProgramTemplate.Name)
                    return;

                _subProgramTemplate.Name = value;

                base.OnPropertyChanged("Name");
            }
        }

        public TestCountEnum TestCount
        {
            get { return _subProgramTemplate.TestCount; }
            set
            {
                if (value == _subProgramTemplate.TestCount)
                    return;

                _subProgramTemplate.TestCount = value;

                base.OnPropertyChanged("TestCount");
            }
        }

        #endregion // Customer Properties

        #region Presentation Properties

        public List<TestCountEnum> TestCountOptions
        {
            get
            {
                return new List<TestCountEnum>()
                {
                    TestCountEnum.One,
                    TestCountEnum.Two
                };
            }
        }

        #endregion // Presentation Properties

    }
}