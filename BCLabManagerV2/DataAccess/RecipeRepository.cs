using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class RecipeRepository : Repository<RecipeClass>, IRecipeRepository
    {
        public RecipeRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
