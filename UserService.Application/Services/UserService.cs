using AutoMapper;
using UserService.Core.Interfaces;
using UserService.Core.Models;

namespace UserService.Application.Services
{
    public class UsersService(IUserRepository repository, IMapper mapper) : IUsersService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IUserRepository _repository = repository;

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _repository.GetAllUsers();
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _repository.GetUserById(id);
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            return await _repository.GetUserByUsername(username);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _repository.GetUserByEmail(email);
        }

        public async Task<bool> DeleteUser(int id)
        {
            return await _repository.Delete(id);
        }

        public async Task<bool> UpdateUserXp(int id, int xp)
        {
            return await  _repository.UpdateUserXp(id, xp);
        }

        public async Task SetUserActivity(int id)
        {
            await _repository.SetUserActivity(id);
        }

        public async Task<List<UserActivity>> GetUserActivityForWeek(int id)
        {
            var activities = await _repository.GetActivityForWeek(id);
            if(activities.Count == 7)
                return activities;
            
            var monday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-(int)DateTime.UtcNow.DayOfWeek + 1));
            if(activities.Count == 0 && monday < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                activities.Add(
                    new UserActivity 
                    { 
                        UserId = id, 
                        ActivityType = "Past", 
                        ActivityDate = monday
                    }
                );
            }

            var lastDay = activities.Last().ActivityDate;
            while(activities.Count < 7)
            {
                lastDay = lastDay.AddDays(1);
                activities.Add(new UserActivity{ UserId = id, ActivityDate = lastDay, ActivityType = "Future" });
            }
            return activities;
        }
    }
}