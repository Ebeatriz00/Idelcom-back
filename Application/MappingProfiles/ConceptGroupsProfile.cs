using Application.DTOs.ConceptGroups;
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
    public class ConceptGroupsProfile : Profile
    {
        public ConceptGroupsProfile()
        {
            CreateMap<ConceptGroupsCreateDto, ConceptGroups>();
            CreateMap<ConceptGroupsUpdateDto, ConceptGroups>();
            CreateMap<ConceptGroups, ConceptGroupsResponseDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
