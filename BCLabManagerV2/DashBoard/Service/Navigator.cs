﻿using BCLabManager.Model;
using BCLabManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BCLabManager.View
{
    public static class Navigator
    {
        private static MainWindow _mainWindow;
        public static void Initialize(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }
        public static void SetMainTabByIndex(int i)   //0:Dashboard 1:Assets 2:Recipe Template 3:Programs
        {
            _mainWindow.MainTab.SelectedIndex = i;
        }
        public static void SetMainTabByHeader(string header)   //0:Dashboard 1:Assets 2:Recipe Template 3:Programs
        {
            //_mainWindow.MainTab.SelectedIndex = i;
            foreach (var item in _mainWindow.MainTab.Items)
            {
                var it = item as TabItem;
                if (it.Header.ToString() == header)
                {
                    _mainWindow.MainTab.SelectedItem = it;
                }
            }
        }
        public static void SetSelectedTestRecord(int id)
        {
            MainWindowViewModel mwv = _mainWindow.DataContext as MainWindowViewModel;
            foreach (var pro in mwv.allProgramsViewModel.AllPrograms)
                foreach (var sub in pro.Recipes)
                {
                    foreach(var tr in sub.TestRecords)
                    {
                        if (tr.Id == id)
                        {
                            _mainWindow.AllProgramsViewInstance.Programlist.SelectedItem = pro;
                            _mainWindow.AllProgramsViewInstance.Recipelist.SelectedItem = sub;
                            _mainWindow.AllProgramsViewInstance.TestRecordList.SelectedItem = tr;
                            return;
                        }
                    }
                }
        }
    }
}
