using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using UserService.Core.Exceptions;
using UserService.Core.Interfaces;
using UserService.Core.Interfaces.Infrastructure;
using UserService.Core.Models;
using UserService.Core.Notifications;

namespace UserService.Application.Services
{
        
    public class UsersService(
        IUserRepository repository, 
        IMapper mapper, 
        IImageProvider imageProvider,
        IMediator mediator) : IUsersService
    {
        private readonly IImageProvider _imageProvider = imageProvider;
        private readonly IMapper _mapper = mapper;
        private readonly IUserRepository _repository = repository;
        private readonly IMediator _mediator = mediator;

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _repository.GetAllUsers();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _repository.GetUserById(id);
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _repository.GetUserByUsername(username);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _repository.GetUserByEmail(email);
        }

        public async Task<bool> DeleteUser(int id)
        {
            bool res = await _repository.Delete(id);
            if(res)
                await _mediator.Publish(new UserDeletedNotification(id));
            return res;
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
        // only works if we have auto-update in DB
        public async Task<List<UserActivity>> GetUserActivityForMonth(int id, int month, int year)
        {
            if(month < 1 || month > 12)
                throw new BadRequestException("Wrong month!");
            var activities = await _repository.GetActivityForMonth(id, month, year);

            var curMonthStart = new DateOnly(year, month, 1);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var curMonthDays = DateTime.DaysInMonth(year, month);
            // if request for future dates
            if(curMonthStart.Year > today.Year || (curMonthStart.Year == today.Year && curMonthStart.Month > today.Month))
            {
                var cnt = curMonthDays;
                while(cnt > 0)
                {
                    activities.Add(new UserActivity { UserId = id, ActivityDate = curMonthStart, ActivityType = "Future"} );
                    curMonthStart = curMonthStart.AddDays(1);
                    --cnt;
                }
                return activities;
            }
            // if request for past dates and no recordings
            if(activities.Count == 0 && (curMonthStart.Year < today.Year || (curMonthStart.Year == today.Year && curMonthStart.Month < today.Month)))
            {
                var cnt = curMonthDays;
                while(cnt > 0)
                {
                    activities.Add(new UserActivity { UserId = id, ActivityDate = curMonthStart, ActivityType = "Past"});
                    curMonthStart = curMonthStart.AddDays(1);
                    --cnt;
                }
                return activities;
            }
            // some records are inside and it's current month
            if(activities.Count != 0 && curMonthStart.Month == today.Month)
            {
                var firstRecord = activities[0];
                var firstRecordDay = firstRecord.ActivityDate;
                // fill records before 
                if(firstRecord.ActivityDate != curMonthStart)
                {
                    while(curMonthStart != firstRecordDay)
                    {
                        firstRecordDay = firstRecordDay.AddDays(-1);
                        activities.Insert(0, new UserActivity{ UserId = id, ActivityDate = firstRecordDay, ActivityType = "Past"});
                    }
                }
                // check today
                var lastRecordDay = activities.Last().ActivityDate;
                if(lastRecordDay != today)
                    activities.Add(new UserActivity{ UserId = id, ActivityDate = today, ActivityType = "Today"} );
                while(activities.Count != curMonthDays)
                {
                    lastRecordDay = lastRecordDay.AddDays(1);
                    activities.Add(new UserActivity{ UserId = id, ActivityDate = lastRecordDay, ActivityType = "Future"} );
                }
                return activities;
            }
            // some records inside, but it was previous month
            var firstRecordDate = activities[0].ActivityDate;
            while(activities.Count != curMonthDays)
            {
                firstRecordDate = firstRecordDate.AddDays(-1);
                activities.Add(new UserActivity{ UserId = id, ActivityDate = firstRecordDate, ActivityType = "Past"} );
            }
            return activities;
        }

        public async Task SetUserFirstName(int id, string newName)
        {
            var user = await _repository.GetUserById(id);
            if(user.FirstName == newName)
                return;
            user.FirstName = newName;
            await _repository.Update(user);
            await _mediator.Publish(new UserChangedNotification(user));
        }

        public async Task SetUserLastName(int id, string newLastName)
        {
            var user = await _repository.GetUserById(id);
            if(user.LastName == newLastName)
                return;
            user.LastName = newLastName;
            await _repository.Update(user);
            await _mediator.Publish(new UserChangedNotification(user));
        }

        public async Task SetUserUsername(int id, string newUserName)
        {
            var user = await _repository.GetUserById(id);
            if(user.Username == newUserName)
                return;
            user.Username = newUserName;
            await _repository.Update(user);
            await _mediator.Publish(new UserChangedNotification(user));
        }

        public async Task SetUserDescription(int id, string descritption)
        {
            var user = await _repository.GetUserById(id);
            user.Description = descritption;
            await _repository.Update(user);
        }

        public async Task SetUserPhoto(int id, IFormFile file)
        {
            var user = await _repository.GetUserById(id);
            var newLink = await _imageProvider.UploadFileAsync(file, user.Id);
            user.Photo = newLink;
            await _repository.Update(user);
            await _mediator.Publish(new UserChangedNotification(user));
        }
    }
}