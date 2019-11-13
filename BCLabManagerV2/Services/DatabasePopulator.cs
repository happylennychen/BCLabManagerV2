using BCLabManager.DataAccess;
using BCLabManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager
{
    public static class DatabasePopulator
    {

        public static void PopulateHistoricData()
        {
            PopulateAssets();
            PopulatePrograms();
            //PopulateOperations();
        }

        private static void PopulateAssets()
        {
            PopulateBatteryTypes();
            PopulateBatteries();
            PopulateTesters();
            PopulateChannels();
            PopulateChambers();
        }

        private static void PopulateBatteryTypes()
        {
            using (var dbContext = new AppDbContext())
            {
                BatteryTypeClass btc = new BatteryTypeClass()
                {
                    Name = "BLP663",
                    Manufactor = "Oppo"
                };
                dbContext.BatteryTypes.Add(btc);
                dbContext.SaveChanges();
            }
        }

        private static void PopulateBatteries()
        {
            using (var dbContext = new AppDbContext())
            {
                for (int i = 0; i < 5; i++)
                {
                    BatteryClass bc = new BatteryClass()
                    {
                        Name = $"BLP663-{i + 1}",
                        BatteryType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663"),
                        CycleCount = 0,
                        AssetUseCount = 0
                    };
                    dbContext.Batteries.Add(bc);
                }
                dbContext.SaveChanges();
            }
        }

        private static void PopulateTesters()
        {
            using (var dbContext = new AppDbContext())
            {
                TesterClass tc = new TesterClass()
                {
                    Name = "17200",
                    Manufactor = "Chroma"
                };
                dbContext.Testers.Add(tc);
                dbContext.SaveChanges();
            }
        }

        private static void PopulateChannels()
        {
            using (var dbContext = new AppDbContext())
            {
                for (int i = 0; i < 4; i++)
                {
                    ChannelClass chc = new ChannelClass()
                    {
                        Name = $"Channel-{i + 1}",
                        Tester = dbContext.Testers.SingleOrDefault(o => o.Name == "17200"),
                        AssetUseCount = 0
                    };
                    dbContext.Channels.Add(chc);
                }
                dbContext.SaveChanges();
            }
        }

        private static void PopulateChambers()
        {
            using (var dbContext = new AppDbContext())
            {
                ChamberClass cmc = new ChamberClass()
                {
                    Name = "PUL-80",
                    Manufactor = "Hongzhan"
                };
                dbContext.Chambers.Add(cmc);
                dbContext.SaveChanges();
            }
        }
        private static void PopulatePrograms()
        {
            PopulateRecipeTemplates();
            PopulateTestPrograms();
        }

        private static void PopulateTestPrograms()
        {
            using (var dbContext = new AppDbContext())
            {
                int i = 1;
                int sub_i = 1;
                ProgramClass obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Oppo BLP663 Static Test";
                obj.BatteryType = dbContext.BatteryTypes.SingleOrDefault(o => o.Id == 1);
                obj.Requester = "Francis";
                obj.RequestTime = DateTime.Parse("2019/02/28");
                obj.Description = "Oppo BLP663 Static Test";
                RecipeClass sub = new RecipeClass(GetRecipeTemplateById(dbContext, 1), obj.Name, 1);
                sub.Id = sub_i++;
                obj.Recipes.Add(sub);
                sub = new RecipeClass(GetRecipeTemplateById(dbContext, 2), obj.Name, 1);
                sub.Id = sub_i++;
                obj.Recipes.Add(sub);
                sub = new RecipeClass(GetRecipeTemplateById(dbContext, 3), obj.Name, 1);
                sub.Id = sub_i++;
                obj.Recipes.Add(sub);
                sub = new RecipeClass(GetRecipeTemplateById(dbContext, 4), obj.Name, 1);
                sub.Id = sub_i++;
                obj.Recipes.Add(sub);
                sub = new RecipeClass(GetRecipeTemplateById(dbContext, 5), obj.Name, 1);
                sub.Id = sub_i++;
                obj.Recipes.Add(sub);
                sub = new RecipeClass(GetRecipeTemplateById(dbContext, 6), obj.Name, 1);
                sub.Id = sub_i++;
                obj.Recipes.Add(sub);
                sub = new RecipeClass(GetRecipeTemplateById(dbContext, 7), obj.Name, 1);
                sub.Id = sub_i++;
                obj.Recipes.Add(sub);
                sub = new RecipeClass(GetRecipeTemplateById(dbContext, 8), obj.Name, 1);
                sub.Id = sub_i++;
                obj.Recipes.Add(sub);
                sub = new RecipeClass(GetRecipeTemplateById(dbContext, 9), obj.Name, 1);
                sub.Id = sub_i++;
                obj.Recipes.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();
            }
        }

        private static void PopulateRecipeTemplates()
        {
            using (var dbContext = new AppDbContext())
            {
                int i = 1;
                RecipeTemplate obj = new RecipeTemplate();
                obj.Id = i++;
                obj.Name = "Room 0.2C charge, -5deg 500mA discharge";
                dbContext.RecipeTemplates.Add(obj);

                obj = new RecipeTemplate();
                obj.Id = i++;
                obj.Name = "Room 0.2C charge, -5deg 1700mA discharge";
                dbContext.RecipeTemplates.Add(obj);

                obj = new RecipeTemplate();
                obj.Id = i++;
                obj.Name = "Room 0.2C charge, -5deg 3000mA discharge";
                dbContext.RecipeTemplates.Add(obj);

                obj = new RecipeTemplate();
                obj.Id = i++;
                obj.Name = "Room 0.2C charge, Room 500mA discharge";
                dbContext.RecipeTemplates.Add(obj);

                obj = new RecipeTemplate();
                obj.Id = i++;
                obj.Name = "Room 0.2C charge, Room 1700mA discharge";
                dbContext.RecipeTemplates.Add(obj);

                obj = new RecipeTemplate();
                obj.Id = i++;
                obj.Name = "Room 0.2C charge, Room 3000mA discharge";
                dbContext.RecipeTemplates.Add(obj);

                obj = new RecipeTemplate();
                obj.Id = i++;
                obj.Name = "Room 0.2C charge, 35deg 500mA discharge";
                dbContext.RecipeTemplates.Add(obj);

                obj = new RecipeTemplate();
                obj.Id = i++;
                obj.Name = "Room 0.2C charge, 35deg 1700mA discharge";
                dbContext.RecipeTemplates.Add(obj);

                obj = new RecipeTemplate();
                obj.Id = i++;
                obj.Name = "Room 0.2C charge, 35deg 3000mA discharge";
                dbContext.RecipeTemplates.Add(obj);

                dbContext.SaveChanges();
            }
        }

        private static RecipeTemplate GetRecipeTemplateById(AppDbContext dbContext, int id)
        {
            var subtemplate = dbContext.RecipeTemplates.SingleOrDefault(o => o.Id == id);
            //dbContext.Entry(subtemplate)
            //    .Reference(o => o.ChargeTemperature)
            //    .Load();
            //dbContext.Entry(subtemplate)
            //    .Reference(o => o.ChargeCurrent)
            //    .Load();
            //dbContext.Entry(subtemplate)
            //    .Reference(o => o.DischargeTemperature)
            //    .Load();
            //dbContext.Entry(subtemplate)
            //    .Reference(o => o.DischargeCurrent)
            //    .Load();
            return subtemplate;
        }


        private static void PopulateOperations()
        {
            TestRecordClass tr;
            RecipeClass sub;
            ProgramClass pro;
            using (var dbContext = new AppDbContext())
            {
                tr = dbContext.TestRecords.SingleOrDefault(i => i.Id == 4);
                sub = dbContext.Recipes.SingleOrDefault(o => o.Id == 1);
                //dbContext.Entry(sub)
                //    .Reference(o => o.ChargeTemperature)
                //    .Load();
                //dbContext.Entry(sub)
                //    .Reference(o => o.ChargeCurrent)
                //    .Load();
                //dbContext.Entry(sub)
                //    .Reference(o => o.DischargeTemperature)
                //    .Load();
                //dbContext.Entry(sub)
                //    .Reference(o => o.DischargeCurrent)
                //    .Load();
                pro = dbContext.Programs.SingleOrDefault(o => o.Id == 1);
                tr.BatteryTypeStr = dbContext.BatteryTypes.SingleOrDefault(o=>o.Id == 1).Name;
                tr.BatteryStr = dbContext.Batteries.SingleOrDefault(o => o.Id == 1).Name;
                tr.ChamberStr = dbContext.Chambers.SingleOrDefault(o => o.Id == 1).Name;
                tr.TesterStr = dbContext.Testers.SingleOrDefault(o => o.Id == 1).Name;
                tr.ChannelStr = dbContext.Channels.SingleOrDefault(o => o.Id == 1).Name;
                tr.StartTime = DateTime.Parse("2019/3/7  17:05:58");
                tr.Steps = "";
                tr.Status = TestStatus.Executing;
                tr.AssignedBattery = dbContext.Batteries.SingleOrDefault(o => o.Id == 1);
                tr.AssignedChamber = dbContext.Chambers.SingleOrDefault(o => o.Id == 1);
                tr.AssignedChannel = dbContext.Channels.SingleOrDefault(o => o.Id == 1);
                dbContext.SaveChanges();
            }
            //tr.AssetsExecute(tr.AssignedBattery, tr.AssignedChamber, tr.AssignedChannel, tr.Steps, tr.StartTime, pro.Name, $"{sub.ChargeTemperature.Name} {sub.ChargeCurrent} charge, {sub.DischargeTemperature} {sub.DischargeCurrent} discharge");
        }
    }
}
