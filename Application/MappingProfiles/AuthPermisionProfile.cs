using Application.DTOs.Auth;
using AutoMapper;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class AuthPermisionProfile : Profile
    {
        public AuthPermisionProfile()
        {
            CreateMap<AuthEffectivePermsDto, AuthEffectivePerms>();
            CreateMap<AuthEffectivePerms, AuthEffectivePermsDto>();
        }
    }
}
