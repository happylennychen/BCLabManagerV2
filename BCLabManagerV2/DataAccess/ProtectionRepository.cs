using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class ProtectionRepository : Repository<Protection>, IProtectionRepository
    {
        public ProtectionRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
