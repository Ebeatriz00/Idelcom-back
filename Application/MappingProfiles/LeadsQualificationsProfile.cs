using Application.DTOs.LeadsQualifications;
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
    public class LeadsQualificationsProfile : Profile
    {
        public LeadsQualificationsProfile()
        {
            CreateMap<LeadsQualificationsCreateDto, LeadsQualifications>();
            CreateMap<LeadsQualificationsUpdateDto, LeadsQualifications>();
            CreateMap<LeadsQualifications, LeadsQualificationsResponseDto>();
            CreateMap<LeadsQualifications, LeadsQualificationsSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
