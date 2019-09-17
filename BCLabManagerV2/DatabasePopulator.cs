﻿using BCLabManager.DataAccess;
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

        private static void PopulateOperations()
        {
            //throw new NotImplementedException();
        }

        private static void PopulatePrograms()
        {
            PopulateChargeTemperatures();
            PopulateChargeCurrents();
            PopulateDischargeTemperatures();
            PopulateDischargeCurrents();
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
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/02/28");
                obj.Description = "Oppo BLP663 Static Test";
                SubProgramClass sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 1));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 2));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 3));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 4));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 5));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 6));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 7));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 8));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 9));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Oppo BLP663 Dynamic Test";
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/02/28");
                obj.Description = "Oppo BLP663 Dynamic Test";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 10));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 11));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 12));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 13));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 14));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 15));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Oppo BLP663 RC";
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/02/28");
                obj.Description = "Oppo BLP663 RC";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 16));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 17));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 18));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 19));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 20));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 21));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Oppo BLP663 Static Test-2";
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/03/12");
                obj.Description = "Oppo BLP663 Static Test-2";

                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 22));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 23));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 24));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Oppo BLP663 Dynamic Test-2";
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/03/14");
                obj.Description = "Oppo BLP663 Dynamic Test-2";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 25));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 26));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 27));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 30));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 31));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Oppo BLP663 Static Test-3";
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/03/25");
                obj.Description = "Oppo BLP663 Static Test-3";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 28));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Aging Test";
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/04/01");
                obj.Description = "Run 500 cycle to see the aging effect";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 29));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "5 time test";
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/04/01");
                obj.Description = "Run 5 same cycle";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 4));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 23));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 26));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Oppo BLP663 Dynamic Test-3";
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/05/07");
                obj.Description = "Oppo BLP663 Dynamic Test-3";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 30));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "Oppo BLP663 Dynamic Test-4";
                obj.Requester = "Francis";
                obj.RequestDate = DateTime.Parse("2019/05/24");
                obj.Description = "Oppo BLP663 Dynamic Test-4";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 31));
                sub.Id = sub_i++;
                obj.SubPrograms.Add(sub);
                dbContext.Programs.Add(obj);

                dbContext.SaveChanges();

                obj = new ProgramClass();
                obj.Id = i++;
                obj.Name = "2000mA Test";
                obj.Requester = "Jon";
                obj.RequestDate = DateTime.Parse("2019/07/09");
                obj.Description = "";
                sub = new SubProgramClass(GetSubProgramTemplateById(dbContext, 23));
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
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o=>o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "-5 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "500mA");
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "-5 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "1700mA");
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "-5 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "3000mA");
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "500mA");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "1700mA");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "3000mA");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "35 deg");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "35 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "500mA");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "35 deg");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "35 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "1700mA");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "35 deg");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "35 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "3000mA");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "-5 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "D01");
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "-5 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "D02");
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "D01");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "D02");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "35 deg");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "35 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "D01");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "35 deg");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "35 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "D02");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "-10 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "3500mA");
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "0 deg");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "0 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "3500mA");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "10 deg");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "10 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "3500mA");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "20 deg");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "20 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "3500mA");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "30 deg");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "30 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "3500mA");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "40 deg");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "40 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "3500mA");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "-5 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "2000mA");
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "2000mA");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "35 deg");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "35 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "2000mA");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "-5 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "D03");
                obj.TestCount = TestCountEnum.Two;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "D03");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "35 deg");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "35 deg");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "D03");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "1200mA");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "3450mA");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "D04");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                obj = new SubProgramTemplate();
                obj.Id = i++;
                obj.ChargeTemperature = dbContext.ChargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.ChargeCurrent = dbContext.ChargeCurrents.SingleOrDefault(o => o.Name == "1C");
                obj.DischargeTemperature = dbContext.DischargeTemperatures.SingleOrDefault(o => o.Name == "Room");
                obj.DischargeCurrent = dbContext.DischargeCurrents.SingleOrDefault(o => o.Name == "D05");
                obj.TestCount = TestCountEnum.One;
                dbContext.SubProgramTemplates.Add(obj);

                dbContext.SaveChanges();
            }
        }

        private static void PopulateDischargeCurrents()
        {
            using (var dbContext = new AppDbContext())
            {
                DischargeCurrentClass obj = new DischargeCurrentClass();
                obj.Name = "500mA";
                dbContext.DischargeCurrents.Add(obj);

                obj = new DischargeCurrentClass();
                obj.Name = "1200mA";
                dbContext.DischargeCurrents.Add(obj);

                obj = new DischargeCurrentClass();
                obj.Name = "1700mA";
                dbContext.DischargeCurrents.Add(obj);

                obj = new DischargeCurrentClass();
                obj.Name = "2000mA";
                dbContext.DischargeCurrents.Add(obj);

                obj = new DischargeCurrentClass();
                obj.Name = "3000mA";
                dbContext.DischargeCurrents.Add(obj);

                obj = new DischargeCurrentClass();
                obj.Name = "3450mA";
                dbContext.DischargeCurrents.Add(obj);

                obj = new DischargeCurrentClass();
                obj.Name = "3500mA";
                dbContext.DischargeCurrents.Add(obj);

                obj = new DischargeCurrentClass();
                obj.Name = "D01";
                dbContext.DischargeCurrents.Add(obj);

                obj = new DischargeCurrentClass();
                obj.Name = "D02";
                dbContext.DischargeCurrents.Add(obj);

                obj = new DischargeCurrentClass();
                obj.Name = "D03";
                dbContext.DischargeCurrents.Add(obj);

                obj = new DischargeCurrentClass();
                obj.Name = "D04";
                dbContext.DischargeCurrents.Add(obj);

                obj = new DischargeCurrentClass();
                obj.Name = "D05";
                dbContext.DischargeCurrents.Add(obj);

                dbContext.SaveChanges();
            }
        }

        private static void PopulateDischargeTemperatures()
        {
            using (var dbContext = new AppDbContext())
            {
                DischargeTemperatureClass obj = new DischargeTemperatureClass();
                obj.Name = "Room";
                dbContext.DischargeTemperatures.Add(obj);

                obj = new DischargeTemperatureClass();
                obj.Name = "-10 deg";
                dbContext.DischargeTemperatures.Add(obj);

                obj = new DischargeTemperatureClass();
                obj.Name = "-5 deg";
                dbContext.DischargeTemperatures.Add(obj);

                obj = new DischargeTemperatureClass();
                obj.Name = "0 deg";
                dbContext.DischargeTemperatures.Add(obj);

                obj = new DischargeTemperatureClass();
                obj.Name = "10 deg";
                dbContext.DischargeTemperatures.Add(obj);

                obj = new DischargeTemperatureClass();
                obj.Name = "20 deg";
                dbContext.DischargeTemperatures.Add(obj);

                obj = new DischargeTemperatureClass();
                obj.Name = "30 deg";
                dbContext.DischargeTemperatures.Add(obj);

                obj = new DischargeTemperatureClass();
                obj.Name = "35 deg";
                dbContext.DischargeTemperatures.Add(obj);

                obj = new DischargeTemperatureClass();
                obj.Name = "40 deg";
                dbContext.DischargeTemperatures.Add(obj);

                dbContext.SaveChanges();
            }
        }

        private static void PopulateChargeCurrents()
        {
            using (var dbContext = new AppDbContext())
            {
                ChargeCurrentClass obj = new ChargeCurrentClass();
                obj.Name = "1C";
                dbContext.ChargeCurrents.Add(obj);

                dbContext.SaveChanges();
            }
        }

        private static void PopulateChargeTemperatures()
        {
            using (var dbContext = new AppDbContext())
            {
                ChargeTemperatureClass obj = new ChargeTemperatureClass();
                obj.Name = "Room";
                dbContext.ChargeTemperatures.Add(obj);

                obj = new ChargeTemperatureClass();
                obj.Name = "0 deg";
                dbContext.ChargeTemperatures.Add(obj);

                obj = new ChargeTemperatureClass();
                obj.Name = "10 deg";
                dbContext.ChargeTemperatures.Add(obj);

                obj = new ChargeTemperatureClass();
                obj.Name = "20 deg";
                dbContext.ChargeTemperatures.Add(obj);

                obj = new ChargeTemperatureClass();
                obj.Name = "30 deg";
                dbContext.ChargeTemperatures.Add(obj);

                obj = new ChargeTemperatureClass();
                obj.Name = "35 deg";
                dbContext.ChargeTemperatures.Add(obj);

                obj = new ChargeTemperatureClass();
                obj.Name = "40 deg";
                dbContext.ChargeTemperatures.Add(obj);

                dbContext.SaveChanges();
            }
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
                        Status = AssetStatusEnum.IDLE
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
                        Status = AssetStatusEnum.IDLE
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

        private static SubProgramTemplate GetSubProgramTemplateById(AppDbContext dbContext, int id)
        {
            var subtemplate = dbContext.SubProgramTemplates.SingleOrDefault(o => o.Id == id);
            dbContext.Entry(subtemplate)
                .Reference(o => o.ChargeTemperature)
                .Load();
            dbContext.Entry(subtemplate)
                .Reference(o => o.ChargeCurrent)
                .Load();
            dbContext.Entry(subtemplate)
                .Reference(o => o.DischargeTemperature)
                .Load();
            dbContext.Entry(subtemplate)
                .Reference(o => o.DischargeCurrent)
                .Load();
            return subtemplate;
        }
    }
}