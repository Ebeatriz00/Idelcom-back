using Application.DTOs.ActivityType;
using AutoMapper;
using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Application.MappingProfiles
{
    public class ActivityTypeProfile : Profile
    {
        public ActivityTypeProfile()
        {
            CreateMap<ActivityTypeCreateDto, ActivityType>();
            CreateMap<ActivityTypeUpdateDto, ActivityType>();
            CreateMap<ActivityType, ActivityTypeResponseDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
