using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.View;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using Prism.Mvvm;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace BCLabManager.ViewModel
{
    /// <summary>
    /// Editable: True
    /// Updateable: 1. Events: True, 2. Records: True
    /// </summary>
    public class AllEventsViewModel : BindableBase
    {
        #region Fields
        //private EventService _eventService;
        RelayCommand _exportCommand;



        private BatteryServiceClass _batteryService;
        private ProjectServiceClass _projectService;
        private ProgramTypeServiceClass _programTypeService;
        private TesterServiceClass _testerService;
        private ChannelServiceClass _channelService;
        private ChamberServiceClass _chamberService;
        private ProgramServiceClass _programService;
        private BatteryTypeServiceClass _batteryTypeService;
        #endregion // Fields

        #region Constructor

        public AllEventsViewModel(/*EventService eventService*/ProgramServiceClass programService, ProjectServiceClass projectService, ProgramTypeServiceClass programTypeService, BatteryServiceClass batteryService, TesterServiceClass testerService, ChannelServiceClass channelService, ChamberServiceClass chamberService, BatteryTypeServiceClass batteryTypeService)
        {
            _programService = programService;

            _projectService = projectService;
            _programTypeService = programTypeService;
            _batteryService = batteryService;
            _testerService = testerService;
            _channelService = channelService;
            _chamberService = chamberService;
            _batteryTypeService = batteryTypeService;
            //_eventService = eventService;
            //this.CreateAllEvents(_eventService.Items);
            //_eventService.Items.CollectionChanged += Items_CollectionChanged;
            this.CreateAllEvents(EventService.Items);
            EventService.Items.CollectionChanged += Items_CollectionChanged;
        }
        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        var Event = item as Event;
                        this.AllEvents.Add(new EventViewModel(Event));
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        var Event = item as Event;
                        var deletetarget = this.AllEvents.SingleOrDefault(o => o.Id == Event.Id);
                        this.AllEvents.Remove(deletetarget);
                    }
                    break;
            }
        }

        void CreateAllEvents(ObservableCollection<Event> Events)
        {
            List<EventViewModel> all =
                (from cmb in Events
                 select new EventViewModel(cmb)).ToList();   //先生成viewmodel list(每一个model生成一个viewmodel，然后拼成list)

            this.AllEvents = new ObservableCollection<EventViewModel>(all);     //再转换成Observable
        }

        #endregion // Constructor

        #region Public Interface

        /// <summary>
        /// Returns a collection of all the EventModelViewModel objects.
        /// </summary>
        public ObservableCollection<EventViewModel> AllEvents { get; private set; }


        public ICommand ExportCommand
        {
            get
            {
                if (_exportCommand == null)
                {
                    _exportCommand = new RelayCommand(
                        param => { this.Export(); }
                        );
                }
                return _exportCommand;
            }
        }

        private void Export()
        {
            Thread t = new Thread(() =>
            {
                if (MessageBox.Show("It will take a while to get the work done, Continue?", "Generate Event Report.", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();
                    var folderPath = GlobalSettings.LocalPath + DateTime.Now.ToString("yyyyMMddHHmmss");
                    Directory.CreateDirectory(folderPath);
                    foreach (var evt in EventService.Items)
                    {
                        if (evt.Description.Contains("Line0"))
                        {
                            int id = Convert.ToInt32(GetStringFromDescription(evt.Description, "Test Record ID"));
                            var tr = _programService.RecipeService.TestRecordService.Items.SingleOrDefault(o => o.Id == id);
                            int lineNumber = Convert.ToInt32(GetStringFromDescription(evt.Description, "Line"));
                            List<string> lines = GetErrorLinesFromFile(tr.TestFilePath, lineNumber);
                            CreateEventFile(folderPath, evt, tr, lines, lineNumber);
                        }
                    }
                    string time = Math.Round(stopwatch.Elapsed.TotalSeconds, 0).ToString() + "S";
                    MessageBox.Show($"Completed. It took {time} to get the job done.");
                    Process.Start(folderPath);
                }
            });
            t.Start();
        }

        private void CreateEventFile(string folderPath, Event evt, TestRecord tr, List<string> lines, int lineNumber)
        {
            string errType = GetStringFromDescription(evt.Description, "Problem");
            string autoFixed = string.Empty;
            if (GetStringFromDescription(evt.Description, "Solution") == "Auto Fixed")
                autoFixed = "-auto fixed";
            string fileName = $"{errType}-{evt.Id}-{lineNumber}-{autoFixed}.txt";
            string filePath = Path.Combine(folderPath, fileName);
            FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine($"Project: {tr.ProjectStr}");
            sw.WriteLine($"Program: {tr.ProgramStr}");
            sw.WriteLine($"Battery: {tr.BatteryStr}");
            sw.WriteLine($"Tester&Channel: {tr.TesterStr}&{tr.ChannelStr}");
            sw.WriteLine($"File Path: {tr.TestFilePath}");
            sw.WriteLine($"Problem: {errType}");
            sw.WriteLine();
            sw.WriteLine($"Error Line:");
            sw.WriteLine($"----------------------------------------------------------");
            foreach (var l in lines)
            {
                sw.WriteLine(l);
            }
            sw.WriteLine($"----------------------------------------------------------");
            sw.Close();
            fs.Close();
        }

        private List<string> GetErrorLinesFromFile(string testFilePath, int lineNumber)
        {
            List<string> output = new List<string>();
            if (!File.Exists(testFilePath))
            {
                MessageBox.Show("File is missing!");
                return null;
            }
            FileStream fs = new FileStream(testFilePath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);

            for (int i = 0; i < lineNumber - 10; i++)
            {
                sr.ReadLine();
            }

            for (int i = 0; i < 20; i++)
            {
                var line = sr.ReadLine();
                if (i == 9)
                    line += "\t*";
                output.Add(line);
            }
            sr.Close();
            fs.Close();
            return output;
        }

        private string GetStringFromDescription(string description, string v)
        {
            string output = string.Empty;
            var a = description.Split('\n');
            foreach (var s in a)
            {
                var b = s.Split(':');
                if (b[0] == v)
                {
                    output = b[1].Trim();
                    break;
                }
            }
            return output;
        }
        #endregion // Public Interface
    }
}
