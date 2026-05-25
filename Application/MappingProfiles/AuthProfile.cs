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
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<AuthRequestDto, Auth>();
            CreateMap<Auth, AuthResponseDto>();
        }
    }
}
