using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public static class EventService
    {
        public static ObservableCollection<Event> Items { get; set; }
        public static void SuperAdd(Event item)
        {
            DatabaseAdd(item);
            Items.Add(item);
        }
        public static void DatabaseAdd(Event item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Events.Insert(item);
                uow.Commit();
            }
        }
        public static void SuperRemove(int id)
        {
            DatabaseRemove(id);
            var item = Items.SingleOrDefault(o => o.Id == id);
            Items.Remove(item);
        }
        public static void DatabaseRemove(int id)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Events.Delete(id);
                uow.Commit();
            }
        }
        public static void SuperUpdate(Event item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public static void DatabaseUpdate(Event item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Events.Update(item);
                uow.Commit();
            }
        }
        public static void DomainUpdate(Event item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.Module = item.Module;
            edittarget.Type = item.Type;
            edittarget.Description = item.Description;
            edittarget.Timestamp = item.Timestamp;
        }
    }
}
