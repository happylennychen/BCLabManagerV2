using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class TesterServieClass//:ModelService<TesterClass>
    {
        public ObservableCollection<TesterClass> Items { get; set; }
        public void SuperAdd(TesterClass item)
        {
            DatabaseAdd(item);
            Items.Add(item);
        }
        public void DatabaseAdd(TesterClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Testers.Insert(item);
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
                uow.Testers.Delete(id);
                uow.Commit();
            }
        }
        public void SuperUpdate(TesterClass item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(TesterClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Testers.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(TesterClass item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.Manufactor = item.Manufactor;
            edittarget.Name = item.Name;
        }
    }
}
