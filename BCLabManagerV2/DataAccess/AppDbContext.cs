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
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=D://BCLabDB.db3");
        }
    }
}
