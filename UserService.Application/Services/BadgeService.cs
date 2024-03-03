using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Core.Models;

namespace UserService.Application.Services
{
    public class BadgeService : IBadgeService
    {
        private IBadgeRepository _repository;

        public BadgeService(IBadgeRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<UserBadge>?> GetAllBadgesForUser(int id)
        {
            var badges = await _repository.GetAllUserBadgesById(id);

            return badges;  
        }

        public async Task<int?> CreateBadge(string name, string photo, string grayPhoto, int required, string task)
        {
            Badge badge = new Badge()
            {
                Name = name,
                Photo = photo,
                GrayPhoto = grayPhoto, 
                Required = required,
                TaskToAchieve = task
            };
            var res = await _repository.Create(badge);
            if(res == null)
                return null;
            return res;
        }

        public async Task<bool> UpdateBadgeInfoForUser(int userId, int badgeId, int progress)
        {
            UserBadge ub = new UserBadge()
            {
                UserId = userId,
                BadgeId = badgeId,
                Progress = progress
            };
            var res = await _repository.UpdateBadgeForUser(ub);
            return res;
        }
    }
}