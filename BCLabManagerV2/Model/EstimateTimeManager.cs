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
        public SubProgramTemplate SubTemplate { get; set; }
        public int TestCount { get; set; }
        public int ExecutedCount { get; set; }
        public TimeSpan AverageTime { get; set; }
    }
    public static class EstimateTimeManager
    {
        public static TimeSpan GetAverageTime(BatteryTypeClass batteryType, SubProgramTemplate subProgramTemplate, TestCountEnum testCount)
        {
            throw new NotImplementedException();
        }
        public static void UpdateAverageTime(BatteryTypeClass batteryType, SubProgramTemplate subProgramTemplate, TestCountEnum testCount, TimeSpan newTime)
        {
            using (var dbContext = new AppDbContext())
            {
                var etr = dbContext.EstimateTimeRecords.SingleOrDefault(o => o.BatteryType.Id == batteryType.Id && o.SubTemplate.Id == subProgramTemplate.Id && o.SubTemplate.TestCount == testCount);
                //if(etr == null)
            }
        }
        public static bool IsContain(BatteryTypeClass batteryType, SubProgramTemplate subProgramTemplate, TestCountEnum testCount)
        {
            //using (var dbContext = new AppDbContext())
            //{
            //    var etr = dbContext.EstimateTimeRecords.Count(o => o.BatteryType.Id == batteryType.Id && o.SubTemplate.Id == subProgramTemplate.Id && o.SubTemplate.TestCount == testCount);
            //    return etr == 1;
            //}
            return true;
        }
    }
}
