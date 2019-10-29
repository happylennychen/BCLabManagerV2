using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class BatteryRepository : Repository<BatteryClass>, IBatteryRepository
    {
        public BatteryRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
