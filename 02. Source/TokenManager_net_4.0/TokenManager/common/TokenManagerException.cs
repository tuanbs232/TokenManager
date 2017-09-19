using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenManager.common
{
    class TokenManagerException : Exception
    {
        public TokenManagerException() : base()
        {
        }

        public TokenManagerException(string message) : base(message)
        {
            
        }

        public TokenManagerException(string message, Exception ex) : base(message, ex)
        {

        }

    }
}
