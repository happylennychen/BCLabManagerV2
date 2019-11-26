using BCLabManager.DataAccess;
using BCLabManager.ViewModel;
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

        public RecipeServiceClass RecipeService { get; set; } = new RecipeServiceClass();
        public void SuperAdd(ProgramClass item)
        {
            DatabaseAdd(item);
            DomainAdd(item);
        }
        public void DatabaseAdd(ProgramClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                item.BatteryType = uow.BatteryTypes.GetById(item.BatteryType.Id);
                foreach (var recipe in item.Recipes)
                    foreach (var stepRuntim in recipe.StepRuntimes)
                    {
                        //int StepId = stepRuntim.Step.Id;
                        int StepTemplateId = stepRuntim.StepTemplate.Id;
                        //stepRuntim.Step = uow.Steps.GetById(StepId);
                        stepRuntim.StepTemplate = uow.StepTemplates.GetById(StepTemplateId);
                    }
                uow.Programs.Insert(item);
                uow.Commit();
            }
        }
        public void DomainAdd(ProgramClass item)
        {
            Items.Add(item);
            foreach (var recipe in item.Recipes)
            {
                RecipeService.Items.Add(recipe);
                foreach (var testRecord in recipe.TestRecords)
                    RecipeService.TestRecordService.Items.Add(testRecord);
            }
        }
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
        public void SuperUpdate(ProgramClass item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(ProgramClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.Programs.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(ProgramClass item)
        {
            //var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            //edittarget.BatteryType = item.BatteryType;
            //edittarget.Name = item.Name;
            //edittarget.CycleCount = item.CycleCount;
            //edittarget.AssetUseCount = item.AssetUseCount;
            //edittarget.Records = item.Records;
        }

        public void UpdateEstimatedTimeChain()
        {
            Order();
            var time = Items[0].RequestTime;
            double c = 0;
            foreach (var item in Items)
            {
                UpdateEstimatedTime(item, ref time, ref c);
                SuperUpdate(item);                               //???这里的Update并不能将item中所有的修改都commit到db，所以只好用下面的foreach来补救
                foreach (var recipe in item.Recipes)
                {
                    RecipeService.SuperUpdate(recipe);
                    foreach (var sr in recipe.StepRuntimes)
                        RecipeService.StepRuntimeService.Update(sr);
                }
            }
        }
        public void Order()
        {
            Items.OrderBy(o => o.Order);
        }

        public void UpdateEstimatedTime(ProgramClass item, ref DateTime time, ref double c)
        {
            if (item.StartTime == DateTime.MinValue)
            {
                item.EST = time;
                foreach (var recipe in item.Recipes)
                    RecipeService.UpdateEstimatedTime(recipe, ref time, ref c);
                item.EET = time;
            }
            else
            {
                time = item.StartTime;
                if (item.EndTime == DateTime.MinValue)
                {
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

        internal void StepStart(ProgramClass program, RecipeClass recipe, StepRuntimeClass stepRuntime, DateTime startTime)
        {
            if (program.StartTime == DateTime.MinValue)
                program.StartTime = startTime;

            if (recipe.StartTime == DateTime.MinValue)
                recipe.StartTime = startTime;

            stepRuntime.StartTime = startTime;

            UpdateEstimatedTimeChain();
        }

        internal void StepEnd(ProgramClass program, RecipeClass recipe, StepRuntimeClass stepRuntime, DateTime endTime)
        {
            stepRuntime.EndTime = endTime;

            if (stepRuntime == recipe.StepRuntimes.Last())
            {
                recipe.EndTime = endTime;

                if (recipe == program.Recipes.Last())
                    program.EndTime = endTime;
            }

            UpdateEstimatedTimeChain();
        }
    }
}
