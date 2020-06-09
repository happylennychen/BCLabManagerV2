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
        public ObservableCollection<ProgramType> Items { get; set; }

        public void SuperAdd(ProgramType item)
        {
            DatabaseAdd(item);
            Items.Add(item);
        }

        public void DatabaseAdd(ProgramType item)
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
        public void SuperUpdate(ProgramType item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(ProgramType item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.ProgramTypes.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(ProgramType item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.Name = item.Name;
            edittarget.Description = item.Description;
        }
    }
}
