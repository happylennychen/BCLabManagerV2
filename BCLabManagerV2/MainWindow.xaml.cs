#define Show
//#define Requester
#define StartWindow
using System;
using System.Collections.Generic;
//using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BCLabManager.ViewModel;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using Microsoft.EntityFrameworkCore;
using System.IO;
using BCLabManager.View;
using Microsoft.Win32;
using Path = System.IO.Path;
using System.Threading;
using System.Linq;

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
                if (list != null && list.Count > 0)
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
                try
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
                }
                catch (Exception error)
                {
                    MessageBox.Show($"Local File Restore Failed.\n{error.Message}");
                }
            });
            t.Start();
        }
        private void RemoteMissingFileRestore_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Cancel == MessageBox.Show("It may take a long time to download, are you sure to proceed?", "Remote File Restore", MessageBoxButton.OKCancel))
                return;
            Thread t = new Thread(() =>
            {
                try
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
                }
                catch (Exception error)
                {
                    MessageBox.Show($"Remote File Restore Failed.\n{error.Message}");
                }
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
        private void TemporaryMethod3_Click(object sender, RoutedEventArgs e)   //Update Project and Recipe
        {
            using (var context = new AppDbContext())
            {
                var projects = context.Projects.ToList();
                foreach (var prj in projects)
                {
                    if (prj.StartTimes == null)
                        prj.StartTimes = new List<DateTime>();
                    if (prj.StopTimes == null)
                        prj.StopTimes = new List<DateTime>();
                }
                var recipes = context.Recipes.Include(rec=>rec.Program).Include(rec=>rec.TestRecords).ToList();
                foreach (var rec in recipes)
                {
                    //if (rec.Program.IsInvalid)
                    //    rec.IsAbandoned = true;
                    if (rec.TestRecords.All(o => o.Status == TestStatus.Abandoned))
                        rec.IsAbandoned = true;
                }
                context.SaveChanges();
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
        private void STD_30Q_Click(object sender, RoutedEventArgs e)   //Update Project and Recipe
        {
            TestRecordServiceClass trService = new TestRecordServiceClass();
            using (var context = new AppDbContext())
            {
                var trs = context.TestRecords
                    .Include(tr => tr.Recipe.Program.Project).Where(tr=>tr.TesterStr == "17200")
                    //.ThenInclude(rec => rec.Program)
                    //.ThenInclude(pro => pro.Project)
                    //.Include(tr=>tr.AssignedChannel)
                    //.ThenInclude(ch=>ch.Tester)
                    //.ThenInclude(tester=>tester.ITesterProcesser)
                    .ToList();
                trs = trs.Where(tr => tr.Status == TestStatus.Completed && tr.ProjectStr == "Bissel P2954 7s" && tr.StdFilePath == null).ToList();
                foreach (var tr in trs)
                {
                    var processer = new Chroma17200Processer();
                    var localTestFileFullPath = FileTransferHelper.Remote2Local(tr.TestFilePath);
                    string localStdFileFullPath = string.Empty;
                    if (!trService.CreateStdFile(processer, localTestFileFullPath, out localStdFileFullPath))
                    {
                        //File.Delete(localTestFileFullPath);
                        continue;
                    }
                    var stdMD5 = string.Empty;
                    var stdFilePath = string.Empty;
                    FileTransferHelper.FileUpload(localStdFileFullPath, out stdFilePath, out stdMD5);
                    if (stdMD5 == string.Empty)
                        continue;
                    tr.StdFilePath = stdFilePath;
                    tr.StdMD5 = stdMD5;
                }
                context.SaveChanges();
            }
        }
        #endregion


        private void Performance_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<DateTime, int> dailyTP, monthlyTH;
            Dictionary<YearWeek, int> weeklyTP;
            Dictionary<DateTime, double> dailyOR;
            Dictionary<string, Dictionary<string, TimeSpan>> projectTime;
            /*using (var dbContext = new AppDbContext())
            {
                var trs = dbContext.TestRecords.ToList().Where(o => o.StartTime != DateTime.MinValue && (o.Status == TestStatus.Completed || o.Status != TestStatus.Invalid || o.Status == TestStatus.Executing));
                dailyTP = GetDailyThroughputs(trs, TestStatus.Completed);
                weeklyTP = GetWeeklyThroughputs(trs, TestStatus.Completed);
                monthlyTH = GetMonthlyThroughputs(trs, TestStatus.Completed);

                dailyOR = GetDailyOccupancyRatio(trs);

                var projects = dbContext.Projects
                    .Include(prj => prj.Programs)
                        .ThenInclude(prog => prog.Recipes)
                            .ThenInclude(rec => rec.TestRecords).ToList();
                foreach (var prj in projects)
                {
                    foreach (var prog in prj.Programs)
                    {
                        dbContext.Entry(prog).Reference(p => p.Type).Load();
                    }
                }
                projectTime = GetProjectTime(projects);

            }*/
            //if (dailyTP != null)
            //{
            /*foreach (var key in dailyTP.Keys)
            {
                //Console.WriteLine($"{key.Year}/{key.Month}/{key.Day}, {dic[key]}");
                RuningLog.Write($"{key.Year}/{key.Month}/{key.Day}, {dailyTP[key]}\n");
            }
            RuningLog.Write($"=============================================================\n");
            if (monthlyTH != null)
            {
                foreach (var key in monthlyTH.Keys)
                {
                    //Console.WriteLine($"{key.Year}/{key.Month}/{key.Day}, {dic[key]}");
                    RuningLog.Write($"{key.Year}/{key.Month}, {monthlyTH[key]}\n");
                }
            }
            RuningLog.Write($"=============================================================\n");
            if (weeklyTP != null)
            {
                foreach (var key in weeklyTP.Keys)
                {
                    //Console.WriteLine($"{key.Year}/{key.Month}/{key.Day}, {dic[key]}");
                    RuningLog.Write($"{key.Year} W{key.Week}, {weeklyTP[key]}\n");
                }
            }
            if (dailyOR != null)
            {
                foreach (var key in dailyOR.Keys)
                {
                    //Console.WriteLine($"{key.Year}/{key.Month}/{key.Day}, {dic[key]}");
                    RuningLog.Write($"{key.Year}/{key.Month}/{key.Day}, {dailyOR[key]}\n");
                }
            }*/
            /*if (projectTime != null)
            {
                foreach (var prj in projectTime.Keys)
                {
                    foreach (var type in projectTime[prj].Keys)
                    {
                        var t = projectTime[prj][type];
                        RuningLog.Write($"{prj}, {type}, {t.TotalDays}\n");
                    }
                }
            }*/
            //}
            List<BatteryType> bts;
            List<Project> prjs;
            List<Program> pros;
            List<Recipe> recs;
            List<TestRecord> trs;
            List<LibFG> lib_fgs;
            List<TableMakerRecord> tmrs;
            List<TableMakerProduct> tmps;
            List<EmulatorResult> ers;
            using (var dbContext = new AppDbContext())
            {
                bts = dbContext.BatteryTypes.ToList();
                prjs = dbContext.Projects.ToList();
                pros = dbContext.Programs.ToList();
                var pts = dbContext.ProgramTypes.ToList();
                recs = dbContext.Recipes.ToList();
                trs = dbContext.TestRecords.ToList().Where(tr => tr.Recipe != null && tr.TesterStr == "17200").ToList();
                ers = dbContext.EmulatorResults.ToList().Where(er=>er.is_valid).ToList();
                var rps = dbContext.ReleasePackages.ToList();
                tmrs = dbContext.TableMakerRecords.ToList().Where(tmr=>tmr.IsValid).ToList();
                tmps = dbContext.TableMakerProducts.ToList().Where(tmp=>tmp.IsValid).ToList();
                lib_fgs = dbContext.lib_fgs.ToList().Where(lib=>lib.is_valid).ToList();  //不管是valid还是invalid，对它的评估都是valid
            }

            //var batteryTypes = trs.Select(tr => tr.Recipe.Program.Project.BatteryType).Distinct().ToList();
            //var ers = trs.Select(tr => tr.EmulatorResults.SingleOrDefault(er=>er.is_valid)).Distinct().ToList();
            /*foreach (var bt in bts.OrderBy(o => o.Id))
            {
                foreach (var prj in bt.Projects.OrderBy(o => o.Id))
                {
                    var erGroup = prj.EmulatorResults.GroupBy(er => er.lib_fg).Select(o => o.Key);
                    if (erGroup == null || erGroup.Count() == 0)
                        continue;
                    RuningLog.Write($"\t{prj.Name}\n");
                    foreach (var item in erGroup.OrderBy(o => o.Id))
                    {
                        var lib = item;
                        List<EmulatorResult> ers = prj.EmulatorResults.Where(er => er.lib_fg == lib).ToList();
                        var tmpGroup = ers.Where(er => er.table_maker_cfile != null && er.table_maker_hfile != null).GroupBy(er => new { er.table_maker_cfile, er.table_maker_hfile }).Select(o => o.Key).ToList();
                        if (tmpGroup.Count == 0)
                        {
                            RuningLog.Write($"\t\tTable Maker Product is not in the record\n");
                        }
                        else
                            foreach (var t in tmpGroup)
                            {
                                if (t.table_maker_cfile.TableMakerRecord != t.table_maker_hfile.TableMakerRecord)
                                    MessageBox.Show("Not Possible");
                                RuningLog.Write($"\t\t{t.table_maker_cfile.TableMakerRecord.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")}\n");
                                RuningLog.Write($"\t\t{t.table_maker_cfile.FilePath}\n");
                                RuningLog.Write($"\t\t{t.table_maker_hfile.FilePath}\n");
                            }
                        RuningLog.Write($"\t\t\t{lib.libfg_dll_path}\n");
                        bool isEVpass = IsEVPass(prj, lib);
                        if (isEVpass)
                        {
                            RuningLog.Write($"\t\t\tEV_PASS");
                        }
                        else
                        {
                            RuningLog.Write($"\t\t\tEV_NOT_PASS");
                        }
                        var part = prj.EmulatorResults.Count(er => er.lib_fg == lib && er.is_valid);
                        var total = trs.Count(tr => tr.Recipe.Program.Project == prj && tr.Recipe.Program.Type.Name == "EV" && tr.Status == TestStatus.Completed);
                        RuningLog.Write($"\t{part}/{total}\n");
                        bool isReleased = IsReleased(prj, lib);
                        if (isReleased)
                        {
                            RuningLog.Write($"\t\t\tRELEASED\n");
                        }
                        else
                        {
                            RuningLog.Write($"\t\t\tNOT_RELEASED\n");
                        }
                    }
                }
            }*/
            foreach (var bt in bts.OrderBy(o => o.Id))
            {
                foreach (var prj in bt.Projects)
                {
                    foreach (var tmr in prj.TableMakerRecords.Where(tmr=>tmr.IsValid))
                    {
                        foreach (var tmp in tmr.Products.Where(tmp=>tmp.IsValid))
                        {
                            var prj_ers = prj.EmulatorResults.Where(er => er.table_maker_cfile == tmp && er.is_valid).ToList();
                            if (prj_ers.Count == 0)
                            {
                                RuningLog.Write($"{bt.Name},{prj.Name},{tmr.Timestamp.ToString("yyyy/MM/dd")},{tmp.FilePath}\n");
                                continue;
                            }
                            var libs = prj_ers.Select(o => o.lib_fg).Distinct().ToList();
                            foreach (var lib in libs.Where(o=>o.is_valid))
                            {
                                var part = prj.EmulatorResults.Count(er => er.lib_fg == lib && er.is_valid);
                                var total = trs.Count(tr => tr.Recipe.Program.Project == prj && tr.Recipe.Program.Type.Name == "EV" && tr.Status == TestStatus.Completed);
                                bool isEVpass = IsEVPass(prj, lib);
                                bool isReleased = IsReleased(prj, lib);
                                RuningLog.Write($"{bt.Name},{prj.Name},{tmr.Timestamp.ToString("yyyy/MM/dd")},{tmp.FilePath},{lib.libfg_dll_path},{part}/{total},{isEVpass}, {isReleased}\n");
                            }
                        }
                    }
                }
            }
            var supportedBTs = tmrs.Select(tmr => tmr.Project.BatteryType).Distinct().ToList();
            var products = tmrs.SelectMany(tmr => tmr.Products).ToList();
            List<TableMakerProduct> EVP = new List<TableMakerProduct>();
            foreach (var pd in products)
            {
                var pd_ers = ers.Where(er => er.table_maker_cfile == pd && er.lib_fg.is_valid).ToList(); 
                var evtrs = trs.Where(tr => tr.Recipe.Program.Project == pd.Project && tr.Recipe.Program.Type.Name == "EV" && tr.Status == TestStatus.Completed).ToList();
                if (pd_ers.Count == evtrs.Count)
                    EVP.Add(pd);
            }
            RuningLog.Write($"{supportedBTs.Count}, {products.Count}, {EVP.Count}\n");
        }

        private bool IsReleased(Project pj, LibFG lib_fg)
        {
            return pj.ReleasePackages.Where(rp => rp.is_valid).Any(rp => rp.lib_fg == lib_fg);
        }

        private bool IsEVPass(Project pj, LibFG lib_fg)
        {
            var trs = pj.Programs.Where(o => o.Type.Name == "EV").SelectMany(pro => pro.Recipes.SelectMany(rec => rec.TestRecords)).Where(tr => tr.TesterStr == "17200" && tr.Status == TestStatus.Completed).ToList();
            return trs.All(tr => tr.EmulatorResults.Where(er => er.is_valid).Any(er => er.lib_fg == lib_fg));
        }

        private Dictionary<string, Dictionary<string, TimeSpan>> GetProjectTime(List<Project> projects)
        {
            Dictionary<string, Dictionary<string, TimeSpan>> output = new Dictionary<string, Dictionary<string, TimeSpan>>();
            foreach (var prj in projects)
            {
                string prjStr = prj.Name;
                Dictionary<string, TimeSpan> programsTime = new Dictionary<string, TimeSpan>();
                foreach (var prog in prj.Programs)
                {
                    var programType = prog.Type.Name;
                    if (!programsTime.ContainsKey(programType))
                    {
                        TimeSpan ts = GetProgramTimeSpan(prog);
                        programsTime.Add(programType, ts);
                    }
                    else
                    {
                        TimeSpan ts = GetProgramTimeSpan(prog);
                        //ts += programsTime[programType];
                        programsTime[programType] += ts;
                    }
                }
                if (programsTime.Count != 0)
                    output.Add(prjStr, programsTime);
            }
            return output;
        }

        private TimeSpan GetProgramTimeSpan(Program prog)
        {
            TimeSpan output = TimeSpan.Zero;
            foreach (var rec in prog.Recipes)
            {
                output += GetRecipeTimeSpan(rec);
            }
            return output;
        }

        private TimeSpan GetRecipeTimeSpan(Recipe rec)
        {
            TimeSpan output = TimeSpan.Zero;
            foreach (var tr in rec.TestRecords)
            {
                if (tr.StartTime != DateTime.MinValue && tr.EndTime != DateTime.MinValue)
                    output += tr.EndTime - tr.StartTime;
            }
            return output;
        }

        private Dictionary<DateTime, double> GetDailyOccupancyRatio(IEnumerable<TestRecord> trs)
        {
            Dictionary<DateTime, double> output = new Dictionary<DateTime, double>();
            var startTime = trs.Min(o => o.StartTime);
            var endTime = trs.Max(o => o.EndTime);
            for (DateTime t = startTime.Date; t < endTime.Date + TimeSpan.FromDays(1); t += TimeSpan.FromDays(1))
            {
                if (!output.ContainsKey(t))
                    output.Add(t, GetOccupancyRatio(t, trs));
            }
            return output;
        }

        private double GetOccupancyRatio(DateTime t, IEnumerable<TestRecord> trs)
        {
            var endTime = new DateTime(t.Year, t.Month, t.Day, 23, 59, 59, 999);
            var innerTRs = trs.Where(tr => (tr.StartTime > t && tr.EndTime < endTime));
            var overTRs = trs.Where(tr => (tr.StartTime < t && tr.EndTime > endTime));
            var leftTRs = trs.Where(tr => (tr.StartTime < t && tr.EndTime > t && tr.EndTime < endTime));
            var rightTRs = trs.Where(tr => (tr.StartTime > t && tr.StartTime < endTime && tr.EndTime > endTime));
            TimeSpan ts = TimeSpan.Zero;
            foreach (var tr in overTRs)
            {
                ts += (endTime - t);
            }
            foreach (var tr in innerTRs)
            {
                ts += (tr.EndTime - tr.StartTime);
            }
            foreach (var tr in leftTRs)
            {
                ts += (tr.EndTime - t);
            }
            foreach (var tr in rightTRs)
            {
                ts += (endTime - tr.StartTime);
            }
            return (ts.TotalMilliseconds) / (4 * (endTime - t).TotalMilliseconds);
        }

        private Dictionary<DateTime, int> GetDailyThroughputs(IEnumerable<TestRecord> trs, TestStatus ts)
        {
            Dictionary<DateTime, int> output = new Dictionary<DateTime, int>();
            var startTime = trs.Min(o => o.StartTime);
            var endTime = trs.Max(o => o.EndTime);
            var selectedTRs = trs.Where(o => o.Status == ts);
            for (DateTime t = startTime.Date; t < endTime.Date + TimeSpan.FromDays(1); t += TimeSpan.FromDays(1))
            {
                var num = selectedTRs.Count(o => o.EndTime.Date == t.Date);
                output.Add(t, num);
            }
            return output;
        }

        private Dictionary<YearWeek, int> GetWeeklyThroughputs(IEnumerable<TestRecord> trs, TestStatus ts)
        {
            Dictionary<YearWeek, int> output = new Dictionary<YearWeek, int>(new YearWeekComparer());
            var dailyThroughpus = GetDailyThroughputs(trs, ts);
            foreach (var t in dailyThroughpus.Keys)
            {
                var week = GetWeek(t);
                //var weekStr = $"{ week[0]} W{week[1]}";
                if (!output.Keys.Contains(week))
                {
                    output.Add(week, dailyThroughpus[t]);
                }
                else
                {
                    output[week] += dailyThroughpus[t];
                }
            }
            return output;
        }

        private YearWeek GetWeek(DateTime t)
        {
            var year = t.Year;
            var week = 1 + t.DayOfYear / 7;
            return new YearWeek { Year = year, Week = week };
        }

        private Dictionary<DateTime, int> GetMonthlyThroughputs(IEnumerable<TestRecord> trs, TestStatus ts)
        {
            Dictionary<DateTime, int> output = new Dictionary<DateTime, int>();
            var dailyThroughpus = GetDailyThroughputs(trs, ts);
            foreach (var t in dailyThroughpus.Keys)
            {
                DateTime month = GetMonth(t);
                if (!output.Keys.Contains(month))
                {
                    output.Add(month, dailyThroughpus[t]);
                }
                else
                {
                    output[month] += dailyThroughpus[t];
                }
            }
            return output;
        }

        private DateTime GetMonth(DateTime t)
        {
            DateTime output = new DateTime(t.Year, t.Month, 1);
            return output;
        }

        private void Dashboard_Selected(object sender, RoutedEventArgs e)
        {
            UpdateWeeklyThroughput();
            UpdateOccupancyRatio();
            UpdateProgress();
        }

        private void UpdateProgress()
        {
        }

        private void UpdateOccupancyRatio()
        {
            Dictionary<DateTime, double> dailyOR;
            using (var dbContext = new AppDbContext())
            {
                var trs = dbContext.TestRecords.ToList().Where(o => o.StartTime != DateTime.MinValue && (o.Status == TestStatus.Completed || o.Status != TestStatus.Invalid || o.Status == TestStatus.Executing));
                dailyOR = GetDailyOccupancyRatio(trs);
            }

            DashBoardViewInstance.ORbarChart.PlotBars(dailyOR.Values);
        }

        private void UpdateWeeklyThroughput()
        {
            Dictionary<YearWeek, int> weeklyTP;
            using (var dbContext = new AppDbContext())
            {
                var trs = dbContext.TestRecords.ToList().Where(o => o.StartTime != DateTime.MinValue && (o.Status == TestStatus.Completed || o.Status != TestStatus.Invalid || o.Status == TestStatus.Executing));
                weeklyTP = GetWeeklyThroughputs(trs, TestStatus.Completed);
            }

            DashBoardViewInstance.wTHbarChart.PlotBars(weeklyTP.Values);
        }
    }

    class YearWeek
    {
        public int Year { get; set; }
        public int Week { get; set; }
    }
    class YearWeekComparer : EqualityComparer<YearWeek>
    {

        public override bool Equals(YearWeek x, YearWeek y)
        {
            if (x.Year == y.Year && x.Week == y.Week)
                return true;
            else
                return false;
        }

        public override int GetHashCode(YearWeek obj)
        {
            return (obj.Year * obj.Week).GetHashCode();
        }
    }
}
