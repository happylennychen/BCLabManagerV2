using BCLabManager.Model;
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
        public DbSet<BatteryType> BatteryTypes { get; set; }
        public DbSet<AssetUsageRecord> AssetUsageRecords { get; set; }
        public DbSet<Battery> Batteries { get; set; }
        public DbSet<Tester> Testers { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Chamber> Chambers { get; set; }
        public DbSet<TestRecord> TestRecords { get; set; }
        public DbSet<StepTemplate> StepTemplates { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<StepRuntime> StepRuntimes { get; set; }
        public DbSet<RecipeTemplate> RecipeTemplates { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<ProgramType> ProgramTypes { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TableMakerProduct> TableMakerProducts { get; set; }
        public DbSet<TableMakerProductType> TableMakerProductTypes { get; set; }
        public DbSet<Coefficient> Coefficients { get; set; }
        public DbSet<ProjectSetting> ProjectSettings { get; set; }
        public DbSet<EmulatorResult> EmulatorResults { get; set; }
        public DbSet<LibFG> lib_fgs { get; set; }
        public DbSet<ReleasePackage> ReleasePackages { get; set; }
        public DbSet<Protection> Protections { get; set; }
        public DbSet<RecipeTemplateGroup> RecipeTemplateGroups { get; set; }
        public DbSet<Event> Events { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqloptionsBuilder.UseNpgsql(@"host=localhost;database=test;useroptionsBuilder.UseNpgsql(@"host=localhost;database=bclabmanager;user id=postgres;password=123456;");
            //optionsBuilder.UseNpgsql(@"host=localhost;database=demo;user id=postgres;password=123456;");
            //optionsBuilder.UseNpgsql(@"host=localhost;database=HighPower;user id=postgres;password=123456;").UseSnakeCaseNamingConvention();
            //optionsBuilder.UseNpgsql(@"host=localhost;database=BCLM;user id=postgres;password=123456;").UseSnakeCaseNamingConvention();
            //optionsBuilder.UseNpgsql(@"host=localhost;database=test;user id=postgres;password=123456;").UseSnakeCaseNamingConvention();
            optionsBuilder.UseNpgsql(@"host=10.22.4.249;database=BCLM;user id=postgres;password=123456;").UseSnakeCaseNamingConvention();
            //optionsBuilder.UseNpgsql(@"host=localhost;database=dptest;user id=postgres;password=123456;").UseSnakeCaseNamingConvention();
        }
    }
}
