using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class JumpBehaviorRepository : Repository<JumpBehavior>, IJumpBehaviorRepository
    {
        public JumpBehaviorRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
