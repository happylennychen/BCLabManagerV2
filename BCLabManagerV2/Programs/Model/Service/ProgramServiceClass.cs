using BCLabManager.DataAccess;
using BCLabManager.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BCLabManager.Model
{
    public class ProgramServiceClass
    {
        public ObservableCollection<ProgramClass> Items { get; set; }

        public RecipeServiceClass RecipeService { get; set; } = new RecipeServiceClass();
        public void SuperAdd(ProgramClass item)
        {
            DomainAdd(item);
            DatabaseAdd(item);
            UpdateEstimatedTimeChain();
        }
        public void RCSuperAdd(ProgramClass item, List<double> currents, List<double> temperatures, ObservableCollection<RecipeTemplate> recTemplates)
        {
            //var rectemplist = GetRCRecipeTemplatesByCurrents(currents);
            //foreach (var temp in temperatures)
            //{
            //    foreach (var rectemp in rectemplist)
            //    {
                    var rec = recTemplates[0];
                    var recRuntime = new RecipeClass(rec, item.Project.BatteryType);
                    recRuntime.Temperature = 25;
                    item.Recipes.Add(recRuntime);
            //    }
            //}
            DatabaseAdd(item);
        }

        private List<RecipeTemplate> GetRCRecipeTemplatesByCurrents(List<double> currents)
        {
            var output = new List<RecipeTemplate>();
            double chargeCurrent = 0.5;
            double idleTime = 1800;
            foreach (var curr in currents)
            {
                var rectemp = GetRCRecipeTempate(chargeCurrent, idleTime, curr);
                output.Add(rectemp);
            }
            return output;
        }

        private RecipeTemplate GetRCRecipeTempate(double chargeCurrent, double idleTime, double curr)
        {
            try
            {
                List<RecipeTemplate> rectemplist;
                using (var dbContext = new AppDbContext())
                {
                    rectemplist = dbContext.RecipeTemplates
                        .Include(recipeTemplate => recipeTemplate.Steps)
                            .ThenInclude(step => step.StepTemplate)
                        .ToList();
                }
                var rectemp = rectemplist.SingleOrDefault(o => o.Steps.Count == 3 &&
               o.Steps[0].StepTemplate.CurrentInput == chargeCurrent &&
               o.Steps[0].StepTemplate.CurrentUnit == CurrentUnitEnum.C &&
               o.Steps[0].StepTemplate.CutOffConditionValue == 1 &&
               o.Steps[0].StepTemplate.CutOffConditionType == CutOffConditionTypeEnum.CRate &&
               o.Steps[0].LoopLabel == null &&

               o.Steps[1].StepTemplate.CurrentInput == 0 &&
               //o.Steps[1].StepTemplate.CurrentUnit == CurrentUnitEnum.C &&
               o.Steps[1].StepTemplate.CutOffConditionValue == idleTime &&
               o.Steps[1].StepTemplate.CutOffConditionType == CutOffConditionTypeEnum.Time_s &&
               o.Steps[1].LoopLabel == null &&

               o.Steps[2].StepTemplate.CurrentInput == curr &&
               o.Steps[2].StepTemplate.CurrentUnit == CurrentUnitEnum.mA &&
               o.Steps[2].StepTemplate.CutOffConditionValue == 0 &&
                //o.Steps[2].StepTemplate.CutOffConditionType == CutOffConditionTypeEnum.CRate
               o.Steps[2].LoopLabel == null
                );
                if (rectemp != null)
                    return rectemp;

                //没找到，需要创建这个recipe template
                rectemp = CreateRCRecipeTemplate(chargeCurrent, idleTime, curr);
                return rectemp;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Get RC RecipeTempate Error!");
                return null;
            }
        }

        private RecipeTemplate CreateRCRecipeTemplate(double chargeCurrent, double idleTime, double curr)
        {
            try
            {
                //List<StepTemplate> steptemplist;
                /*using (var uow = new UnitOfWork(new AppDbContext()))
                {
                    steptemplist = uow.StepTemplates.GetAll().ToList();
                    var chargesteptemp = steptemplist.SingleOrDefault(o => o.CurrentInput == chargeCurrent && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    if (chargesteptemp == null)
                    {
                        //using (var uow = new UnitOfWork(new AppDbContext()))
                        //{
                        chargesteptemp = new StepTemplate() { CurrentInput = chargeCurrent, CurrentUnit = CurrentUnitEnum.C, CutOffConditionValue = 1, CutOffConditionType = CutOffConditionTypeEnum.CRate };
                        //    uow.StepTemplates.Insert(chargesteptemp);
                        //    uow.Commit();
                        //}
                    }

                    var idlesteptemp = steptemplist.SingleOrDefault(o => o.CurrentInput == 0 && o.CutOffConditionValue == idleTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    if (idlesteptemp == null)
                    {
                        //using (var uow = new UnitOfWork(new AppDbContext()))
                        //{
                        idlesteptemp = new StepTemplate() { CurrentInput = 0, CurrentUnit = CurrentUnitEnum.mA, CutOffConditionValue = idleTime, CutOffConditionType = CutOffConditionTypeEnum.Time_s };
                        //    uow.StepTemplates.Insert(idlesteptemp);
                        //    uow.Commit();
                        //}
                    }

                    var dsgsteptemp = steptemplist.SingleOrDefault(o => o.CurrentInput == curr && o.CutOffConditionValue == 0);
                    if (dsgsteptemp == null)
                    {
                        //using (var uow = new UnitOfWork(new AppDbContext()))
                        //{
                        dsgsteptemp = new StepTemplate() { CurrentInput = curr, CurrentUnit = CurrentUnitEnum.mA, CutOffConditionValue = 0, CutOffConditionType = CutOffConditionTypeEnum.CRate };
                        //    uow.StepTemplates.Insert(dsgsteptemp);
                        //    uow.Commit();
                        //}
                    }

                    RecipeTemplate output;
                    output = new RecipeTemplate() { Name = $"{curr / -1000}A" };
                    var step = new StepClass();
                    step.StepTemplate = chargesteptemp;
                    output.Steps.Add(step);

                    step = new StepClass();
                    step.StepTemplate = idlesteptemp;
                    output.Steps.Add(step);

                    step = new StepClass();
                    step.StepTemplate = dsgsteptemp;
                    output.Steps.Add(step);
                    uow.RecipeTemplates.Insert(output);
                    uow.Commit();

                    return output;
                }*/
                using (var dbContext = new AppDbContext())
                {
                    //steptemplist = dbContext.StepTemplates.GetAll().ToList();
                    var chargesteptemp = dbContext.StepTemplates.SingleOrDefault(o => o.CurrentInput == chargeCurrent && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    if (chargesteptemp == null)
                    {
                        //using (var uow = new UnitOfWork(new AppDbContext()))
                        //{
                        chargesteptemp = new StepTemplate() { CurrentInput = chargeCurrent, CurrentUnit = CurrentUnitEnum.C, CutOffConditionValue = 1, CutOffConditionType = CutOffConditionTypeEnum.CRate };
                        //    uow.StepTemplates.Insert(chargesteptemp);
                        //    uow.Commit();
                        //}
                    }

                    var idlesteptemp = dbContext.StepTemplates.SingleOrDefault(o => o.CurrentInput == 0 && o.CutOffConditionValue == idleTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    if (idlesteptemp == null)
                    {
                        //using (var uow = new UnitOfWork(new AppDbContext()))
                        //{
                        idlesteptemp = new StepTemplate() { CurrentInput = 0, CurrentUnit = CurrentUnitEnum.mA, CutOffConditionValue = idleTime, CutOffConditionType = CutOffConditionTypeEnum.Time_s };
                        //    uow.StepTemplates.Insert(idlesteptemp);
                        //    uow.Commit();
                        //}
                    }

                    var dsgsteptemp = dbContext.StepTemplates.SingleOrDefault(o => o.CurrentInput == curr && o.CutOffConditionValue == 0);
                    if (dsgsteptemp == null)
                    {
                        //using (var uow = new UnitOfWork(new AppDbContext()))
                        //{
                        dsgsteptemp = new StepTemplate() { CurrentInput = curr, CurrentUnit = CurrentUnitEnum.mA, CutOffConditionValue = 0, CutOffConditionType = CutOffConditionTypeEnum.CRate };
                        //    uow.StepTemplates.Insert(dsgsteptemp);
                        //    uow.Commit();
                        //}
                    }

                    RecipeTemplate output;
                    output = new RecipeTemplate() { Name = $"{curr / -1000}A" };
                    var step = new StepClass();
                    step.StepTemplate = chargesteptemp;
                    output.Steps.Add(step);

                    step = new StepClass();
                    step.StepTemplate = idlesteptemp;
                    output.Steps.Add(step);

                    step = new StepClass();
                    step.StepTemplate = dsgsteptemp;
                    output.Steps.Add(step);
                    dbContext.RecipeTemplates.Add(output);
                    dbContext.SaveChanges();

                    return output;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Create RC RecipeTemplate Error!");
                return null;
            }
        }

        public void DatabaseAdd(ProgramClass item)  //不应该自建ID，应该给出order
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                int pid = item.Project.Id;
                int btid = item.Project.BatteryType.Id;
                item.Project = uow.Projects.GetById(pid);
                item.Project.BatteryType = uow.BatteryTypes.GetById(btid);

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
            //if (Items.Count == 0)
            //    return;
            //Order();
            //DateTime startTime;
            //StepRuntimeClass startPoint = FindStartPoint(out startTime);
            //UpdateEstimatedTimeChain(startPoint, startTime);

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
            ////var time = startTime;
            //var time = Items[0].RequestTime;
            //double c = 0;
            //bool isActive = false;
            //foreach (var item in Items)
            //{
            //    UpdateEstimatedTime(item, startPoint, ref time, ref c, ref isActive);
            //    if (isActive)
            //    {
            //        SuperUpdate(item);                               //???这里的Update并不能将item中所有的修改都commit到db，所以只好用下面的foreach来补救
            //        foreach (var recipe in item.Recipes)
            //        {
            //            RecipeService.SuperUpdate(recipe);
            //            foreach (var sr in recipe.StepRuntimes)
            //                RecipeService.StepRuntimeService.Update(sr);
            //        }
            //    }
            //}
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

        internal void Invalidate(ProgramClass program)
        {
            program.IsInvalid = true;
            foreach (var rec in program.Recipes)
            {
                RecipeService.Abandon(rec);
            }
            DatabaseUpdate(program);
        }
    }
}
