using Application.DTOs.ActivityState;
using AutoMapper;
using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Application.MappingProfiles
{
    public class ActivityStateProfile : Profile
    {
        public ActivityStateProfile()
        {
            CreateMap<ActivityStateCreateDto, ActivityState>();
            CreateMap<ActivityStateUpdateDto, ActivityState>();
            CreateMap<ActivityState, ActivityStateResponseDto>();
            CreateMap<ActivityState, ActivityStateSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
