using Application.DTOs.Users;
using AutoMapper;
using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile() {
            CreateMap<UsersCreateDto, Users>();
            CreateMap<UsersUpdateDto, Users>();
            CreateMap<UsersSettingUpdateDto, Users>();
            CreateMap<UsersPasswordChangeDto, Users>();
            CreateMap<Users, UsersResponseDto>();
            CreateMap<Users, UsersResponseIdDto>();
            CreateMap<Users, UsersSettingResponseIdDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
        }
    }
}
