using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class StepV2Repository : Repository<StepV2>, IStepV2Repository
    {
        public StepV2Repository(AppDbContext context)
            : base(context)
        {

        }
    }
}
