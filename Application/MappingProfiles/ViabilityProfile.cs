using Application.DTOs.Viability;
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
    public class ViabilityProfile : Profile
    {
        public ViabilityProfile()
        {
            CreateMap<ViabilityCreateDto, Viability>();
            CreateMap<ViabilityUpdateDto, Viability>();
            CreateMap<Viability, ViabilityResponseDto>();
            CreateMap<Viability, ViabilityGetByIdDto>();
            CreateMap<Viability, ViabilitySelectDto>();
            CreateMap<Viability, ViabilityStatusToggleDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
