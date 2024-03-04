using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Core.Models;

namespace UserService.Core.Interfaces
{
    public interface IUsersService
    {

        public Task<IEnumerable<User>> GetAllUsers();

        public Task<User?> GetUserById(int id);

        public Task<User?> GetUserByUsername(string username);

        public Task<User?> GetUserByEmail(string email);
        public Task<bool> DeleteUser(int id);
        public Task<bool> UpdateUserXp(int id, int xp);
    }
}