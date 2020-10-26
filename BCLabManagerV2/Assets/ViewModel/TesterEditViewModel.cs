using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.Model;
using BCLabManager.View;
using BCLabManager.DataAccess;
using System.Windows.Input;
using BCLabManager.Properties;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// Editable: true
    /// Updateable: no need
    /// </summary>
    public class TesterEditViewModel : BindableBaseWithName//, IDataErrorInfo
    {
        #region Fields

        readonly Tester _tester;
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public TesterEditViewModel(Tester tester)  //构造函数里面之所以要testerrepository,是因为IsNewBattery需要用此进行判断
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

                RaisePropertyChanged("Id");
            }
        }
        public string Manufacturer
        {
            get { return _tester.Manufacturer; }
            set
            {
                if (value == _tester.Manufacturer)
                    return;

                _tester.Manufacturer = value;

                RaisePropertyChanged("Manufacturer");
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

                RaisePropertyChanged("Name");
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
