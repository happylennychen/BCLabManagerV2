using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BCLabManager.Model
{
    public class TableMakerProductServiceClass
    {
        public ObservableCollection<TableMakerProduct> Items { get; set; }

        public void SuperAdd(TableMakerProduct item)
        {
            //FileOperation(item);
            DatabaseAdd(item);
            Items.Add(item);
        }

        public void DatabaseAdd(TableMakerProduct item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                item.Type = uow.TableMakerProductTypes.GetById(item.Type.Id);
                uow.TableMakerProducts.Insert(item);
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
                uow.TableMakerProducts.Delete(id);
                uow.Commit();
            }
        }
        public void SuperUpdate(TableMakerProduct item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(TableMakerProduct item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.TableMakerProducts.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(TableMakerProduct item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.Type = item.Type;
            edittarget.FilePath = item.FilePath;
            edittarget.IsValid = item.IsValid;
        }
    }
}
