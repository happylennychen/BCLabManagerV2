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

        private void CreateFolder(string batteryType, string project)
        {
            string rawDataPath = $@"Q:\807\Software\WH BC Lab\Data\{batteryType}\{project}\Raw Data";
            string testFilePath = $@"Q:\807\Software\WH BC Lab\Data\{batteryType}\{project}\Test Data";
            string evResultPath = $@"Q:\807\Software\WH BC Lab\Data\{batteryType}\{project}\Ev Result";
            string productPath = $@"Q:\807\Software\WH BC Lab\Data\{batteryType}\{project}\Product";
            string sourceFilePath = $@"Q:\807\Software\WH BC Lab\Data\{batteryType}\{project}\Source Data";
            string headerPath = $@"Q:\807\Software\WH BC Lab\Data\{batteryType}\{project}\Header";

            if (!Directory.Exists(rawDataPath))
                Directory.CreateDirectory(rawDataPath);
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
            edittarget.RatedCapacity = item.RatedCapacity;
            edittarget.VoltagePoints = item.VoltagePoints;
        }
    }
}
