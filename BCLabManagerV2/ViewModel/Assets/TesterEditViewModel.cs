using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.Model;
using BCLabManager.View;
using BCLabManager.DataAccess;
using System.Windows.Input;
using BCLabManager.Properties;

namespace BCLabManager.ViewModel
{
    public class TesterEditViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        readonly TesterClass _tester;
        //bool _isSelected;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public TesterEditViewModel(TesterClass tester)  //构造函数里面之所以要testerrepository,是因为IsNewBattery需要用此进行判断
        {
            if (tester == null)
                throw new ArgumentNullException("tester");

            _tester = tester;
            _isOK = false;
        }

        #endregion // Constructor

        #region TesterClass Properties
        public int Id
        {
            get { return _tester.Id; }
            set
            {
                if (value == _tester.Id)
                    return;

                _tester.Id = value;

                base.OnPropertyChanged("Id");
            }
        }
        public string Manufactor
        {
            get { return _tester.Manufactor; }
            set
            {
                if (value == _tester.Manufactor)
                    return;

                _tester.Manufactor = value;

                base.OnPropertyChanged("Manufactor");
            }
        }

        public string Name
        {
            get { return _tester.Name; }
            set
            {
                if (value == _tester.Name)
                    return;

                _tester.Name = value;

                base.OnPropertyChanged("Name");
            }
        }

        #endregion // Customer Properties

        #region Presentation Properties

        /// <summary>
        /// Returns a command that saves the customer.
        /// </summary>
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new RelayCommand(
                        param => { this.OK(); },
                        param => this.CanOK
                        );
                }
                return _okCommand;
            }
        }

        public bool IsOK
        {
            get { return _isOK; }
            set { _isOK = value; }
        }

        #endregion // Presentation Properties

        #region Public Methods

        public void OK()        //就是将属性IsOK设置成true，从而在外层进行下一步动作
        {
            IsOK = true;
        }

        #endregion // Public Methods

        #region Private Helpers

        /// <summary>
        /// Returns true if this customer was created by the user and it has not yet
        /// been saved to the customer repository.
        /// </summary>
        bool IsNewTester
        {
            get
            {
                //int number = (
                //    from bat in _testerRepository.GetItems()
                //    where bat.Name == _tester.Name
                //    select bat).Count();
                //if (number != 0)
                //    return false;
                //return !_testerRepository.ContainsItem(_tester);
                return true;
            }
        }

        /// <summary>
        /// Returns true if the customer is valid and can be saved.
        /// </summary>
        bool CanOK
        {
            get { return IsNewTester; }
        }

        #endregion // Private Helpers
    }
}
