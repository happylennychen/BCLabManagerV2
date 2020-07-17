using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Properties;
using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// Editable: no need
    /// Updateable: true
    /// </summary>
    public class TableMakerProductViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields

        readonly TableMakerProduct _tableMakerProduct;

        #endregion // Fields

        #region Constructor

        public TableMakerProductViewModel(TableMakerProduct tableMakerProduct)
        {
            if (tableMakerProduct == null)
                throw new ArgumentNullException("tableMakerProduct");

            _tableMakerProduct = tableMakerProduct;

            _tableMakerProduct.PropertyChanged += _tableMakerProduct_PropertyChanged;
        }

        private void _tableMakerProduct_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        #endregion // Constructor

        #region TableMakerProductClass Properties

        public int Id
        {
            get { return _tableMakerProduct.Id; }
        }
        public string FilePath
        {
            get { return _tableMakerProduct.FilePath; }
        }

        public bool IsValid
        {
            get { return _tableMakerProduct.IsValid; }
        }
        public TableMakerProductType TableMakerProductType
        {
            get { return _tableMakerProduct.Type; }
        }

        public Project Project
        {
            get { return _tableMakerProduct.Project; }
            set
            {
                if (value == _tableMakerProduct.Project)
                    return;

                _tableMakerProduct.Project = value;

                RaisePropertyChanged("Project");
            }
        }
        public TableMakerProductType Type
        {
            get { return _tableMakerProduct.Type; }
            set 
            {
                if (value == _tableMakerProduct.Type)
                    return;

                _tableMakerProduct.Type = value;

                RaisePropertyChanged("Type");
            }
        }
        #endregion
    }
}