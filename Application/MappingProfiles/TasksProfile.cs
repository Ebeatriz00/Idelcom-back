using Application.DTOs.Tasks;
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
    public class TasksProfile : Profile
    {
        public TasksProfile()
        {
            CreateMap<TasksCreateDto, Tasks>();
            CreateMap<TasksUpdateDto, Tasks>();
            CreateMap<TaksOpporDeleteDto, Tasks>();
            CreateMap<TasksProjectDeleteDto, Tasks>();
            CreateMap<Tasks, TasksResponseDto>();
            CreateMap<Tasks, TasksProjectResponseDto>();
            CreateMap<Tasks, TasksByIdDto>();
            CreateMap<Tasks, TasksSelectDto>();
            

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
