using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UserService.Core.Models;
using UserService.DataAccess.Entities;
using UserService.WebApi.Dtos;

namespace UserService.WebApi.Profiles
{
    public class BadgeProfile : Profile
    {
        public BadgeProfile()
        {
            CreateMap<Badge, BadgeResponse>();
            CreateMap<Badge, BadgeEntity>();
            CreateMap<BadgeEntity, Badge>();
            CreateMap<BadgeRequest, Badge>()
                .ForMember(b => b.TaskToAchieve, opt => opt.MapFrom(src => src.Task));
        }
    }
}