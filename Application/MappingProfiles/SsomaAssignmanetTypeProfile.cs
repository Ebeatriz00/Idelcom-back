using Application.DTOs.Ssoma;
using Application.DTOs.SsomaAssignmanetType;
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
    public class SsomaAssignmanetTypeProfile : Profile
    {
        public SsomaAssignmanetTypeProfile() {
            CreateMap<SsomaAssignmanetTypeCreateDto, SsomaAssignmanetType>();
            CreateMap<SsomaAsiggnmanetTypeUpdateDto, SsomaAssignmanetType>();
            CreateMap<SsomaAssignmanetType, SsomaAssignmanetTypeResponseDto>();
            CreateMap<SsomaAssignmanetType, SsomaAssignmanetTypeByIdDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
