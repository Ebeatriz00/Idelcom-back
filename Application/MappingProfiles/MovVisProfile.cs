using Application.DTOs.MovVis;
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
    public class MovVisProfile : Profile
    {
        public MovVisProfile()
        {
            CreateMap<MovVisCreateDto, MovVis>();
            CreateMap<MovVisUpdateDto, MovVis>();
            CreateMap<MovVis, MovVisResponseDto>();
            CreateMap<MovVis, MovVisSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
