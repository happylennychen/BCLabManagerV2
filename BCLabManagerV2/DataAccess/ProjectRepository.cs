using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(AppDbContext context)
            : base(context)
        {

        }

        public new void Update(Project prj)
        {
            var old_prj = _context.Projects.Single(o => o.Id == prj.Id);
            old_prj.Name = prj.Name;
            old_prj.AbsoluteMaxCapacity = prj.AbsoluteMaxCapacity;
            old_prj.Customer = prj.Customer;
            old_prj.CutoffDischargeVoltage = prj.CutoffDischargeVoltage;
            old_prj.LimitedChargeVoltage = prj.LimitedChargeVoltage;
            old_prj.VoltagePoints = prj.VoltagePoints;
            old_prj.Description = prj.Description;
            //old_prj.
            var bt = _context.BatteryTypes.Single(o => o.Id == prj.BatteryType.Id);
            old_prj.BatteryType = bt;
        }
    }
}
