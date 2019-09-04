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
    public class SubProgramViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields
        public readonly SubProgramClass _subprogram;            //为了将其添加到Program里面去(见ProgramViewModel Add)，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public SubProgramViewModel(SubProgramClass subprogram)
        {
            _subprogram = subprogram;
            this.CreateTestRecords();
            _subprogram.TestRecordAdded += _subprogram_TestRecordAdded;
        }

        private void _subprogram_TestRecordAdded(object sender, TestRecordAddedEventArgs e)
        {
            if (e.IsFirst)
            {
                Test1Records.Add(new TestRecordViewModel(e.NewTestRecord));
            }
            else
            {
                Test2Records.Add(new TestRecordViewModel(e.NewTestRecord));
            }
        }

        void CreateTestRecords()
        {
            List<TestRecordViewModel> all1 =
                (from ft in _subprogram.FirstTestRecords
                 select new TestRecordViewModel(ft)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.Test1Records = new ObservableCollection<TestRecordViewModel>(all1);     //再转换成Observable


            List<TestRecordViewModel> all2 =
                (from st in _subprogram.SecondTestRecords
                 select new TestRecordViewModel(st)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.Test2Records = new ObservableCollection<TestRecordViewModel>(all2);     //再转换成Observable
        }

        #endregion // Constructor

        #region SubProgramClass Properties

        public int Id
        {
            get { return _subprogram.Id; }
            set
            {
                if (value == _subprogram.Id)
                    return;

                _subprogram.Id = value;

                base.OnPropertyChanged("Id");
            }
        }
        public string Name
        {
            get { return _subprogram.Name; }
            set
            {
                if (value == _subprogram.Name)
                    return;

                _subprogram.Name = value;

                base.OnPropertyChanged("Name");
            }
        }

        public TestCountEnum TestCount
        {
            get { return _subprogram.TestCount; }
            set
            {
                if (value == _subprogram.TestCount)
                    return;

                _subprogram.TestCount = value;

                base.OnPropertyChanged("TestCount");
            }
        }

        public ObservableCollection<TestRecordViewModel> Test1Records { get; private set; }        //这个是当前sub program所拥有的test1

        public ObservableCollection<TestRecordViewModel> Test2Records { get; private set; }        //这个是当前sub program所拥有的test2

        #endregion // Customer Properties
    }
}