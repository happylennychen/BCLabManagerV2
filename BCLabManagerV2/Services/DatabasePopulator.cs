using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Services;
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
            //ReportLoader.Init();
            PopulateAssets();
            #region blp663
            //PopulateProgram1(); //Tested
            //PopulateProgram2(); //Tested
            //PopulateProgram3(); //Tested
            //PopulateProgram4(); //Tested
            //PopulateProgram5(); //Tested
            //PopulateProgram6(); //Tested
            //PopulateProgram7(); //Tested
            //PopulateProgram8(); //Tested
            //PopulateProgram9(); //Tested
            //PopulateProgram10(); //Tested
            //                     //PopulateProgram11();
            //PopulateProgram12(); //Tested
            ////PopulateProgram13();
            //PopulateProgram14(); //Tested
            //PopulateProgram15(); //Tested
            //PopulateProgram16(); //Tested
            //PopulateProgram17(); //Tested
            //PopulateProgram18(); //Tested
            //PopulateProjects();
            //PopulateTestPrograms();
            #endregion

            #region HG2
            PopulateProjects();
            CreateStepTemplates();
            CreateRecipeTemplates();
            #endregion
        }
        #region Assets
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
            //int bt_num = ReportLoader.GetBatteryTypeNumber();
            //for (int i = 0; i < bt_num; i++)
            //{
            //    using (var uow = new UnitOfWork(new AppDbContext()))
            //    {
            //        BatteryTypeClass bt = ReportLoader.LoadBatteryTypeByIndex(i);
            //        if (bt != null)
            //        {
            //            uow.BatteryTypes.Insert(bt);
            //            uow.Commit();
            //        }
            //    }
            //}
            BatteryTypeClass bt = new BatteryTypeClass();
            bt.Name = "BLP663";
            bt.Manufactor = "Oppo";
            bt.Material = "Li-ion Plymer Battery";
            bt.LimitedChargeVoltage = 4400;
            bt.RatedCapacity = 3365;
            bt.NominalVoltage = 3850;
            bt.TypicalCapacity = 3450;
            bt.CutoffDischargeVoltage = 3200;
            PopulateOneBatteryType(bt);

            bt = new BatteryTypeClass();
            bt.Name = "32700-6000mAh";
            bt.Manufactor = "FbTech";
            bt.Material = "lithium-ion";
            bt.LimitedChargeVoltage = 3650;
            bt.RatedCapacity = 6000;
            bt.NominalVoltage = 3200;
            bt.TypicalCapacity = 6000;
            bt.CutoffDischargeVoltage = 2000;
            PopulateOneBatteryType(bt);

            bt = new BatteryTypeClass();
            bt.Name = "INR18650-25R";
            bt.Manufactor = "SamSung SDI Co., Ltd";
            bt.Material = "lithium-ion";
            bt.LimitedChargeVoltage = 4200;
            bt.RatedCapacity = 2500;
            bt.NominalVoltage = 3600;
            bt.TypicalCapacity = 2450;
            bt.CutoffDischargeVoltage = 2500;
            PopulateOneBatteryType(bt);

            bt = new BatteryTypeClass();
            bt.Name = "H26";
            bt.Manufactor = "LG";
            bt.Material = "lithium-ion";
            bt.LimitedChargeVoltage = 4200;
            bt.RatedCapacity = 2600;
            bt.TypicalCapacity = 2600;
            bt.NominalVoltage = 3600;
            bt.CutoffDischargeVoltage = 2500;
            bt.FullyChargedEndCurrent = 50;
            bt.FullyChargedEndingTimeout = 0;
            PopulateOneBatteryType(bt);

            bt = new BatteryTypeClass();
            bt.Name = "HG2";
            bt.Manufactor = "LG";
            bt.Material = "lithium-ion";
            bt.LimitedChargeVoltage = 4200;
            bt.RatedCapacity = 3000;
            bt.TypicalCapacity = 3000;
            bt.NominalVoltage = 3600;
            bt.CutoffDischargeVoltage = 2500;
            bt.FullyChargedEndCurrent = 50;
            bt.FullyChargedEndingTimeout = 0;
            PopulateOneBatteryType(bt);
        }

        private static void PopulateOneBatteryType(BatteryTypeClass bt)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                if (bt != null)
                {
                    uow.BatteryTypes.Insert(bt);
                    uow.Commit();
                }
            }
        }

        private static void PopulateProjects()
        {
            PopulateOneProject("High Power", "HG2", "O2Micro", "", "");
            //PopulateOneProject("High Power 2", "HG2", "O2Micro", "Change Cut off voltage from 2.5v to 3v", "");
        }
        private static void PopulateOneProject(string name, string batteryType, string customer, string description, string voltagePoints)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                ProjectClass proj = new ProjectClass();
                proj.Name = name;
                proj.Customer = customer;
                proj.Description = description;
                proj.VoltagePoints = voltagePoints;
                proj.BatteryType = uow.BatteryTypes.SingleOrDefault(o => o.Name == batteryType);
                proj.CutoffDischargeVoltage = proj.BatteryType.CutoffDischargeVoltage;
                proj.LimitedChargeVoltage = proj.BatteryType.LimitedChargeVoltage;
                proj.AbsoluteMaxCapacity = proj.BatteryType.RatedCapacity;
                var evSetting = new EvSettingClass();
                evSetting.DischargeEndVoltage = proj.BatteryType.CutoffDischargeVoltage;
                evSetting.FullyChargedEndCurrent = proj.BatteryType.FullyChargedEndCurrent;
                evSetting.FullyChargedEndingTimeout = proj.BatteryType.FullyChargedEndingTimeout;
                evSetting.LimitedChargeVoltage = proj.BatteryType.LimitedChargeVoltage;
                evSetting.DesignCapacity = proj.BatteryType.TypicalCapacity;
                proj.EvSettings.Add(evSetting);
                uow.Projects.Insert(proj);
                uow.Commit();
            }
        }
        private static void PopulateBatteries()
        {
            //int bt_num = ReportLoader.GetBatteryNumber();
            //for (int i = 0; i < bt_num; i++)
            //{
            //    using (var uow = new UnitOfWork(new AppDbContext()))
            //    {
            //        int btId = 0;
            //        BatteryClass bat = ReportLoader.LoadBatteryByIndex(i, ref btId);
            //        if (bat != null)
            //        {
            //            bat.BatteryType = uow.BatteryTypes.SingleOrDefault(o => o.Id == btId);
            //            uow.Batteries.Insert(bat);
            //            uow.Commit();
            //        }
            //    }
            //}
            BatteryClass b;
            for (int i = 1; i < 5; i++)
            {
                b = new BatteryClass();
                b.Name = "BLP663-" + i.ToString();
                PopulateOneBattery(b, 1);
            }
            for (int i = 1; i < 9; i++)
            {
                b = new BatteryClass();
                b.Name = "32700-6000-" + i.ToString();
                PopulateOneBattery(b, 2);
            }
            for (int i = 1; i < 11; i++)
            {
                b = new BatteryClass();
                b.Name = "INR18650-25R-" + i.ToString();
                PopulateOneBattery(b, 3);
            }
            for (int i = 1; i < 4; i++)
            {
                b = new BatteryClass();
                b.Name = "H26-" + i.ToString();
                PopulateOneBattery(b, 4);
            }
            for (int i = 1; i < 11; i++)
            {
                b = new BatteryClass();
                b.Name = "HG2-" + i.ToString();
                PopulateOneBattery(b, 5);
            }
        }

        private static void PopulateOneBattery(BatteryClass b, int btId)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                if (b != null)
                {
                    b.BatteryType = uow.BatteryTypes.SingleOrDefault(o => o.Id == btId);
                    uow.Batteries.Insert(b);
                    uow.Commit();
                }
            }
        }

        private static void PopulateTesters()
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                TesterClass tst = new TesterClass();
                tst.Name = "17200";
                tst.Manufactor = "Chroma";
                if (tst != null)
                {
                    uow.Testers.Insert(tst);
                    uow.Commit();
                }
            }
        }

        private static void PopulateChannels()
        {
            int ch_num = 4;
            for (int i = 0; i < ch_num; i++)
            {
                using (var uow = new UnitOfWork(new AppDbContext()))
                {
                    ChannelClass ch = new ChannelClass();
                    ch.Name = "Ch " + (i + 1).ToString();
                    if (ch != null)
                    {
                        ch.Tester = uow.Testers.SingleOrDefault(o => o.Id == 1);
                        uow.Channels.Insert(ch);
                        uow.Commit();
                    }
                }
            }
        }

        private static void PopulateChambers()
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                ChamberClass cmb = new ChamberClass();
                cmb.Manufactor = "HongZhan";
                cmb.Name = "PUL-80";
                cmb.LowestTemperature = -40;
                cmb.HighestTemperature = 150;
                if (cmb != null)
                {
                    uow.Chambers.Insert(cmb);
                    uow.Commit();
                }
            }
        }
        #endregion
        /*
        #region blp663 programs
        #region program 1
        private static void PopulateProgram1()
        {
            PopulateStepTemplates();
            PopulateRecipeTemplates();
        }

        private static void PopulateStepTemplates()
        {
            double chargeRate = 0.2;
            int restTime = 3600;
            CreateStepTemplate(chargeRate, CurrentUnitEnum.C, 1, CutOffConditionTypeEnum.CRate);

            CreateStepTemplate(0, CurrentUnitEnum.mA, restTime, CutOffConditionTypeEnum.Time_s);

            List<double> cPoints = new List<double>() { -500, -1700, -3000 };
            //List<double> tPoints = new List<double>() { -5, 25, 35 };
            CreateStepTemplatesByCurrents(cPoints);
        }

        private static void PopulateRecipeTemplates()
        {
            double chargeRate = 0.2;
            int restTime = 3600;
            List<double> cPoints = new List<double>() { -500, -1700, -3000 };
            List<double> tPoints = new List<double>() { -5, 25, 35 };

            foreach (var t in tPoints)
            {
                foreach (var c in cPoints)
                {
                    int order = 0;
                    using (var dbContext = new AppDbContext())
                    {
                        RecipeTemplate obj = new RecipeTemplate();
                        obj.Name = $"25 deg {chargeRate}C charge, {t} deg {c}mA discharge";

                        var newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
                        obj.Steps.Add(newStep);

                        dbContext.RecipeTemplates.Add(obj);

                        dbContext.SaveChanges();
                    }
                }
            }
        }
        #endregion
        #region program 2
        private static void PopulateProgram2()
        {
            PopulateStepTemplates2();
            PopulateRecipeTemplates2();
        }

        private static void PopulateStepTemplates2()
        {
            CreateStepTemplate(-5, -500, CurrentUnitEnum.mA, 900, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(-5, -1500, CurrentUnitEnum.mA, 1200, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(-5, -3000, CurrentUnitEnum.mA, 600, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -500, CurrentUnitEnum.mA, 900, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -1500, CurrentUnitEnum.mA, 1200, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -3000, CurrentUnitEnum.mA, 600, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(35, -500, CurrentUnitEnum.mA, 900, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(35, -1500, CurrentUnitEnum.mA, 1200, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(35, -3000, CurrentUnitEnum.mA, 600, CutOffConditionTypeEnum.Time_s);

            CreateStepTemplate(-5, -5175, CurrentUnitEnum.mA, 900, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(-5, -3450, CurrentUnitEnum.mA, 1200, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(-5, -1725, CurrentUnitEnum.mA, 600, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -5175, CurrentUnitEnum.mA, 900, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -3450, CurrentUnitEnum.mA, 1200, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -1725, CurrentUnitEnum.mA, 600, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(35, -5175, CurrentUnitEnum.mA, 900, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(35, -3450, CurrentUnitEnum.mA, 1200, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(35, -1725, CurrentUnitEnum.mA, 600, CutOffConditionTypeEnum.Time_s);
        }

        private static void PopulateRecipeTemplates2()
        {
            double chargeRate = 0.2;
            int restTime = 3600;
            List<double> tPoints = new List<double>() { -5, 25, 35 };
            List<string> cPoints = new List<string>() { "D01", "D02" };
            Dictionary<String, double[]> dic = new Dictionary<string, double[]>();
            dic.Add("D01", new double[] { -500, -1500, -3000 });
            dic.Add("D02", new double[] { -5175, -3450, -1725 });

            foreach (var t in tPoints)
            {
                foreach (var c in cPoints)
                {
                    int order = 0;
                    using (var dbContext = new AppDbContext())
                    {
                        RecipeTemplate obj = new RecipeTemplate();
                        obj.Name = $"25 deg {chargeRate}C charge, {t} deg {c} discharge";

                        var newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == chargeRate &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.C &&
                                                                                        o.CutOffConditionValue == 1 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t &&
                                                                                        o.CurrentInput == dic[c][0] &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.mA &&
                                                                                        o.CutOffConditionValue == 900 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        newStep.LoopLabel = "a";
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t &&
                                                                                        o.CurrentInput == dic[c][1] &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.mA &&
                                                                                        o.CutOffConditionValue == 1200 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t &&
                                                                                        o.CurrentInput == dic[c][2] &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.mA &&
                                                                                        o.CutOffConditionValue == 600 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        newStep.LoopTarget = "a";
                        newStep.CompareMark = CompareMarkEnum.LargerThan;
                        newStep.CRate = 0;
                        obj.Steps.Add(newStep);

                        dbContext.RecipeTemplates.Add(obj);
                        dbContext.SaveChanges();
                    }
                }
            }
        }
        #endregion
        #region program 3
        private static void PopulateProgram3()
        {
            PopulateStepTemplates3();
            PopulateRecipeTemplates3();
        }

        private static void PopulateStepTemplates3()
        {
            List<double> cPoints = new List<double>() { -3500 };
            List<double> tPoints = new List<double>() { -10, 0, 10, 20, 30, 40 };
            CreateRCStepTemplates(cPoints, tPoints);
        }

        private static void PopulateRecipeTemplates3()
        {
            double chargeRate = 0.2;
            int restTime = 3600;
            List<double> cPoints = new List<double>() { -3500 };
            List<double> tPoints = new List<double>() { -10, 0, 10, 20, 30, 40 };

            foreach (var t in tPoints)
            {
                foreach (var c in cPoints)
                {
                    int order = 0;
                    using (var dbContext = new AppDbContext())
                    {
                        RecipeTemplate obj = new RecipeTemplate();
                        obj.Name = $"25 deg {chargeRate}C charge, {t} deg {c}mA discharge";

                        var newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
                        obj.Steps.Add(newStep);

                        dbContext.RecipeTemplates.Add(obj);
                        dbContext.SaveChanges();
                    }
                }
            }
        }
        #endregion
        #region program 4
        private static void PopulateProgram4()
        {
            PopulateStepTemplates4();
            PopulateRecipeTemplates4();
        }

        private static void PopulateStepTemplates4()
        {
            List<double> cPoints = new List<double>() { -2000 };
            List<double> tPoints = new List<double>() { -5, 25, 35 };
            CreateRCStepTemplates(cPoints, tPoints);
        }

        private static void PopulateRecipeTemplates4()
        {
            double chargeRate = 0.2;
            int restTime = 3600;
            List<double> cPoints = new List<double>() { -2000 };
            List<double> tPoints = new List<double>() { -5, 25, 35 };

            foreach (var t in tPoints)
            {
                foreach (var c in cPoints)
                {
                    int order = 0;
                    using (var dbContext = new AppDbContext())
                    {
                        RecipeTemplate obj = new RecipeTemplate();
                        obj.Name = $"25 deg {chargeRate}C charge, {t} deg {c}mA discharge";

                        var newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
                        obj.Steps.Add(newStep);

                        dbContext.RecipeTemplates.Add(obj);
                        dbContext.SaveChanges();
                    }
                }
            }
        }
        #endregion
        #region program 5
        private static void PopulateProgram5()
        {
            PopulateStepTemplates5();
            PopulateRecipeTemplates5();
        }

        private static void PopulateStepTemplates5()
        {
            CreateStepTemplate(-5, -200, CurrentUnitEnum.mA, 900, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(-5, -100, CurrentUnitEnum.mA, 1200, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(-5, -2000, CurrentUnitEnum.mA, 600, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -200, CurrentUnitEnum.mA, 900, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -100, CurrentUnitEnum.mA, 1200, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -2000, CurrentUnitEnum.mA, 600, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(35, -200, CurrentUnitEnum.mA, 900, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(35, -100, CurrentUnitEnum.mA, 1200, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(35, -2000, CurrentUnitEnum.mA, 600, CutOffConditionTypeEnum.Time_s);
        }

        private static void PopulateRecipeTemplates5()
        {
            double chargeRate = 0.2;
            int restTime = 3600;
            List<double> tPoints = new List<double>() { -5, 25, 35 };
            List<string> cPoints = new List<string>() { "D03" };
            Dictionary<String, double[]> dic = new Dictionary<string, double[]>();
            dic.Add("D03", new double[] { -200, -100, -2000 });

            foreach (var t in tPoints)
            {
                foreach (var c in cPoints)
                {
                    int order = 0;
                    using (var dbContext = new AppDbContext())
                    {
                        RecipeTemplate obj = new RecipeTemplate();
                        obj.Name = $"25 deg {chargeRate}C charge, {t} deg {c} discharge";

                        var newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == chargeRate &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.C &&
                                                                                        o.CutOffConditionValue == 1 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t &&
                                                                                        o.CurrentInput == dic[c][0] &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.mA &&
                                                                                        o.CutOffConditionValue == 900 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        newStep.LoopLabel = "a";
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t &&
                                                                                        o.CurrentInput == dic[c][1] &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.mA &&
                                                                                        o.CutOffConditionValue == 1200 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t &&
                                                                                        o.CurrentInput == dic[c][2] &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.mA &&
                                                                                        o.CutOffConditionValue == 600 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        newStep.LoopTarget = "a";
                        newStep.CompareMark = CompareMarkEnum.LargerThan;
                        newStep.CRate = 0;
                        obj.Steps.Add(newStep);

                        dbContext.RecipeTemplates.Add(obj);
                        dbContext.SaveChanges();
                    }
                }
            }
        }
        #endregion
        #region program 6
        private static void PopulateProgram6()
        {
            PopulateStepTemplates6();
            PopulateRecipeTemplates6();
        }

        private static void PopulateStepTemplates6()
        {
            List<double> cPoints = new List<double>() { -1200 };
            List<double> tPoints = new List<double>() { 25 };
            CreateRCStepTemplates(cPoints, tPoints);
        }

        private static void PopulateRecipeTemplates6()
        {
            double chargeRate = 0.2;
            int restTime = 3600;
            List<double> cPoints = new List<double>() { -1200 };
            List<double> tPoints = new List<double>() { 25 };

            foreach (var t in tPoints)
            {
                foreach (var c in cPoints)
                {
                    int order = 0;
                    using (var dbContext = new AppDbContext())
                    {
                        RecipeTemplate obj = new RecipeTemplate();
                        obj.Name = $"25 deg {chargeRate}C charge, {t} deg {c}mA discharge";

                        var newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
                        obj.Steps.Add(newStep);

                        dbContext.RecipeTemplates.Add(obj);
                        dbContext.SaveChanges();
                    }
                }
            }
        }
        #endregion
        #region program 7
        private static void PopulateProgram7()
        {
            PopulateStepTemplates7();
            PopulateRecipeTemplates7();
        }

        private static void PopulateStepTemplates7()
        {
            double chargeRate = 1;
            CreateStepTemplate(25, chargeRate, CurrentUnitEnum.C, 1, CutOffConditionTypeEnum.CRate);
            CreateStepTemplate(25, -1, CurrentUnitEnum.C, 0, CutOffConditionTypeEnum.C_mAH);
        }

        private static void PopulateRecipeTemplates7()
        {
            double chargeRate = 1;
            int restTime = 3600;
            ushort loop = 500;
            double t = 25;
            double c = -1;
            int order = 0;
            using (var dbContext = new AppDbContext())
            {
                RecipeTemplate obj = new RecipeTemplate();
                obj.Name = $"25 deg {chargeRate}C charge, {t} deg {c}C discharge *{loop}";

                var newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 0);
                newStep.LoopTarget = "a";
                newStep.LoopCount = loop;
                obj.Steps.Add(newStep);

                dbContext.RecipeTemplates.Add(obj);
                dbContext.SaveChanges();
            }
        }
        #endregion
        #region program 8
        private static void PopulateProgram8()
        {
            PopulateStepTemplates8();
            PopulateRecipeTemplates8();
        }

        private static void PopulateStepTemplates8()
        {
        }

        private static void PopulateRecipeTemplates8()
        {
            double chargeRate = 0.2;
            int restTime = 3600;
            ushort loop = 5;


            double curr = -500;
            using (var dbContext = new AppDbContext())
            {
                RecipeTemplate obj = new RecipeTemplate();
                obj.Name = $"25 deg {chargeRate}C charge, 25 deg {curr}mA discharge *{loop}";

                var newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == curr && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
                newStep.LoopTarget = "a";
                newStep.LoopCount = loop;
                obj.Steps.Add(newStep);

                dbContext.RecipeTemplates.Add(obj);
                dbContext.SaveChanges();
            }
            curr = -2000;
            using (var dbContext = new AppDbContext())
            {
                RecipeTemplate obj = new RecipeTemplate();
                obj.Name = $"25 deg {chargeRate}C charge, 25 deg {curr}mA discharge *{loop}";

                var newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == curr && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
                newStep.LoopTarget = "a";
                newStep.LoopCount = loop;
                obj.Steps.Add(newStep);

                dbContext.RecipeTemplates.Add(obj);
                dbContext.SaveChanges();
            }


            List<double> tPoints = new List<double>() { 25 };
            List<string> cPoints = new List<string>() { "D03" };
            Dictionary<String, double[]> dic = new Dictionary<string, double[]>();
            dic.Add("D03", new double[] { -200, -100, -2000 });

            foreach (var t in tPoints)
            {
                foreach (var c in cPoints)
                {
                    int order = 0;
                    using (var dbContext = new AppDbContext())
                    {
                        RecipeTemplate obj = new RecipeTemplate();
                        obj.Name = $"25 deg {chargeRate}C charge, {t} deg {c} discharge *{loop}";

                        var newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == chargeRate &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.C &&
                                                                                        o.CutOffConditionValue == 1 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                        newStep.LoopLabel = "b";
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t &&
                                                                                        o.CurrentInput == dic[c][0] &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.mA &&
                                                                                        o.CutOffConditionValue == 900 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        newStep.LoopLabel = "a";
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t &&
                                                                                        o.CurrentInput == dic[c][1] &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.mA &&
                                                                                        o.CutOffConditionValue == 1200 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t &&
                                                                                        o.CurrentInput == dic[c][2] &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.mA &&
                                                                                        o.CutOffConditionValue == 600 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        newStep.LoopTarget = "a";
                        newStep.CompareMark = CompareMarkEnum.LargerThan;
                        newStep.CRate = 0;
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        newStep.LoopTarget = "b";
                        newStep.LoopCount = loop;
                        obj.Steps.Add(newStep);

                        dbContext.RecipeTemplates.Add(obj);
                        dbContext.SaveChanges();
                    }
                }
            }
        }
        #endregion
        #region program 9
        private static void PopulateProgram9()
        {
            PopulateStepTemplates9();
            PopulateRecipeTemplates9();
        }

        private static void PopulateStepTemplates9()
        {
            CreateStepTemplate(25, 0, CurrentUnitEnum.mA, 1800, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -200, CurrentUnitEnum.mA, 60, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -500, CurrentUnitEnum.mA, 60, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -800, CurrentUnitEnum.mA, 60, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -1000, CurrentUnitEnum.mA, 60, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -1500, CurrentUnitEnum.mA, 60, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -2000, CurrentUnitEnum.mA, 60, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(0, 0, CurrentUnitEnum.mA, 0, CutOffConditionTypeEnum.Time_s);
        }

        private static void PopulateRecipeTemplates9()
        {
            double chargeRate = 0.2;
            int restTime = 1800;
            ushort loop = 3;
            double t = 25;
            double[] cPoints = new double[] { -200, -500, -800, -1000, -1500, -2000 };
            int order = 0;
            using (var dbContext = new AppDbContext())
            {
                RecipeTemplate obj = new RecipeTemplate();
                obj.Name = $"25 deg {chargeRate}C charge, {t} deg D04 discharge *{loop}";

                var newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 0 && o.CurrentInput == 0 && o.CutOffConditionValue == 0 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopLabel = "b";
                obj.Steps.Add(newStep);

                foreach (var c in cPoints)
                {
                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 60 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    newStep.LoopTarget = "c";
                    newStep.CompareMark = CompareMarkEnum.SmallThan;
                    newStep.CRate = 0;
                    obj.Steps.Add(newStep);
                }

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 0 && o.CurrentInput == 0 && o.CutOffConditionValue == 0 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopTarget = "b";
                newStep.CompareMark = CompareMarkEnum.LargerThan;
                newStep.CRate = 0;
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 0 && o.CurrentInput == 0 && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopTarget = "a";
                newStep.LoopLabel = "c";
                newStep.LoopCount = loop;
                obj.Steps.Add(newStep);

                dbContext.RecipeTemplates.Add(obj);
                dbContext.SaveChanges();
            }
        }
        #endregion
        #region program 10
        private static void PopulateProgram10()
        {
            PopulateStepTemplates10();
            PopulateRecipeTemplates10();
        }

        private static void PopulateStepTemplates10()
        {
            CreateStepTemplate(25, 0.5, CurrentUnitEnum.C, 1, CutOffConditionTypeEnum.CRate);
            CreateStepTemplate(25, -2000, CurrentUnitEnum.mA, 300, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -200, CurrentUnitEnum.mA, 600, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -1000, CurrentUnitEnum.mA, 300, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -800, CurrentUnitEnum.mA, 600, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(25, -1500, CurrentUnitEnum.mA, 300, CutOffConditionTypeEnum.Time_s);
        }

        private static void PopulateRecipeTemplates10()
        {
            double chargeRate1 = 0.5;
            double chargeRate2 = 1;
            int restTime = 1800;
            double t = 25;
            int order = 0;
            using (var dbContext = new AppDbContext())
            {
                RecipeTemplate obj = new RecipeTemplate();
                obj.Name = $"25 deg charge, {t} deg D05 discharge";

                var newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate1 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == -2000 && o.CutOffConditionValue == 300 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == -200 && o.CutOffConditionValue == 600 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == -1000 && o.CutOffConditionValue == 300 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopTarget = "a";
                newStep.CompareMark = CompareMarkEnum.LargerThan;
                newStep.CRate = 0.2;
                obj.Steps.Add(newStep);


                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate2 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == -800 && o.CutOffConditionValue == 600 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopLabel = "b";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == -2000 && o.CutOffConditionValue == 300 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == -500 && o.CutOffConditionValue == 900 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopTarget = "b";
                newStep.CompareMark = CompareMarkEnum.LargerThan;
                newStep.CRate = 0.2;
                obj.Steps.Add(newStep);



                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate2 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == -1500 && o.CutOffConditionValue == 300 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopLabel = "c";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == -200 && o.CutOffConditionValue == 600 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == -2000 && o.CutOffConditionValue == 300 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopTarget = "c";
                newStep.CompareMark = CompareMarkEnum.LargerThan;
                newStep.CRate = 0.2;
                obj.Steps.Add(newStep);

                dbContext.RecipeTemplates.Add(obj);
                dbContext.SaveChanges();
            }
        }
        #endregion

        #region program 12
        private static void PopulateProgram12()
        {
            PopulateStepTemplates12();
            PopulateRecipeTemplates12();
        }

        private static void PopulateStepTemplates12()
        {
            //int restTime1 = 1800;
            int restTime2 = 2400;

            //CreateStepTemplate(25, 0, CurrentUnitEnum.mA, restTime1, CutOffConditionTypeEnum.Time_s);

            CreateStepTemplate(25, 0, CurrentUnitEnum.mA, restTime2, CutOffConditionTypeEnum.Time_s);

            CreateStepTemplate(25, -1, CurrentUnitEnum.C, 0.2, CutOffConditionTypeEnum.CRate);
        }

        private static void PopulateRecipeTemplates12()
        {
            double chargeRate = 1;
            int restTime1 = 1800;
            int restTime2 = 2400;
            ushort loop = 2;
            double t = 25;
            double c = -1;
            int order = 0;
            using (var dbContext = new AppDbContext())
            {
                RecipeTemplate obj = new RecipeTemplate();
                obj.Name = $"25 deg {chargeRate}C charge, {t} deg {c}C discharge *{loop}";

                var newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime1 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopTarget = "a";
                newStep.LoopCount = loop;
                obj.Steps.Add(newStep);

                dbContext.RecipeTemplates.Add(obj);
                dbContext.SaveChanges();
            }
        }
        #endregion

        #region program 14
        private static void PopulateProgram14()
        {
            PopulateStepTemplates14();
            PopulateRecipeTemplates14();
        }

        private static void PopulateStepTemplates14()
        {
            CreateStepTemplate(25, -1995, CurrentUnitEnum.mA, 0.2, CutOffConditionTypeEnum.CRate);
        }

        private static void PopulateRecipeTemplates14()
        {
            double chargeRate = 1;
            int restTime1 = 1800;
            int restTime2 = 2400;
            ushort loop = 2;
            double t = 25;
            double c = -1995;
            int order = 0;
            using (var dbContext = new AppDbContext())
            {
                RecipeTemplate obj = new RecipeTemplate();
                obj.Name = $"25 deg {chargeRate}C charge, {t} deg {c}C discharge *{loop}";

                var newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime1 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);
                newStep.Order = order++;

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);
                newStep.Order = order++;

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);
                newStep.Order = order++;

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);
                newStep.Order = order++;

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopTarget = "a";
                newStep.LoopCount = loop;
                obj.Steps.Add(newStep);
                newStep.Order = order++;

                dbContext.RecipeTemplates.Add(obj);
                dbContext.SaveChanges();
            }
        }
        #endregion
        #region program 15
        private static void PopulateProgram15()
        {
            PopulateStepTemplates15();
            PopulateRecipeTemplates15();
        }

        private static void PopulateStepTemplates15()
        {
            CreateStepTemplate(-5, -1995, CurrentUnitEnum.mA, 0.2, CutOffConditionTypeEnum.CRate);
        }

        private static void PopulateRecipeTemplates15()
        {
            double chargeRate = 1;
            int restTime1 = 1800;
            int restTime2 = 3600;
            ushort loop = 2;
            double t1 = 25;
            double t2 = -5;
            double c = -1995;
            int order = 0;
            using (var dbContext = new AppDbContext())
            {
                RecipeTemplate obj = new RecipeTemplate();
                obj.Name = $"25 deg {chargeRate}C charge, {t2} deg {c}C discharge *{loop}";

                var newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t1 && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t1 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime1 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t1 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t1 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t2 && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t1 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopTarget = "a";
                newStep.LoopCount = loop;
                obj.Steps.Add(newStep);

                dbContext.RecipeTemplates.Add(obj);
                dbContext.SaveChanges();
            }
        }
        #endregion
        #region program 16
        private static void PopulateProgram16()
        {
            PopulateStepTemplates16();
            PopulateRecipeTemplates16();
        }

        private static void PopulateStepTemplates16()
        {
            int restTime1 = 1800;
            int restTime2 = 3600;
            double chargeCurrent = 1000;
            CreateStepTemplate(-5, 0, CurrentUnitEnum.mA, restTime1, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(-5, 0, CurrentUnitEnum.mA, restTime2, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(-5, chargeCurrent, CurrentUnitEnum.mA, 1, CutOffConditionTypeEnum.CRate);
        }

        private static void PopulateRecipeTemplates16()
        {
            int restTime1 = 1800;
            int restTime2 = 3600;
            double chargeCurrent = 1000;
            ushort loop = 2;
            double t = -5;
            double c = -1995;
            int order = 0;
            using (var dbContext = new AppDbContext())
            {
                RecipeTemplate obj = new RecipeTemplate();
                obj.Name = $"{t} deg {chargeCurrent} mA charge, {t} deg {c}C discharge *{loop}";

                var newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);


                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == 0 && o.CutOffConditionValue == restTime1 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == chargeCurrent && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopTarget = "a";
                newStep.LoopCount = loop;
                obj.Steps.Add(newStep);

                dbContext.RecipeTemplates.Add(obj);
                dbContext.SaveChanges();
            }
        }
        #endregion
        #region program 17
        private static void PopulateProgram17()
        {
            PopulateStepTemplates17();
            PopulateRecipeTemplates17();
        }

        private static void PopulateStepTemplates17()
        {
            int restTime1 = 2400;
            double t1 = 40;
            double t2 = -15;
            //int restTime2 = 3600;
            double c = -1995;

            CreateStepTemplate(t1, 0, CurrentUnitEnum.mA, restTime1, CutOffConditionTypeEnum.Time_s);

            CreateStepTemplate(t1, c, CurrentUnitEnum.mA, 0.2, CutOffConditionTypeEnum.CRate);

            CreateStepTemplate(t2, 0, CurrentUnitEnum.mA, restTime1, CutOffConditionTypeEnum.Time_s);

            CreateStepTemplate(t2, c, CurrentUnitEnum.mA, 0.2, CutOffConditionTypeEnum.CRate);
        }

        private static void PopulateRecipeTemplates17()
        {
            int restTime1 = 3600;
            int restTime2 = 2400;
            double chargeRate = 1;
            double t1 = 40;
            double t2 = -15;
            double c = -1995;
            int order = 0;
            using (var dbContext = new AppDbContext())
            {
                RecipeTemplate obj = new RecipeTemplate();
                obj.Name = $"25 deg {chargeRate}C charge, {t1} deg {c} mA discharge";

                var newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime1 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t1 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t1 && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);

                dbContext.RecipeTemplates.Add(obj);




                order = 0;
                obj = new RecipeTemplate();
                obj.Name = $"25 deg {chargeRate}C charge, {t2} deg {c} mA discharge";

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime1 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t2 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t2 && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);

                dbContext.RecipeTemplates.Add(obj);
                dbContext.SaveChanges();
            }
        }
        #endregion
        #region program 18
        private static void PopulateProgram18()
        {
            PopulateStepTemplates18();
            PopulateRecipeTemplates18();
        }

        private static void PopulateStepTemplates18()
        {
            int restTime1 = 2400;
            double t = 0;
            double c = -1995;

            CreateStepTemplate(t, 0, CurrentUnitEnum.mA, restTime1, CutOffConditionTypeEnum.Time_s);

            CreateStepTemplate(t, c, CurrentUnitEnum.mA, 0.2, CutOffConditionTypeEnum.CRate);
        }

        private static void PopulateRecipeTemplates18()
        {
            int restTime1 = 3600;
            int restTime2 = 2400;
            double chargeRate = 1;
            double t = 0;
            double c = -1995;
            int order = 0;
            using (var dbContext = new AppDbContext())
            {
                RecipeTemplate obj = new RecipeTemplate();
                obj.Name = $"25 deg {chargeRate}C charge, {t} deg {c} mA discharge";

                var newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime1 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);

                dbContext.RecipeTemplates.Add(obj);
                dbContext.SaveChanges();
            }
        }
        #endregion


        private static void PopulateTestPrograms()
        {
            int index = 1;
            using (var dbContext = new AppDbContext())//1
            {
                int number = 9;
                BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
                var pro = new ProgramClass();
                pro.Name = "Oppo BLP663 Static Test";
                pro.Project = dbContext.Projects.SingleOrDefault(o => o.Customer == "O2Micro");
                pro.Requester = "Francis";
                pro.RequestTime = DateTime.Parse("2019/02/28");

                RecipeTemplate recTemp;
                RecipeClass rec;

                for (int i = 0; i < number; i++, index++)
                {
                    recTemp = GetRecipeTemplateById(dbContext, index);
                    rec = new RecipeClass(recTemp, bType);
                    pro.Recipes.Add(rec);
                }

                dbContext.Programs.Add(pro);
                dbContext.SaveChanges();
            }
            //        using (var dbContext = new AppDbContext())//2
            //        {
            //            int number = 6;
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "Oppo BLP663 Dynamic Test";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Francis";
            //            pro.RequestTime = DateTime.Parse("2019/02/28");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;


            //            for (int i = 0; i < number; i++, index++)
            //            {
            //                recTemp = GetRecipeTemplateById(dbContext, index);
            //                rec = new RecipeClass(recTemp, bType);
            //                pro.Recipes.Add(rec);
            //            }

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//3
            //        {
            //            int number = 6;
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "Oppo BLP663 RC";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Francis";
            //            pro.RequestTime = DateTime.Parse("2019/02/28");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;


            //            for (int i = 0; i < number; i++, index++)
            //            {
            //                recTemp = GetRecipeTemplateById(dbContext, index);
            //                rec = new RecipeClass(recTemp, bType);
            //                pro.Recipes.Add(rec);
            //            }

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//4
            //        {
            //            int number = 3;
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "Oppo BLP663 Static Test-2";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Francis";
            //            pro.RequestTime = DateTime.Parse("2019/03/12");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;


            //            for (int i = 0; i < number; i++, index++)
            //            {
            //                recTemp = GetRecipeTemplateById(dbContext, index);
            //                rec = new RecipeClass(recTemp, bType);
            //                pro.Recipes.Add(rec);
            //            }

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//5
            //        {
            //            int number = 3;
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "Oppo BLP663 Dynamic Test-2";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Francis";
            //            pro.RequestTime = DateTime.Parse("2019/03/14");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;


            //            for (int i = 0; i < number; i++, index++)
            //            {
            //                recTemp = GetRecipeTemplateById(dbContext, index);
            //                rec = new RecipeClass(recTemp, bType);
            //                pro.Recipes.Add(rec);
            //            }

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//6
            //        {
            //            int number = 1;
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "Oppo BLP663 Static Test-3";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Francis";
            //            pro.RequestTime = DateTime.Parse("2019/03/14");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;


            //            for (int i = 0; i < number; i++, index++)
            //            {
            //                recTemp = GetRecipeTemplateById(dbContext, index);
            //                rec = new RecipeClass(recTemp, bType);
            //                pro.Recipes.Add(rec);
            //            }

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//7
            //        {
            //            int number = 1;
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "500 Cycles";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Francis";
            //            pro.RequestTime = DateTime.Parse("2019/04/01");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;


            //            for (int i = 0; i < number; i++, index++)
            //            {
            //                recTemp = GetRecipeTemplateById(dbContext, index);
            //                rec = new RecipeClass(recTemp, bType);
            //                pro.Recipes.Add(rec);
            //            }

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//8
            //        {
            //            int number = 3;
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "Composite Test";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Francis";
            //            pro.RequestTime = DateTime.Parse("2019/04/01");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;


            //            for (int i = 0; i < number; i++, index++)
            //            {
            //                recTemp = GetRecipeTemplateById(dbContext, index);
            //                rec = new RecipeClass(recTemp, bType);
            //                pro.Recipes.Add(rec);
            //            }

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//9
            //        {
            //            int number = 1;
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "Oppo BLP663 Dynamic Test-3";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Francis";
            //            pro.RequestTime = DateTime.Parse("2019/05/07");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;


            //            for (int i = 0; i < number; i++, index++)
            //            {
            //                recTemp = GetRecipeTemplateById(dbContext, index);
            //                rec = new RecipeClass(recTemp, bType);
            //                pro.Recipes.Add(rec);
            //            }

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//10
            //        {
            //            int number = 1;
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "Oppo BLP663 Dynamic Test-4";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Francis";
            //            pro.RequestTime = DateTime.Parse("2019/05/24");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;


            //            for (int i = 0; i < number; i++, index++)
            //            {
            //                recTemp = GetRecipeTemplateById(dbContext, index);
            //                rec = new RecipeClass(recTemp, bType);
            //                pro.Recipes.Add(rec);
            //            }

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//11
            //        {
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "2000mA Test";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Jon";
            //            pro.RequestTime = DateTime.Parse("2019/07/09");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;

            //            recTemp = GetRecipeTemplateById(dbContext, 23);
            //            rec = new RecipeClass(recTemp, bType);
            //            pro.Recipes.Add(rec);

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//12
            //        {
            //            int number = 1;
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "Aging Factor";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Francis";
            //            pro.RequestTime = DateTime.Parse("2019/10/31");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;


            //            for (int i = 0; i < number; i++, index++)
            //            {
            //                recTemp = GetRecipeTemplateById(dbContext, index);
            //                rec = new RecipeClass(recTemp, bType);
            //                pro.Recipes.Add(rec);
            //            }

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//13
            //        {
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "Aging Factor 2";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Jon";
            //            pro.RequestTime = DateTime.Parse("2019/11/11");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;

            //            recTemp = GetRecipeTemplateById(dbContext, 35);
            //            rec = new RecipeClass(recTemp, bType);
            //            pro.Recipes.Add(rec);

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//14
            //        {
            //            int number = 1;
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "Aging Factor 3";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Jon";
            //            pro.RequestTime = DateTime.Parse("2019/11/13");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;


            //            for (int i = 0; i < number; i++, index++)
            //            {
            //                recTemp = GetRecipeTemplateById(dbContext, index);
            //                rec = new RecipeClass(recTemp, bType);
            //                pro.Recipes.Add(rec);
            //            }

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//15
            //        {
            //            int number = 1;
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "Aging Factor 4";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Francis";
            //            pro.RequestTime = DateTime.Parse("2019/11/14");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;


            //            for (int i = 0; i < number; i++, index++)
            //            {
            //                recTemp = GetRecipeTemplateById(dbContext, index);
            //                rec = new RecipeClass(recTemp, bType);
            //                pro.Recipes.Add(rec);
            //            }

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//16
            //        {
            //            int number = 1;
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "Aging Factor 5";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Francis";
            //            pro.RequestTime = DateTime.Parse("2019/11/15");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;


            //            for (int i = 0; i < number; i++, index++)
            //            {
            //                recTemp = GetRecipeTemplateById(dbContext, index);
            //                rec = new RecipeClass(recTemp, bType);
            //                pro.Recipes.Add(rec);
            //            }

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//17
            //        {
            //            int number = 2;
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "Aging Factor 6";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Jon";
            //            pro.RequestTime = DateTime.Parse("2019/11/25");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;


            //            for (int i = 0; i < number; i++, index++)
            //            {
            //                recTemp = GetRecipeTemplateById(dbContext, index);
            //                rec = new RecipeClass(recTemp, bType);
            //                pro.Recipes.Add(rec);
            //            }

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
            //        using (var dbContext = new AppDbContext())//18
            //        {
            //            int number = 1;
            //            BatteryTypeClass bType = dbContext.BatteryTypes.SingleOrDefault(o => o.Name == "BLP663");
            //            var pro = new ProgramClass();
            //            pro.Name = "Aging Factor 7";
            //            pro.BatteryType = bType;
            //            pro.Requester = "Jon";
            //            pro.RequestTime = DateTime.Parse("2019/11/27");

            //            RecipeTemplate recTemp;
            //            RecipeClass rec;


            //            for (int i = 0; i < number; i++, index++)
            //            {
            //                recTemp = GetRecipeTemplateById(dbContext, index);
            //                rec = new RecipeClass(recTemp, bType);
            //                pro.Recipes.Add(rec);
            //            }

            //            dbContext.Programs.Add(pro);
            //            dbContext.SaveChanges();
            //        }
        }
        #endregion
        */
        #region HG2
        private static void CreateStepTemplates()
        {
            CreateStepTemplate(0.5, CurrentUnitEnum.C, 1, CutOffConditionTypeEnum.CRate);
            CreateStepTemplate(0, CurrentUnitEnum.C, 1800, CutOffConditionTypeEnum.Time_s);
            //CreateStepTemplate(-1, CurrentUnitEnum.C, 0, CutOffConditionTypeEnum.CRate);
            CreateStepTemplate(-600, CurrentUnitEnum.mA, 0, CutOffConditionTypeEnum.CRate);

            //List<double> cPoints = new List<double>() { -2000, -3000, -6000, -10000, -14000, -17000, -19000};
            CreateDischargeStepTemplatesByCurrents(new List<double>() { -2000, -3000, -6000, -10000, -14000, -17000, -19000 });
            CreateStepTemplate(0, CurrentUnitEnum.C, 600, CutOffConditionTypeEnum.Time_s);
            CreateDischargeStepTemplatesByCurrents(new List<double>() { -2800, -9000, -11000, -15000 });
            CreateDischargeStepTemplatesByCurrentsAndTime(new List<double>() { -2800, -9000, -11000, -15000 }, 600);
            CreateStepTemplate(0, CurrentUnitEnum.C, 60, CutOffConditionTypeEnum.Time_s);

            CreateDischargeStepTemplatesByCurrentsAndTime(new List<double>() { -3000, -11000, -17000 }, 300);

            CreateStepTemplate(-0.3, CurrentUnitEnum.C, 0, CutOffConditionTypeEnum.CRate);

            CreateDischargeStepTemplatesByCurrentsAndTime(new List<double>() { -3000, -11000, -17000 }, 120);

            CreateStepTemplate(-2050, CurrentUnitEnum.mA, 0, CutOffConditionTypeEnum.CRate);
            CreateStepTemplate(-2050, CurrentUnitEnum.mA, 600, CutOffConditionTypeEnum.Time_s);
        }
        private static void CreateRecipeTemplateGroup(List<double> cPoints, int restTime, double chargeRate)
        {
            foreach (var c in cPoints)
            {
                using (var dbContext = new AppDbContext())
                {
                    RecipeTemplate obj = new RecipeTemplate();
                    obj.Name = $"{c / -1000.0}A";

                    var newStep = new StepClass();
                    //newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    //newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    //newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    dbContext.RecipeTemplates.Add(obj);
                    dbContext.SaveChanges();
                }
            }
        }
        private static void CreateStaticRecipeTemplateGroup(string recipeName, List<double> cPoints, ushort loop)
        {
            foreach (var c in cPoints)
            {
                using (var dbContext = new AppDbContext())
                {
                    RecipeTemplate obj = new RecipeTemplate();
                    obj.Name = $"{recipeName}-{c / -1000.0}A";

                    var newStep = new StepClass();
                    //newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.CurrentInput == 0.5 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    if (loop > 1)
                        newStep.LoopLabel = "a";
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.CurrentInput == 0 && o.CutOffConditionValue == 600 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    //newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 600 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.CurrentInput == 0 && o.CutOffConditionValue == 60 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    if (loop > 1)
                    {
                        newStep.LoopTarget = "a";
                        newStep.LoopCount = loop;
                    }
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    //newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(o => o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    dbContext.RecipeTemplates.Add(obj);
                    dbContext.SaveChanges();
                }
            }
        }
        private static void CreateRecipeTemplates()
        {
            CreateRecipeTemplateGroup(new List<double>() { -600, -900, -2000, -3000, -6000, -10000, -14000, -17000, -19000 }, 1800, 0.5);
            CreateRecipeTemplateGroup(new List<double>() { -2800, -9000, -11000, -15000 }, 600, 0.5);
            CreateStaticRecipeTemplateGroup("Static2", new List<double>() { -2800, -9000, -11000, -15000 },5);   //static2
            CreateDynamicRecipeTemplateGroup("Dynamic1", new List<List<double>>() { new List<double>() { -3000, -11000, -17000 }, new List<double>() { -17000, -11000, -3000 }, new List<double>() { -11000, -17000, -3000 }, new List<double>() { -3000, -17000, -11000 } }, 5);
            CreateDynamicRecipeTemplateGroup("Dynamic2", new List<List<double>>() { new List<double>() { -3000, -11000, -17000 }, new List<double>() { -17000, -11000, -3000 }, new List<double>() { -11000, -17000, -3000 }, new List<double>() { -3000, -17000, -11000 } }, 1);
            CreateStaticRecipeTemplateGroup("Static4", new List<double>() { -2800, -9000, -11000, -15000 }, 1);   //static2
        }

        private static void CreateDynamicRecipeTemplateGroup(string recipeName, List<List<double>> list, ushort loop)
        {
            foreach (var cPoints in list)
            {
                using (var dbContext = new AppDbContext())
                {
                    RecipeTemplate obj = new RecipeTemplate();
                    obj.Name = $"{recipeName}-{cPoints[0] / -1000.0}A-{cPoints[1] / -1000.0}A-{cPoints[2] / -1000.0}A";


                    var newStep = new StepClass();
                    newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    if (loop > 1)
                        newStep.LoopLabel = "a";
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(
                        o => o.CurrentInput == 0.5 &&
                        o.CurrentUnit == CurrentUnitEnum.C &&
                        o.CutOffConditionValue == 1 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(
                        o => o.CurrentInput == cPoints[0] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 300 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(
                        o => o.CurrentInput == cPoints[1] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 300 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.StepTemplate = dbContext.StepTemplates.SingleOrDefault(
                        o => o.CurrentInput == cPoints[2] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 0 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    if (loop > 1)
                    {
                        newStep.LoopTarget = "a";
                        newStep.LoopCount = 5;
                    }
                    obj.Steps.Add(newStep);

                    dbContext.RecipeTemplates.Add(obj);
                    dbContext.SaveChanges();
                }
            }
        }
        #endregion

        private static void CreateStepTemplate(double ci, CurrentUnitEnum cu, double cocv, CutOffConditionTypeEnum coct)
        {
            using (var dbContext = new AppDbContext())
            {
                if (!dbContext.StepTemplates.Any(o => o.CurrentInput == ci && o.CurrentUnit == cu && o.CutOffConditionValue == cocv && o.CutOffConditionType == coct))
                {
                    StepTemplate output = new StepTemplate();
                    output.CurrentInput = ci;
                    output.CurrentUnit = cu;
                    output.CutOffConditionValue = cocv;
                    output.CutOffConditionType = coct;
                    dbContext.StepTemplates.Add(output);
                    dbContext.SaveChanges();
                }
            }
        }

        private static void CreateDischargeStepTemplatesByCurrents(List<double> cPoints)
        {
            foreach (var c in cPoints)
            {
                CreateStepTemplate(c, CurrentUnitEnum.mA, 0, CutOffConditionTypeEnum.CRate);
            }
        }

        private static void CreateDischargeStepTemplatesByCurrentsAndTime(List<double> cPoints, double time)
        {
            foreach (var c in cPoints)
            {
                CreateStepTemplate(c, CurrentUnitEnum.mA, time, CutOffConditionTypeEnum.Time_s);
            }
        }

        private static RecipeTemplate GetRecipeTemplateById(AppDbContext dbContext, int id)
        {
            var subtemplate = dbContext.RecipeTemplates.SingleOrDefault(o => o.Id == id);
            dbContext.Entry(subtemplate)
                .Collection(o => o.Steps)
                .Load();

            foreach (var step in subtemplate.Steps)
            {
                dbContext.Entry(step)
                    .Reference(o => o.StepTemplate)
                    .Load();
            }
            return subtemplate;
        }
    }
}
