using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserService.Core.Exceptions;
using UserService.Core.Interfaces;
using UserService.Core.Models;
using UserService.DataAccess.Entities;

namespace UserService.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMapper _mapper;
        private readonly UserServiceContext _context;

        private const string botsEmail = "bot@bot.com";
        private const string botsPassword = "iambot";

        public UserRepository(UserServiceContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            var userEntities = await _context.Users.AsNoTracking().ToListAsync();

            return userEntities.Select(u => _mapper.Map<User>(u));
        }

        public async Task<int> Create(User user)
        {
            var find = _context.Users.FirstOrDefault(u => u.Email == user.Email);
            if(find != null)
                throw new ConflictException("User already registered");
            var userEntity = _mapper.Map<UserEntity>(user);
            userEntity.CreatedAt = DateTime.UtcNow;
            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();
            return userEntity.Id;
        }

        public async Task AddBot(User bot)
        {
            var botEntity = _mapper.Map<UserEntity>(bot);
            botEntity.CreatedAt = DateTime.UtcNow;
            botEntity.IsBot = true;
            botEntity.Email = botsEmail;
            botEntity.Password = botsPassword;
            await _context.Users.AddAsync(botEntity);
            await _context.SaveChangesAsync();
        }

        public async Task AddBots(List<User> bots)
        {
            var botEntities = bots.Select(b => 
            {
                var entity = _mapper.Map<UserEntity>(b);
                entity.CreatedAt = DateTime.UtcNow;
                entity.IsBot = true;
                entity.Email = botsEmail;
                entity.Password = botsPassword;
                return entity;
            });
            await _context.Users.AddRangeAsync(botEntities);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> SetVerificationToken(int id, string token, DateTime expires)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new NotFoundException($"User with id: {id} not found");
            user.VerificationToken = token;
            user.VerificationTokenExpires = expires;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetResetToken(int id, string token, DateTime expires)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new NotFoundException($"User with id: {id} not found");
            user.ResetToken = token;
            user.ResetTokenExpires = expires;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetRefreshToken(int id, string? token, DateTime expires)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new NotFoundException($"User with id: {id} not found");
            user.RefreshToken = token;
            user.RefreshTokenExpires = expires;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> Update(User user)
        {
            var entity = _mapper.Map<UserEntity>(user);
            entity.RefreshTokenExpires = user.RefreshTokenExpires;
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task<bool> Delete(int id)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id) ?? throw new NotFoundException("User is not found");
            await _context.Users.Where(u => u.Id == id).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> GetUserById(int id)
        {
            var user =  await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id) ?? throw new NotFoundException("User is not found");
            return _mapper.Map<User>(user);
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var user =  await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username) ?? throw new NotFoundException("User is not found");
            return _mapper.Map<User>(user);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user =  await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email) ?? throw new NotFoundException("User is not found");
            return _mapper.Map<User>(user);
        }

        public async Task<bool> UpdateUserXp(int id, int xp)
        {
            var user = await GetUserById(id);
            user.Xp += xp;
            await Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task VerifyUser(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token) ?? throw new NotFoundException("No user with this token");
            if(user.VerifiedAt != null)
                throw new ConflictException("User already verified");
            if (user.VerificationTokenExpires < DateTime.UtcNow)
                throw new ConflictException("Token has expired!");
            user.VerifiedAt = DateTime.UtcNow;
            user.VerificationToken = null;
            user.VerificationTokenExpires = null;
            await _context.SaveChangesAsync();
        }

        public async Task ResetPassword(string passwordHash, string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == token) ?? throw new NotFoundException("No user with this token");
            if(user.ResetTokenExpires < DateTime.UtcNow)
                throw new TokenException("Token expired!");
            user.ResetToken = null;
            user.ResetTokenExpires = null;
            user.Password = passwordHash;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUniqueVerificationToken(string token)
        {
            return !await _context.Users.Select(u => u.VerificationToken).AnyAsync(t => t == token);
        }

        public async Task<bool> IsUniqueResetToken(string token)
        {
            return !await _context.Users.Select(u => u.ResetToken).AnyAsync(t => t == token);
        }

        public async Task SetUserActivity(int id)
        {
            if(!await HasUserWithId(id))
                throw new NotFoundException("No user with this id");
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if(await _context.UsersActivities.AnyAsync(ua => ua.ActivityDate == today && ua.UserId == id))
                return;
            var userActivity = new UserActivityEntity
            {
                UserId = id,
                ActivityDate = DateOnly.FromDateTime(DateTime.UtcNow),
                ActivityType = "Active"
            };
            await _context.UsersActivities.AddAsync(userActivity);
            await _context.Users.Where(u => u.Id == id).ExecuteUpdateAsync(s => s
                .SetProperty(s => s.Streak, s => s.Streak + 1)
                .SetProperty(s => s.MaxStreak, s => s.Streak + 1 > s.MaxStreak ? s.Streak + 1 : s.MaxStreak)
            );
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserActivity>> GetAllUserActivity(int id)
        {
            if(!await HasUserWithId(id))
                throw new NotFoundException("No user with this id");
            return _context.UsersActivities.AsNoTracking().Where(ua => ua.UserId == id).Select(a => _mapper.Map<UserActivity>(a));
        }

        public async Task<IEnumerable<UserActivity>> GetActivityByPage(int id, int page, int pageSize)
        {
            if(!await HasUserWithId(id))
                throw new NotFoundException("No user with this id");
            return  await _context.UsersActivities
                .AsNoTracking()
                .Where(ua => ua.UserId == id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => _mapper.Map<UserActivity>(a))
                .ToListAsync();
        }

        public async Task<List<UserActivity>> GetActivityForMonth(int id, int month, int year)
        {
            if(!await HasUserWithId(id))
                throw new NotFoundException("No user with this id");
            return await _context.UsersActivities
                .AsNoTracking()
                .Where(ua => ua.UserId == id && ua.ActivityDate.Month == month && ua.ActivityDate.Year == year)
                .Select(ua => _mapper.Map<UserActivity>(ua))
                .ToListAsync();
        }

        public async Task<List<UserActivity>> GetActivityForWeek(int id)
        {
            if(!await HasUserWithId(id))
                throw new NotFoundException("No user with this id");
            var currentDay = DateTime.UtcNow;
            var firstDayOfWeek = DateOnly.FromDateTime(currentDay.AddDays(-(int)currentDay.DayOfWeek + 1));
            var lastDayOfWeek = firstDayOfWeek.AddDays(6);

            return await _context.UsersActivities
                .AsNoTracking()
                .Where(
                    ua => ua.UserId == id && 
                    ua.ActivityDate >=  firstDayOfWeek && 
                    ua.ActivityDate <= lastDayOfWeek
                )
                .OrderBy(ua => ua.ActivityDate)
                .Select(ua => _mapper.Map<UserActivity>(ua))
                .ToListAsync();
        }

        public async Task IncreaseUserHearts(int id, int cnt = 1)
        {
            // logic without exception handling
            await _context.Users.Where(u => u.Id == id).ExecuteUpdateAsync(u => u
                .SetProperty(u => u.Hearts, u => u.Hearts + cnt >= 5 ? 5 : u.Hearts + cnt));
        }

        public async Task DecreaseUserHearts(int id, int cnt = 1)
        {
            // logic without exception handling
            await _context.Users.Where(u => u.Id == id).ExecuteUpdateAsync(u => u
                .SetProperty(u => u.Hearts, u => u.Hearts <= cnt ? 0 : u.Hearts - cnt));
        }

        public async Task IncreaseUserFreezers(int id, int cnt = 1)
        {
            // logic without exception handling
            await _context.Users.Where(u => u.Id == id).ExecuteUpdateAsync(u => u
                .SetProperty(u => u.Freezer, u => u.Freezer + cnt));
        }

        public async Task IncreaseUserCrystalls(int id, int cnt = 1)
        {
            // logic without exception handling
            await _context.Users.Where(u => u.Id == id).ExecuteUpdateAsync(u => u
                .SetProperty(u => u.Crystall, u => u.Crystall + cnt));
        }

        public async Task DecreaseUserCrystall(int id, int cnt = 1)
        {
            // logic without exceptions handling
            await _context.Users.Where(u => u.Id == id).ExecuteUpdateAsync(u => u
                .SetProperty(u => u.Crystall, u => u.Crystall - cnt > 0 ? u.Crystall - cnt : 0));
        }

        public async Task<bool> HasUserWithId(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> HasUserWithEmail(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
    }
}