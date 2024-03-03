using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
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
                await _context.UserBadges.AddAsync(new Entities.UserBadge() { UserId = id, BadgeId = badgeId});
            }
            await _context.SaveChangesAsync();
            return badgeId;
        }

        public async Task<IEnumerable<Core.Models.UserBadge>?> GetAllUserBadgesById(int id)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if(user == null)
                return null;
            return _context.UserBadges
                .AsNoTracking()
                .Where(ub => ub.UserId == id)
                .OrderBy(ub => ub.Progress / ub.Badge.Required)
                .Select(ub => new Core.Models.UserBadge
                {
                    Badge = _mapper.Map<Badge>(ub.Badge),
                    Progress = ub.Progress,
                    Achieved = ub.Achieved
                });
        }

        public async Task<bool> UpdateBadgeForUser(Core.Models.UserBadge badge)
        {
            var userBadge = await _context.UserBadges.FirstOrDefaultAsync(ub => ub.UserId == badge.UserId && ub.BadgeId == badge.BadgeId);

            if(userBadge == null)
                return false;

            userBadge.Progress = badge.Progress;
            if(userBadge.Progress >= badge.Badge.Required)
            {
                badge.Achieved = true;
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}