using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class TesterActionRepository : Repository<TesterAction>, ITesterActionRepository
    {
        public TesterActionRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
