using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BCLabManager.Model
{
    public class StepTemplateServiceClass
    {
        public ObservableCollection<StepTemplate> Items { get; set; }
        public void SuperAdd(StepTemplate item)
        {
            DatabaseAdd(item);
            DomainAdd(item);
        }
        public void DatabaseAdd(StepTemplate item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                if (!uow.StepTemplates.GetAll().Any(o => o.CurrentInput == item.CurrentInput && o.CurrentUnit == item.CurrentUnit && o.CutOffConditionValue == item.CutOffConditionValue && o.CutOffConditionType == item.CutOffConditionType))
                {
                    uow.StepTemplates.Insert(item);
                    uow.Commit();
                }
                else
                {
                    MessageBox.Show("Already existed.");
                }
            }
        }
        public void DomainAdd(StepTemplate item)
        {
            if(!Items.Any(o => o.CurrentInput == item.CurrentInput && o.CurrentUnit == item.CurrentUnit && o.CutOffConditionValue == item.CutOffConditionValue && o.CutOffConditionType == item.CutOffConditionType))
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
                uow.StepTemplates.Delete(id);
                uow.Commit();
            }
        }
        public void SuperUpdate(StepTemplate item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(StepTemplate item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.StepTemplates.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(StepTemplate item)
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
