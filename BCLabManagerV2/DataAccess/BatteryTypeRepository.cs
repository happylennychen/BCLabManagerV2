using BCLabManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.DataAccess
{
    public class BatteryTypeRepository:Repository<BatteryType>, IBatteryTypeRepository
    {
        public BatteryTypeRepository(AppDbContext context)
            :base(context)
        {

        }
    }
}
