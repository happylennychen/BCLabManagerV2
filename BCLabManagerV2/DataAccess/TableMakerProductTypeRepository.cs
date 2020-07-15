using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class TableMakerProductTypeRepository : Repository<TableMakerProductType>, ITableMakerProductTypeRepository
    {
        public TableMakerProductTypeRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
