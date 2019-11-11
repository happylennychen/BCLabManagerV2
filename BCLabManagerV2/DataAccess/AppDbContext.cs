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
        public DbSet<BatteryTypeClass> BatteryTypes { get; set; }
        public DbSet<BatteryClass> Batteries { get; set; }
        public DbSet<TesterClass> Testers { get; set; }
        public DbSet<ChannelClass> Channels { get; set; }
        public DbSet<ChamberClass> Chambers { get; set; }
        public DbSet<TestRecordClass> TestRecords { get; set; }
        public DbSet<RecipeTemplate> RecipeTemplates { get; set; }
        public DbSet<RecipeClass> Recipes { get; set; }
        public DbSet<ProgramClass> Programs { get; set; }
        public DbSet<EstimateTimeRecord> EstimateTimeRecords { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={GlobalSettings.DbPath}");
        }
    }
}
