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
        private IJwtProvider _provider;
        private IPasswordHasher _hasher;
        public AuthService(IUserRepository repository, IPasswordHasher hasher, IJwtProvider provider)
        {
            _hasher = hasher;
            _repository = repository;
            _provider = provider;
        }

        public async Task<string> Login(string email, string password)
        {
            var user = await _repository.GetUserByEmail(email) ?? throw new Exception("Failed to login");
            var result = _hasher.Verify(password, user.Password);
            if(!result)
                throw new Exception("Failed to login");
            var token = _provider.GenerateToken(user);

            return token;
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