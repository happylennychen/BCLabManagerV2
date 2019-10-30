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
    public class TesterViewModel : ViewModelBase//, IDataErrorInfo
    {
        #region Fields

        readonly TesterClass _tester;
        readonly TesterRepository _testerRepository;
        //bool _isSelected;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public TesterViewModel(TesterClass tester, TesterRepository testerRepository)  //构造函数里面之所以要testerrepository,是因为IsNewBattery需要用此进行判断
        {
            if (tester == null)
                throw new ArgumentNullException("tester");

            if (testerRepository == null)
                throw new ArgumentNullException("testerRepository");

            _tester = tester;
            _testerRepository = testerRepository;
            _isOK = false;
        }

        #endregion // Constructor

        #region TesterClass Properties

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
                int number = (
                    from bat in _testerRepository.GetItems()
                    where bat.Name == _tester.Name
                    select bat).Count();
                if (number != 0)
                    return false;
                return !_testerRepository.ContainsItem(_tester);
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
