using Application.DTOs.Area;
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
    public class AreaProfile : Profile
    {
        public AreaProfile()
        {
            CreateMap<AreaCreateDto, Area>();
            CreateMap<AreaUpdateDto, Area>();
            CreateMap<Area, AreaResponseDto>();
            CreateMap<Area, AreaSelectDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));


        }
    }
}
