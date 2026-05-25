using Application.DTOs.Profiles;
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
    public class ProfilesProfile : Profile
    {
        public ProfilesProfile() { 
            CreateMap<ProfilesCreateDto, Profiles>();
            CreateMap<ProfilesUpdateDto, Profiles>();
            CreateMap<Profiles, ProfilesResponseDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
