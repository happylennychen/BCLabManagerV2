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
            PopulateOperations();
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
                    Manufactor = "Oppo",
                    TypicalCapacity = 3500
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
            PopulateTemperatures();
            PopulatePercentageCurrents();
            PopulateAbsoluteCurrents();
            PopulateDynamicCurrents();
            PopulateSubProgramTemplates();
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
                obj.RequestDate = DateTime.Parse("2019/02/28");
                obj.Description = "Oppo BLP663 Static Test";
                SubProgramClass sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 1), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 2), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 3), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 4), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 5), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 6), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 7), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 8), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 9), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Oppo BLP663 Dynamic Test";
                obj.BatteryType = dbContext.BatteryTypes.SingleOrDefault(o => o.Id == 1);
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/02/28");
                obj.Description = "Oppo BLP663 Dynamic Test";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 10), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 11), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 12), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 13), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 14), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 15), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Oppo BLP663 RC";
                obj.BatteryType = dbContext.BatteryTypes.SingleOrDefault(o => o.Id == 1);
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/02/28");
                obj.Description = "Oppo BLP663 RC";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 16), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 17), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 18), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 19), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 20), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 21), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Oppo BLP663 Static Test-2";
                obj.BatteryType = dbContext.BatteryTypes.SingleOrDefault(o => o.Id == 1);
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/03/12");
                obj.Description = "Oppo BLP663 Static Test-2";

                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 22), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 23), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 24), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Oppo BLP663 Dynamic Test-2";
                obj.BatteryType = dbContext.BatteryTypes.SingleOrDefault(o => o.Id == 1);
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/03/14");
                obj.Description = "Oppo BLP663 Dynamic Test-2";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 25), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 26), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 27), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 30), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 31), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Oppo BLP663 Static Test-3";
                obj.BatteryType = dbContext.BatteryTypes.SingleOrDefault(o => o.Id == 1);
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/03/25");
                obj.Description = "Oppo BLP663 Static Test-3";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 28), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Aging Test";
                obj.BatteryType = dbContext.BatteryTypes.SingleOrDefault(o => o.Id == 1);
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/04/01");
                obj.Description = "Run 500 cycle to see the aging effect";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 29), obj.Name, 500);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "5 time test";
                obj.BatteryType = dbContext.BatteryTypes.SingleOrDefault(o => o.Id == 1);
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/04/01");
                obj.Description = "Run 5 same cycle";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 4), obj.Name, 5);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 23), obj.Name, 5);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 26), obj.Name, 5);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Oppo BLP663 Dynamic Test-3";
                obj.BatteryType = dbContext.BatteryTypes.SingleOrDefault(o => o.Id == 1);
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/05/07");
                obj.Description = "Oppo BLP663 Dynamic Test-3";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 30), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Oppo BLP663 Dynamic Test-4";
                obj.BatteryType = dbContext.BatteryTypes.SingleOrDefault(o => o.Id == 1);
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/05/24");
                obj.Description = "Oppo BLP663 Dynamic Test-4";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 31), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "2000mA Test";
                obj.BatteryType = dbContext.BatteryTypes.SingleOrDefault(o => o.Id == 1);
                obj.Requester = "Jon";
                obj.RequestDate = DateTime.Parse("2019/07/09");
                obj.Description = "";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 23), obj.Name, 1);
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();
            }
        }

        private static void PopulateSubProgramTemplates()
        {
            using (var dbContext = new AppDbContext())
            {
                int i = 1;
                SubProgramTemplate obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = -5;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 500;
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = -5;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 1700;
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = -5;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 3000;
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 500;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 1700;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 3000;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = 35;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = 35;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 500;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = 35;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = 35;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 1700;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = 35;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = 35;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 3000;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = -5;
                obj.DischargeCurrentType = CurrentTypeEnum.Dynamic;
                obj.DischargeCurrent = 1;
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = -5;
                obj.DischargeCurrentType = CurrentTypeEnum.Dynamic;
                obj.DischargeCurrent = 2;
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.DischargeCurrentType = CurrentTypeEnum.Dynamic;
                obj.DischargeCurrent = 1;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.DischargeCurrentType = CurrentTypeEnum.Dynamic;
                obj.DischargeCurrent = 2;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = 35;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = 35;
                obj.DischargeCurrentType = CurrentTypeEnum.Dynamic;
                obj.DischargeCurrent = 1;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = 35;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = 35;
                obj.DischargeCurrentType = CurrentTypeEnum.Dynamic;
                obj.DischargeCurrent = 2;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = -10;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 3500;
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = 0;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = 0;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 3500;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = 10;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = 10;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 3500;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = 20;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = 20;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 3500;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = 30;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = 30;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 3500;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = 40;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = 40;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 3500;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = -5;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 2000;
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 2000;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = 35;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = 35;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 2000;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = -5;
                obj.DischargeCurrentType = CurrentTypeEnum.Dynamic;
                obj.DischargeCurrent = 3;
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.DischargeCurrentType = CurrentTypeEnum.Dynamic;
                obj.DischargeCurrent = 4;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = 35;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = 35;
                obj.DischargeCurrentType = CurrentTypeEnum.Dynamic;
                obj.DischargeCurrent = 3;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 1200;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.DischargeCurrentType = CurrentTypeEnum.Absolute;
                obj.DischargeCurrent = 3450;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.DischargeCurrentType = CurrentTypeEnum.Dynamic;
                obj.DischargeCurrent = 4;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.ChargeCurrentType = CurrentTypeEnum.Percentage;
                obj.ChargeCurrent = 1;
                obj.DischargeTemperature = GlobalSettings.RoomTemperatureConstant;
                obj.DischargeCurrentType = CurrentTypeEnum.Dynamic;
                obj.DischargeCurrent = 5;
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                dbContext.SaveChanges();
            }
        }

        private static void PopulateAbsoluteCurrents()
        {
            using (var dbContext = new AppDbContext())
            {
                AbsoluteCurrentClass obj = new AbsoluteCurrentClass();
                obj.Value = 500;
                dbContext.AbsoluteCurrents.Add(obj);

                obj = new AbsoluteCurrentClass();
                obj.Value = 1200;
                dbContext.AbsoluteCurrents.Add(obj);

                obj = new AbsoluteCurrentClass();
                obj.Value = 1700;
                dbContext.AbsoluteCurrents.Add(obj);

                obj = new AbsoluteCurrentClass();
                obj.Value = 2000;
                dbContext.AbsoluteCurrents.Add(obj);

                obj = new AbsoluteCurrentClass();
                obj.Value = 3000;
                dbContext.AbsoluteCurrents.Add(obj);

                obj = new AbsoluteCurrentClass();
                obj.Value = 3450;
                dbContext.AbsoluteCurrents.Add(obj);

                obj = new AbsoluteCurrentClass();
                obj.Value = 3500;
                dbContext.AbsoluteCurrents.Add(obj);

                dbContext.SaveChanges();
            }
        }

        private static void PopulatePercentageCurrents()
        {
            using (var dbContext = new AppDbContext())
            {
                PercentageCurrentClass obj = new PercentageCurrentClass();
                obj.Value = 1;
                dbContext.PercentageCurrents.Add(obj);

                dbContext.SaveChanges();
            }
        }

        private static void PopulateDynamicCurrents()
        {
            using (var dbContext = new AppDbContext())
            {
                DynamicCurrentClass obj = new DynamicCurrentClass();
                obj.Value = 1;
                dbContext.DynamicCurrents.Add(obj);

                obj = new DynamicCurrentClass();
                obj.Value = 2;
                dbContext.DynamicCurrents.Add(obj);

                obj = new DynamicCurrentClass();
                obj.Value = 3;
                dbContext.DynamicCurrents.Add(obj);

                obj = new DynamicCurrentClass();
                obj.Value = 4;
                dbContext.DynamicCurrents.Add(obj);

                obj = new DynamicCurrentClass();
                obj.Value = 5;
                dbContext.DynamicCurrents.Add(obj);

                dbContext.SaveChanges();
            }
        }

        private static void PopulateTemperatures()
        {
            using (var dbContext = new AppDbContext())
            {
                TemperatureClass obj = new TemperatureClass();
                obj.Value = GlobalSettings.RoomTemperatureConstant;
                dbContext.Temperatures.Add(obj);

                obj = new TemperatureClass();
                obj.Value = 0;
                dbContext.Temperatures.Add(obj);

                obj = new TemperatureClass();
                obj.Value = 10;
                dbContext.Temperatures.Add(obj);

                obj = new TemperatureClass();
                obj.Value = 20;
                dbContext.Temperatures.Add(obj);

                obj = new TemperatureClass();
                obj.Value = 30;
                dbContext.Temperatures.Add(obj);

                obj = new TemperatureClass();
                obj.Value = 35;
                dbContext.Temperatures.Add(obj);

                obj = new TemperatureClass();
                obj.Value = 40;
                dbContext.Temperatures.Add(obj);

                dbContext.SaveChanges();
            }
        }

        private static SubProgramTemplate GetSubProgramTemplateById(AppDbContext dbContext, int id)
        {
            var subtemplate = dbContext.SubProgramTemplates.SingleOrDefault(o => o.Id == id);
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
            SubProgramClass sub;
            ProgramClass pro;
            using (var dbContext = new AppDbContext())
            {
                tr = dbContext.TestRecords.SingleOrDefault(i => i.Id == 4);
                sub = dbContext.SubPrograms.SingleOrDefault(o => o.Id == 1);
                //dbContext.Entry(sub)
                //    .Reference(o => o.ChargeTemperature)
                //    .Load();
                //dbContext.Entry(sub)
                //    .Reference(o => o.ChargeCurrent)
                //    .Load();
                //dbContext.Entry(sub)
                //    .Reference(o => o.ChargeCurrent)
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
            tr.AssetsExecute(tr.AssignedBattery, tr.AssignedChamber, tr.AssignedChannel, tr.Steps, tr.StartTime, pro.Name, sub.Name);
        }
    }
}
