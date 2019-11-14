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
        public void Add(TesterClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Testers.Insert(item);
                uow.Commit();
            }
            Items.Add(item);
        }
        public void Remove(int id)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Testers.Delete(id);
                uow.Commit();
            }

            var item = Items.SingleOrDefault(o => o.Id == id);
            Items.Remove(item);
        }
        public void Update(TesterClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Testers.Update(item);
                uow.Commit();
            }
            var edittarget = Items.SingleOrDefault(o=>o.Id == item.Id);
            edittarget.Manufactor = item.Manufactor;
            edittarget.Name = item.Name;
        }
    }
}
