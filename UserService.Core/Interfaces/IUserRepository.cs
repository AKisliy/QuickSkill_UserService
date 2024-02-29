using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Core.Models;

namespace UserService.Core.Interfaces
{
    public interface IUserRepository
    {
        public bool SaveChanges();

        public Task<bool> SaveChangesAsync();

        public Task<IEnumerable<User>> GetAllUsers();

        public Task<User?> GetUserById(int id);
        public Task<User?> GetUserByUsername(string username);
        public Task<User?> GetUserByEmail(string email);

        public Task<int> Create(User user);

        public Task<int> Update(User user);

        public Task<int> Delete(User user);
    }
}