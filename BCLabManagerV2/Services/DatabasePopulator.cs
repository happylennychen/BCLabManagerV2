using BCLabManager.DataAccess;
using BCLabManager.Model;
using BCLabManager.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            CreatePrograms();
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
            PopulateProgramTypes();
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
            bt.Manufacturer = "Oppo";
            bt.Material = "Li-ion Plymer Battery";
            bt.LimitedChargeVoltage = 4400;
            bt.RatedCapacity = 3365;
            bt.NominalVoltage = 3850;
            bt.TypicalCapacity = 3450;
            bt.CutoffDischargeVoltage = 3200;
            PopulateOneBatteryType(bt);

            bt = new BatteryTypeClass();
            bt.Name = "32700-6000mAh";
            bt.Manufacturer = "FbTech";
            bt.Material = "lithium-ion";
            bt.LimitedChargeVoltage = 3650;
            bt.RatedCapacity = 6000;
            bt.NominalVoltage = 3200;
            bt.TypicalCapacity = 6000;
            bt.CutoffDischargeVoltage = 2000;
            PopulateOneBatteryType(bt);

            bt = new BatteryTypeClass();
            bt.Name = "INR18650-25R";
            bt.Manufacturer = "SamSung SDI Co., Ltd";
            bt.Material = "lithium-ion";
            bt.LimitedChargeVoltage = 4200;
            bt.RatedCapacity = 2500;
            bt.NominalVoltage = 3600;
            bt.TypicalCapacity = 2450;
            bt.CutoffDischargeVoltage = 2500;
            PopulateOneBatteryType(bt);

            bt = new BatteryTypeClass();
            bt.Name = "H26";
            bt.Manufacturer = "LG";
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
            bt.Manufacturer = "LG";
            bt.Material = "lithium-ion";
            bt.LimitedChargeVoltage = 4200;
            bt.RatedCapacity = 3000;
            bt.TypicalCapacity = 3000;
            bt.NominalVoltage = 3600;
            bt.CutoffDischargeVoltage = 2500;
            bt.FullyChargedEndCurrent = 50;
            bt.FullyChargedEndingTimeout = 0;
            PopulateOneBatteryType(bt);

            bt = new BatteryTypeClass();
            bt.Name = "M26";
            bt.Manufacturer = "LG";
            bt.Material = "lithium-ion";
            bt.LimitedChargeVoltage = 4200;
            bt.RatedCapacity = 2600;
            bt.TypicalCapacity = 2500;
            bt.NominalVoltage = 3650;
            bt.CutoffDischargeVoltage = 2750;
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
            PopulateOneProject("High Power 0", "HG2", "O2Micro", "", "", 2500);
            PopulateOneProject("High Power", "HG2", "O2Micro", "", "", 3000);
            PopulateOneProject("O2Sim1", "M26", "O2Micro", "", "", 3000);
            //PopulateOneProject("High Power 2", "HG2", "O2Micro", "Change Cut off voltage from 2.5v to 3v", "");
        }
        private static void PopulateOneProject(string name, string batteryType, string customer, string description, string voltagePoints, int codv)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                ProjectClass proj = new ProjectClass();
                proj.Name = name;
                proj.Customer = customer;
                proj.Description = description;
                proj.VoltagePoints = voltagePoints;
                proj.BatteryType = uow.BatteryTypes.SingleOrDefault(o => o.Name == batteryType);
                //proj.CutoffDischargeVoltage = proj.BatteryType.CutoffDischargeVoltage;
                proj.CutoffDischargeVoltage = codv;
                proj.LimitedChargeVoltage = proj.BatteryType.LimitedChargeVoltage;
                //proj.AbsoluteMaxCapacity = proj.BatteryType.RatedCapacity;    //应该是实验之后才知道
                //var evSetting = new EvSettingClass();
                //evSetting.DischargeEndVoltage = proj.BatteryType.CutoffDischargeVoltage;
                //evSetting.FullyChargedEndCurrent = proj.BatteryType.FullyChargedEndCurrent;
                //evSetting.FullyChargedEndingTimeout = proj.BatteryType.FullyChargedEndingTimeout;
                //evSetting.LimitedChargeVoltage = proj.BatteryType.LimitedChargeVoltage;
                //evSetting.DesignCapacity = proj.BatteryType.TypicalCapacity;
                //proj.EvSettings.Add(evSetting);
                uow.Projects.Insert(proj);
                uow.Commit();
            }
            var service = new ProjectServiceClass();
            service.CreateFolder(batteryType, name);
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
            //            bat.BatteryType = uow.BatteryTypes.Single(o => o.Id == btId);
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
            for (int i = 1; i < 15; i++)
            {
                b = new BatteryClass();
                b.Name = "HG2-" + i.ToString();
                PopulateOneBattery(b, 5);
            }
            for (int i = 1; i < 11; i++)
            {
                b = new BatteryClass();
                b.Name = "M26-" + i.ToString();
                PopulateOneBattery(b, 6);
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
                tst.Manufacturer = "Chroma";
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
                cmb.Manufacturer = "HongZhan";
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
        private static void PopulateProgramTypes()
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                ProgramTypeClass pt = new ProgramTypeClass();
                pt.Name = "RC";
                pt.Description = "Standard RC Program";
                uow.ProgramTypes.Insert(pt);
                uow.Commit();
            }
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                ProgramTypeClass pt = new ProgramTypeClass();
                pt.Name = "OCV";
                pt.Description = "Standard OCV Program";
                uow.ProgramTypes.Insert(pt);
                uow.Commit();
            }
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                ProgramTypeClass pt = new ProgramTypeClass();
                pt.Name = "EV";
                pt.Description = "EV Program, used by Emulator";
                uow.ProgramTypes.Insert(pt);
                uow.Commit();
            }
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                ProgramTypeClass pt = new ProgramTypeClass();
                pt.Name = "MISC";
                pt.Description = "Miscellaneous Program";
                uow.ProgramTypes.Insert(pt);
                uow.Commit();
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
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
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
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == chargeRate &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.C &&
                                                                                        o.CutOffConditionValue == 1 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t &&
                                                                                        o.CurrentInput == dic[c][0] &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.mA &&
                                                                                        o.CutOffConditionValue == 900 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        newStep.LoopLabel = "a";
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t &&
                                                                                        o.CurrentInput == dic[c][1] &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.mA &&
                                                                                        o.CutOffConditionValue == 1200 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t &&
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
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
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
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
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
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == chargeRate &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.C &&
                                                                                        o.CutOffConditionValue == 1 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t &&
                                                                                        o.CurrentInput == dic[c][0] &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.mA &&
                                                                                        o.CutOffConditionValue == 900 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        newStep.LoopLabel = "a";
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t &&
                                                                                        o.CurrentInput == dic[c][1] &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.mA &&
                                                                                        o.CutOffConditionValue == 1200 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t &&
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
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
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
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 0);
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
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == curr && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
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
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == curr && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
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
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == chargeRate &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.C &&
                                                                                        o.CutOffConditionValue == 1 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                        newStep.LoopLabel = "b";
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t &&
                                                                                        o.CurrentInput == dic[c][0] &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.mA &&
                                                                                        o.CutOffConditionValue == 900 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        newStep.LoopLabel = "a";
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t &&
                                                                                        o.CurrentInput == dic[c][1] &&
                                                                                        o.CurrentUnit == CurrentUnitEnum.mA &&
                                                                                        o.CutOffConditionValue == 1200 &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 &&
                                                                                        o.CurrentInput == 0 &&
                                                                                        o.CutOffConditionValue == restTime &&
                                                                                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                        obj.Steps.Add(newStep);

                        newStep = new StepClass();
                        newStep.Order = order++;
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t &&
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
                        newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 &&
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
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 0 && o.CurrentInput == 0 && o.CutOffConditionValue == 0 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopLabel = "b";
                obj.Steps.Add(newStep);

                foreach (var c in cPoints)
                {
                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 60 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    newStep.LoopTarget = "c";
                    newStep.CompareMark = CompareMarkEnum.SmallThan;
                    newStep.CRate = 0;
                    obj.Steps.Add(newStep);
                }

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 0 && o.CurrentInput == 0 && o.CutOffConditionValue == 0 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopTarget = "b";
                newStep.CompareMark = CompareMarkEnum.LargerThan;
                newStep.CRate = 0;
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 0 && o.CurrentInput == 0 && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
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
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate1 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == -2000 && o.CutOffConditionValue == 300 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == -200 && o.CutOffConditionValue == 600 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == -1000 && o.CutOffConditionValue == 300 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopTarget = "a";
                newStep.CompareMark = CompareMarkEnum.LargerThan;
                newStep.CRate = 0.2;
                obj.Steps.Add(newStep);


                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate2 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == -800 && o.CutOffConditionValue == 600 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopLabel = "b";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == -2000 && o.CutOffConditionValue == 300 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == -500 && o.CutOffConditionValue == 900 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopTarget = "b";
                newStep.CompareMark = CompareMarkEnum.LargerThan;
                newStep.CRate = 0.2;
                obj.Steps.Add(newStep);



                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate2 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == -1500 && o.CutOffConditionValue == 300 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                newStep.LoopLabel = "c";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == -200 && o.CutOffConditionValue == 600 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == -2000 && o.CutOffConditionValue == 300 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
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
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime1 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
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
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime1 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);
                newStep.Order = order++;

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);
                newStep.Order = order++;

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);
                newStep.Order = order++;

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);
                newStep.Order = order++;

                newStep = new StepClass();
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
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
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t1 && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t1 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime1 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t1 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t1 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t2 && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t1 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
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
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);


                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == 0 && o.CutOffConditionValue == restTime1 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == chargeCurrent && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                newStep.LoopLabel = "a";
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
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
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime1 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t1 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t1 && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
                obj.Steps.Add(newStep);

                dbContext.RecipeTemplates.Add(obj);




                order = 0;
                obj = new RecipeTemplate();
                obj.Name = $"25 deg {chargeRate}C charge, {t2} deg {c} mA discharge";

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime1 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t2 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t2 && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
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
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == 25 && o.CurrentInput == 0 && o.CutOffConditionValue == restTime1 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == 0 && o.CutOffConditionValue == restTime2 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.Coefficient.Temperature == t && o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0.2);
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
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
                var pro = new ProgramClass();
                pro.Name = "Oppo BLP663 Static Test";
                pro.Project = dbContext.Projects.Single(o => o.Customer == "O2Micro");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
            //            BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "BLP663");
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
        #region HG2 & M26
        private static void CreateStepTemplates()
        {
            CreateStepTemplate(0.5, CurrentUnitEnum.C, 1, CutOffConditionTypeEnum.CRate);
            CreateStepTemplate(0, CurrentUnitEnum.C, 1800, CutOffConditionTypeEnum.Time_s);
            //CreateStepTemplate(-1, CurrentUnitEnum.C, 0, CutOffConditionTypeEnum.CRate);
            CreateStepTemplate(-600, CurrentUnitEnum.mA, 0, CutOffConditionTypeEnum.CRate);
            CreateStepTemplate(-900, CurrentUnitEnum.mA, 0, CutOffConditionTypeEnum.CRate);

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
            CreateStepTemplate(-2050, CurrentUnitEnum.mA, 300, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(-2050, CurrentUnitEnum.mA, 600, CutOffConditionTypeEnum.Time_s);


            CreateStepTemplate(-17000, CurrentUnitEnum.mA, 0.2, CutOffConditionTypeEnum.CRate);
            CreateStepTemplate(-11000, CurrentUnitEnum.mA, 0.2, CutOffConditionTypeEnum.CRate);
            CreateStepTemplate(-3000, CurrentUnitEnum.mA, 60, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(-11000, CurrentUnitEnum.mA, 60, CutOffConditionTypeEnum.Time_s);
            CreateStepTemplate(-17000, CurrentUnitEnum.mA, 60, CutOffConditionTypeEnum.Time_s);

            CreateStepTemplate(0.5, CurrentUnitEnum.C, 0.8, CutOffConditionTypeEnum.CRate);

            //CreateStepTemplate(-17000, CurrentUnitEnum.mA, 60, CutOffConditionTypeEnum.Time_s);
            //CreateStepTemplate(-3000, CurrentUnitEnum.mA, 60, CutOffConditionTypeEnum.Time_s);
            //CreateStepTemplate(-11000, CurrentUnitEnum.mA, 60, CutOffConditionTypeEnum.Time_s);
            //CreateStepTemplate(-17000, CurrentUnitEnum.mA, 30, CutOffConditionTypeEnum.Time_s);
            //CreateStepTemplate(-3000, CurrentUnitEnum.mA, 30, CutOffConditionTypeEnum.Time_s);
        }
        private static void CreateRecipeTemplateGroup(List<double> cPoints, int restTime, double chargeRate)
        {
            foreach (var c in cPoints)
            {
                using (var dbContext = new AppDbContext())
                {
                    int order = 1;
                    RecipeTemplate obj = new RecipeTemplate();
                    obj.Name = $"{c / -1000.0}A";

                    var newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == chargeRate && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0 && o.CutOffConditionValue == restTime && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    dbContext.RecipeTemplates.Add(obj);
                    dbContext.SaveChanges();
                }
            }
        }
        private static void CreateStaticRecipeTemplateGroup(List<double> cPoints, ushort loop, int index)
        {
            foreach (var c in cPoints)
            {
                using (var dbContext = new AppDbContext())
                {
                    int order = 1;
                    RecipeTemplate obj = new RecipeTemplate();
                    obj.Name = $"{c / -1000.0}A-N{index}";

                    var newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0.5 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    if (loop > 1)
                        newStep.LoopLabel = "a";
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0 && o.CutOffConditionValue == 600 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 600 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0 && o.CutOffConditionValue == 60 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    if (loop > 1)
                    {
                        newStep.LoopTarget = "a";
                        newStep.LoopCount = loop;
                    }
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == c && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    dbContext.RecipeTemplates.Add(obj);
                    dbContext.SaveChanges();
                }
            }
        }

        private static void CreateExtraTemplate()
        {
            using (var dbContext = new AppDbContext())
            {
                int order = 1;
                RecipeTemplate obj = new RecipeTemplate();
                obj.Name = $"Extra";

                var newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0.5 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0 && o.CutOffConditionValue == 600 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == -3000 && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0 && o.CutOffConditionValue == 600 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                obj.Steps.Add(newStep);

                newStep = new StepClass();
                newStep.Order = order++;
                newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0.5 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 0.8 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                obj.Steps.Add(newStep);

                dbContext.RecipeTemplates.Add(obj);
                dbContext.SaveChanges();
            }
        }
        private static void CreateRecipeTemplates()
        {
            CreateRecipeTemplateGroup(new List<double>() { -600, -900, -2000, -3000, -6000, -10000, -14000, -17000, -19000 }, 1800, 0.5); //RC-01 OCV
            CreateRecipeTemplateGroup(new List<double>() { -2800, -9000, -11000, -15000 }, 600, 0.5);   //static 1
            CreateStaticRecipeTemplateGroup(new List<double>() { -2800, -9000, -11000, -15000 }, 5, 2);   //static2 3
            CreateDynamicRecipeTemplateGroup(new List<List<double>>() { new List<double>() { -3000, -11000, -17000 }, new List<double>() { -17000, -11000, -3000 }, new List<double>() { -11000, -17000, -3000 }, new List<double>() { -3000, -17000, -11000 } }, 5, 1);   //dynamic 1
            CreateExtraTemplate();
            CreateDynamicRecipeTemplateGroup(new List<List<double>>() { new List<double>() { -3000, -11000, -17000 }, new List<double>() { -17000, -11000, -3000 }, new List<double>() { -11000, -17000, -3000 }, new List<double>() { -3000, -17000, -11000 } }, 1, 2);   //dynamic 2
            CreateStaticRecipeTemplateGroup(new List<double>() { -2800, -9000, -11000, -15000 }, 1, 3);   //static4

            CreateDynamic3RecipeTemplateGroup(new List<List<double>>() { new List<double>() { -3000, -11000, -17000 }, new List<double>() { -17000, -11000, -3000 }, new List<double>() { -11000, -17000, -3000 }, new List<double>() { -3000, -17000, -11000 } });
            CreateDynamic4RecipeTemplateGroup(new List<List<double>>() { new List<double>() { -3000, -11000, -17000 }, new List<double>() { -17000, -11000, -3000 }, new List<double>() { -11000, -17000, -3000 }, new List<double>() { -3000, -17000, -11000 } });
            CreateDynamic5RecipeTemplateGroup(new List<List<double>>() { new List<double>() { -3000, -11000, -17000 }, new List<double>() { -17000, -11000, -3000 }, new List<double>() { -11000, -17000, -3000 }, new List<double>() { -3000, -17000, -11000 } });

            CreateDynamic6RecipeTemplateGroup(new List<List<double>>() { new List<double>() { -17000, -3000 }, new List<double>() { -11000, -3000 }, new List<double>() { -11000, -11000 }, new List<double>() { -17000, -17000 } });
            CreateDynamic12RecipeTemplateGroup(new List<List<double>>() { new List<double>() { -17000, -3000, 60 }, new List<double>() { -11000, -3000, 60 }, new List<double>() { -17000, -11000, 60 }, new List<double>() { -17000, -3000, 30 } });
            CreateDynamic13RecipeTemplateGroup(new List<List<double>>() { new List<double>() { -17000, -3000, 10 }, new List<double>() { -17000, -3000, 5 }, new List<double>() { -6000, -3000, 10 }, new List<double>() { -6000, -3000, 5 } });
        }

        private static void CreateDynamicRecipeTemplateGroup(List<List<double>> list, ushort loop, int index)
        {
            foreach (var cPoints in list)
            {
                using (var dbContext = new AppDbContext())
                {
                    int order = 1;
                    RecipeTemplate obj = new RecipeTemplate();
                    if (index == 1)
                        obj.Name = $"{cPoints[0] / -1000.0}A-{cPoints[1] / -1000.0}A-{cPoints[2] / -1000.0}A";
                    else
                        obj.Name = $"{cPoints[0] / -1000.0}A-{cPoints[1] / -1000.0}A-{cPoints[2] / -1000.0}A-N{index}";


                    var newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    if (loop > 1)
                        newStep.LoopLabel = "a";
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0.5 &&
                        o.CurrentUnit == CurrentUnitEnum.C &&
                        o.CutOffConditionValue == 1 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[0] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 300 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[1] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 300 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
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
        private static void CreateDynamic3RecipeTemplateGroup(List<List<double>> list)
        {
            foreach (var cPoints in list)
            {
                using (var dbContext = new AppDbContext())
                {
                    int order = 1;
                    RecipeTemplate obj = new RecipeTemplate();
                    obj.Name = $"{cPoints[0] / -1000.0}A-{cPoints[1] / -1000.0}A-{cPoints[2] / -1000.0}A-2.05A";


                    var newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0.5 &&
                        o.CurrentUnit == CurrentUnitEnum.C &&
                        o.CutOffConditionValue == 1 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[0] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 300 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[1] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 300 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[2] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 0 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == -2050 &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 0 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    dbContext.RecipeTemplates.Add(obj);
                    dbContext.SaveChanges();
                }
            }
        }
        private static void CreateDynamic4RecipeTemplateGroup(List<List<double>> list)
        {
            foreach (var cPoints in list)
            {
                using (var dbContext = new AppDbContext())
                {
                    int order = 1;
                    RecipeTemplate obj = new RecipeTemplate();
                    obj.Name = $"2.05A-{cPoints[0] / -1000.0}A-{cPoints[1] / -1000.0}A-{cPoints[2] / -1000.0}A-2.05A";


                    var newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0.5 &&
                        o.CurrentUnit == CurrentUnitEnum.C &&
                        o.CutOffConditionValue == 1 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == -2050 &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 300 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[0] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 300 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[1] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 300 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[2] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 0 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == -2050 &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 0 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    dbContext.RecipeTemplates.Add(obj);
                    dbContext.SaveChanges();
                }
            }
        }
        private static void CreateDynamic5RecipeTemplateGroup(List<List<double>> list)
        {
            foreach (var cPoints in list)
            {
                using (var dbContext = new AppDbContext())
                {
                    int order = 1;
                    RecipeTemplate obj = new RecipeTemplate();
                    obj.Name = $"2.05A-{cPoints[0] / -1000.0}A-{cPoints[1] / -1000.0}A-{cPoints[2] / -1000.0}A";


                    var newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0.5 &&
                        o.CurrentUnit == CurrentUnitEnum.C &&
                        o.CutOffConditionValue == 1 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == -2050 &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[0] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 300 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[1] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 300 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[2] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 0 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    dbContext.RecipeTemplates.Add(obj);
                    dbContext.SaveChanges();
                }
            }
        }
        private static void CreateDynamic6RecipeTemplateGroup(List<List<double>> list)
        {
            foreach (var cPoints in list)
            {
                using (var dbContext = new AppDbContext())
                {
                    int order = 1;
                    RecipeTemplate obj = new RecipeTemplate();
                    obj.Name = $"{cPoints[0] / -1000.0}A-{cPoints[1] / -1000.0}A";


                    var newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0.5 &&
                        o.CurrentUnit == CurrentUnitEnum.C &&
                        o.CutOffConditionValue == 1 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[0] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 0.2 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0 &&
                        o.CurrentUnit == CurrentUnitEnum.C &&
                        o.CutOffConditionValue == 60 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    newStep.LoopLabel = "a";
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[1] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == 300 &&
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
        private static void CreateDynamic12RecipeTemplateGroup(List<List<double>> list)
        {
            foreach (var cPoints in list)
            {
                CreateStepTemplate(cPoints[0], CurrentUnitEnum.mA, cPoints[2], CutOffConditionTypeEnum.Time_s);
                CreateStepTemplate(cPoints[1], CurrentUnitEnum.mA, cPoints[2], CutOffConditionTypeEnum.Time_s);
            }
            foreach (var cPoints in list)
            {
                using (var dbContext = new AppDbContext())
                {
                    int order = 1;
                    RecipeTemplate obj = new RecipeTemplate();
                    obj.Name = $"{cPoints[0] / -1000.0}A-{cPoints[1] / -1000.0}A-{cPoints[2]}S";


                    var newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0.5 &&
                        o.CurrentUnit == CurrentUnitEnum.C &&
                        o.CutOffConditionValue == 1 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[0] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == cPoints[2] &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    newStep.LoopLabel = "a";
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[1] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == cPoints[2] &&
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
        private static void CreateDynamic13RecipeTemplateGroup(List<List<double>> list)
        {
            foreach (var cPoints in list)
            {
                CreateStepTemplate(cPoints[0], CurrentUnitEnum.mA, cPoints[2], CutOffConditionTypeEnum.Time_s);
                CreateStepTemplate(cPoints[1], CurrentUnitEnum.mA, cPoints[2], CutOffConditionTypeEnum.Time_s);
            }
            foreach (var cPoints in list)
            {
                using (var dbContext = new AppDbContext())
                {
                    int order = 1;
                    RecipeTemplate obj = new RecipeTemplate();
                    obj.Name = $"{cPoints[0] / -1000.0}A-{cPoints[1] / -1000.0}A-{cPoints[2]}S";


                    var newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0.5 &&
                        o.CurrentUnit == CurrentUnitEnum.C &&
                        o.CutOffConditionValue == 1 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == 0 &&
                        o.CutOffConditionValue == 600 &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[0] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == cPoints[2] &&
                        o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
                    newStep.LoopLabel = "a";
                    obj.Steps.Add(newStep);

                    newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate = dbContext.StepTemplates.Single(
                        o => o.CurrentInput == cPoints[1] &&
                        o.CurrentUnit == CurrentUnitEnum.mA &&
                        o.CutOffConditionValue == cPoints[2] &&
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
        private static void Create_HG2_HighPower_MISC_Battery_Initial(int index)
        {
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                if (index == 1)
                    pro.Name = "MISC-Battery-Initial";
                else
                    pro.Name = $"MISC-Battery-Initial-T{index}";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "MISC");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                recTemp = GetRecipeTemplateByName(dbContext, "3A");
                rec = new RecipeClass(recTemp, bType);
                rec.Temperature = 25;
                pro.Recipes.Add(rec);
                rec = new RecipeClass(recTemp, bType);
                rec.Temperature = 25;
                pro.Recipes.Add(rec);
                rec = new RecipeClass(recTemp, bType);
                rec.Temperature = 25;
                pro.Recipes.Add(rec);
                rec = new RecipeClass(recTemp, bType);
                rec.Temperature = 25;
                pro.Recipes.Add(rec);

                dbContext.Programs.Add(pro);
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_OCV_02C()
        {
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "OCV-0.2C";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "OCV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                recTemp = GetRecipeTemplateByName(dbContext, "0.6A");
                rec = new RecipeClass(recTemp, bType);
                rec.Temperature = 25;
                pro.Recipes.Add(rec);

                dbContext.Programs.Add(pro);
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_RC(int index)
        {
            var tPoints = new List<double>() { -10, -2.5, 5, 15, 25, 35, 45, 55 };
            var cPoints = new List<string>() { "2A", "3A", "6A", "10A", "14A", "17A", "19A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                if (index == 1)
                    pro.Name = "RC";
                else
                    pro.Name = $"RC-N{index}";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "RC");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {

                        recTemp = GetRecipeTemplateByName(dbContext, c);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Static_01()
        {
            var tPoints = new List<double>() { -5, 2.5, 7, 10, 20, 28, 33 };
            var cPoints = new List<string>() { "2.8A", "9A", "11A", "15A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Static-N1";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, c);
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Static_02()
        {
            var tPoints = new List<double>() { 7, 10, 20, 28, 33 };
            var cPoints = new List<string>() { "2.8A", "9A", "11A", "15A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Static-N2";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power 0" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"{c}-N2");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Static_03()
        {
            var tPoints = new List<double>() { 7, 10, 20, 28, 33 };
            var cPoints = new List<string>() { "2.8A", "9A", "11A", "15A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Static-N3";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"{c}-N2");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Dynamic_01()
        {
            var tPoints = new List<double>() { 7, 10, 20, 28, 33 };
            var cPoints = new List<string>() { "3A-11A-17A", "17A-11A-3A", "11A-17A-3A", "3A-17A-11A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Dynamic-N1";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, c);
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_MISC_EXTRA_01()
        {
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "MISC-Extra";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                recTemp = GetRecipeTemplateByName(dbContext, "Extra");
                rec = new RecipeClass(recTemp, bType);
                rec.Temperature = 28;
                pro.Recipes.Add(rec);
                rec = new RecipeClass(recTemp, bType);
                rec.Temperature = 28;
                pro.Recipes.Add(rec);

                dbContext.Programs.Add(pro);
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Dynamic_02()
        {
            var tPoints = new List<double>() { 7, 10, 20, 28, 33 };
            var cPoints = new List<string>() { "3A-11A-17A", "17A-11A-3A", "11A-17A-3A", "3A-17A-11A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Dynamic-N2";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"{c}-N2");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Static_04()
        {
            var tPoints = new List<double>() { 7, 10, 20, 28, 33 };
            var cPoints = new List<string>() { "2.8A", "9A", "11A", "15A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Static-N4";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"{c}-N3");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_OCV_03C(int index)
        {
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                if (index == 1)
                    pro.Name = "OCV-0.3C";
                else
                    pro.Name = $"OCV-0.3C-N{index}";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "OCV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                recTemp = GetRecipeTemplateByName(dbContext, "0.9A");
                rec = new RecipeClass(recTemp, bType);
                rec.Temperature = 25;
                pro.Recipes.Add(rec);
                rec = new RecipeClass(recTemp, bType);
                rec.Temperature = 25;
                pro.Recipes.Add(rec);
                rec = new RecipeClass(recTemp, bType);
                rec.Temperature = 25;
                pro.Recipes.Add(rec);
                rec = new RecipeClass(recTemp, bType);
                rec.Temperature = 25;
                pro.Recipes.Add(rec);

                dbContext.Programs.Add(pro);
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Dynamic_03()
        {
            var tPoints = new List<double>() { 7, 10, 20, 28, 33 };
            var cPoints = new List<string>() { "3A-11A-17A", "17A-11A-3A", "11A-17A-3A", "3A-17A-11A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Dynamic-N3";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"{c}-2.05A");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Dynamic_04()
        {
            var tPoints = new List<double>() { 7, 10, 20, 28, 33 };
            var cPoints = new List<string>() { "3A-11A-17A", "17A-11A-3A", "11A-17A-3A", "3A-17A-11A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Dynamic-N4";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"2.05A-{c}-2.05A");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Dynamic_05()
        {
            var tPoints = new List<double>() { 7, 10, 20, 28, 33 };
            var cPoints = new List<string>() { "3A-11A-17A", "17A-11A-3A", "11A-17A-3A", "3A-17A-11A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Dynamic-N5";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"2.05A-{c}");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Dynamic_06()
        {
            var tPoints = new List<double>() { 7, 10, 20, 28, 33 };
            var cPoints = new List<string>() { "17A-3A", "11A-3A", "11A-11A", "17A-17A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Dynamic-N6";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"{c}");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Dynamic_05_N2()
        {
            var tPoints = new List<double>() { 7, 10, 20, 28, 33 };
            var cPoints = new List<string>() { "3A-11A-17A", "17A-11A-3A", "11A-17A-3A", "3A-17A-11A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Dynamic-N5-N2";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"2.05A-{c}");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Static_05()
        {
            var tPoints = new List<double>() { -5, 2.5 };
            var cPoints = new List<string>() { "2.8A", "9A", "11A", "15A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Static-N5";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"{c}-N3");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Dynamic_07()
        {
            var tPoints = new List<double>() { -5, 2.5 };
            var cPoints = new List<string>() { "3A-11A-17A", "17A-11A-3A", "11A-17A-3A", "3A-17A-11A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Dynamic-N7";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"{c}-N2");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Dynamic_08()
        {
            var tPoints = new List<double>() { -5, 2.5 };
            var cPoints = new List<string>() { "3A-11A-17A", "17A-11A-3A", "11A-17A-3A", "3A-17A-11A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Dynamic-N8";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"{c}-2.05A");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Dynamic_09()
        {
            var tPoints = new List<double>() { -5, 2.5 };
            var cPoints = new List<string>() { "3A-11A-17A", "17A-11A-3A", "11A-17A-3A", "3A-17A-11A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Dynamic-N9";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"2.05A-{c}-2.05A");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Dynamic_10()
        {
            var tPoints = new List<double>() { -5, 2.5 };
            var cPoints = new List<string>() { "3A-11A-17A", "17A-11A-3A", "11A-17A-3A", "3A-17A-11A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Dynamic-N10";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"2.05A-{c}");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Dynamic_11()
        {
            var tPoints = new List<double>() { -5, 2.5 };
            var cPoints = new List<string>() { "17A-3A", "11A-3A", "11A-11A", "17A-17A" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Dynamic-N11";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"{c}");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Dynamic_12()
        {
            var tPoints = new List<double>() { -5, 2.5, 7, 10, 20, 28, 33 };
            var cPoints = new List<string>() { "17A-3A-60S", "11A-3A-60S", "17A-11A-60S", "17A-3A-30S" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Dynamic-N12";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"{c}");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_HG2_HighPower_EV_Dynamic_13()
        {
            var tPoints = new List<double>() { 2.5 };
            var cPoints = new List<string>() { "17A-3A-10S", "17A-3A-5S", "6A-3A-10S", "6A-3A-5S" };
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "HG2");
                var pro = new ProgramClass();
                pro.Name = "EV-Dynamic-N13";
                pro.Project = dbContext.Projects.Single(o => o.Name == "High Power" && o.BatteryType.Name == "HG2");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in cPoints)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, $"{c}");
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }


        private static void Create_M26_O2Sim1_MISC_Battery_Initial(int index)
        {
            RecipeTemplate rt = new RecipeTemplate();
            int order = 1;
            rt.Name = $"2.5A";
            rt.Steps.Add(new StepClass() { Order = order++, StepTemplate = new StepTemplate() { CurrentInput = 0, CurrentUnit = CurrentUnitEnum.C, CutOffConditionValue = 600, CutOffConditionType = CutOffConditionTypeEnum.Time_s } });
            rt.Steps.Add(new StepClass() { Order = order++, StepTemplate = new StepTemplate() { CurrentInput = 0.5, CurrentUnit = CurrentUnitEnum.C, CutOffConditionValue = 1, CutOffConditionType = CutOffConditionTypeEnum.CRate } });
            rt.Steps.Add(new StepClass() { Order = order++, StepTemplate = new StepTemplate() { CurrentInput = 0, CurrentUnit = CurrentUnitEnum.C, CutOffConditionValue = 600, CutOffConditionType = CutOffConditionTypeEnum.Time_s } });
            rt.Steps.Add(new StepClass() { Order = order++, StepTemplate = new StepTemplate() { CurrentInput = -2500, CurrentUnit = CurrentUnitEnum.mA, CutOffConditionValue = 0, CutOffConditionType = CutOffConditionTypeEnum.CRate }});
            var recipeName = (SuperCreateRecipeTemplate(rt));
            //CreateStepTemplate(-2500, CurrentUnitEnum.mA, 0, CutOffConditionTypeEnum.CRate);
            //using (var dbContext = new AppDbContext())//1
            //{
            //    RecipeTemplate rt = new RecipeTemplate();
            //    int order = 1;
            //    rt.Name = "2.5A";
            //    var newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0 && o.CutOffConditionValue == 60 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
            //    rt.Steps.Add(newStep);

            //    newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0.5 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
            //    rt.Steps.Add(newStep);

            //    newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0 && o.CutOffConditionValue == 60 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
            //    rt.Steps.Add(newStep);

            //    newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == -2500 && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
            //    rt.Steps.Add(newStep);

            //    dbContext.RecipeTemplates.Add(rt);
            //    dbContext.SaveChanges();
            //}
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "M26");
                var pro = new ProgramClass();
                if (index == 1)
                    pro.Name = "MISC-Battery-Initial";
                else
                    pro.Name = $"MISC-Battery-Initial-T{index}";
                pro.Project = dbContext.Projects.Single(o => o.Name == "O2Sim1" && o.BatteryType.Name == "M26");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "MISC");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                recTemp = GetRecipeTemplateByName(dbContext, "2.5A");
                rec = new RecipeClass(recTemp, bType);
                rec.Temperature = 25;
                pro.Recipes.Add(rec);
                rec = new RecipeClass(recTemp, bType);
                rec.Temperature = 25;
                pro.Recipes.Add(rec);
                rec = new RecipeClass(recTemp, bType);
                rec.Temperature = 25;
                pro.Recipes.Add(rec);
                rec = new RecipeClass(recTemp, bType);
                rec.Temperature = 25;
                pro.Recipes.Add(rec);

                dbContext.Programs.Add(pro);
                dbContext.SaveChanges();
            }
        }
        private static void Create_M26_O2Sim1_OCV02()
        {
            //CreateStepTemplate(-500, CurrentUnitEnum.mA, 0, CutOffConditionTypeEnum.CRate);
            //CreateStepTemplate(-750, CurrentUnitEnum.mA, 0, CutOffConditionTypeEnum.CRate);
            //using (var dbContext = new AppDbContext())//1
            //{
            //    RecipeTemplate rt = new RecipeTemplate();
            //    int order = 1;
            //    rt.Name = "0.5A";
            //    var newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0.5 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
            //    rt.Steps.Add(newStep);

            //    newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0 && o.CutOffConditionValue == 60 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
            //    rt.Steps.Add(newStep);

            //    newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == -500 && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
            //    rt.Steps.Add(newStep);

            //    dbContext.RecipeTemplates.Add(rt);
            //    dbContext.SaveChanges();
            //}
            //using (var dbContext = new AppDbContext())//1
            //{
            //    RecipeTemplate rt = new RecipeTemplate();
            //    int order = 1;
            //    rt.Name = "0.75A";
            //    var newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0.5 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
            //    rt.Steps.Add(newStep);

            //    newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0 && o.CutOffConditionValue == 60 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
            //    rt.Steps.Add(newStep);

            //    newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == -750 && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
            //    rt.Steps.Add(newStep);

            //    dbContext.RecipeTemplates.Add(rt);
            //    dbContext.SaveChanges();
            //}
            List<string> recipeNames = new List<string>();
            List<double> cpoints = new List<double>() { -500, -750 };
            foreach (var cpoint in cpoints)
            {
                RecipeTemplate rt = new RecipeTemplate();
                int order = 1;
                rt.Name = $"{cpoint/-1000.0}A";
                rt.Steps.Add(new StepClass() { Order = order++, StepTemplate = new StepTemplate() { CurrentInput = 0.5, CurrentUnit = CurrentUnitEnum.C, CutOffConditionValue = 1, CutOffConditionType = CutOffConditionTypeEnum.CRate } });
                rt.Steps.Add(new StepClass() { Order = order++, StepTemplate = new StepTemplate() { CurrentInput = 0, CurrentUnit = CurrentUnitEnum.C, CutOffConditionValue = 600, CutOffConditionType = CutOffConditionTypeEnum.Time_s } });
                rt.Steps.Add(new StepClass() { Order = order++, StepTemplate = new StepTemplate() { CurrentInput = cpoint, CurrentUnit = CurrentUnitEnum.mA, CutOffConditionValue = 0, CutOffConditionType = CutOffConditionTypeEnum.CRate } });
                recipeNames.Add(SuperCreateRecipeTemplate(rt));
            }
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "M26");
                var pro = new ProgramClass();
                pro.Name = "OCV-0.2C-0.3C";
                pro.Project = dbContext.Projects.Single(o => o.Name == "O2Sim1" && o.BatteryType.Name == "M26");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "OCV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                foreach (var recname in recipeNames)
                {
                    RecipeTemplate recTemp;
                    RecipeClass rec;
                    recTemp = GetRecipeTemplateByName(dbContext, recname);
                    rec = new RecipeClass(recTemp, bType);
                    rec.Temperature = 25;
                    pro.Recipes.Add(rec);
                    rec = new RecipeClass(recTemp, bType);
                    rec.Temperature = 25;
                    pro.Recipes.Add(rec);
                    rec = new RecipeClass(recTemp, bType);
                    rec.Temperature = 25;
                    pro.Recipes.Add(rec);
                    rec = new RecipeClass(recTemp, bType);
                    rec.Temperature = 25;
                    pro.Recipes.Add(rec);
                }

                dbContext.Programs.Add(pro);
                dbContext.SaveChanges();
            }
        }
        private static void Create_M26_O2Sim1_RC()
        {
            //CreateStepTemplate(-1000, CurrentUnitEnum.mA, 0, CutOffConditionTypeEnum.CRate);
            //CreateStepTemplate(-5000, CurrentUnitEnum.mA, 0, CutOffConditionTypeEnum.CRate);
            //CreateStepTemplate(-8000, CurrentUnitEnum.mA, 0, CutOffConditionTypeEnum.CRate);
            //using (var dbContext = new AppDbContext())//1
            //{
            //    RecipeTemplate rt = new RecipeTemplate();
            //    int order = 1;
            //    rt.Name = "1A";
            //    var newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0.5 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
            //    rt.Steps.Add(newStep);

            //    newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0 && o.CutOffConditionValue == 60 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
            //    rt.Steps.Add(newStep);

            //    newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == -1000 && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
            //    rt.Steps.Add(newStep);

            //    dbContext.RecipeTemplates.Add(rt);
            //    dbContext.SaveChanges();
            //}
            //using (var dbContext = new AppDbContext())//1
            //{
            //    RecipeTemplate rt = new RecipeTemplate();
            //    int order = 1;
            //    rt.Name = "5A";
            //    var newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0.5 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
            //    rt.Steps.Add(newStep);

            //    newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0 && o.CutOffConditionValue == 60 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
            //    rt.Steps.Add(newStep);

            //    newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == -5000 && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
            //    rt.Steps.Add(newStep);

            //    dbContext.RecipeTemplates.Add(rt);
            //    dbContext.SaveChanges();
            //}
            //using (var dbContext = new AppDbContext())//1
            //{
            //    RecipeTemplate rt = new RecipeTemplate();
            //    int order = 1;
            //    rt.Name = "8A";
            //    var newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0.5 && o.CurrentUnit == CurrentUnitEnum.C && o.CutOffConditionValue == 1 && o.CutOffConditionType == CutOffConditionTypeEnum.CRate);
            //    rt.Steps.Add(newStep);

            //    newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == 0 && o.CutOffConditionValue == 60 && o.CutOffConditionType == CutOffConditionTypeEnum.Time_s);
            //    rt.Steps.Add(newStep);

            //    newStep = new StepClass();
            //    newStep.Order = order++;
            //    newStep.StepTemplate = dbContext.StepTemplates.Single(o => o.CurrentInput == -8000 && o.CurrentUnit == CurrentUnitEnum.mA && o.CutOffConditionValue == 0);
            //    rt.Steps.Add(newStep);

            //    dbContext.RecipeTemplates.Add(rt);
            //    dbContext.SaveChanges();
            //}
            var tPoints = new List<double>() { -10, -2.5, 5, 15, 25, 35, 45 };
            //var cPoints = new List<string>() { "0.5A", "1A", "2A", "3A", "5A", "8A", "10A" };
            var cPoints = new List<double>() { -500, -1000, -2000, -3000, -5000, -8000, -10000 };
            var recipeNames = new List<String>();
            foreach (var cpoint in cPoints)
            {
                RecipeTemplate rt = new RecipeTemplate();
                int o = 1;
                rt.Name = $"{cpoint / -1000.0}A";
                rt.Steps.Add(new StepClass() { Order = o++, StepTemplate = new StepTemplate() { CurrentInput = 0.5, CurrentUnit = CurrentUnitEnum.C, CutOffConditionValue = 1, CutOffConditionType = CutOffConditionTypeEnum.CRate } });
                rt.Steps.Add(new StepClass() { Order = o++, StepTemplate = new StepTemplate() { CurrentInput = 0, CurrentUnit = CurrentUnitEnum.C, CutOffConditionValue = 600, CutOffConditionType = CutOffConditionTypeEnum.Time_s } });
                rt.Steps.Add(new StepClass() { Order = o++, StepTemplate = new StepTemplate() { CurrentInput = cpoint, CurrentUnit = CurrentUnitEnum.mA, CutOffConditionValue = 0, CutOffConditionType = CutOffConditionTypeEnum.CRate } });
                recipeNames.Add(SuperCreateRecipeTemplate(rt));
            }
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "M26");
                var pro = new ProgramClass();
                pro.Name = "RC";
                pro.Project = dbContext.Projects.Single(o => o.Name == "O2Sim1" && o.BatteryType.Name == "M26");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "RC");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in recipeNames)
                    {

                        recTemp = GetRecipeTemplateByName(dbContext, c);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_M26_O2Sim1_EV_Static_01()
        {
            var tPoints = new List<double>() { -5, 2.5, 7, 10, 20, 28, 33 };
            //var cPoints = new List<double>() { "8A", "2.5A", "6A", "9A" };
            var cPoints = new List<double>() { -800, -2500, -6000, -9000 };
            var recipeNames = new List<String>();
            foreach (var cpoint in cPoints)
            {
                RecipeTemplate rt = new RecipeTemplate();
                rt.Name = $"{cpoint / -1000.0}A";
                rt.Steps.Add(new StepClass() { Order = 1, StepTemplate = new StepTemplate() { CurrentInput = 0.5, CurrentUnit = CurrentUnitEnum.C, CutOffConditionValue = 1, CutOffConditionType = CutOffConditionTypeEnum.CRate } });
                rt.Steps.Add(new StepClass() { Order = 2, StepTemplate = new StepTemplate() { CurrentInput = 0, CurrentUnit = CurrentUnitEnum.C, CutOffConditionValue = 600, CutOffConditionType = CutOffConditionTypeEnum.Time_s } });
                rt.Steps.Add(new StepClass() { Order = 3, StepTemplate = new StepTemplate() { CurrentInput = cpoint, CurrentUnit = CurrentUnitEnum.mA, CutOffConditionValue = 0, CutOffConditionType = CutOffConditionTypeEnum.CRate } });
                rt.Steps.Add(new StepClass() { Order = 4, StepTemplate = new StepTemplate() { CurrentInput = 0, CurrentUnit = CurrentUnitEnum.C, CutOffConditionValue = 60, CutOffConditionType = CutOffConditionTypeEnum.Time_s } });
                recipeNames.Add(SuperCreateRecipeTemplate(rt));
            }
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "M26");
                var pro = new ProgramClass();
                pro.Name = "EV-Static-N1";
                pro.Project = dbContext.Projects.Single(o => o.Name == "O2Sim1" && o.BatteryType.Name == "M26");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in recipeNames)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, c);
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void Create_M26_O2Sim1_EV_Dynamic_01()
        {
            var tPoints = new List<double>() { -5, 2.5, 7, 10, 20, 28, 33 };
            //var cPoints = new List<double>() { "8A", "2.5A", "6A", "9A" };
            var cPointsList = new List<List<double>>() {
                new List<double>(){ -8000, -1000, 60 },
                new List<double>(){ -5000, -1000, 60 },
                new List<double>(){ -8000, -5000, 60 },
                new List<double>(){ -8000, -1000, 30 },
                new List<double>(){ -8000, -1000, 10 },
                new List<double>(){ -8000, -1000, 5 },
                new List<double>(){ -3000, -1000, 10 },
                new List<double>(){ -3000, -1000, 5 }
            };
            var recipeNames = new List<String>();
            foreach (var cpoints in cPointsList)
            {
                RecipeTemplate rt = new RecipeTemplate();
                int o = 1;
                rt.Name = $"{cpoints[0] / -1000.0}A-{cpoints[1] / -1000.0}A-{cpoints[2]}S";
                rt.Steps.Add(new StepClass() { Order = o++, StepTemplate = new StepTemplate() { CurrentInput = 0, CurrentUnit = CurrentUnitEnum.C, CutOffConditionValue = 600, CutOffConditionType = CutOffConditionTypeEnum.Time_s } });
                rt.Steps.Add(new StepClass() { Order = o++, StepTemplate = new StepTemplate() { CurrentInput = 0.5, CurrentUnit = CurrentUnitEnum.C, CutOffConditionValue = 1, CutOffConditionType = CutOffConditionTypeEnum.CRate } });
                rt.Steps.Add(new StepClass() { Order = o++, StepTemplate = new StepTemplate() { CurrentInput = 0, CurrentUnit = CurrentUnitEnum.C, CutOffConditionValue = 600, CutOffConditionType = CutOffConditionTypeEnum.Time_s } });
                rt.Steps.Add(new StepClass() { Order = o++, StepTemplate = new StepTemplate() { CurrentInput = cpoints[0], CurrentUnit = CurrentUnitEnum.mA, CutOffConditionValue = cpoints[2], CutOffConditionType = CutOffConditionTypeEnum.Time_s }, LoopLabel = "a" });
                rt.Steps.Add(new StepClass() { Order = o++, StepTemplate = new StepTemplate() { CurrentInput = cpoints[1], CurrentUnit = CurrentUnitEnum.mA, CutOffConditionValue = cpoints[2], CutOffConditionType = CutOffConditionTypeEnum.Time_s }, CompareMark = CompareMarkEnum.LargerThan, CRate = 0, LoopTarget = "a" });
                recipeNames.Add(SuperCreateRecipeTemplate(rt));
            }
            using (var dbContext = new AppDbContext())//1
            {
                BatteryTypeClass bType = dbContext.BatteryTypes.Single(o => o.Name == "M26");
                var pro = new ProgramClass();
                pro.Name = "EV-Dynamic-N1";
                pro.Project = dbContext.Projects.Single(o => o.Name == "O2Sim1" && o.BatteryType.Name == "M26");
                pro.Type = dbContext.ProgramTypes.Single(o => o.Name == "EV");
                pro.Requester = "Jim";
                pro.RequestTime = DateTime.Now;

                RecipeTemplate recTemp;
                RecipeClass rec;
                foreach (var t in tPoints)
                {
                    foreach (var c in recipeNames)
                    {
                        var id = GetRecipeTemplateIdByName(dbContext, c);
                        recTemp = GetRecipeTemplateById(dbContext, id);
                        rec = new RecipeClass(recTemp, bType);
                        rec.Temperature = t;
                        pro.Recipes.Add(rec);

                        dbContext.Programs.Add(pro);
                    }
                }
                dbContext.SaveChanges();
            }
        }
        private static void CreatePrograms()
        {
            Create_HG2_HighPower_MISC_Battery_Initial(1);
            Create_HG2_HighPower_OCV_02C();
            Create_HG2_HighPower_RC(1);
            Create_HG2_HighPower_EV_Static_01();
            Create_HG2_HighPower_EV_Static_02();
            Create_HG2_HighPower_EV_Static_03();
            Create_HG2_HighPower_EV_Dynamic_01();
            Create_HG2_HighPower_MISC_EXTRA_01();
            Create_HG2_HighPower_EV_Dynamic_02();
            Create_HG2_HighPower_EV_Static_04();
            Create_HG2_HighPower_OCV_03C(1);
            Create_HG2_HighPower_EV_Dynamic_03();
            Create_HG2_HighPower_EV_Dynamic_04();
            Create_HG2_HighPower_EV_Dynamic_05();
            Create_HG2_HighPower_EV_Dynamic_06();
            Create_HG2_HighPower_EV_Dynamic_05_N2();
            Create_HG2_HighPower_MISC_Battery_Initial(2);
            Create_HG2_HighPower_OCV_03C(2);
            Create_HG2_HighPower_RC(2);
            Create_HG2_HighPower_EV_Static_05();
            Create_HG2_HighPower_EV_Dynamic_07();
            Create_HG2_HighPower_EV_Dynamic_08();
            Create_HG2_HighPower_EV_Dynamic_09();
            Create_HG2_HighPower_EV_Dynamic_10();
            Create_HG2_HighPower_EV_Dynamic_11();
            Create_HG2_HighPower_EV_Dynamic_12();
            Create_HG2_HighPower_EV_Dynamic_13();

            Create_M26_O2Sim1_MISC_Battery_Initial(1);
            Create_M26_O2Sim1_OCV02();
            Create_M26_O2Sim1_RC();
            Create_M26_O2Sim1_EV_Static_01();
            Create_M26_O2Sim1_EV_Dynamic_01();
        }
        #endregion
        #region Common
        public static void CreateStepTemplate(double ci, CurrentUnitEnum cu, double cocv, CutOffConditionTypeEnum coct)
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
            var subtemplate = dbContext.RecipeTemplates.Single(o => o.Id == id);
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

        private static RecipeTemplate GetRecipeTemplateByName(AppDbContext dbContext, string name)
        {
            var subtemplate = dbContext.RecipeTemplates.Single(o => o.Name == name);
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

        private static int GetRecipeTemplateIdByName(AppDbContext dbContext, string name)
        {
            return dbContext.RecipeTemplates.SingleOrDefault(o => o.Name == name).Id;
        }

        private static string SuperCreateRecipeTemplate(RecipeTemplate rt)  //传入一个rt，创建此rt并添加到db，传回合法名字
        {
            string name;
            if (CheckRecipeTemplateExistance(rt, out name))
            {
                return name;
            }
            else
            {
                rt.Name = GetValidRecipeTemplateName(rt.Name);
                CreateRecipeTemplate(rt);
                return rt.Name;
            }
        }

        private static void CreateRecipeTemplate(RecipeTemplate rt)
        {
            foreach (var st in rt.Steps)
            {
                CreateStepTemplate(st.StepTemplate.CurrentInput, st.StepTemplate.CurrentUnit, st.StepTemplate.CutOffConditionValue, st.StepTemplate.CutOffConditionType);
            }
            using (var dbContext = new AppDbContext())
            {
                int order = 1;
                RecipeTemplate obj = new RecipeTemplate();
                obj.Name = rt.Name;

                foreach (var step in rt.Steps)
                {
                    var newStep = new StepClass();
                    newStep.Order = order++;
                    newStep.StepTemplate =
                        dbContext.StepTemplates.SingleOrDefault(o => o.CurrentInput == step.StepTemplate.CurrentInput
                        && o.CurrentUnit == step.StepTemplate.CurrentUnit
                        && o.CutOffConditionValue == step.StepTemplate.CutOffConditionValue
                        && o.CutOffConditionType == step.StepTemplate.CutOffConditionType);
                    newStep.CompareMark = step.CompareMark;
                    newStep.CRate = step.CRate;
                    newStep.LoopLabel = step.LoopLabel;
                    newStep.LoopTarget = step.LoopTarget;
                    newStep.LoopCount = step.LoopCount;
                    obj.Steps.Add(newStep);
                }

                dbContext.RecipeTemplates.Add(obj);
                dbContext.SaveChanges();
            }
        }

        private static string GetValidRecipeTemplateName(string name)
        {
            using (var dbContext = new AppDbContext())
            {
                var rts = dbContext.RecipeTemplates;
                if (rts.Any(o => o.Name == name))
                {
                    for (int i = 2; i < 100; i++)
                    {
                        var newname = $"{name}-N{i}";
                        if (!rts.Any(o => o.Name == newname))
                            return newname;
                    }
                    return "";
                }
                else
                    return name;
            }
        }

        private static bool CheckRecipeTemplateExistance(RecipeTemplate rt, out string name)
        {
            using (var dbContext = new AppDbContext())
            {
                var rts = dbContext.RecipeTemplates
                    .Include(o => o.Steps)
                        .ThenInclude(o => o.StepTemplate)
                    .ToList();
                var rttemp = rts.SingleOrDefault(o => StepsCompare(o.Steps, rt.Steps));
                if (rttemp != null)
                {
                    name = rttemp.Name;
                    return true;
                }
                else
                {
                    name = "";
                    return false;
                }
            }
        }

        private static bool StepsCompare(ObservableCollection<StepClass> steps1, ObservableCollection<StepClass> steps2)
        {
            if (steps1.Count != steps2.Count)
                return false;
            steps1.OrderBy(o => o.Order);
            steps2.OrderBy(o => o.Order);
            for (int i = 0; i < steps1.Count; i++)
            {
                if (!StepCompare(steps1[i], steps2[i]))
                    return false;
            }
            return true;
        }

        private static bool StepCompare(StepClass step1, StepClass step2)
        {
            return step1.Order == step2.Order
                && StepTemplateCompare(step1.StepTemplate, step2.StepTemplate)
                && step1.LoopLabel == step2.LoopLabel
                && step1.LoopTarget == step2.LoopTarget
                && step1.LoopCount == step2.LoopCount
                && step1.CompareMark == step2.CompareMark
                && step1.CRate == step2.CRate;
        }

        private static bool StepTemplateCompare(StepTemplate stepTemplate1, StepTemplate stepTemplate2)
        {
            return stepTemplate1.CurrentInput == stepTemplate2.CurrentInput
                && stepTemplate1.CurrentUnit == stepTemplate2.CurrentUnit
                && stepTemplate1.CutOffConditionValue == stepTemplate2.CutOffConditionValue
                && stepTemplate1.CutOffConditionType == stepTemplate2.CutOffConditionType;
        }
        #endregion
    }
}
