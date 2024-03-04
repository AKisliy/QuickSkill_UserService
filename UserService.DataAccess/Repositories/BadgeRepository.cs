using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Models;
using UserService.DataAccess.Entities;

namespace UserService.DataAccess.Repositories
{
    public class BadgeRepository : IBadgeRepository
    {
        private UserServiceContext _context;
        private IMapper _mapper;

        public BadgeRepository(UserServiceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int?> Create(Badge badge)
        {
            var b = await _context.Badges.FirstOrDefaultAsync(b => b.Name == badge.Name);
            if(b != null)
                return null;
            var entity = _mapper.Map<BadgeEntity>(badge);
            _context.Badges.Add(entity);
            await _context.SaveChangesAsync();
            int badgeId = entity.Id;
            foreach(int id in _context.Users.AsNoTracking().Select(u => u.Id))
            {
                await _context.UserBadges.AddAsync(new UserBadgeEntity() { UserId = id, BadgeId = badgeId});
            }
            await _context.SaveChangesAsync();
            return badgeId;
        }

        public async Task<IEnumerable<UserBadge>?> GetAllUserBadgesById(int id)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if(user == null)
                return null;
            return _context.UserBadges
                .AsNoTracking()
                .Where(ub => ub.UserId == id)
                .OrderBy(ub => ub.Progress / (ub.Badge.Required == 0 ? 1 : ub.Badge.Required))
                .Select(ub => new UserBadge
                {
                    UserId = ub.User.Id,
                    BadgeId = ub.Badge.Id,
                    User = _mapper.Map<User>(ub.User),
                    Badge = _mapper.Map<Badge>(ub.Badge),
                    Progress = ub.Progress,
                    Achieved = ub.Achieved
                });
        }

        public async Task<bool> UpdateBadgeForUser(UserBadge badge)
        {
            var userBadge = await _context.UserBadges.FirstOrDefaultAsync(ub => ub.UserId == badge.UserId && ub.BadgeId == badge.BadgeId);

            if(userBadge == null)
                return false;

            userBadge.Progress = badge.Progress;
            if(userBadge.Progress >= badge.Badge.Required)
            {
                userBadge.Achieved = true;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Badge?> GetBadgeById(int id)
        {
            var badge = await _context.Badges.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);

            if(badge == null)
                return null;

            return _mapper.Map<Badge>(badge);
        }

        public async Task<bool> Delete(int id)
        {
            int res = await _context.Badges.Where(b => b.Id == id).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBadge(Badge badge)
        {
            var badgeEntity = await _context.Badges.FirstOrDefaultAsync(b => b.Id == badge.Id);

            if(badgeEntity == null)
                return false;
            badgeEntity.Name = badge.Name;
            badgeEntity.Photo = badge.Photo;
            badgeEntity.GrayPhoto = badge.GrayPhoto;
            badgeEntity.TaskToAchieve = badge.TaskToAchieve;
            badgeEntity.Required = badge.Required;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}