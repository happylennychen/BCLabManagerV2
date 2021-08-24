using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;
using Microsoft.EntityFrameworkCore;

namespace BCLabManager.DataAccess
{
    public class TableMakerRecordRepository : Repository<TableMakerRecord>, ITableMakerRecordRepository
    {
        public TableMakerRecordRepository(AppDbContext context)
            : base(context)
        {

        }

        new public void Insert(TableMakerRecord obj)
        {
            _context.Entry(obj.Project).State = EntityState.Unchanged; 
            foreach (var prd in obj.Products)
            {
                _context.Entry(prd.Type).State = EntityState.Unchanged;
            }
            //_context.Entry(obj.Project.BatteryType).State = EntityState.Unchanged;
            _context.Entry(obj.Project.BatteryType).State = EntityState.Unchanged;
            table.Add(obj);
        }
    }
}
