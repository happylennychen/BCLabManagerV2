using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class ChamberRepository : Repository<ChamberClass>, IChamberRepository
    {
        public ChamberRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
