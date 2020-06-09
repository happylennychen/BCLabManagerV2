using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCLabManager.Model;

namespace BCLabManager.DataAccess
{
    public class ChannelRepository : Repository<Channel>, IChannelRepository
    {
        public ChannelRepository(AppDbContext context)
            : base(context)
        {

        }
    }
}
