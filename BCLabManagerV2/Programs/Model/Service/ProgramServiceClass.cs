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
            UpdateEstimatedTimeChain();
        }
        public void DatabaseAdd(ProgramClass item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                item.BatteryType = uow.BatteryTypes.GetById(item.BatteryType.Id);

                var nextId = 1;
                if (RecipeService.StepRuntimeService.Items.Count != 0)
                    nextId = RecipeService.StepRuntimeService.Items.Max(o => o.Id) + 1;   //?????help efcore to produce correct id
                foreach (var recipe in item.Recipes)
                    foreach (var stepRuntim in recipe.StepRuntimes)
                    {
                        //int StepId = stepRuntim.Step.Id;
                        int StepTemplateId = stepRuntim.StepTemplate.Id;
                        //stepRuntim.Step = uow.Steps.GetById(StepId);
                        stepRuntim.StepTemplate = uow.StepTemplates.GetById(StepTemplateId);
                        stepRuntim.Id = nextId++;
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
                foreach (var tr in recipe.StepRuntimes)
                    RecipeService.StepRuntimeService.Items.Add(tr);
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
            if (Items.Count == 0)
                return;
            Order();
            DateTime startTime;
            StepRuntimeClass startPoint = FindStartPoint(out startTime);
            UpdateEstimatedTimeChain(startPoint, startTime);

        }
        //找到最后一个有实际时间的step runtime，如果都没有，那就返回第一个
        private StepRuntimeClass FindStartPoint(out DateTime startTime)
        {
            StepRuntimeClass startPoint = null;
            startTime = Items[0].RequestTime;
            foreach (var item in Items)
            {
                foreach (var rec in item.Recipes)
                {
                    foreach (var step in rec.StepRuntimes)
                    {
                        if (step.EndTime != DateTime.MinValue)
                        {
                            startPoint = step;
                            startTime = step.EndTime;
                        }
                        else if (step.StartTime != DateTime.MinValue)
                        {
                            startPoint = step;
                            startTime = step.StartTime;
                        }
                    }
                }
            }
            if (startPoint == null)
                startPoint = Items[0].Recipes[0].StepRuntimes[0];
            return startPoint;
        }

        public void Order()
        {
            Items.OrderBy(o => o.Order);
        }


        public void UpdateEstimatedTimeChain(StepRuntimeClass startPoint, DateTime startTime)
        {
            //var time = startTime;
            var time = Items[0].RequestTime;
            double c = 0;
            bool isActive = false;
            foreach (var item in Items)
            {
                UpdateEstimatedTime(item, startPoint, ref time, ref c, ref isActive);
                if (isActive)
                {
                    SuperUpdate(item);                               //???这里的Update并不能将item中所有的修改都commit到db，所以只好用下面的foreach来补救
                    foreach (var recipe in item.Recipes)
                    {
                        RecipeService.SuperUpdate(recipe);
                        foreach (var sr in recipe.StepRuntimes)
                            RecipeService.StepRuntimeService.Update(sr);
                    }
                }
            }
        }
        public void UpdateEstimatedTime(ProgramClass item, StepRuntimeClass startPoint, ref DateTime time, ref double c, ref bool isActive)
        {
            if (item.StartTime == DateTime.MinValue)
            {
                item.EST = time;
                foreach (var recipe in item.Recipes)
                    RecipeService.UpdateEstimatedTime(recipe, startPoint, ref time, ref c, ref isActive);
                item.EET = time;
            }
            else
            {
                time = item.StartTime;
                if (item.EndTime == DateTime.MinValue)
                {
                    foreach (var recipe in item.Recipes)
                        RecipeService.UpdateEstimatedTime(recipe, startPoint, ref time, ref c, ref isActive);
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
