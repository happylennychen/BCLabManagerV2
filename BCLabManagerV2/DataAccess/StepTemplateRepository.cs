﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class StepTemplateRepository : Repository<StepTemplate>, IStepTemplateRepository
    {
        public StepTemplateRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
