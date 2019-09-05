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
        readonly ProgramRepository _programRepository;
        readonly SubProgramRepository _subprogramRepository;
        SubProgramTemplateViewModel _leftselectedSubProgram;
        SubProgramViewModel _rightselectedSubProgram;
        RelayCommand _okCommand;
        RelayCommand _addCommand;
        RelayCommand _removeCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public ProgramViewModel(ProgramClass programmodel)
        {

            _program = programmodel;
            this.CreateSubPrograms();
            _program.PropertyChanged += _program_PropertyChanged;
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
    }
}