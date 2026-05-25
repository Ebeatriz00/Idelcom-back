using Application.DTOs.SsomaMovementType;
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
    public class SsomaMovementTypeProfile : Profile
    {
        public SsomaMovementTypeProfile()
        {
            CreateMap<SsomaMovementTypeCreateDto, SsomaMovementType>();
            CreateMap<SsomaMovementTypeUpdateDto, SsomaMovementType>();
            CreateMap<SsomaMovementType, SsomaMovementTypeResponseDto>();
            CreateMap<SsomaMovementType, SsomaMovementTypeSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
