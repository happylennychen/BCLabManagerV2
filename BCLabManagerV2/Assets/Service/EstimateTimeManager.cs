using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{
    public class EstimateTimeRecord
    {
        public int Id { get; set; }
        public BatteryTypeClass BatteryType { get; set; }
        public RecipeTemplate SubTemplate { get; set; }
        public int TestCount { get; set; }
        public int ExecutedCount { get; set; }
        public TimeSpan AverageTime { get; set; }
    }
    public static class EstimateTimeManager
    {
        public static TimeSpan GetAverageTime(BatteryTypeClass batteryType, RecipeTemplate RecipeTemplate)
        {
            throw new NotImplementedException();
        }
        public static void UpdateAverageTime(BatteryTypeClass batteryType, RecipeTemplate RecipeTemplate, TimeSpan newTime)
        {
            using (var dbContext = new AppDbContext())
            {
                var etr = dbContext.EstimateTimeRecords.SingleOrDefault(o => o.BatteryType.Id == batteryType.Id && o.SubTemplate.Id == RecipeTemplate.Id);
                //if(etr == null)
            }
        }
    }
}
