using Application.DTOs.MovOper;
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
    public class MovOperProfile : Profile
    {
        public MovOperProfile()
        {
            CreateMap<MovOperCreateDto, MovOper>();
            CreateMap<MovOperUpdateDto, MovOper>();
            CreateMap<MovOper, MovOperResponseDto>();
            CreateMap<MovOper, MovOperSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
