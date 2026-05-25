using Application.DTOs.Periods;
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
    public class PeriodsProfile : Profile
    {
        public PeriodsProfile()
        {
            CreateMap<PeriodsCreateDto, Periods>();
            CreateMap<PeriodsUpdateDto, Periods>();
            CreateMap<Periods, PeriodsResponseDto>();
            CreateMap<Periods, PeriodsByIdDto>(); 
            CreateMap<Periods, PeriodsSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
