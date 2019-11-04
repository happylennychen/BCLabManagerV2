using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class BatteryTypeServieClass//:ModelService<BatteryTypeClass>
    {
        public ObservableCollection<BatteryTypeClass> Items { get; set; }
        public void Add(BatteryTypeClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.BatteryTypes.Insert(item);
                uow.Commit();
            }
            Items.Add(item);
        }
        public void Remove(int id)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.BatteryTypes.Delete(id);
                uow.Commit();
            }

            var item = Items.SingleOrDefault(o => o.Id == id);
            Items.Remove(item);
        }
        public void Update(BatteryTypeClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.BatteryTypes.Update(item);
                uow.Commit();
            }
            var edittarget = Items.SingleOrDefault(o=>o.Id == item.Id);
            edittarget.LimitedChargeVoltage = item.LimitedChargeVoltage;
            edittarget.Manufactor = item.Manufactor;
            edittarget.Material = item.Material;
            edittarget.Name = item.Name;
            edittarget.NominalVoltage = item.NominalVoltage;
            edittarget.RatedCapacity = item.RatedCapacity;
            edittarget.TypicalCapacity = item.TypicalCapacity;
        }
    }
}
