using Application.DTOs.Uom;
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
    public class UomProfile : Profile
    {
        public UomProfile()
        {
            CreateMap<UomCreateDto, Uom>();
            CreateMap<UomUpdateDto, Uom>();
            CreateMap<Uom, UomResponseDto>();
            CreateMap<Uom, UomSelectDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
