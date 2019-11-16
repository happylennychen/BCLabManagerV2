using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class ChamberServieClass
    {
        public ObservableCollection<ChamberClass> Items { get; set; }
        public void SuperAdd(ChamberClass item)
        {
            DatabaseAdd(item);
            Items.Add(item);
        }
        public void DatabaseAdd(ChamberClass item)
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
        public void SuperUpdate(ChamberClass item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(ChamberClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Chambers.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(ChamberClass item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.HighestTemperature = item.HighestTemperature;
            edittarget.LowestTemperature = item.LowestTemperature;
            edittarget.Manufactor = item.Manufactor;
            edittarget.Name = item.Name;
            edittarget.AssetUseCount = item.AssetUseCount;
            edittarget.Records = item.Records;
        }
        public void Execute(ChamberClass item, DateTime startTime, string programName, string recipeName)
        {
            item.AssetUseCount++;
            item.Records.Add(new AssetUsageRecordClass(startTime, item.AssetUseCount, programName, recipeName));

            SuperUpdate(item);
        }
        public void Commit(ChamberClass item, DateTime endTime, string programName, string recipeName)
        {
            item.AssetUseCount--;
            item.Records.Add(new AssetUsageRecordClass(endTime, item.AssetUseCount, programName, recipeName));

            SuperUpdate(item);
        }
    }
}
