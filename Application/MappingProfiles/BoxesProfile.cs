using Application.DTOs.Boxes;
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
    public class BoxesProfile : Profile
    {
        public BoxesProfile() {
            CreateMap<BoxesCreateDto, Boxes>();
            CreateMap<BoxesUpdateDto, Boxes>();
            CreateMap<Boxes, BoxesResponseDto>();
            CreateMap<Boxes, BoxesByIdDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
