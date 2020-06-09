using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class BatteryServiceClass
    {
        public ObservableCollection<Battery> Items { get; set; }
        public void SuperAdd(Battery item)
        {
            DatabaseAdd(item);
            Items.Add(item);
        }

        public void DatabaseAdd(Battery item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                item.BatteryType = uow.BatteryTypes.GetById(item.BatteryType.Id);
                uow.Batteries.Insert(item);
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
                uow.Batteries.Delete(id);
                uow.Commit();
            }
        }
        public void SuperUpdate(Battery item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(Battery item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Batteries.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(Battery item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.BatteryType = item.BatteryType;
            edittarget.Name = item.Name;
            edittarget.CycleCount = item.CycleCount;
            edittarget.AssetUseCount = item.AssetUseCount;
            edittarget.Records = item.Records;
        }
        public void Execute(Battery item, DateTime startTime, string programName, string recipeName)
        {
            item.AssetUseCount++;
            item.Records.Add(new AssetUsageRecord(startTime, item.AssetUseCount, programName, recipeName));

            SuperUpdate(item);
        }
        public void Commit(Battery item, DateTime endTime, string programName, string recipeName, double newCycleCount)
        {
            item.AssetUseCount--;
            item.Records.Add(new AssetUsageRecord(endTime, item.AssetUseCount, programName, recipeName));
            item.CycleCount += newCycleCount;

            SuperUpdate(item);
        }
    }
}
