using Application.DTOs.MovSunat;
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
    public class MovSunatProfile : Profile
    {
        public MovSunatProfile()
        {
            CreateMap<MovSunatCreateDto, MovSunat>();
            CreateMap<MovSunatUpdateDto, MovSunat>();
            CreateMap<MovSunat, MovSunatResponseDto>();
            CreateMap<MovSunat, MovSunatSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
