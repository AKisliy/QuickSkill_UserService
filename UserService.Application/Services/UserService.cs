using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoMapper;
using UserService.Core.Interfaces;
using UserService.Core.Models;

namespace UserService.Application.Services
{
    public class UsersService(IUserRepository repository, IMapper mapper) : IUsersService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IUserRepository _repository = repository;

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _repository.GetAllUsers();
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _repository.GetUserById(id);
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            return await _repository.GetUserByUsername(username);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _repository.GetUserByEmail(email);
        }
    }
}