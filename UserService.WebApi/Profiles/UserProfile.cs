using AutoMapper;
using Shared;
using UserService.Core.Models;
using UserService.DataAccess.Entities;
using UserService.Infrastructure;
using UserService.WebApi.Dtos;

namespace UserService.WebApi.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResponse>();
            CreateMap<UserRegisterRequest, User>()
                .ForMember(x => x.Username, o => o.MapFrom(src => Generator.GenerateUsername(src.Firstname, src.Lastname, src.Email)));
            CreateMap<UserEntity, User>();
            CreateMap<User, UserEntity>()
                .ForMember(x => x.UserBadges, o => o.Ignore())
                .ForMember(x => x.UserActivities, o => o.Ignore())
                .ForMember(x => x.VerifiedAt, o => o.MapFrom(src => src.VerifiedAt.HasValue
                                              ? DateTime.SpecifyKind(src.VerifiedAt.Value, DateTimeKind.Utc)
                                              : (DateTime?)null))
                .ForMember(x => x.RefreshTokenExpires, o => o.MapFrom(src => src.RefreshTokenExpires.HasValue
                                              ? DateTime.SpecifyKind(src.RefreshTokenExpires.Value, DateTimeKind.Utc)
                                              : (DateTime?)null));

            CreateMap<User, OtherUserResponse>();
            CreateMap<User, UserCreatedEvent>()
                .ForMember(x => x.UserId, o => o.MapFrom(src => src.Id));
            CreateMap<Bot, User>();
            CreateMap<User, UserChangedEvent>();
        }
    }
}