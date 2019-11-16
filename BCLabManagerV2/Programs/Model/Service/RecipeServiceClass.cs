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
            edittarget.Loop = item.Loop;
            edittarget.StartTime = item.StartTime;
            edittarget.TestRecords = item.TestRecords;
            edittarget.Name = item.Name;
        }
        public TestRecordServiceClass TestRecordService { get; set; } = new TestRecordServiceClass();
        public StepRuntimeServiceClass StepRuntimeService { get; set; } = new StepRuntimeServiceClass();

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

        //internal TimeSpan GetDuration(RecipeClass recipe, ref double CBegin)
        //{
        //    TimeSpan duration = new TimeSpan();
        //    foreach (var sr in recipe.StepRuntimes)
        //    {
        //        duration += StepRuntimeService.GetDuration(sr, ref CBegin);
        //    }
        //    return duration;
        //}

        internal void UpdateEstimatedTime(RecipeClass item, ref DateTime time, ref double c)
        {
            //if (item.EndTime != DateTime.MinValue)  //完成状态
            //    return;
            //else if (item.StartTime == DateTime.MinValue)    //初始状态
            //{
            //    item.EST = GetStartTime(item);
            //    item.ED = GetDuration(item);
            //    item.EET = item.EST + item.ED;
            //}
            ////运行状态
            //else
            //{
            //    item.ED = GetDuration(item);
            //    item.EET = item.StartTime + item.ED;
            //}
            if (item.StartTime == DateTime.MinValue)
            {
                item.EST = time;
                foreach (var sr in item.StepRuntimes)
                    StepRuntimeService.UpdateEstimatedTime(sr, ref time, ref c);
                //time += RecipeService.StepRuntimeService.GetDuration(item, ref c);
                item.EET = time;
            }
            else
            {
                time = item.StartTime;
                if (item.EndTime == DateTime.MinValue)
                {
                    //time += RecipeService.StepRuntimeService.GetDuration(item, ref c);
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

        //private TimeSpan GetDuration(RecipeClass item)
        //{
        //    TimeSpan duration = new TimeSpan();
        //    double CBegin = 0;
        //    foreach (var recipe in item.Recipes)
        //    {
        //        //duration += RecipeService.GetDuration(recipe, ref CBegin);
        //        RecipeService.UpdateEstimatedTime(recipe, ref CBegin);
        //        duration += recipe.ED;
        //    }
        //    return duration;
        //}

        //private DateTime GetStartTime(RecipeClass item)    //初始状态的才调用这个函数
        //{
        //    if (IsFirstOne(item))
        //    {
        //        return item.RequestTime;
        //    }
        //    else
        //    {
        //        var previousitem = GetPreviousItem(item);
        //        if (previousitem.EndTime != DateTime.MinValue)
        //            return previousitem.EndTime;
        //        else
        //            return previousitem.EET;
        //    }
        //}
    }
}
