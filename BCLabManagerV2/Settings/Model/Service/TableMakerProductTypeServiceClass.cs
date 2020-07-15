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
    public class TableMakerProductTypeServiceClass
    {
        public ObservableCollection<TableMakerProductType> Items { get; set; }

        public void SuperAdd(TableMakerProductType item)
        {
            DatabaseAdd(item);
            Items.Add(item);
        }

        public void DatabaseAdd(TableMakerProductType item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.TableMakerProductTypes.Insert(item);
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
                uow.TableMakerProductTypes.Delete(id);
                uow.Commit();
            }
        }
        public void SuperUpdate(TableMakerProductType item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(TableMakerProductType item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.TableMakerProductTypes.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(TableMakerProductType item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.Description = item.Description;
        }
    }
}
