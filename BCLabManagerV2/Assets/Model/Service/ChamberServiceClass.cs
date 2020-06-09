using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class ChamberServiceClass
    {
        public ObservableCollection<Chamber> Items { get; set; }
        public void SuperAdd(Chamber item)
        {
            DatabaseAdd(item);
            Items.Add(item);
        }
        public void DatabaseAdd(Chamber item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Chambers.Insert(item);
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
                uow.Chambers.Delete(id);
                uow.Commit();
            }
        }
        public void SuperUpdate(Chamber item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(Chamber item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Chambers.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(Chamber item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.HighestTemperature = item.HighestTemperature;
            edittarget.LowestTemperature = item.LowestTemperature;
            edittarget.Manufacturer = item.Manufacturer;
            edittarget.Name = item.Name;
            edittarget.AssetUseCount = item.AssetUseCount;
            edittarget.Records = item.Records;
        }
        public void Execute(Chamber item, DateTime startTime, string programName, string recipeName)
        {
            item.AssetUseCount++;
            item.Records.Add(new AssetUsageRecord(startTime, item.AssetUseCount, programName, recipeName));

            SuperUpdate(item);
        }
        public void Commit(Chamber item, DateTime endTime, string programName, string recipeName)
        {
            item.AssetUseCount--;
            item.Records.Add(new AssetUsageRecord(endTime, item.AssetUseCount, programName, recipeName));

            SuperUpdate(item);
        }
    }
}
