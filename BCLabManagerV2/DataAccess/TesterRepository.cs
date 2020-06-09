using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class TesterRepository : Repository<Tester>, ITesterRepository
    {
        public TesterRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
