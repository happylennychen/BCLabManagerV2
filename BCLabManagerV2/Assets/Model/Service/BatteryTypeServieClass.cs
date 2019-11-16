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
        public void SuperAdd(BatteryTypeClass item)
        {
            DatabaseAdd(item);
            Items.Add(item);
        }
        public void DatabaseAdd(BatteryTypeClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.BatteryTypes.Insert(item);
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
                uow.BatteryTypes.Delete(id);
                uow.Commit();
            }
        }
        public void SuperUpdate(BatteryTypeClass item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(BatteryTypeClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.BatteryTypes.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(BatteryTypeClass item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
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
