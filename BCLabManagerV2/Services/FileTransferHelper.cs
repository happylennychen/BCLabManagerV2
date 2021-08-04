using BCLabManager.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BCLabManager
{
    static public class FileTransferHelper
    {
        public static void FileCopyWithRetry(string sourcePath, string targetPath)
        {
#if! Test
            //var thread = new Thread(() =>
            {
                int i;
                for (i = 0; i < 5; i++)
                {
                    if (FileCopyWithLog(sourcePath, targetPath))
                    {
                        break;
                    }
                    MessageBox.Show($"Copy to server failed! i = {i}");
                }
                if (i == 5)
                {
                    if (!File.Exists(targetPath))
                    {
                        Event evt = new Event();
                        evt.Module = Module.NAS;
                        evt.Timestamp = DateTime.Now;
                        evt.Type = EventType.Error;
                        evt.Description = $"Test File Missing!. File Name: {Path.GetFileName(sourcePath)}";
                        EventService.SuperAdd(evt);
                        MessageBox.Show(evt.Description);
                        return;
                    }

                    FileInfo fi1 = new FileInfo(sourcePath);
                    FileInfo fi2 = new FileInfo(targetPath);
                    if (fi1.Length != fi2.Length)
                    {
                        Event evt = new Event();
                        evt.Module = Module.NAS;
                        evt.Timestamp = DateTime.Now;
                        evt.Type = EventType.Error;
                        evt.Description = $"Original file length is {fi1.Length}B, server file length is {fi2.Length}B.";
                        EventService.SuperAdd(evt);
                        MessageBox.Show(evt.Description);
                    }
                }
            }//);
            //thread.Start();
#endif
        }

        public static bool FileCopyWithLog(string sourcePath, string targetPath)
        {
            File.Copy(sourcePath, targetPath, true);
            if (!File.Exists(targetPath))
            {
                return false;
            }

            FileInfo fi1 = new FileInfo(sourcePath);
            FileInfo fi2 = new FileInfo(targetPath);
            RuningLog.Write($"Source File: {sourcePath}, Size: {fi1.Length}, Target File: {targetPath}, Size: {fi2.Length}, Difference: {fi1.Length - fi2.Length}\n");
            if (fi1.Length != fi2.Length)
                return false;

            return true;
        }

        public static string FileCombine(List<string> rawDataFullPathList, string fileFullPath)   //默认按顺序导入
        {
            bool isFirst = true;
            foreach (var raw in rawDataFullPathList)
            {
                if (isFirst)
                {
                    isFirst = false;
                    //File.WriteAllText(filename, File.ReadAllText(raw.FilePath));
                    try
                    {
                        File.Copy(raw, fileFullPath, true);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                else
                {
                    var lines = File.ReadAllLines(raw).ToList();
                    lines.RemoveRange(0, 10);
                    File.AppendAllLines(fileFullPath, lines);
                }
            }
            return fileFullPath;
        }
    }
}
