using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Win32;
using Prism.Mvvm;
using System.IO;
using System.Windows;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// A UI-friendly wrapper for a Customer object.
    /// </summary>
    public class SplitterViewModel : BindableBase//, IDataErrorInfo
    {
        #region Fields
        readonly TestRecord _record;
        RelayCommand _okCommand;
        RelayCommand _openFilesCommand;

        #endregion // Fields

        #region Constructor

        public SplitterViewModel()
        {
        }

        #endregion // Constructor

        #region Presentation Properties

        private ObservableCollection<string> _fileList;

        public ObservableCollection<string> FileList
        {
            get { return _fileList; }
            set
            {
                if (value == _fileList)
                    return;
                _fileList = value;

                RaisePropertyChanged("FileList");
            }
        }

        private ObservableCollection<Spliter> _splitterList = new ObservableCollection<Spliter>();

        public ObservableCollection<Spliter> SplitterList
        {
            get { return _splitterList; }
            set
            {
                if (value == _splitterList)
                    return;
                _splitterList = value;

                RaisePropertyChanged("SplitterList");
            }
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
                        param => { this.OK(); }
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
            foreach (var fp in FileList)
            {
                List<String> subfilepaths = Splite(fp, SplitterList.Select(o=>o.Str).ToList());
                foreach (var sfp in subfilepaths)
                    UpdateTime(sfp);
            }
        }

        public ICommand OpenFilesCommand
        {
            get
            {
                if (_openFilesCommand == null)
                {
                    _openFilesCommand = new RelayCommand(
                        param => { this.OpenFiles(); }//,
                                                      //param => this.CanExecute
                        );
                }
                return _openFilesCommand;
            }
        }
        public void OpenFiles()
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == true)
            {
                FileList = new ObservableCollection<string>(dialog.FileNames.ToList());
            }
        }

        private void UpdateTime(string fp)
        {
            //FileStream fs = new FileStream(fp, FileMode.Open);
            //StreamReader sr = new StreamReader(fs);
            //StreamWriter sw = new StreamWriter(fs);
            var lines = File.ReadAllLines(fp);
            string starttime = lines[10].Split(',')[3];
            string endtime = lines[lines.Length - 1].Split(',')[3];
            lines[2] = lines[2].Remove(17, 19).Insert(17, starttime);
            lines[3] = lines[3].Remove(15, 19).Insert(15, endtime);
            File.WriteAllLines(fp, lines);
        }

        private List<string> Splite(string filepath, List<string> spliterStringList)
        {
            string newspliter = string.Empty, oldspliter = string.Empty;
            List<string> subfilepaths = new List<string>();
            for (int index = 0; index <= spliterStringList.Count; index++)
            {
                string spliter = string.Empty;
                if (index != spliterStringList.Count)
                {
                    spliter = spliterStringList[index];
                }
                oldspliter = newspliter;
                newspliter = spliter;
                subfilepaths.Add(CreateNewFile(filepath, newspliter, oldspliter, index + 1));
            }
            return subfilepaths;
        }

        private string CreateNewFile(string filepath, string newspliter, string oldspliter, int index)
        {
            List<string> lines;
            try
            {
                lines = File.ReadAllLines(filepath).ToList();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
            string targetline;
            int lineIndex;
            if (newspliter != string.Empty)
            {
                targetline = lines.First(o => o.Contains(newspliter));
                lineIndex = lines.IndexOf(targetline);
                lines.RemoveRange(lineIndex, lines.Count - lineIndex);
            }
            if (oldspliter != string.Empty)
            {
                targetline = lines.First(o => o.Contains(oldspliter));
                lineIndex = lines.IndexOf(targetline);
                lines.RemoveRange(10, lineIndex - 10);
            }
            var newfilepath = GetNewFilePath(filepath, index);
            File.WriteAllLines(newfilepath, lines);
            return newfilepath;
        }

        private string GetNewFilePath(string filepath, int index)
        {
            string oldfilename = Path.GetFileNameWithoutExtension(filepath);
            string newfilename = oldfilename + "-" + index.ToString();
            //return filepath.Replace(oldfilename, newfilename);
            string newfilepath = Path.Combine(Path.GetDirectoryName(filepath), newfilename) + Path.GetExtension(filepath);
            return newfilepath;
        }
    }

    public class Spliter:BindableBase
    {
        private string _str;
        public string Str
        {
            get { return _str; }
            set { SetProperty(ref _str, value); }
        }
    }
}
