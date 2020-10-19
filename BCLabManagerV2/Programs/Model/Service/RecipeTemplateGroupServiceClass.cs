using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class RecipeTemplateGroupServiceClass
    {
        public ObservableCollection<RecipeTemplateGroup> Items { get; set; }
        //public StepServiceClass StepService { get; set; } = new StepServiceClass();
        public void SuperAdd(RecipeTemplateGroup item)
        {
            DatabaseAdd(item);
            DomainAdd(item);
        }
        public void DatabaseAdd(RecipeTemplateGroup item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.RecipeTemplateGroups.Insert(item);
                uow.Commit();
            }
        }
        public void DomainAdd(RecipeTemplateGroup item)
        {
            Items.Add(item);
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
                uow.RecipeTemplateGroups.Delete(id);
                uow.Commit();
            }
        }
        public void SuperUpdate(RecipeTemplateGroup item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(RecipeTemplateGroup item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.RecipeTemplateGroups.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(RecipeTemplateGroup item)
        {
            //var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            //edittarget.BatteryType = item.BatteryType;
            //edittarget.Name = item.Name;
            //edittarget.CycleCount = item.CycleCount;
            //edittarget.AssetUseCount = item.AssetUseCount;
            //edittarget.Records = item.Records;
        }
    }
}
