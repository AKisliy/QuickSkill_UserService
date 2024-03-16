using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UserService.Core.Enums;
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
                throw new InvalidDataException("User already registered");
            var userEntity = _mapper.Map<UserEntity>(user);
            userEntity.CreatedAt = DateTime.UtcNow;
            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();
            return userEntity.Id;
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

        public async Task<int> Update(User user)
        {
            await _context.Users.Where(u => u.Id == user.Id).ExecuteUpdateAsync(s => s
                .SetProperty(u => u.Description, _ => user.Description)
                .SetProperty(u => u.Email, _ => user.Email)
                .SetProperty(u => u.FirstName, _ => user.FirstName)
                .SetProperty(u => u.GoalDays, _ => user.GoalDays)
                .SetProperty(u => u.GoalText, _ => user.GoalText)
                .SetProperty(u => u.LastName, _ => user.LastName)
                .SetProperty(u => u.Username, _ => user.Username)
                .SetProperty(u => u.Password, _ => user.Password)
                .SetProperty(u => u.Photo, _ => user.Photo)
                .SetProperty(u => u.UserLevel, _ => user.UserLevel)
                .SetProperty(u => u.Xp, _ => user.Xp)
                .SetProperty(u => u.Status, _ => user.Status));
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task<bool> Delete(int id)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if(user == null)
                return false;
            await _context.Users.Where(u => u.Id == id).ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetUserById(int id)
        {
            var user =  await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if(user == null)
                return null;
            return _mapper.Map<User>(user);
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            var user =  await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
            if(user == null)
                return null;
            return _mapper.Map<User>(user);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            var user =  await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
            if(user == null)
                return null;
            return _mapper.Map<User>(user);
        }

        public async Task<bool> UpdateUserXp(int id, int xp)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(user == null)
                return false;
            user.Xp += xp;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task VerifyUser(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token) ?? throw new NotFoundException("No user with this token");
            if(user.VerifiedAt != null)
                throw new Exception("User already verified");
            if (user.VerificationTokenExpires < DateTime.UtcNow)
                throw new Exception("Token has expired!");
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

        public async Task<bool> HasUserWithId(int id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }
    }
}