#define Show
//#define Requester
//#define StartWindow
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BCLabManager.ViewModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Collections.ObjectModel;
using BCLabManager.View;
using Microsoft.Win32;
using Npgsql;

namespace BCLabManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _mainWindowViewModel;
        public MainWindowViewModel mainWindowViewModel { get { return _mainWindowViewModel; } set { _mainWindowViewModel = value; } }
        public MainWindow()
        {
#if StartWindow
            var startupWindow = new StartupWindow();
            startupWindow.Show();
#endif
            try
            {
#if Show
                InitializeComponent();
#if !Requester
#endif
                InitializeNavigator();
                mainWindowViewModel = new MainWindowViewModel();
                DataContext = mainWindowViewModel;
#if Requester
                UpdateUIForRequester();
#endif
#else
                App.Current.Shutdown();
#endif
            }
            catch (DatabaseAccessException e)
            {
#if StartWindow
                startupWindow.Close();
#endif
                MessageBox.Show($"{e.Message}\n" +
                    $"{e.InnerException}\n" +
                    $"\n" +
                    $"Please setup database in the configuration window correctly!");
            }
            catch (Exception e)
            {
#if StartWindow
                startupWindow.Close();
#endif
                MessageBox.Show($"{e.Message}\n" +
                    $"{e.InnerException}");
                App.Current.Shutdown();
            }
            finally
            {
#if StartWindow
                if (startupWindow.IsActive)
                    startupWindow.Close();
#endif
            }
        }
        private void InitializeNavigator()
        {
            Navigator.Initialize(this);
        }

        private void Event_Click(object sender, RoutedEventArgs e)
        {
            AllEventsView allEventsView = new AllEventsView();
            var vm = new AllEventsViewModel
                 (
                 mainWindowViewModel.ProgramService,
                 mainWindowViewModel.ProjectService,
                 mainWindowViewModel.ProgramTypeService,
                 mainWindowViewModel.BatteryService,
                 mainWindowViewModel.TesterService,
                 mainWindowViewModel.ChannelService,
                 mainWindowViewModel.ChamberService,
                 mainWindowViewModel.BatteryTypeService
                 );
            allEventsView.DataContext = vm;// new AllEventsViewModel(/*EventService*/);
            allEventsView.ShowDialog();
        }


        private void Config_Click(object sender, RoutedEventArgs e)
        {
            Configuration();
        }
        private void Configuration()
        {
            Configuration conf = new Configuration();
            conf.RemotePath = GlobalSettings.RemotePath;
            conf.EnableTest = GlobalSettings.EnableTest;
            conf.MappingPath = GlobalSettings.MappingPath;
            conf.DatabaseHost = GlobalSettings.DatabaseHost;
            conf.DatabaseName = GlobalSettings.DatabaseName;
            conf.DatabaseUser = GlobalSettings.DatabaseUser;
            conf.DatabasePassword = GlobalSettings.DatabasePassword;
            ConfigurationView configView = new ConfigurationView();
            var vm = new ConfigurationViewModel(conf);
            configView.DataContext = vm;// new AllEventsViewModel(/*EventService*/);
            configView.ShowDialog();
        }
        private void UpdateUIForRequester()
        {
            AllTestersViewInstance.ButtonPanel.IsEnabled = false;
            AllChannelsViewInstance.ButtonPanel.IsEnabled = false;
            AllBatteriesViewInstance.ButtonPanel.IsEnabled = false;
            AllChambersViewInstance.ButtonPanel.IsEnabled = false;
            AllProgramTypesViewInstance.Visibility = Visibility.Collapsed;
            AllTableMakerProductTypesViewInstance.Visibility = Visibility.Collapsed;
            AllProgramsViewInstance.RecipeButtonPanel.IsEnabled = false;
            AllProgramsViewInstance.FreeTestRecordButtonPanel.IsEnabled = false;
            AllProgramsViewInstance.DirectCommitBtn.IsEnabled = false;
            AllProgramsViewInstance.ExecuteBtn.IsEnabled = false;
            AllProgramsViewInstance.CommitBtn.IsEnabled = false;
            AllProgramsViewInstance.InvalidateBtn.IsEnabled = false;
            AllProgramsViewInstance.AddBtn.IsEnabled = false;
            Grid.SetColumnSpan(ProjectBorder, 3);
            Grid.SetColumn(ProjectSettingBorder, 4);
            Grid.SetColumnSpan(ProjectSettingBorder, 5);
            //Title = "BCLM-R v0.2.0.5";
            var array = Title.Split();
            Title = $"{array[0]}-R {array[1]}";
        }
    }
}
