using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class TestRecordInvalidateViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        RelayCommand _okCommand;
        bool _isOK;

        #endregion // Fields

        #region Constructor

        public TestRecordInvalidateViewModel()     //
        {
        }

        #endregion // Constructor

        #region Presentation Properties
        public String Comment
        {
            get;set;
        }

        #endregion // Presentation Properties


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
                        param => { this.OK(); }//,
                                               //param => this.CanExecute
                        );
                }
                return _okCommand;
            }
        }
        /// <summary>
        /// Saves the customer to the repository.  This method is invoked by the SaveCommand.
        /// </summary>
        public void OK()
        {
            IsOK = true;
        }

        public bool IsOK
        {
            get { return _isOK; }
            set { _isOK = value; }
        }
    }
}
