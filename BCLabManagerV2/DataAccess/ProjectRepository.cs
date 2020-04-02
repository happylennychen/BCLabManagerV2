﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class ProjectRepository : Repository<ProjectClass>, IProjectRepository
    {
        public ProjectRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}