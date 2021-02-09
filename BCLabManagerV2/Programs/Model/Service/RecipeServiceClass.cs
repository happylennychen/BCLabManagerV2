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
        public ObservableCollection<Recipe> Items { get; set; }
        public TestRecordServiceClass TestRecordService { get; set; } = new TestRecordServiceClass();
        public StepRuntimeServiceClass StepRuntimeService { get; set; } = new StepRuntimeServiceClass();
        public RecipeTemplateServiceClass RecipeTemplateService { get; set; } = new RecipeTemplateServiceClass();
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
        public void SuperUpdate(Recipe item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(Recipe item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Recipies.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(Recipe item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.EndTime = item.EndTime;
            edittarget.IsAbandoned = item.IsAbandoned;
            edittarget.StartTime = item.StartTime;
            edittarget.TestRecords = item.TestRecords;
            edittarget.Name = item.Name;
        }

        internal void Invalidate(Recipe recipe, TestRecord testRecord, string comment)
        {
            TestRecordService.Invalidate(testRecord, comment);

            //var newTestRecord = new TestRecord();
            //recipe.TestRecords.Add(newTestRecord);
            //SuperUpdate(recipe);
            //TestRecordService.DomainAdd(newTestRecord);   //Issue 2322
        }

        internal void AddTestRecord(Recipe recipe)
        {
            var newTestRecord = new TestRecord();
            recipe.TestRecords.Add(newTestRecord);
            SuperUpdate(recipe);
            TestRecordService.DomainAdd(newTestRecord);
        }

        internal void Abandon(Recipe recipe)
        {
            recipe.IsAbandoned = true;
            foreach (var testRecord in recipe.TestRecords)
            {
                TestRecordService.Abandon(testRecord);
            }
        }

        internal void UpdateEstimatedTime(Recipe item, StepRuntime startPoint, ref DateTime time, ref double c, ref bool isActive)
        {
            if (item.StartTime == DateTime.MinValue)
            {
                item.EST = time;
                foreach (var sr in item.StepRuntimes)
                    StepRuntimeService.UpdateEstimatedTime(sr, startPoint, ref time, ref c, ref isActive);
                item.EET = time;
            }
            else
            {
                time = item.StartTime;
                if (item.EndTime == DateTime.MinValue)
                {
                    foreach (var sr in item.StepRuntimes)
                        StepRuntimeService.UpdateEstimatedTime(sr, startPoint, ref time, ref c, ref isActive);
                    item.EET = time;
                }
                else
                {
                    time = item.EndTime;
                }
            }
        }

        internal void Attach(Recipe recipe, TestRecord record)
        {
            var rec = new Recipe();
            var tr = new TestRecord();
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                rec = uow.Recipies.SingleOrDefault(o => o.Id == recipe.Id);
                tr = uow.TestRecords.SingleOrDefault(o => o.Id == record.Id);
                rec.TestRecords.Add(tr);
                uow.Commit();
            }
            recipe.TestRecords.Add(record);
        }

        internal void UpdateTime(Recipe recipe)
        {
            var quary = recipe.TestRecords.Where(o => o.Status != TestStatus.Abandoned && o.StartTime!=DateTime.MinValue && o.EndTime!=DateTime.MinValue);
            if (quary != null && quary.Count() != 0)
            {
                recipe.StartTime = quary.Min(o => o.StartTime);
                recipe.EndTime = quary.Max(o => o.EndTime);
                SuperUpdate(recipe);
            }
        }
    }
}
