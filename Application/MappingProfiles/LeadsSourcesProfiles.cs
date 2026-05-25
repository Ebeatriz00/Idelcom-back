using Application.DTOs.LeadsSources;
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
    public class LeadsSourcesProfiles : Profile
    {
        public LeadsSourcesProfiles() { 
            CreateMap<LeadsSourcesCreateDto, LeadsSources>();
            CreateMap<LeadsSourcesUpdateDto, LeadsSources>();
            CreateMap<LeadsSources, LeadsSourcesResponseDto>();
            CreateMap<LeadsSources, LeadsSourcesSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
