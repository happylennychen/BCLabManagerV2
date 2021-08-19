#define Test
using BCLabManager.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BCLabManager
{
    static public class FileTransferHelper
    {
        public static string FileCopyWithMD5Check(string sourcePath, string targetPath)
        {
#if !Test
            if (!FileCopyWithLog(sourcePath, targetPath))
            {
                MessageBox.Show($"Copy to server failed!");
            }
            string localMD5Code, remoteMD5Code;
            localMD5Code = GetMD5(sourcePath);
            remoteMD5Code = GetMD5(targetPath);
            if (localMD5Code != remoteMD5Code)
            {
                Event evt = new Event();
                evt.Module = Module.NAS;
                evt.Timestamp = DateTime.Now;
                evt.Type = EventType.Error;
                evt.Description = $"Test File MD5 Check Failed!. File Name: {Path.GetFileName(sourcePath)}";
                EventService.SuperAdd(evt);
                MessageBox.Show(evt.Description);
                return string.Empty;
            }
            return localMD5Code;
#else
            return string.Empty;
#endif
        }

        private static string GetMD5(string path)
        {
            string output = string.Empty;
            FileStream fs = new FileStream(path, FileMode.Open);

            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(fs);

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                output = sBuilder.ToString();
            }
            fs.Close();
            return output;
        }

        public static bool FileCopyWithLog(string sourcePath, string targetPath)
        {
            try
            {
                if (!File.Exists(sourcePath))
                    return false;
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
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

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

        public static string GetRemotePath(string path, int level)
        {
            int index = FindNthCharInString(path, '\\', level);
            var substring = path.Substring(0, index + 1);
            return path.Replace(substring, GlobalSettings.RemotePath);
        }

        public static string GetLocalPath(string path, int level)
        {
            int index = FindNthCharInString(path, '\\', level);
            var substring = path.Substring(0, index + 1);
            return path.Replace(substring, GlobalSettings.LocalFolder);
        }
        private static int FindNthCharInString(string str, char v, int count)
        {
            int index = str.Length - 1;
            for (int i = 0; i < count; i++)
                index = str.LastIndexOf(v, index - 1);
            return index;
        }
    }
}
