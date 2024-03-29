﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BCLabManager.View
{
    /// <summary>
    /// Interaction logic for ProjectSettingTypeView.xaml
    /// </summary>
    public partial class ProjectSettingView : Window
    {
        public ProjectSettingView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var projectSetting = DataContext as ViewModel.ProjectSettingCreateViewModel;
            if (projectSetting.Project != null)
            {
                this.Close();
            }
        }
    }
}
