using Application.DTOs.SubTasks;
using Application.DTOs.SubTasks.Application.DTOs.SubTasks;
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
    public class SubTasksProfile : Profile
    {
        public SubTasksProfile()
        {
            CreateMap<SubTasksCreateDto, SubTasks>();
            CreateMap<SubTasksUpdateDto, SubTasks>();
            CreateMap<SubTasks, SubTasksResponseDto>();
            CreateMap<SubTasks, SubTasksByIdDto>();
            CreateMap<SubTasksDeleteDto, SubTasks>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
        }
    }
}
