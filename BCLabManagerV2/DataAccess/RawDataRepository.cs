using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class RawDataRepository : Repository<RawDataClass>, IRawDataRepository
    {
        public RawDataRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
