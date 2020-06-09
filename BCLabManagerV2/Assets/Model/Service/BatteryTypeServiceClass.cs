using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class BatteryTypeServiceClass//:ModelService<BatteryTypeClass>
    {
        public ObservableCollection<BatteryType> Items { get; set; }
        public void SuperAdd(BatteryType item)
        {
            DatabaseAdd(item);
            Items.Add(item);
        }
        public void DatabaseAdd(BatteryType item)
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
        public void SuperUpdate(BatteryType item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(BatteryType item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.BatteryTypes.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(BatteryType item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.LimitedChargeVoltage = item.LimitedChargeVoltage;
            edittarget.Manufacturer = item.Manufacturer;
            edittarget.Material = item.Material;
            edittarget.Name = item.Name;
            edittarget.NominalVoltage = item.NominalVoltage;
            edittarget.RatedCapacity = item.RatedCapacity;
            edittarget.TypicalCapacity = item.TypicalCapacity;
        }
    }
}
