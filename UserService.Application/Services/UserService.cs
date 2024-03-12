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
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            // handle case when user just registered
            if(activities.Count == 0 && monday <= DateOnly.FromDateTime(DateTime.UtcNow))
            {
                string type = "Past";
                if(monday == today)
                    type = "Today";
 
                activities.Add(
                    new UserActivity 
                    { 
                        UserId = id, 
                        ActivityType = type, 
                        ActivityDate = monday
                    }
                );
            }
            // handle situation when there's no recorded activities for the whole week
            if(activities[0].ActivityDate != monday)
            {
                var firstDay = activities[0].ActivityDate;
                while(firstDay != monday)
                {
                    firstDay = firstDay.AddDays(-1);
                    activities.Insert(0, new UserActivity{ UserId = id, ActivityType = "Past", ActivityDate = firstDay});
                }
            }
            // complete activities to 7 days
            var lastDay = activities.Last().ActivityDate;
            while(activities.Count < 7)
            {
                lastDay = lastDay.AddDays(1);
                string status = "Today";
                if(lastDay > today)
                    status = "Future";
                else if(lastDay < today)
                    status = "Past";
                activities.Add(new UserActivity{UserId = id, ActivityDate = lastDay, ActivityType = status});
            }
            return activities;
        }
    }
}