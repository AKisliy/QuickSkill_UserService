using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.TagHelpers;
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
            CreateMap<User, UserResponse>()
                .ForMember(x => x.Id, o => o.MapFrom(src => src.Id));
            CreateMap<UserRegisterRequest, User>()
                .ForMember(x => x.Username, o => o.MapFrom(src => Generator.GenerateUsername(src.Firstname, src.Lastname, src.Email)));
            CreateMap<UserEntity, User>();
            CreateMap<User, UserEntity>()
                .ForMember(x => x.UserBadges, o => o.Ignore())
                .ForMember(x => x.UserActivities, o => o.Ignore());
        }
    }
}