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
    /// A UI-friendly wrapper for a Program object.
    /// </summary>
    public class ProgramViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        public ProgramClass _program;            //为了AllProgramsViewModel中的Edit，不得不开放给viewmodel。以后再想想有没有别的办法。

        #endregion // Fields

        #region Constructor

        public ProgramViewModel(ProgramClass program)
        {

            _program = program;
            this.CreateSubPrograms();
            _program.PropertyChanged += _program_PropertyChanged;
            var trlist = GetAllTestRecords(program);
            foreach(var tr in trlist)
                tr.StatusChanged += Tr_StatusChanged;
        }

        private void Tr_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            OnPropertyChanged("WaitingPercentage");
            OnPropertyChanged("ExecutingPercentage");
            OnPropertyChanged("CompletedPercentage");
            OnPropertyChanged("InvalidPercentage");
            OnPropertyChanged("AbandonedPercentage");
        }

        private void _program_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        void CreateSubPrograms()
        {
            List<SubProgramViewModel> all =
                (from sub in _program.SubPrograms
                 select new SubProgramViewModel(sub)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            //foreach (SubProgramModelViewModel batmod in all)
            //batmod.PropertyChanged += this.OnSubProgramModelViewModelPropertyChanged;

            this.SubPrograms = new ObservableCollection<SubProgramViewModel>(all);     //再转换成Observable
            //this.AllCustomers.CollectionChanged += this.OnCollectionChanged;
        }
        #endregion // Constructor

        #region ProgramClass Properties

        public int Id
        {
            get { return _program.Id; }
            set
            {
                if (value == _program.Id)
                    return;

                _program.Id = value;

                base.OnPropertyChanged("Id");
            }
        }

        public string Name
        {
            get { return _program.Name; }
            set
            {
                if (value == _program.Name)
                    return;

                _program.Name = value;

                base.OnPropertyChanged("Name");
            }
        }

        public BatteryTypeClass BatteryType
        {
            get { return _program.BatteryType; }
            set
            {
                if (value == _program.BatteryType)
                    return;

                _program.BatteryType = value;

                base.OnPropertyChanged("BatteryType");
            }
        }

        public string Requester
        {
            get { return _program.Requester; }
            set
            {
                if (value == _program.Requester)
                    return;

                _program.Requester = value;

                base.OnPropertyChanged("Requester");
            }
        }

        public string Description
        {
            get { return _program.Description; }
            set
            {
                if (value == _program.Description)
                    return;

                _program.Description = value;

                base.OnPropertyChanged("Description");
            }
        }

        public DateTime RequestDate
        {
            get { return _program.RequestDate; }
            set
            {
                if (value == _program.RequestDate)
                    return;

                _program.RequestDate = value;

                base.OnPropertyChanged("RequestDate");
            }
        }

        public ObservableCollection<SubProgramViewModel> SubPrograms { get; set; }        //这个是当前program所拥有的subprograms
        #endregion // Customer Properties
        #region Presentation logic
        public string WaitingPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_program);
                return ((double)alltr.Count(o => o.Status == TestStatus.Waiting) / (double)alltr.Count).ToString() + "*";
            }
        }

        public string ExecutingPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_program);
                return ((double)alltr.Count(o => o.Status == TestStatus.Executing) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string CompletedPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_program);
                return ((double)alltr.Count(o => o.Status == TestStatus.Completed) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string InvalidPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_program);
                return ((double)alltr.Count(o => o.Status == TestStatus.Invalid) / (double)alltr.Count).ToString() + "*";
            }
        }
        public string AbandonedPercentage
        {
            get
            {
                List<TestRecordClass> alltr = GetAllTestRecords(_program);
                return ((double)alltr.Count(o => o.Status == TestStatus.Abandoned) / (double)alltr.Count).ToString() + "*";
            }
        }
        #endregion

        private List<TestRecordClass> GetAllTestRecords(ProgramClass program)
        {
            List<TestRecordClass> output = new List<TestRecordClass>();
            foreach (var sub in program.SubPrograms)
            {
                foreach (var tr in sub.FirstTestRecords)
                    output.Add(tr);
                foreach (var tr in sub.SecondTestRecords)
                    output.Add(tr);
            }
            return output;
        }
    }
}