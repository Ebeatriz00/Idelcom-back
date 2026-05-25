using Application.DTOs.MovementTypes;
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
    public class MovementTypesProfile : Profile
    {
        public MovementTypesProfile()
        {
            CreateMap<MovementTypesCreateDto, MovementTypes>();
            CreateMap<MovementTypesUpdateDto, MovementTypes>();
            CreateMap<MovementTypes, MovementTypesResponseDto>();
            CreateMap<MovementTypes, MovementTypesByIdDto>();
            CreateMap<MovementTypes, MovementTypesSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
