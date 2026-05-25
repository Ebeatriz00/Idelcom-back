using Application.DTOs.PMCondition;
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
    public class PMConditionProfile : Profile
    {
        public PMConditionProfile()
        {
            CreateMap<PMConditionCreateDto, PMCondition>();
            CreateMap<PMConditionUpdateDto, PMCondition>();
            CreateMap<PMCondition, PMConditionResponseDto>();
            CreateMap<PMCondition, PMConditionSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
