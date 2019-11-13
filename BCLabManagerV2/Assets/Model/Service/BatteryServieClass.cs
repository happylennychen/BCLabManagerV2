using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class BatteryServieClass
    {
        public ObservableCollection<BatteryClass> Items { get; set; }
        public void Add(BatteryClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                item.BatteryType = uow.BatteryTypes.GetById(item.BatteryType.Id);
                uow.Batteries.Insert(item);
                uow.Commit();
            }
            Items.Add(item);
        }
        public void Remove(int id)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Batteries.Delete(id);
                uow.Commit();
            }

            var item = Items.SingleOrDefault(o => o.Id == id);
            Items.Remove(item);
        }
        public void Update(BatteryClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Batteries.Update(item);
                uow.Commit();
            }
            var edittarget = Items.SingleOrDefault(o=>o.Id == item.Id);
            edittarget.BatteryType = item.BatteryType;
            edittarget.Name = item.Name;
            edittarget.CycleCount = item.CycleCount;
            edittarget.AssetUseCount = item.AssetUseCount;
            edittarget.Records = item.Records;
        }
        public void Execute(BatteryClass item, DateTime startTime, string programName, string recipeName)
        {
            item.AssetUseCount++;
            item.Records.Add(new AssetUsageRecordClass(startTime, item.AssetUseCount, programName, recipeName));

            Update(item);
        }
    }
}
