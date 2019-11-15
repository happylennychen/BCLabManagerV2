using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class ProgramServiceClass
    {
        public ObservableCollection<ProgramClass> Items { get; set; }
        public void Add(ProgramClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                item.BatteryType = uow.BatteryTypes.GetById(item.BatteryType.Id);
                uow.Programs.Insert(item);
                uow.Commit();
            }
            Items.Add(item);
            foreach (var recipe in item.Recipes)
            {
                RecipeService.Items.Add(recipe);
                foreach (var testRecord in recipe.TestRecords)
                    RecipeService.TestRecordService.Items.Add(testRecord);
            }
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
        public void Update(ProgramClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Programs.Update(item);
                uow.Commit();
            }
            //var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            //edittarget.BatteryType = item.BatteryType;
            //edittarget.Name = item.Name;
            //edittarget.CycleCount = item.CycleCount;
            //edittarget.AssetUseCount = item.AssetUseCount;
            //edittarget.Records = item.Records;
        }

        public RecipeServiceClass RecipeService { get; set; } = new RecipeServiceClass();

        public void UpdateEstimatedTimeChain()
        {
            Order();
            var time = Items[0].RequestTime;
            double c = 0;
            foreach (var item in Items)
            {
                UpdateEstimatedTime(item, ref time, ref c);
                Update(item);
            }
        }
        public void Order()
        {
            Items.OrderBy(o => o.Order);
        }

        public void UpdateEstimatedTime(ProgramClass item, ref DateTime time, ref double c)
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
                foreach (var recipe in item.Recipes)
                    RecipeService.UpdateEstimatedTime(recipe, ref time, ref c);
                //time += RecipeService.StepRuntimeService.GetDuration(item, ref c);
                item.EET = time;
            }
            else
            {
                time = item.StartTime;
                if (item.EndTime == DateTime.MinValue)
                {
                    //time += RecipeService.StepRuntimeService.GetDuration(item, ref c);
                    foreach (var recipe in item.Recipes)
                        RecipeService.UpdateEstimatedTime(recipe, ref time, ref c);
                    item.EET = time;
                }
                else
                {
                    time = item.EndTime;
                }
            }
        }

        //private TimeSpan GetDuration(ProgramClass item)     //side effect: Recipes中的预估时间得到更新
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

        //private DateTime GetStartTime(ProgramClass item)    //初始状态的才调用这个函数
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

        //private bool IsFirstOne(ProgramClass item)
        //{
        //    return item.Order == 0;
        //}

        //private ProgramClass GetPreviousItem(ProgramClass item)
        //{
        //    return Items.SingleOrDefault(o => o.Order == item.Order - 1);
        //}
    }
}
