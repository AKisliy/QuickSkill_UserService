using UserService.Core.Models;

namespace UserService.Core.Interfaces
{
    public interface IUserRepository
    {
        public bool SaveChanges();

        public Task<bool> SaveChangesAsync();

        public Task<IEnumerable<User>> GetAllUsers();

        public Task<User> GetUserById(int id);
        public Task<User> GetUserByUsername(string username);
        public Task<User> GetUserByEmail(string email);

        public Task<int> Create(User user);

        public Task<int> Update(User user);

        public Task<bool> Delete(int id);

        public Task<bool> UpdateUserXp(int id, int xp);

        public Task<bool> SetVerificationToken(int id, string token, DateTime expires);
        public Task<bool> SetResetToken(int id, string token, DateTime expires);

        public Task VerifyUser(string token);

        public Task<bool> IsUniqueResetToken(string token);

        public Task<bool> IsUniqueVerificationToken(string token);
        public Task ResetPassword(string passwordHash, string token);
        public Task SetUserActivity(int id);
        public Task<IEnumerable<UserActivity>> GetAllUserActivity(int id);
        public Task<IEnumerable<UserActivity>> GetActivityByPage(int id, int page, int pageSize);
        public Task<List<UserActivity>> GetActivityForMonth(int id, int month, int year);
        public Task<List<UserActivity>> GetActivityForWeek(int id);
        public Task<bool> HasUserWithEmail(string email);
    }
}