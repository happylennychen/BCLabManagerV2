using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class CutOffBehaviorRepository : Repository<CutOffBehavior>, ICutOffBehaviorRepository
    {
        public CutOffBehaviorRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
