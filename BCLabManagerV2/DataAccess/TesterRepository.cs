using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class TesterRepository : Repository<TesterClass>, ITesterRepository
    {
        public TesterRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
