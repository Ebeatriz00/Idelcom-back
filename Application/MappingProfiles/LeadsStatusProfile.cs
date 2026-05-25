using Application.DTOs.LeadsStatus;
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
    public class LeadsStatusProfile : Profile
    {
        public LeadsStatusProfile()
        {
            CreateMap<LeadsStatusCreateDto, LeadsStatus>();
            CreateMap<LeadsStatusUpdateDto, LeadsStatus>();
            CreateMap<LeadsStatus, LeadsStatusResponseDto>();
            CreateMap<LeadsStatus, LeadsStatusSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
