using AutoMapper;
using UserService.Core.Models;
using UserService.DataAccess.Entities;

namespace UserService.WebApi.Profiles
{
    public class ActivityProfile : Profile
    {
        public ActivityProfile()
        {
            CreateMap<UserActivityEntity, UserActivity>();
        }
    }
}