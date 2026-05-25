using Application.DTOs.PMVis;
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
    public class PMVisProfile : Profile
    {
        public PMVisProfile()
        {
            CreateMap<PMVisCreateDto, PMVis>();
            CreateMap<PMVisUpdateDto, PMVis>();
            CreateMap<PMVis, PMVisResponseDto>();
            CreateMap<PMVis, PMVisSelectDto>(); 

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
