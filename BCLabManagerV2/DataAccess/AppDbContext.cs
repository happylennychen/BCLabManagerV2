﻿using BCLabManager.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.DataAccess
{
    public class AppDbContext : DbContext
    {
        public DbSet<BatteryTypeClass> BatteryTypes { get; set; }
        public DbSet<AssetUsageRecordClass> AssetUsageRecords { get; set; }
        public DbSet<BatteryClass> Batteries { get; set; }
        public DbSet<TesterClass> Testers { get; set; }
        public DbSet<ChannelClass> Channels { get; set; }
        public DbSet<ChamberClass> Chambers { get; set; }
        public DbSet<TestRecordClass> TestRecords { get; set; }
        public DbSet<StepTemplate> StepTemplates { get; set; }
        public DbSet<StepClass> Steps { get; set; }
        public DbSet<StepRuntimeClass> StepRuntimes { get; set; }
        public DbSet<RecipeTemplate> RecipeTemplates { get; set; }
        public DbSet<RecipeClass> Recipes { get; set; }
        public DbSet<ProgramClass> Programs { get; set; }
        public DbSet<ProgramTypeClass> ProgramTypes { get; set; }
        public DbSet<ProjectClass> Projects { get; set; }
        public DbSet<ProjectProductClass> ProjectProducts { get; set; }
        public DbSet<ProjectProductTypeClass> ProjectProductTypes { get; set; }
        public DbSet<CoefficientClass> Coefficients { get; set; }
        public DbSet<EmulatorSettingClass> EmulatorSettings { get; set; }
        public DbSet<EmulatorResultClass> EmulatorResults { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite($"Data Source={GlobalSettings.DbPath}");
            //optionsBuilder.UseNpgsql(@"host=localhost;database=bclabmanager;user id=postgres;password=123456;");
            //optionsBuilder.UseNpgsql(@"host=localhost;database=demo;user id=postgres;password=123456;");
            optionsBuilder.UseNpgsql(@"host=localhost;database=HighPower;user id=postgres;password=123456;").UseSnakeCaseNamingConvention();
        }
    }
}
