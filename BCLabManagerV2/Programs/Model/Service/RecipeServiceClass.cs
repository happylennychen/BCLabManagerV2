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
        public void Add(RecipeClass item)
        {
            //using (var uow = new UnitOfWork(new AppDbContext()))
            //{
            //    item.BatteryType = uow.BatteryTypes.GetById(item.BatteryType.Id);
            //    uow.Batteries.Insert(item);
            //    uow.Commit();
            //}
            //Items.Add(item);
        }
        public void Remove(int id)
        {
            //using (var uow = new UnitOfWork(new AppDbContext()))
            //{
            //    uow.Batteries.Delete(id);
            //    uow.Commit();
            //}

            //var item = Items.SingleOrDefault(o => o.Id == id);
            //Items.Remove(item);
        }
        public void Update(RecipeClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Recipies.Update(item);
                uow.Commit();
            }
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.CompleteTime = item.CompleteTime;
            edittarget.IsAbandoned = item.IsAbandoned;
            edittarget.Loop = item.Loop;
            edittarget.StartTime = item.StartTime;
            edittarget.TestRecords = item.TestRecords;
            edittarget.Name = item.Name;
        }
        public TestRecordServiceClass TestRecordService { get; set; } = new TestRecordServiceClass();

        internal void Invalidate(RecipeClass recipe, TestRecordClass testRecord, string comment)
        {
            TestRecordService.Invalidate(testRecord, comment);
            var newTestRecord = new TestRecordClass();
            //TestRecordService.Add(newTestRecord);
            recipe.TestRecords.Add(newTestRecord);
            Update(recipe);
        }

        internal void Abandon(RecipeClass recipe)
        {
            recipe.IsAbandoned = true;
            foreach (var testRecord in recipe.TestRecords)
            {
                TestRecordService.Abandon(testRecord);
            }
        }
    }
}
