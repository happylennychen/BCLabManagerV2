using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class CutOffConditionRepository : Repository<CutOffCondition>, ICutOffConditionRepository
    {
        public CutOffConditionRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
