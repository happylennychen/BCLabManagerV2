using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class RecipeTemplateGroupRepository : Repository<RecipeTemplateGroup>, IRecipeTemplateGroupRepository
    {
        public RecipeTemplateGroupRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
