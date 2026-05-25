using Application.DTOs.Activity;
using AutoMapper;
using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Application.MappingProfiles
{
    public class ActivityProfile :  Profile
    {
        public ActivityProfile()
        {
            CreateMap<ActivityOpporCreateDto, Activity>();
            CreateMap<ActivityOpporDeleteDto, Activity>();
            CreateMap<ActivityDeleteProjectDto, Activity>();
            CreateMap<Activity, ActivityPriorityOpporDto>();
            CreateMap<Activity, ActivityStateOpporDto>();
            CreateMap<ActivityProjectCreateDto, Activity>();
        }
    }
}
