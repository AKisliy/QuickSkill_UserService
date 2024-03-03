using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Core.Models;

namespace UserService.Core.Interfaces.Repositories
{
    public interface IBadgeRepository
    {

        public Task<int?> Create(Badge badge);

        public Task<IEnumerable<Core.Models.UserBadge>?> GetAllUserBadgesById(int id);

        public Task<bool> UpdateBadgeForUser(Core.Models.UserBadge badge);
    }
}