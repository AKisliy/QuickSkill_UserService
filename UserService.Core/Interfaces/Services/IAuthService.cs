using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Core.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<bool> Register(string firstName, string lastName, string email, string password);
    }
}