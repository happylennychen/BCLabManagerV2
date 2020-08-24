using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCLabManager.Model
{ 
    public class StartPointCheckException:Exception
    {
        public StartPointCheckException()
        {

        }
        public StartPointCheckException(string message)
            :base(message)
        {

        }
        public StartPointCheckException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }

    public class MidPointCheckException : Exception
    {
        public MidPointCheckException()
        {

        }
        public MidPointCheckException(string message)
            : base(message)
        {

        }
        public MidPointCheckException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
    public class EndPointCheckException : Exception
    {
        public EndPointCheckException()
        {

        }
        public EndPointCheckException(string message)
            : base(message)
        {

        }
        public EndPointCheckException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
    public class ContinuityCheckException : Exception
    {
        public ContinuityCheckException()
        {

        }
        public ContinuityCheckException(string message)
            : base(message)
        {

        }
        public ContinuityCheckException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
