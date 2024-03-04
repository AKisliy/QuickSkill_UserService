using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using UserService.Core.Interfaces;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Interfaces.Services;
using UserService.Core.Models;

namespace UserService.Application.Services
{
    public class BadgeService : IBadgeService
    {
        private IBadgeRepository _badgeRepo;
        private IUserRepository _userRepo;

        public BadgeService(IBadgeRepository badgeRepo, IUserRepository userRepo)
        {
            _badgeRepo = badgeRepo;
            _userRepo = userRepo;
        }
        public async Task<IEnumerable<UserBadge>?> GetAllBadgesForUser(int id)
        {
            return await _badgeRepo.GetAllUserBadgesById(id);
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
            return await _badgeRepo.Create(badge);
        }

        public async Task<bool> UpdateBadgeInfoForUser(int userId, int badgeId, int progress)
        {
            var user = await _userRepo.GetUserById(userId);
            var badge = await _badgeRepo.GetBadgeById(badgeId);

            if(user == null || badge == null)
                return false;
            UserBadge ub = new()
            {
                UserId = userId,
                BadgeId = badgeId,
                Progress = progress,
                User = user,
                Badge = badge
            };
            return await _badgeRepo.UpdateBadgeForUser(ub);
        }

        public async Task<Badge?> GetBadgeById(int id)
        {
            return await _badgeRepo.GetBadgeById(id);
        }

        public async Task<bool> DeleteBadge(int id)
        {
            return await _badgeRepo.Delete(id);
        }

        public async Task<bool> UpdateBadge(Badge badge)
        {
            return await _badgeRepo.UpdateBadge(badge);
        }
    }
}