using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Core.Models;

namespace UserService.Core.Interfaces.Services
{
    public interface IBadgeService
    {

        public Task<IEnumerable<UserBadge>> GetAllBadgesForUser(int id);

        public Task<int?> CreateBadge(string name, string photo, string grayPhoto, int required, string task);

        public Task UpdateBadgeInfoForUser(int userId, int badgeId, int progress);

        public Task<Badge> GetBadgeById(int id);
        public Task DeleteBadge(int id);

        public Task UpdateBadge(Badge badge);
    }
}