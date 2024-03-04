using UserService.Core.Models;

namespace UserService.Core.Interfaces.Repositories
{
    public interface IBadgeRepository
    {

        public Task<int?> Create(Badge badge);

        public Task<IEnumerable<UserBadge>?> GetAllUserBadgesById(int id);

        public Task<bool> UpdateBadgeForUser(UserBadge badge);

        public Task<Badge?> GetBadgeById(int id);
        public Task<bool> Delete(int id);
        public Task<bool> UpdateBadge(Badge badge);
    }
}