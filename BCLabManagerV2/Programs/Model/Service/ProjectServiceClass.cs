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
        public ObservableCollection<ProjectClass> Items { get; set; }

        public void SuperAdd(ProjectClass item)
        {
            DatabaseAdd(item);
            Items.Add(item);
            CreateFolder(item.BatteryType.Name, item.Name);
        }

        public void CreateFolder(string batteryType, string project)
        {
            string testFilePath = $@"{GlobalSettings.RootPath}{batteryType}\{project}\{GlobalSettings.TestDataFolderName}";
            string evResultPath = $@"{GlobalSettings.RootPath}{batteryType}\{project}\{GlobalSettings.EvResultFolderName}";
            string productPath = $@"{GlobalSettings.RootPath}{batteryType}\{project}\{GlobalSettings.ProductFolderName}";
            string sourceFilePath = $@"{GlobalSettings.RootPath}{batteryType}\{project}\{GlobalSettings.SourceDataFolderName}";
            string headerPath = $@"{GlobalSettings.RootPath}{batteryType}\{project}\{GlobalSettings.HeaderFolderName}";

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

        public void DatabaseAdd(ProjectClass item)
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
        public void SuperUpdate(ProjectClass item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(ProjectClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Projects.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(ProjectClass item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.BatteryType = item.BatteryType;
            edittarget.Customer = item.Customer;
            edittarget.CutoffDischargeVoltage = item.CutoffDischargeVoltage;
            edittarget.Description = item.Description;
            edittarget.EvSettings = item.EvSettings;
            edittarget.LimitedChargeVoltage = item.LimitedChargeVoltage;
            edittarget.ProjectProducts = item.ProjectProducts;
            edittarget.AbsoluteMaxCapacity = item.AbsoluteMaxCapacity;
            edittarget.VoltagePoints = item.VoltagePoints;
        }
    }
}
