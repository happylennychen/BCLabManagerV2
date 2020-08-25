using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{ 
    public class ProcessException:Exception
    {
        public ProcessException()
        {

        }
        public ProcessException(string message)
            :base(message)
        {

        }
        public ProcessException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
