using Application.DTOs.MovClas;
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
    public class MovClasProfile : Profile
    {
        public MovClasProfile()
        {
            CreateMap<MovClasCreateDto, MovClas>();
            CreateMap<MovClasUpdateDto, MovClas>();
            CreateMap<MovClas, MovClasResponseDto>();
            CreateMap<MovClas, MovClasSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
