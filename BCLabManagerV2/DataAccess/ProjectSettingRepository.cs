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
    }
}
