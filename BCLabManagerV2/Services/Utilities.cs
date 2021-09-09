using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BCLabManager
{
    static public class Utilities
    {
        public static List<Int32> LoadVCFGFile(string fullpath)
        {
            List<Int32> output = new List<Int32>();
            try
            {
                FileStream file = new FileStream(fullpath, FileMode.Open);
                StreamReader sr = new StreamReader(file);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    output.Add(Convert.ToInt32(line));
                }
                sr.Close();
                file.Close();
                output.Sort();
            }
            catch(Exception e)
            {
                MessageBox.Show("Load voltage configuration file failed!\n" +
                    $"{e.Message}");
            }
            return output;
        }
    }
}
