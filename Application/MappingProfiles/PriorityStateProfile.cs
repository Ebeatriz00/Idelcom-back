using Application.DTOs.PriorityState;
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
    public class PriorityStateProfile : Profile
    {
        public PriorityStateProfile()
        {
            CreateMap<PriorityStateCreateDto, PriorityState>();
            CreateMap<PriorityStateUpdateDto, PriorityState>();
            CreateMap<PriorityState, PriorityStateResponseDto>();
            CreateMap<PriorityState, PriorityStateSelectDto>();
            CreateMap<PriorityState, PriorityStateByIdDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
