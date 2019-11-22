using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class RecipeTemplateServiceClass
    {
        public ObservableCollection<RecipeTemplate> Items { get; set; }
        public StepTemplateServiceClass StepTemplateService { get; set; } = new StepTemplateServiceClass();
        public void SuperAdd(RecipeTemplate item)
        {
            DatabaseAdd(item);
            DomainAdd(item);
        }
        public void DatabaseAdd(RecipeTemplate item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                foreach (var step in item.Steps)
                    step.StepTemplate = uow.StepTemplates.GetById(step.StepTemplate.Id);
                uow.RecipeTemplates.Insert(item);
                uow.Commit();
            }
        }
        public void DomainAdd(RecipeTemplate item)
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
                uow.RecipeTemplates.Delete(id);
                uow.Commit();
            }
        }
        public void SuperUpdate(RecipeTemplate item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(RecipeTemplate item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.RecipeTemplates.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(RecipeTemplate item)
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
