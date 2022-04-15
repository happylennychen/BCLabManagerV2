using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BCLabManager.Model
{
    public class TableMakerRecordServiceClass
    {
        public ObservableCollection<TableMakerRecord> Items { get; set; }

        public void SuperAdd(TableMakerRecord item)
        {
            //FileOperation(item);
            DatabaseAdd(item);
            Items.Add(item);
        }

        //public void FileOperation(TableMakerRecord item)
        //{
        //    string root = $@"{GlobalSettings.RemotePath}{item.Project.BatteryType.Name}\{item.Project.Name}";
        //    string temproot = $@"{GlobalSettings.LocalPath}{item.Project.BatteryType.Name}\{item.Project.Name}";
        //    string temptestfilepath = string.Empty;
        //    temptestfilepath = CopyToFolder(item.FilePath, temproot);
        //    item.FilePath = $@"{root}\{GlobalSettings.ProductFolderName}\{Path.GetFileName(temptestfilepath)}";
        //    CopyToServer(temptestfilepath, item.FilePath);
        //}


        //private void CopyToServer(string tempPath, string serverPath)
        //{
        //    var thread = new Thread(() => {
        //        File.Copy(tempPath, serverPath, true);

        //        if (!File.Exists(serverPath))
        //        {
        //            MessageBox.Show("Table Maker Product File Missing!");
        //        }
        //    });
        //    thread.Start();
        //}

        //private string CopyToFolder(string filepath, string root)
        //{
        //    var newPath = Path.Combine($@"{root}\{GlobalSettings.ProductFolderName}", Path.GetFileName(filepath));
        //    //var tempPath = Path.Combine($@"{GlobalSettings.LocalPath}{GlobalSettings.ProductFolderName}", Path.GetFileName(filepath));
        //    //File.Copy(filepath, tempPath, true);
        //    File.Copy(filepath, newPath, true);
        //    return newPath;
        //}
        public void DatabaseAdd(TableMakerRecord item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.TableMakerRecords.Insert(item);
                uow.Commit();
            }
        }
        public void SuperRemove(int id)
        {
            DatabaseRemove(id);

            var item = Items.SingleOrDefault(o => o.Id == id);
            Items.Remove(item);
        }
        public void DatabaseRemove(int id)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.TableMakerProducts.Delete(id);
                uow.Commit();
            }
        }
        public void SuperUpdate(TableMakerRecord item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(TableMakerRecord item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.TableMakerRecords.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(TableMakerRecord item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.EOD = item.EOD;
            edittarget.Description = item.Description;
            edittarget.TableMakerVersion = item.TableMakerVersion;
            edittarget.IsValid = item.IsValid;
            edittarget.Products = item.Products;
            edittarget.Project = item.Project;
            edittarget.OCVSources = item.OCVSources;
            edittarget.RCSources = item.RCSources;
            edittarget.VoltagePoints = item.VoltagePoints;
        }
    }
}
