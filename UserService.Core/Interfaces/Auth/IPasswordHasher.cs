using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Core.Interfaces.Auth
{
    public interface IPasswordHasher
    {
        string Generate(string password);
    }
}