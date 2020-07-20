using BCLabManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BCLabManager.Model
{
    public class ProjectSettingServiceClass
    {
        public ObservableCollection<ProjectSetting> Items { get; set; }

        public void SuperAdd(ProjectSetting item)
        {
            //FileOperation(item);
            DatabaseAdd(item);
            Items.Add(item);
        }
        public void DatabaseAdd(ProjectSetting item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                item.Project = uow.Projects.GetById(item.Project.Id);
                uow.ProjectSettings.Insert(item);
                uow.Commit();
            }
        }
        public void SuperRemove(int id)
        {
            DatabaseRemove(id);

            var item = Items.SingleOrDefault(o => o.Id == id);
            Items.Remove(item);
        }
        public void DatabaseRemove(int id)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.ProjectSettings.Delete(id);
                uow.Commit();
            }
        }
        public void SuperUpdate(ProjectSetting item)
        {
            DatabaseUpdate(item);
            DomainUpdate(item);
        }
        public void DatabaseUpdate(ProjectSetting item)
        {
            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                uow.ProjectSettings.Update(item);
                uow.Commit();
            }
        }
        public void DomainUpdate(ProjectSetting item)
        {
            var edittarget = Items.SingleOrDefault(o => o.Id == item.Id);
            edittarget.design_capacity_mahr = item.design_capacity_mahr;
            edittarget.limited_charge_voltage_mv = item.limited_charge_voltage_mv;
            edittarget.fully_charged_end_current_ma = item.fully_charged_end_current_ma;
            edittarget.fully_charged_ending_time_ms = item.fully_charged_ending_time_ms;
            edittarget.discharge_end_voltage_mv = item.discharge_end_voltage_mv;
            edittarget.threshold_1st_facc_mv = item.threshold_1st_facc_mv;
            edittarget.threshold_2nd_facc_mv = item.threshold_2nd_facc_mv;
            edittarget.threshold_3rd_facc_mv = item.threshold_3rd_facc_mv;
            edittarget.threshold_4th_facc_mv = item.threshold_4th_facc_mv;
            edittarget.initial_ratio_fcc = item.initial_ratio_fcc;
            edittarget.accumulated_capacity_mahr = item.accumulated_capacity_mahr;
            edittarget.dsg_low_volt_mv = item.dsg_low_volt_mv;
            edittarget.dsg_low_temp_01dc = item.dsg_low_temp_01dc;
            edittarget.initial_soc_start_ocv = item.initial_soc_start_ocv;
            edittarget.system_line_impedance = item.system_line_impedance;
            edittarget.is_valid = item.is_valid;
            edittarget.extend_cfg = item.extend_cfg;
            edittarget.Project = item.Project;
        }
    }
}
