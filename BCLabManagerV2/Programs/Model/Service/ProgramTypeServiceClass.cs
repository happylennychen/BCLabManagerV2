using BCLabManager.DataAccess;
using BCLabManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class ProgramTypeServiceClass
    {
        public ObservableCollection<ProgramTypeClass> Items { get; set; }

        public void SuperAdd(ProgramTypeClass item)
        {
            DatabaseAdd(item);
            Items.Add(item);
        }

        public void DatabaseAdd(ProgramTypeClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.ProgramTypes.Insert(item);
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
                uow.ProgramTypes.Delete(id);
                uow.Commit();
            }
        }
        public void SuperUpdate(ProgramTypeClass item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(ProgramTypeClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.ProgramTypes.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(ProgramTypeClass item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.Name = item.Name;
            edittarget.Description = item.Description;
        }
    }
}
