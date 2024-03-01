using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UserService.Core.Interfaces;
using UserService.Core.Interfaces.Auth;
using UserService.Core.Interfaces.Services;
using UserService.Core.Models;
using UserService.Infrastructure;

namespace UserService.Application.Services
{
    public class AuthService : IAuthService
    {
        private IUserRepository _repository;
        private IPasswordHasher _hasher;
        public AuthService(IUserRepository repository, IPasswordHasher hasher)
        {
            _hasher = hasher;
            _repository = repository;   
        }
        public async Task<bool> Register(string firstName, string lastName, string email, string password)
        {
            try{
                User user = User.Create(firstName, lastName, Generator.GenerateUsername(firstName, lastName, email), email, _hasher.Generate(password));
                await _repository.Create(user);
                return true;
            }
            catch(InvalidDataException)
            {
                return false;
            }
        }
    }
}