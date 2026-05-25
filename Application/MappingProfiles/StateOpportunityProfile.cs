using Application.DTOs.StateOpportunity;
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
    public class StateOpportunityProfile : Profile
    {
        public StateOpportunityProfile() {

            CreateMap<StateOpportunityCreateDto, StateOpportunity>();
            CreateMap<StateOpportunityUpdateDto, StateOpportunity>();
            CreateMap<StateOpportunity, StateOpportunityResponseDto>();
            CreateMap<StateOpportunity, StateOpportunitySelectDto>();
            CreateMap<StateOpportunity, StateOpportunityByIdDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));

        }
    }
}
