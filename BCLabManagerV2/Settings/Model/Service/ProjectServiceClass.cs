using BCLabManager.DataAccess;
using BCLabManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class ProjectServiceClass
    {
        public ObservableCollection<Project> Items { get; set; }

        public void SuperAdd(Project item)
        {
            DatabaseAdd(item);
            Items.Add(item);
            CreateFolder(item.BatteryType.Name, item.Name);
        }

        public void CreateFolder(string batteryType, string project)
        {
            CreateFolder(GlobalSettings.RootPath, batteryType, project);
            CreateFolder(GlobalSettings.TempraryFolder, batteryType, project);
        }

        private void CreateFolder(string rootPath, string batteryType, string project)
        {
            string testFilePath = $@"{rootPath}{batteryType}\{project}\{GlobalSettings.TestDataFolderName}";
            string evResultPath = $@"{rootPath}{batteryType}\{project}\{GlobalSettings.EvResultFolderName}";
            string productPath = $@"{rootPath}{batteryType}\{project}\{GlobalSettings.ProductFolderName}";
            string sourceFilePath = $@"{rootPath}{batteryType}\{project}\{GlobalSettings.SourceDataFolderName}";
            string headerPath = $@"{rootPath}{batteryType}\{project}\{GlobalSettings.HeaderFolderName}";

            if (!Directory.Exists(testFilePath))
                Directory.CreateDirectory(testFilePath);
            if (!Directory.Exists(evResultPath))
                Directory.CreateDirectory(evResultPath);
            if (!Directory.Exists(productPath))
                Directory.CreateDirectory(productPath);
            if (!Directory.Exists(sourceFilePath))
                Directory.CreateDirectory(sourceFilePath);
            if (!Directory.Exists(headerPath))
                Directory.CreateDirectory(headerPath);
        }

        public void DatabaseAdd(Project item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                item.BatteryType = uow.BatteryTypes.GetById(item.BatteryType.Id);
                uow.Projects.Insert(item);
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
                uow.Projects.Delete(id);
                uow.Commit();
            }
        }
        public void SuperUpdate(Project item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(Project item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Projects.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(Project item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.BatteryType = item.BatteryType;
            edittarget.Customer = item.Customer;
            edittarget.CutoffDischargeVoltage = item.CutoffDischargeVoltage;
            edittarget.Description = item.Description;
            edittarget.ProjectSettings = item.ProjectSettings;
            edittarget.LimitedChargeVoltage = item.LimitedChargeVoltage;
            //edittarget.TableMakerProducts = item.TableMakerProducts;
            edittarget.AbsoluteMaxCapacity = item.AbsoluteMaxCapacity;
            edittarget.VoltagePoints = item.VoltagePoints;
        }
    }
}
