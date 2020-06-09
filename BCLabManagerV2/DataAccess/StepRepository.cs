using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class StepRepository : Repository<Step>, IStepRepository
    {
        public StepRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
