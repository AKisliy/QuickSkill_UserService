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

        public Task<User> GetUserById(int id);

        public Task<User> GetUserByUsername(string username);

        public Task<User> GetUserByEmail(string email);

        public Task<bool> DeleteUser(int id);

        public Task<bool> UpdateUserXp(int id, int xp);

        public Task SetUserActivity(int id);

        public Task<List<UserActivity>> GetUserActivityForWeek(int id);

        public Task<List<UserActivity>> GetUserActivityForMonth(int id, int month, int year);

        public Task SetUserFirstName(int id, string newName);

        public Task SetUserLastName(int id, string newLastName);

        public Task SetUserUsername(int id, string newUserName);

        public Task SetUserDescription(int id, string descritption);

        public Task SetUserPhoto(int id, string photoUrl);
    }
}