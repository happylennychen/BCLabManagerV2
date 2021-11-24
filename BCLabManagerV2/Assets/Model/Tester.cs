using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class Tester : BindableBase
    {
        public int Id { get; set; }
        //public String Manufacturer { get; set; }
        //public String Name { get; set; }
        private string _manufacturer;
        public string Manufacturer
        {
            get { return _manufacturer; }
            set { SetProperty(ref _manufacturer, value); }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        [NotMapped]
        public ITesterProcesser ITesterProcesser{get;set;}
        public Tester()
        { }

        //public Tester(String Manufacturer, String Name)
        //{
        //    //this.TesterID = NextID;
        //    this.Manufacturer = Manufacturer;
        //    this.Name = Name;
        //}

        //public void Update(String Manufacturer, String Name)
        //{
        //    this.Manufacturer = Manufacturer;
        //    this.Name = Name;
        //}

        public override string ToString()
        {
            return this.Name;
        }

        //public DateTime[] GetTimeFromRawData(ObservableCollection<string> fileList)
        //{
        //    DateTime[] output = new DateTime[2];
        //    List<DateTime> StartTimes = new List<DateTime>();
        //    List<DateTime> EndTimes = new List<DateTime>();
        //    foreach (var fn in fileList)
        //    {
        //        DateTime[] timepair = GetTimesFromFile(fn);
        //        StartTimes.Add(timepair[0]);
        //        EndTimes.Add(timepair[1]);
        //    }
        //    output[0] = GetEarliest(StartTimes);
        //    output[1] = GetLatest(EndTimes);
        //    return output;
        //}

        //private DateTime[] GetTimesFromFile(string fn)
        //{
        //    DateTime[] output = new DateTime[2];
        //    FileStream fs = new FileStream(fn, FileMode.Open);
        //    StreamReader sw = new StreamReader(fs);
        //    sw.ReadLine();
        //    sw.ReadLine();
        //    string startTimeLine = sw.ReadLine();
        //    string startTimeStr = startTimeLine.Substring(17, 19);
        //    output[0] = DateTime.Parse(startTimeStr);
        //    string endTimeLine = sw.ReadLine();
        //    string endTimeStr = endTimeLine.Substring(15, 19);
        //    output[1] = DateTime.Parse(endTimeStr);
        //    sw.Close();
        //    fs.Close();
        //    return output;
        //}

        //private DateTime GetEarliest(List<DateTime> startTimes)
        //{
        //    return startTimes.Min();
        //}

        //private DateTime GetLatest(List<DateTime> endTimes)
        //{
        //    return endTimes.Max();
        //}

        internal void BuildProcesser()  //根据 Manufacturer 和 Name，来创建TestProcesser
        {
            switch(Manufacturer+Name)
            {
                case "Chroma17200":
                    ITesterProcesser = new Chroma17200Processer();
                    break;
                case "ZKEEBC-X":
                    ITesterProcesser = new ZKEEBC_X_Processer();
                    break;
                case "Chroma17208":
                    ITesterProcesser = new Chroma17208Processer();
                    break;
                case "Chroma17216":
                    ITesterProcesser = new Chroma17208Processer();
                    break;
                default:
                    ITesterProcesser = new PseudoProcesser();
                    break;
            }
        }
    }
}
