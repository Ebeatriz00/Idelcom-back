using Application.DTOs.UsersPreferences;
using AutoMapper;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class UsersPreferencesProfile : Profile
    {
        public UsersPreferencesProfile() {
            CreateMap<UsersPrefeNotiUpdateDto, UsersPreferences>();
            CreateMap<UsersPrefeUpdateDto, UsersPreferences>();
            CreateMap<UsersPrefeSettingUpdateDto, UsersPreferences>();
            CreateMap<UsersPreferences, UsersPrefeNotifDto>();
            CreateMap<UsersPreferences, UsersPrefeDto>();
            CreateMap<UsersPreferences, UsersPrefeSettingDto>();
        }
       
    }
}
