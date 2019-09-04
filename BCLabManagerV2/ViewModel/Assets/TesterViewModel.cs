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

        #endregion // Fields

        #region Constructor

        public TesterViewModel(TesterClass tester)  //构造函数里面之所以要testerrepository,是因为IsNewBattery需要用此进行判断
        {
            _tester = tester;
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
    }
}
