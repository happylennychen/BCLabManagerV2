using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class TestRecordRepository : Repository<TestRecord>, ITestRecordRepository
    {
        public TestRecordRepository(AppDbContext context)
            : base(context)
        {
        }
        public IEnumerable<TestRecord> GetAllFreeTestRecords()
        {
            return GetAll().Where(o => o.ProjectStr == null && o.ProgramStr == string.Empty && o.RecipeStr == string.Empty);
        }
    }
}
