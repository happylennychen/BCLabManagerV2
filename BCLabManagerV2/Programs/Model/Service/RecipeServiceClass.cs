using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class RecipeServiceClass
    {
        public ObservableCollection<RecipeClass> Items { get; set; }
        public TestRecordServiceClass TestRecordService { get; set; } = new TestRecordServiceClass();
        public StepRuntimeServiceClass StepRuntimeService { get; set; } = new StepRuntimeServiceClass();
        //public void Add(RecipeClass item)
        //{
        //using (var uow = new UnitOfWork(new AppDbContext()))
        //{
        //    item.BatteryType = uow.BatteryTypes.GetById(item.BatteryType.Id);
        //    uow.Batteries.Insert(item);
        //    uow.Commit();
        //}
        //Items.Add(item);
        //}
        //public void Remove(int id)
        //{
        //using (var uow = new UnitOfWork(new AppDbContext()))
        //{
        //    uow.Batteries.Delete(id);
        //    uow.Commit();
        //}

        //var item = Items.SingleOrDefault(o => o.Id == id);
        //Items.Remove(item);
        //}
        public void SuperUpdate(RecipeClass item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(RecipeClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Recipies.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(RecipeClass item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.EndTime = item.EndTime;
            edittarget.IsAbandoned = item.IsAbandoned;
            edittarget.StartTime = item.StartTime;
            edittarget.TestRecords = item.TestRecords;
            edittarget.Name = item.Name;
        }

        internal void Invalidate(RecipeClass recipe, TestRecordClass testRecord, string comment)
        {
            TestRecordService.Invalidate(testRecord, comment);
            var newTestRecord = new TestRecordClass();
            //TestRecordService.Add(newTestRecord);
            recipe.TestRecords.Add(newTestRecord);
            SuperUpdate(recipe);
        }

        internal void Abandon(RecipeClass recipe)
        {
            recipe.IsAbandoned = true;
            foreach (var testRecord in recipe.TestRecords)
            {
                TestRecordService.Abandon(testRecord);
            }
        }

        internal void UpdateEstimatedTime(RecipeClass item, ref DateTime time, ref double c)
        {
            if (item.StartTime == DateTime.MinValue)
            {
                item.EST = time;
                foreach (var sr in item.StepRuntimes)
                    StepRuntimeService.UpdateEstimatedTime(sr, ref time, ref c);
                item.EET = time;
            }
            else
            {
                time = item.StartTime;
                if (item.EndTime == DateTime.MinValue)
                {
                    foreach (var sr in item.StepRuntimes)
                        StepRuntimeService.UpdateEstimatedTime(sr, ref time, ref c);
                    item.EET = time;
                }
                else
                {
                    time = item.EndTime;
                }
            }
        }
    }
}
