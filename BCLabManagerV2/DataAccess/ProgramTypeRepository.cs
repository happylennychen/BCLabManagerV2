﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class ProgramTypeRepository : Repository<ProgramTypeClass>, IProgramTypeRepository
    {
        public ProgramTypeRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
