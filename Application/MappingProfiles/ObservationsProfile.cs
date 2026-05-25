using Application.DTOs.Observations;
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
    public class ObservationsProfile : Profile
    {
        public ObservationsProfile()
        {

            CreateMap<ObservationsCreateDto, Observations>();
            CreateMap<Observations, ObservationsResponseDto>();
            CreateMap<ObservationUpdateDto, Observations>();
            CreateMap<ObservationsDateUpdateDto, Observations>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
