﻿//#define Test
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
            if (!FileCopy(sourcePath, targetPath))
            {
                MessageBox.Show($"File copy failed!");
            }
            string sourceMD5, targetMD5;
            sourceMD5 = GetMD5(sourcePath);
            targetMD5 = GetMD5(targetPath);
            if (sourceMD5 != targetMD5)
            {
                Event evt = new Event();
                evt.Module = Module.FileOperation;
                evt.Timestamp = DateTime.Now;
                evt.Type = EventType.Error;
                evt.Description = $"Test File MD5 Check Failed!. File Name: {Path.GetFileName(sourcePath)}";
                EventService.SuperAdd(evt);
                MessageBox.Show(evt.Description);
                return string.Empty;
            }
            return sourceMD5;
#else
            return string.Empty;
#endif
        }

        public static string GetMD5(string path)
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

        public static bool FileCopy(string sourcePath, string targetPath)
        {
            try
            {
                File.Copy(sourcePath, targetPath, true);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

            return true;
        }

        public static bool CheckFileMD5(string filePath, string MD5)
        {
            return MD5 == GetMD5(filePath);
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

        public static bool FileDownload(string remotePath, string MD5)
        {
            var localPath = FileTransferHelper.Remote2Local(remotePath);
            var universalPath = FileTransferHelper.Remote2Universal(remotePath);
            if (!File.Exists(localPath))
            {
                if (!File.Exists(universalPath))
                {
                    MessageBox.Show($"No such file.{remotePath}");
                    Event evt = new Event();
                    evt.Module = Module.FileOperation;
                    evt.Timestamp = DateTime.Now;
                    evt.Type = EventType.Error;
                    evt.Description = $"Cannot access file {remotePath}.";
                    EventService.SuperAdd(evt);
                    return false;
                }
                FileTransferHelper.FileCopyWithMD5Check(universalPath, localPath);
            }
            //}
            if (MD5 != null && MD5 != string.Empty)
                if (!FileTransferHelper.CheckFileMD5(localPath, MD5))
                {
                    MessageBox.Show($"{remotePath} MD5 Check Failed!");
                    Event evt = new Event();
                    evt.Module = Module.FileOperation;
                    evt.Timestamp = DateTime.Now;
                    evt.Type = EventType.Error;
                    evt.Description = $"{remotePath} MD5 Check Failed!";
                    EventService.SuperAdd(evt);
                    return false;
                }
            return true;
        }

        public static bool FileRestore(string remotePath, string MD5)
        {
            var localPath = FileTransferHelper.Remote2Local(remotePath);
            var universalPath = FileTransferHelper.Remote2Universal(remotePath);
            if (!File.Exists(universalPath))
            {
                if (!File.Exists(localPath))
                {
                    MessageBox.Show($"No such file.{remotePath}");
                    Event evt = new Event();
                    evt.Module = Module.FileOperation;
                    evt.Timestamp = DateTime.Now;
                    evt.Type = EventType.Error;
                    evt.Description = $"Cannot access file {remotePath}.";
                    EventService.SuperAdd(evt);
                    return false;
                }
                FileTransferHelper.FileCopyWithMD5Check(localPath, universalPath);
            }
            //}
            if (MD5 != null && MD5 != string.Empty)
                if (!FileTransferHelper.CheckFileMD5(universalPath, MD5))
                {
                    MessageBox.Show($"{remotePath} MD5 Check Failed!");
                    Event evt = new Event();
                    evt.Module = Module.FileOperation;
                    evt.Timestamp = DateTime.Now;
                    evt.Type = EventType.Error;
                    evt.Description = $"{remotePath} MD5 Check Failed!";
                    EventService.SuperAdd(evt);
                    return false;
                }
            return true;
        }

        public static string Local2Remote(string path)
        {
            if (path.Contains(GlobalSettings.LocalFolder))
                return path.Replace(GlobalSettings.LocalFolder, GlobalSettings.RemotePath);
            else return path;
        }

        public static string Remote2Local(string path)
        {
            if (path.Contains(GlobalSettings.RemotePath))
                return path.Replace(GlobalSettings.RemotePath, GlobalSettings.LocalFolder);
            else return path;
        }
        public static string Mapping2Remote(string path)
        {
            if (path.Contains(GlobalSettings.MappingPath))
                return path.Replace(GlobalSettings.MappingPath, GlobalSettings.RemotePath);
            else return path;
        }
        public static string Remote2Mapping(string path)
        {
            if (path.Contains(GlobalSettings.RemotePath))
                return path.Replace(GlobalSettings.RemotePath, GlobalSettings.MappingPath);
            else return path;
        }
        public static string Remote2Universal(string path)
        {
            string output = path;
            if (GlobalSettings.EnableTest)
            {
                if (path.Contains(GlobalSettings.RemotePath))
                    output = path.Replace(GlobalSettings.RemotePath, GlobalSettings.MappingPath);
            }
            return output;
        }
        public static string Local2Universal(string path)
        {
            if (path.Contains(GlobalSettings.LocalFolder))
                return path.Replace(GlobalSettings.LocalFolder, GlobalSettings.UniversalPath);
            else return path;
        }
        public static string Universal2Local(string path)
        {
            string output = path;
            if (GlobalSettings.EnableTest)
            {
                if (path.Contains(GlobalSettings.MappingPath))
                    output = path.Replace(GlobalSettings.MappingPath, GlobalSettings.LocalFolder);
            }
            else
            {
                if (path.Contains(GlobalSettings.RemotePath))
                    output = path.Replace(GlobalSettings.RemotePath, GlobalSettings.LocalFolder);
            }
            return output;
        }
        /*public static string GetRemotePath(string path, int level)
        {
            int index = FindNthCharInString(path, '\\', level);
            var substring = path.Substring(0, index + 1);
            return path.Replace(substring, GlobalSettings.UniversalPath);
        }

        public static string GetLocalPath(string path, int level)       //替换从末尾算起第level层的斜杠左边的路径
        {
            int index = FindNthCharInString(path, '\\', level);
            var substring = path.Substring(0, index + 1);
            return path.Replace(substring, GlobalSettings.LocalFolder);
        }
        private static int FindNthCharInString(string str, char v, int count)   //找到str中第count个v对应的index
        {
            int index = str.Length - 1;
            for (int i = 0; i < count; i++)
                index = str.LastIndexOf(v, index - 1);
            return index;
        }
        
        public static string GetLocalPath(string path)       //不带level，需要根据路径中的特性找到对应的level
        {
            int level = GetLevel(path);
            int index = FindNthCharInString(path, '\\', level);
            var substring = path.Substring(0, index + 1);
            return path.Replace(substring, GlobalSettings.LocalFolder);
        }

        public static string GetRemotePath(string path)
        {
            int level = GetLevel(path);
            int index = FindNthCharInString(path, '\\', level);
            var substring = path.Substring(0, index + 1);
            return path.Replace(substring, GlobalSettings.UniversalPath);
        }

        public static int GetLevel(string path)
        {
            int level = -1;
            if (path.Contains(GlobalSettings.LocalFolder))
            {
                level = GetRelativeLevel(path, GlobalSettings.LocalFolder);
            }
            else if (path.Contains(GlobalSettings.UniversalPath))
            {
                level = GetRelativeLevel(path, GlobalSettings.UniversalPath);
            }
            else if (path.Contains(GlobalSettings.TestDataFolderName))
            {
                level = GetRelativeLevel(path, GlobalSettings.TestDataFolderName);
                level += 2;
            }
            else if (path.Contains(GlobalSettings.ProductFolderName))
            {
                level = GetRelativeLevel(path, GlobalSettings.ProductFolderName);
                level += 2;
            }
            return level;
        }

        public static int GetRelativeLevel(string path, string str)
        {
            var index = path.IndexOf(str);
            var sub = path.Substring(index+str.Length);
            var level = sub.Count(x => x == '\\');
            return level;
        }
        */
        public static void FileUpload(string localPath, out string remotePath, out string MD5)
        {
            remotePath = FileTransferHelper.Local2Universal(localPath);
            MD5 = FileTransferHelper.FileCopyWithMD5Check(localPath, remotePath);
            remotePath = FileTransferHelper.Mapping2Remote(remotePath);
        }
    }
}
