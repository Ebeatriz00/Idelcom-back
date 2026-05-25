using Application.DTOs.MovPer;
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
    public class MovPerProfile : Profile
    {
        public MovPerProfile()
        {
            CreateMap<MovPerCreateDto, MovPer>();
            CreateMap<MovPerUpdateDto, MovPer>();
            CreateMap<MovPer, MovPerResponseDto>();
            CreateMap<MovPer, MovPerSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
