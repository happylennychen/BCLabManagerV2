using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class ProgramRepository : Repository<ProgramClass>, IProgramRepository
    {
        public ProgramRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
