﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class RecipeTemplateRepository : Repository<RecipeTemplate>, IRecipeTemplateRepository
    {
        public RecipeTemplateRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
