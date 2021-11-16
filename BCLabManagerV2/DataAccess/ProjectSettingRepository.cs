using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class ProjectSettingRepository : Repository<ProjectSetting>, IProjectSettingRepository
    {
        public ProjectSettingRepository(AppDbContext context)
            : base(context)
        {

        }

        public new void Update(ProjectSetting ps)
        {
            var old_ps = _context.ProjectSettings.Single(o => o.Id == ps.Id);
            old_ps.design_capacity_mahr = ps.design_capacity_mahr;
            old_ps.limited_charge_voltage_mv = ps.limited_charge_voltage_mv;
            old_ps.fully_charged_end_current_ma = ps.fully_charged_end_current_ma;
            old_ps.fully_charged_ending_time_ms = ps.fully_charged_ending_time_ms;
            old_ps.discharge_end_voltage_mv = ps.discharge_end_voltage_mv;
            old_ps.threshold_1st_facc_mv = ps.threshold_1st_facc_mv;
            old_ps.threshold_2nd_facc_mv = ps.threshold_2nd_facc_mv;
            old_ps.threshold_3rd_facc_mv = ps.threshold_3rd_facc_mv;
            old_ps.threshold_4th_facc_mv = ps.threshold_4th_facc_mv;
            old_ps.initial_ratio_fcc = ps.initial_ratio_fcc;
            old_ps.accumulated_capacity_mahr = ps.accumulated_capacity_mahr;
            old_ps.dsg_low_volt_mv = ps.dsg_low_volt_mv;
            old_ps.dsg_low_temp_01dc = ps.dsg_low_temp_01dc;
            old_ps.initial_soc_start_ocv = ps.initial_soc_start_ocv;
            old_ps.system_line_impedance = ps.system_line_impedance;
            old_ps.is_valid = ps.is_valid;
            old_ps.extend_cfg = ps.extend_cfg;
        }
    }
}
