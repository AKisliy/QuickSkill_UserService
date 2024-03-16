using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Core.Exceptions
{
    public class CredentialsException : Exception
    {
        public CredentialsException(string message) : base(message)
        {
            
        }
    }
}