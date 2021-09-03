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
using Path = System.IO.Path;

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
        private void LocalFileExistenceCheck_Click(object sender, RoutedEventArgs e)
        {
            List<string[]> list = FileExistenceCheck(FileTransferHelper.Remote2Local);
            if (list.Count > 0)
            {
                string str = $"{list.Count} Missing Files:\n";
                foreach (var arr in list)
                {
                    str += $"{arr[0]}\n";
                }
                RuningLog.Write(str);
                MessageBox.Show($"{list.Count} files are missing. Check running log for the details.");
            }
            else
                MessageBox.Show($"All files are existed.");
        }
        private void RemoteFileExistenceCheck_Click(object sender, RoutedEventArgs e)
        {
            List<string[]> list = FileExistenceCheck(FileTransferHelper.Remote2Universal);
            if (list.Count > 0)
            {
                string str = $"{list.Count} Missing Files:\n";
                foreach (var arr in list)
                {
                    str += $"{arr[0]}\n";
                }
                RuningLog.Write(str);
                MessageBox.Show($"{list.Count} files are missing. Check running log for the details.");
            }
            else
                MessageBox.Show($"All files are existed.");
        }


        private void FileCheck_Click(object sender, RoutedEventArgs e)
        {
            List<string> MissingList = new List<string>();
            List<string> BrokenList = new List<string>();
            List<string> MD5EmptyList = new List<string>();
            List<string> RestoreList = new List<string>();
            List<string> RestoreMD5List = new List<string>();
            foreach (var tmr in mainWindowViewModel.TableMakerRecordService.Items)
            {
                foreach (var tmp in tmr.Products)
                {
                    string filepath = FileTransferHelper.Remote2Universal(tmp.FilePath);
                    if (!File.Exists(filepath))
                    {
                        MissingList.Add(filepath);
                        if (Restore(filepath, tmp.MD5))
                            RestoreList.Add(filepath);
                    }
                    else
                    {
                        if (tmp.MD5 != null && tmp.MD5 != string.Empty)
                        {
                            if (!FileTransferHelper.CheckFileMD5(filepath, tmp.MD5))
                            {
                                BrokenList.Add(filepath);
                                if (Restore(filepath, tmp.MD5))
                                    RestoreList.Add(filepath);
                            }
                        }
                        else
                        {
                            MD5EmptyList.Add(filepath);
                            using (var context = new AppDbContext())
                            {
                                var dbtmp = context.TableMakerProducts.SingleOrDefault(o => o.Id == tmp.Id);
                                dbtmp.MD5 = FileTransferHelper.GetMD5(filepath);
                                context.SaveChanges();
                            }
                            RestoreMD5List.Add(filepath);
                        }
                    }
                }
            }

            foreach (var tr in mainWindowViewModel.ProgramService.RecipeService.TestRecordService.Items)
            {
                string filepath = FileTransferHelper.Remote2Universal(tr.TestFilePath);
                if (!File.Exists(filepath))
                {
                    MissingList.Add(filepath);
                    if (Restore(filepath, tr.MD5))
                        RestoreList.Add(filepath);
                }
                else
                {
                    if (tr.MD5 != null && tr.MD5 != string.Empty)
                    {
                        if (!FileTransferHelper.CheckFileMD5(filepath, tr.MD5))
                        {
                            BrokenList.Add(filepath);
                            if (Restore(filepath, tr.MD5))
                                RestoreList.Add(filepath);
                        }
                    }
                    else
                    {
                        MD5EmptyList.Add(filepath);
                        using (var context = new AppDbContext())
                        {
                            var dbtr = context.TestRecords.SingleOrDefault(o => o.Id == tr.Id);
                            dbtr.MD5 = FileTransferHelper.GetMD5(filepath);
                            context.SaveChanges();
                        }
                        RestoreMD5List.Add(filepath);
                    }
                }
            }
            MessageBox.Show($"{MissingList.Count} files are missing.\n" +
                $"{BrokenList.Count} files are broken.\n" +
                $"{MD5EmptyList.Count} files' MD5 is empty,\n" +
                $"{RestoreList.Count} files restored.\n" +
                $"{RestoreMD5List.Count} files' MD5 is restored");
        }

        private bool Restore(string filepath, string MD5)
        {
            string localPath = FileTransferHelper.Universal2Local(filepath);
            if (File.Exists(localPath))
            {
                if (MD5 != null && MD5 != string.Empty)
                {
                    if (FileTransferHelper.CheckFileMD5(localPath, MD5))
                    {
                        var direc = Path.GetDirectoryName(filepath);
                        if (!Directory.Exists(direc))
                            Directory.CreateDirectory(direc);
                        File.Copy(localPath, filepath, true);
                        return true;
                    }
                }
                else
                {
                    var direc = Path.GetDirectoryName(filepath);
                    if (!Directory.Exists(direc))
                        Directory.CreateDirectory(direc);
                    File.Copy(localPath, filepath, true);
                    return true;
                }
            }
            return false;
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

        private void LocalFileMD5Check_Click(object sender, RoutedEventArgs e)
        {
            List<string[]> list = FileMD5Check(FileTransferHelper.Remote2Local);
            if (list.Count > 0)
            {
                var emptylist = list.Where(o => o[1] == null || o[1] == string.Empty).ToList();
                var brokenlist = list.Where(o => o[1] != null && o[1] != string.Empty).ToList();
                string str = $"{brokenlist.Count} broken Files:\n";
                foreach (var arr in brokenlist)
                {
                    str += $"{arr[0]}, {arr[1]}\n";
                }
                str = $"{emptylist.Count} MD5 Empty Files:\n";
                foreach (var arr in emptylist)
                {
                    str += $"{arr[0]}, {arr[1]}\n";
                }
                RuningLog.Write(str);
                MessageBox.Show($"{brokenlist.Count} files are broken.\n" +
                    $"{emptylist.Count} files' MD5 is empty.\n" +
                    $" Check running log for the details.");
            }
            else
                MessageBox.Show($"All files are fine.");
        }

        private void RemoteFileMD5Check_Click(object sender, RoutedEventArgs e)
        {
            List<string[]> list = FileMD5Check(FileTransferHelper.Remote2Universal);
            if (list.Count > 0)
            {
                var emptylist = list.Where(o => o[1] == null || o[1] == string.Empty).ToList();
                var brokenlist = list.Where(o => o[1] != null && o[1] != string.Empty).ToList();
                string str = $"{brokenlist.Count} broken Files:\n";
                foreach (var arr in brokenlist)
                {
                    str += $"{arr[0]}, {arr[1]}\n";
                }
                str = $"{emptylist.Count} MD5 Empty Files:\n";
                foreach (var arr in emptylist)
                {
                    str += $"{arr[0]}, {arr[1]}\n";
                }
                RuningLog.Write(str);
                MessageBox.Show($"{brokenlist.Count} files are broken.\n" +
                    $"{emptylist.Count} files' MD5 is empty.\n" +
                    $" Check running log for the details.");
            }
            else
                MessageBox.Show($"All files are fine.");
        }
        private void LocalFileRestore_Click(object sender, RoutedEventArgs e)
        {
            List<string> RestoreList = new List<string>();
            List<string[]> list = FileExistenceCheck(FileTransferHelper.Remote2Local);
            foreach (var file in list)
            {
                if (FileTransferHelper.FileDownload(file[0], file[1]))
                    RestoreList.Add(file[0]);
            }
            string str = $"{RestoreList.Count} files restored:\n";
            foreach (var file in RestoreList)
            {
                str += $"{file}\n";
            }
            RuningLog.Write(str);
            MessageBox.Show($"{RestoreList.Count} files restored.\n" +
                $" Check running log for the details.");
        }
        private void RemoteFileRestore_Click(object sender, RoutedEventArgs e)
        {

        }
        private void LocalFileMD5Repair_Click(object sender, RoutedEventArgs e)
        {

        }
        private void RemoteFileMD5Commit_Click(object sender, RoutedEventArgs e)
        {
            List<string> MissingList = new List<string>();
            List<string> BrokenList = new List<string>();
            List<string> MD5EmptyList = new List<string>();
            List<string> RestoreList = new List<string>();
            List<string> RestoreMD5List = new List<string>();
            foreach (var tmr in mainWindowViewModel.TableMakerRecordService.Items)
            {
                foreach (var tmp in tmr.Products)
                {
                    string filepath = FileTransferHelper.Remote2Universal(tmp.FilePath);
                    if (tmp.MD5 == null || tmp.MD5 == string.Empty)
                    {
                        if (File.Exists(filepath))
                        {
                            using (var context = new AppDbContext())
                            {
                                var dbtmp = context.TableMakerProducts.SingleOrDefault(o => o.Id == tmp.Id);
                                dbtmp.MD5 = FileTransferHelper.GetMD5(filepath);
                                context.SaveChanges();
                            }
                            RestoreMD5List.Add(filepath);
                        }
                    }
                }
            }

            foreach (var tr in mainWindowViewModel.ProgramService.RecipeService.TestRecordService.Items)
            {
                string filepath = FileTransferHelper.Remote2Universal(tr.TestFilePath);
                if (tr.MD5 == null || tr.MD5 == string.Empty)
                {
                    if (File.Exists(filepath))
                    {
                        MD5EmptyList.Add(filepath);
                        using (var context = new AppDbContext())
                        {
                            var dbtr = context.TestRecords.SingleOrDefault(o => o.Id == tr.Id);
                            dbtr.MD5 = FileTransferHelper.GetMD5(filepath);
                            context.SaveChanges();
                        }
                        RestoreMD5List.Add(filepath);
                    }
                }
            }
            MessageBox.Show($"{MissingList.Count} files are missing.\n" +
                $"{BrokenList.Count} files are broken.\n" +
                $"{MD5EmptyList.Count} files' MD5 is empty,\n" +
                $"{RestoreList.Count} files restored.\n" +
                $"{RestoreMD5List.Count} files' MD5 is restored");
        }

        private List<string[]> FileExistenceCheck(Func<string, string> relocate)
        {
            List<string[]> MissingList = new List<string[]>();
            foreach (var tmr in mainWindowViewModel.TableMakerRecordService.Items)
            {
                foreach (var tmp in tmr.Products)
                {
                    if (tmp.FilePath == string.Empty || tmp.FilePath == null)
                        continue;
                    string filepath = relocate(tmp.FilePath);
                    if (!File.Exists(filepath))
                    {
                        MissingList.Add(new string[] { tmp.FilePath, tmp.MD5 });
                    }
                }
            }

            foreach (var tr in mainWindowViewModel.ProgramService.RecipeService.TestRecordService.Items)
            {
                if (tr.TestFilePath == string.Empty || tr.TestFilePath == null)
                    continue;
                string filepath = relocate(tr.TestFilePath);
                if (!File.Exists(filepath))
                {
                    MissingList.Add(new string[] { tr.TestFilePath, tr.MD5 });
                }
            }
            return MissingList;
        }

        private List<string[]> FileMD5Check(Func<string, string> relocate)
        {
            List<string[]> BrokenList = new List<string[]>();
            //List<string> RestoreList = new List<string>();
            foreach (var tmr in mainWindowViewModel.TableMakerRecordService.Items)
            {
                foreach (var tmp in tmr.Products)
                {
                    if (tmp.FilePath == string.Empty || tmp.FilePath == null)
                        continue;
                    string filepath = relocate(tmp.FilePath);
                    if (tmp.MD5 != null && tmp.MD5 != string.Empty)
                    {
                        if (!FileTransferHelper.CheckFileMD5(filepath, tmp.MD5))
                        {
                            BrokenList.Add(new string[] { filepath, tmp.MD5 }); //Wrong MD5
                        }
                    }
                    else
                    {
                        BrokenList.Add(new string[] { filepath, tmp.MD5 }); //Empty MD5
                    }
                }
            }

            foreach (var tr in mainWindowViewModel.ProgramService.RecipeService.TestRecordService.Items)
            {
                if (tr.TestFilePath == string.Empty || tr.TestFilePath == null)
                    continue;
                string filepath = relocate(tr.TestFilePath);
                if (tr.MD5 != null && tr.MD5 != string.Empty)
                {
                    if (File.Exists(filepath))
                        if (!FileTransferHelper.CheckFileMD5(filepath, tr.MD5))
                        {
                            BrokenList.Add(new string[] { filepath, tr.MD5 }); //Wrong MD5
                        }
                }
                else
                {
                    BrokenList.Add(new string[] { filepath, tr.MD5 }); //Empty MD5
                }
            }
            return BrokenList;
        }
    }
}
