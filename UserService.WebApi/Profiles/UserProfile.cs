using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        }
    }
}