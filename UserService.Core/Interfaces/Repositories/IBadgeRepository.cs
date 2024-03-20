using UserService.Core.Models;

namespace UserService.Core.Interfaces.Repositories
{
    public interface IBadgeRepository
    {
        public Task<int?> Create(Badge badge);

        public Task<IEnumerable<UserBadge>> GetAllUserBadgesById(int id);

        public Task UpdateBadgeForUser(UserBadge badge);

        public Task<Badge> GetBadgeById(int id);

        public Task Delete(int id);

        public Task UpdateBadge(Badge badge);

        public Task OnUserCreate(int id);
    }
}