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
        public DbSet<TestRecordClass> TestRecords { get; set; }
        public DbSet<SubProgramTemplate> SubProgramTemplates { get; set; }
        public DbSet<SubProgramClass> SubPrograms { get; set; }
        public DbSet<ProgramClass> Programs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=D://BCLabDB.db3");
        }
    }
}
