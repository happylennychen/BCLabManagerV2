using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class StepRuntimeRepository : Repository<StepRuntimeClass>, IStepRuntimeRepository
    {
        public StepRuntimeRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
