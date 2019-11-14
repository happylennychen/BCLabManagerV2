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
        public void Add(ChamberClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Chambers.Insert(item);
                uow.Commit();
            }
            Items.Add(item);
        }
        public void Remove(int id)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Chambers.Delete(id);
                uow.Commit();
            }

            var item = Items.SingleOrDefault(o => o.Id == id);
            Items.Remove(item);
        }
        public void Update(ChamberClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Chambers.Update(item);
                uow.Commit();
            }
            var edittarget = Items.SingleOrDefault(o=>o.Id == item.Id);
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

            Update(item);
        }
        public void Commit(ChamberClass item, DateTime endTime, string programName, string recipeName)
        {
            item.AssetUseCount--;
            item.Records.Add(new AssetUsageRecordClass(endTime, item.AssetUseCount, programName, recipeName));

            Update(item);
        }
    }
}
