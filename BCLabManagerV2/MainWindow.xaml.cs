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
using System.Threading;

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
        private void LocalFileExistenceCheck_Click(object sender, RoutedEventArgs e)    //将本地缺失文件列表放入Running Log
        {
            uint existNum;
            List<string[]> list = FileExistenceCheck(FileTransferHelper.Remote2Local, out existNum);
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
                MessageBox.Show($"All {existNum.ToString()} files are existed.");
        }
        private void RemoteFileExistenceCheck_Click(object sender, RoutedEventArgs e)   //将远程缺失文件列表放入Running Log
        {
            uint existNum;
            List<string[]> list = FileExistenceCheck(FileTransferHelper.Remote2Universal, out existNum);
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
                MessageBox.Show($"All {existNum.ToString()} files are existed.");
        }

        private void LocalFileMD5Check_Click(object sender, RoutedEventArgs e)  //将本地损坏文件和没有MD5的文件列表放入Running Log
        {
            List<string[]> list = FileMD5Check(FileTransferHelper.Remote2Local);
            if (list.Count > 0)
            {
                var emptylist = list.Where(o => o[1] == null || o[1] == string.Empty).ToList();
                var brokenlist = list.Where(o => o[1] != null && o[1] != string.Empty).ToList();
                string str = $"{brokenlist.Count} MD5 Broken Files:\n";
                foreach (var arr in brokenlist)
                {
                    str += $"{arr[0]}, {arr[1]}\n";
                }
                str += $"{emptylist.Count} MD5 Empty Files:\n";
                foreach (var arr in emptylist)
                {
                    str += $"{arr[0]}\n";
                }
                RuningLog.Write(str);
                MessageBox.Show($"{brokenlist.Count} files' MD5 are broken.\n" +
                    $"{emptylist.Count} files' MD5 are empty.\n" +
                    $" Check running log for the details.");
            }
            else
                MessageBox.Show($"All files are fine.");
        }

        private void RemoteFileMD5Check_Click(object sender, RoutedEventArgs e) //将远程损坏文件和没有MD5的文件列表放入Running Log
        {
            if (MessageBoxResult.Cancel == MessageBox.Show("It may take a long time to download, are you sure to proceed?", "Remote File MD5 Check", MessageBoxButton.OKCancel))
                return;
            Thread t = new Thread(() =>
            {
                List<string[]> list;
                try
                {
                    list = FileMD5Check(FileTransferHelper.Remote2Universal);
                }
                catch (Exception error)
                {
                    MessageBox.Show($"MD5 Check Failed.\n{error.Message}");
                    return;
                }
                if (list!=null && list.Count > 0)
                {
                    var emptylist = list.Where(o => o[1] == null || o[1] == string.Empty).ToList();
                    var brokenlist = list.Where(o => o[1] != null && o[1] != string.Empty).ToList();
                    string str = $"{brokenlist.Count} MD5 Broken Files:\n";
                    foreach (var arr in brokenlist)
                    {
                        str += $"{arr[0]}, {arr[1]}\n";
                    }
                    str = $"{emptylist.Count} MD5 Empty Files:\n";
                    foreach (var arr in emptylist)
                    {
                        str += $"{arr[0]}\n";
                    }
                    RuningLog.Write(str);
                    MessageBox.Show($"{brokenlist.Count} files' MD5 are broken.\n" +
                        $"{emptylist.Count} files' MD5 are empty.\n" +
                        $" Check running log for the details.");
                }
                else
                    MessageBox.Show($"All files are fine.");
            });
            t.Start();
        }
        private void LocalMissingFileDownload_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Cancel == MessageBox.Show("It may take a long time to download, are you sure to proceed?", "Local File Restore", MessageBoxButton.OKCancel))
                return;
            Thread t = new Thread(() =>
            {
                List<string> RestoreList = new List<string>();
                uint existNum;
                List<string[]> list = FileExistenceCheck(FileTransferHelper.Remote2Local, out existNum);
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (FileTransferHelper.FileDownload(item[0], item[1]))
                            RestoreList.Add(item[0]);
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
                else
                    MessageBox.Show($"All {existNum.ToString()} files are existed.");
            });
            t.Start();
        }
        private void RemoteMissingFileRestore_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Cancel == MessageBox.Show("It may take a long time to download, are you sure to proceed?", "Remote File Restore", MessageBoxButton.OKCancel))
                return;
            Thread t = new Thread(() =>
            {
                List<string> RestoreList = new List<string>();
                uint existNum;
                List<string[]> list = FileExistenceCheck(FileTransferHelper.Remote2Universal, out existNum);
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (FileTransferHelper.FileRestore(item[0], item[1]))
                            RestoreList.Add(item[0]);
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
                else
                    MessageBox.Show($"All {existNum.ToString()} files are existed.");
            });
            t.Start();
        }
        private void RemoteFileMD5Commit_Click(object sender, RoutedEventArgs e)    //如果远程文件存在，但没有MD5，则计算远程文件的MD5，提交到数据库
        {
            Thread t = new Thread(() =>
            {
                List<string> RestoreMD5List = new List<string>();
                foreach (var tmr in mainWindowViewModel.TableMakerRecordService.Items)
                {
                    foreach (var tmp in tmr.Products)
                    {
                        if (tmp.FilePath == string.Empty || tmp.FilePath == null)   //一般不会有这样的情况
                            continue;
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
                    if (tr.TestFilePath == null || tr.TestFilePath == string.Empty) //有些test record还没有数据
                        continue;
                    string filepath = FileTransferHelper.Remote2Universal(tr.TestFilePath);
                    if (tr.MD5 == null || tr.MD5 == string.Empty)
                    {
                        if (File.Exists(filepath))
                        {
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
                MessageBox.Show($"{RestoreMD5List.Count} files' MD5 is restored");
            });
            t.Start();
        }

        private List<string[]> FileExistenceCheck(Func<string, string> relocate, out uint existNum)//检查所有文件是否存在，如果不存在，就放入返回值里。返回数组元素0是文件地址，1是MD5
        {
            List<string[]> MissingList = new List<string[]>();
            existNum = 0;
            foreach (var tmr in mainWindowViewModel.TableMakerRecordService.Items)
            {
                foreach (var tmp in tmr.Products)
                {
                    if (tmp.FilePath == string.Empty || tmp.FilePath == null)   //一般不会有这样的情况
                        continue;
                    string filepath = relocate(tmp.FilePath);
                    if (!File.Exists(filepath))
                    {
                        MissingList.Add(new string[] { tmp.FilePath, tmp.MD5 });
                    }
                    else
                        existNum++;
                }
            }

            foreach (var tr in mainWindowViewModel.ProgramService.RecipeService.TestRecordService.Items)
            {
                if (tr.TestFilePath == string.Empty || tr.TestFilePath == null || tr.Status == TestStatus.Abandoned) //有些test record还没有数据
                    continue;
                string filepath = relocate(tr.TestFilePath);
                if (!File.Exists(filepath))
                {
                    MissingList.Add(new string[] { tr.TestFilePath, tr.MD5 });
                }
                else
                    existNum++;
            }
            return MissingList;
        }

        private List<string[]> FileMD5Check(Func<string, string> relocate)  //检查所有文件的MD5，如果MD5是空的，或者MD5是错的，就放入返回值里。不考虑文件不存在的情况。返回数组元素0是文件地址，1是数据库中记录的MD5
        {
            List<string[]> BrokenList = new List<string[]>();
            //List<string> RestoreList = new List<string>();
            foreach (var tmr in mainWindowViewModel.TableMakerRecordService.Items)
            {
                foreach (var tmp in tmr.Products)
                {
                    if (tmp.FilePath == string.Empty || tmp.FilePath == null)   //一般不会有这样的情况
                        continue;
                    string filepath = relocate(tmp.FilePath);
                    if (tmp.MD5 != null && tmp.MD5 != string.Empty)
                    {
                        if (File.Exists(filepath))
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
                if (tr.TestFilePath == string.Empty || tr.TestFilePath == null || tr.Status == TestStatus.Abandoned) //有些test record还没有数据
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

        #region Temporary Method
        private void TemporaryMethod_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new AppDbContext())
            {
                var project = context.Projects.SingleOrDefault(o => o.Name == "O2Sim4");
                var programs = context.Programs.Include(o => o.Project).ToList();
                var programNames = programs.Where(o => o.Project.Id == project.Id).Select(o => o.Name);
                //var programs = context.Programs.Include(o=>o.Project).Where(o => o.Project.Id == project.Id);
                foreach (var proName in programNames)
                {
                    //if (proName == "EV-Static-N2")
                    //    continue;
                    var program = context.Programs.SingleOrDefault(o => o.Project.Id == project.Id && o.Name == proName);
                    //foreach (var program in programs)
                    {
                        context.Entry(program).Collection(p => p.Recipes).Load();
                        foreach (var rec in program.Recipes)
                        {
                            context.Entry(rec).Collection(r => r.TestRecords).Load();
                            context.Entry(rec).Reference(r => r.RecipeTemplate).Load();
                        }
                        foreach (var rec in program.Recipes)
                        {
                            if (rec.Name.Contains('_'))
                                rec.Name = rec.Name.Replace('_', '-');
                            //else
                            //    continue;
                            if (rec.RecipeTemplate.Name.Contains('_'))
                                rec.RecipeTemplate.Name = rec.RecipeTemplate.Name.Replace('_', '-');
                            //else
                            //    continue;
                            foreach (var tr in rec.TestRecords)
                            {
                                tr.RecipeStr = $"{tr.Temperature}Deg-{rec.Name}";
                                if (tr.Status != TestStatus.Waiting && tr.TestFilePath.Contains("csv"))
                                {
                                    if (!tr.TestFilePath.Contains(tr.RecipeStr))
                                    {
                                        tr.TestFilePath = ReplaceRecipeString(tr);
                                    }
                                    UpdateFileName(tr);
                                }
                            }
                        }
                        program.RecipeTemplates = program.Recipes.Select(o => o.Name).Distinct().ToList();
                    }
                    context.SaveChanges();
                }
            }
        }

        private void TemporaryMethod1_Click(object sender, RoutedEventArgs e)  //查MD5错误文件的MD5码
        {
            List<string[]> list = FileMD5Check(FileTransferHelper.Remote2Local);
            if (list.Count > 0)
            {
                string str = string.Empty;
                foreach (var item in list)
                {
                    var localMD5 = FileTransferHelper.GetMD5(item[0]);
                    var remotePath = FileTransferHelper.Local2Remote(item[0]);
                    var remoteMD5 = FileTransferHelper.GetMD5(remotePath);
                    str += $"Local File Path: {item[0]}, Local File MD5: {localMD5}\n" +
                        $"Remote File Path: {remotePath}, Remote File MD5: {remoteMD5}\n" +
                        $"MD5 in database: {item[1]}\n";
                }
                RuningLog.Write(str);
                MessageBox.Show($"{list.Count} local files are broken.\n" +
                    $" Check running log for the details.");
            }
            else
                MessageBox.Show($"All files are fine.");
        }
        private void TemporaryMethod2_Click(object sender, RoutedEventArgs e)  //查单个文件的MD5码
        {
            string filePath = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select the file you want to check.";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
                var MD5 = FileTransferHelper.GetMD5(filePath);
                MessageBox.Show(MD5);
            }
        }

        private void UpdateFileName(TestRecord tr)
        {
            var testFilePath = tr.TestFilePath;
            var folder = Path.GetDirectoryName(testFilePath);
            var timestamp = tr.StartTime.ToString("yyyyMMddHHmmss");
            var files = Directory.GetFiles(folder, $"*{timestamp}*");
            if (files.Length == 1)
            {
                if (files[0] != testFilePath)
                    File.Move(files[0], testFilePath);
            }
            else if (files.Length > 1)
            {
                MessageBox.Show("Same timestamp!");
            }
            else
            {
                MessageBox.Show("Cannot find file!");
            }
        }

        private string ReplaceRecipeString(TestRecord tr)
        {
            var a = tr.TestFilePath.IndexOf(tr.ProgramStr) + tr.ProgramStr.Length + 1;
            var b = tr.TestFilePath.IndexOf(tr.TesterStr) - 1;
            var recname = tr.TestFilePath.Substring(a, b - a);
            return tr.TestFilePath.Replace(recname, tr.RecipeStr);
        }
        #endregion
    }
}
